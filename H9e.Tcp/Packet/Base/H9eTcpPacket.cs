using H9e.Core.Annotations;
using H9e.Core.Compress;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace H9e.Tcp.Packet {
    public class H9eTcpPacket : IH9eTcpPacket {

        #region 包类列表

        private readonly static object DictLock = new object();
        private readonly static Dictionary<string, Type> PacketDict = new Dictionary<string, Type>();
        private static readonly Dictionary<Type, IEnumerable<PropertyInfo>> PropertyCache = new Dictionary<Type, IEnumerable<PropertyInfo>>();
        public static void RegisterPacket<T>(string tag) {
            if (!PacketDict.ContainsKey(tag)) {
                lock (DictLock) {
                    if (!PacketDict.ContainsKey(tag)) {
                        PacketDict[tag] = typeof(T);
                    }
                }
            }
        }
        private static IEnumerable<PropertyInfo> GetProperties(Type type) {
            if (!PropertyCache.TryGetValue(type, out var properties)) {
                PropertyInfo[] allProperties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                var key = allProperties.Where(p => p.GetCustomAttributes<KeyAttribute>().Any());
                var ignores = allProperties.Where(p => p.GetCustomAttributes<IgnoreAttribute>().Any());
                var orders = allProperties.Where(p => p.GetCustomAttributes<IndexAttribute>().Any()).OrderBy(p => p.GetCustomAttribute<IndexAttribute>().Order);
                properties = key.Concat(orders).Concat(allProperties.Except(key).Except(orders).Except(ignores));
                PropertyCache[type] = properties;
            }
            return properties;
        }

        #endregion

        #region 压缩方式

        private static ICompress Compress = BaseCompress.GetInstance();
        public static void SetCompress(ICompress compress) {
            Compress = compress;
        }

        #endregion

        [Key]
        public string PacketTag { get; set; }

        public H9eTcpPacket(string tag) {
            PacketTag = tag;
        }

        public byte[] ToBytes() {
            List<byte> bytes = new List<byte>();
            IEnumerable<PropertyInfo> properties = GetProperties(GetType());
            foreach (PropertyInfo property in properties) {
                object value = property.GetValue(this);
                if (value == null) {
                    bytes.AddRange(BitConverter.GetBytes(0));
                } else {
                    switch (value) {
                        case string str:
                            var buffer = Encoding.UTF8.GetBytes(str);
                            bytes.AddRange(BitConverter.GetBytes(buffer.Length));
                            bytes.AddRange(buffer);
                            break;
                        case int i:
                            bytes.AddRange(BitConverter.GetBytes(i));
                            break;
                        case long l:
                            bytes.AddRange(BitConverter.GetBytes(l));
                            break;
                        case byte[] buf:
                            bytes.AddRange(BitConverter.GetBytes(buf.Length));
                            bytes.AddRange(buf);
                            break;
                        case byte b:
                            bytes.Add(b);
                            break;
                    }
                }
            }
            byte[] data = Compress.Compress(bytes.ToArray());
            bytes.Clear();
            bytes.AddRange(BitConverter.GetBytes(data.Length));
            bytes.AddRange(data);
            data = bytes.ToArray();
            bytes.Clear();
            //便于GC回收
            bytes = null;
            return data;
        }

        public static IH9eTcpPacket ToInstance(byte[] bytes) {
            byte[] data;
            try {
                data = Compress.Decompress(bytes);
            } catch (Exception) {
                data = bytes;
            }
            string ptype = Encoding.UTF8.GetString(data, sizeof(int), BitConverter.ToInt32(data, 0));
            Type type = null;
            lock (DictLock) {
                if (PacketDict.ContainsKey(ptype)) {
                    type = PacketDict[ptype];
                }
            }
            if (type == null) {
                throw new IOException($"{ptype} 未知协议包");
            }
            object instance = Activator.CreateInstance(type);
            int offset = 0;
            IEnumerable<PropertyInfo> properties = GetProperties(type);
            foreach (PropertyInfo property in properties) {
                if (offset >= data.Length) {
                    break;
                }
                if (property.PropertyType == typeof(string)) {
                    int length = BitConverter.ToInt32(data, offset);
                    if (length == 0) {
                        property.SetValue(instance, null);
                        offset += sizeof(int);
                        continue;
                    }
                    offset += sizeof(int);
                    string value = Encoding.UTF8.GetString(data, offset, length);
                    property.SetValue(instance, value);
                    offset += length;
                } else if (property.PropertyType == typeof(int)) {
                    int value = BitConverter.ToInt32(data, offset);
                    property.SetValue(instance, value);
                    offset += sizeof(int);
                } else if (property.PropertyType == typeof(long)) {
                    long value = BitConverter.ToInt64(data, offset);
                    property.SetValue(instance, value);
                    offset += sizeof(long);
                } else if (property.PropertyType == typeof(byte[])) {
                    int length = BitConverter.ToInt32(data, offset);
                    if (length == 0) {
                        property.SetValue(instance, null);
                        offset += sizeof(int);
                        continue;
                    }
                    offset += sizeof(int);
                    byte[] value = new byte[length];
                    Array.Copy(data, offset, value, 0, length);
                    property.SetValue(instance, value);
                    offset += length;
                } else if (property.PropertyType == typeof(byte)) {
                    byte value = data[offset];
                    property.SetValue(instance, value);
                    offset += sizeof(byte);
                }
            }

            return instance as IH9eTcpPacket;
        }
    }
}
