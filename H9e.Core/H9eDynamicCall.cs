using System;
using System.IO;
using System.Reflection;

namespace H9e.Core {
    public class H9eDynamicCall {

#if NET20_OR_GREATER

        public static object Call(string dllPath, string clazz, string method, params object[] argv) {
            if (!File.Exists(dllPath)) {
                throw new FileNotFoundException($"DLL file not found: {dllPath}");
            }
            AppDomain newDomain = AppDomain.CreateDomain("DynamicAssemblyDomain");
            try {
                Assembly assembly = newDomain.Load(AssemblyName.GetAssemblyName(dllPath)) ?? throw new FileLoadException($"Failed to load assembly: {dllPath}");
                Type type = assembly.GetType(clazz) ?? throw new TypeLoadException($"Failed to load type: {clazz}");
                MethodInfo methodInfo = type.GetMethod(method) ?? throw new MissingMethodException($"Method not found: {method}");
                object instance = Activator.CreateInstance(type) ?? throw new InvalidOperationException($"Failed to create instance of type: {clazz}");
                object result = methodInfo.Invoke(instance, argv) ?? throw new InvalidOperationException($"Method returned null: {method}");
                assembly = null; // Release the assembly reference
                return result;
            } catch (Exception ex) {
                throw ex;
            } finally {
                AppDomain.Unload(newDomain);
            }
        }

        public static object StaticCall(string dllPath, string clazz, string method, params object[] argv) {
            if (!File.Exists(dllPath)) {
                throw new FileNotFoundException($"DLL file not found: {dllPath}");
            }
            AppDomain newDomain = AppDomain.CreateDomain("DynamicAssemblyDomain");
            try {
                Assembly assembly = newDomain.Load(AssemblyName.GetAssemblyName(dllPath)) ?? throw new FileLoadException($"Failed to load assembly: {dllPath}");
                Type type = assembly.GetType(clazz) ?? throw new TypeLoadException($"Failed to load type: {clazz}");
                MethodInfo methodInfo = type.GetMethod(method) ?? throw new MissingMethodException($"Method not found: {method}");
                object result = methodInfo.Invoke(null, argv) ?? throw new InvalidOperationException($"Method returned null: {method}");
                assembly = null; // Release the assembly reference
                return result;
            } catch (Exception ex) {
                throw ex;
            } finally {
                AppDomain.Unload(newDomain);
            }
        }
#else

       public static object Call(string dllPath, string clazz, string method, params object[] argv) {
            if (!File.Exists(dllPath)) {
                throw new FileNotFoundException($"DLL file not found: {dllPath}");
            }
            Assembly assembly = Assembly.LoadFile(dllPath) ?? throw new FileLoadException($"Failed to load assembly: {dllPath}");
            Type type = assembly.GetType(clazz) ?? throw new TypeLoadException($"Failed to load type: {clazz}");
            MethodInfo methodInfo = type.GetMethod(method) ?? throw new MissingMethodException($"Method not found: {method}");
            object instance = Activator.CreateInstance(type) ?? throw new InvalidOperationException($"Failed to create instance of type: {clazz}");
            object result = methodInfo.Invoke(instance, argv) ?? throw new InvalidOperationException($"Method returned null: {method}");
            assembly = null; // Release the assembly reference
            return result;
        }

        public static object StaticCall(string dllPath, string clazz, string method, params object[] argv) {
            Assembly assembly = Assembly.LoadFile(dllPath);
            Type type = assembly.GetType(clazz) ?? throw new TypeLoadException($"Failed to load type: {clazz}");
            MethodInfo methodInfo = type.GetMethod(method) ?? throw new MissingMethodException($"Method not found: {method}");
            object result = methodInfo.Invoke(null, argv) ?? throw new InvalidOperationException($"Method returned null: {method}");
            assembly = null; // Release the assembly reference
            return result;
        }

#endif



    }
}
