using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace H9e.Core {
    public class H9eDynamicCall {



        public static object Call(string dllPath, string clazz, string method, params object[] argv) {
            AppDomain newDomain = AppDomain.CreateDomain("DynamicAssemblyDomain");
            try {
                Assembly assembly = newDomain.Load(AssemblyName.GetAssemblyName(dllPath));
                Type type = assembly.GetType(Path.GetFileNameWithoutExtension(dllPath));
                MethodInfo methodInfo = type.GetMethod(method);
                object instance = Activator.CreateInstance(type);
                object result = methodInfo.Invoke(instance, argv);
                return result;
            } finally {
                AppDomain.Unload(newDomain);
            }
        }

    }
}
