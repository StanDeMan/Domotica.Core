#nullable enable
using Serilog;
using System;
using System.Reflection;

namespace Domotica.Core.Functionality
{
    public sealed class ExtendAssembly
    {
        public Method? Method { get; set; }

        public bool IsLoaded { get; set; }

        public ExtendAssembly(
            string assemblyPath,
            string assemblyName,
            string className,
            object?[]? ctorParams = null)
        {
            if (assemblyPath == null)
                throw new ArgumentNullException(nameof(assemblyPath));

            if (assemblyName == null)
                throw new ArgumentNullException(nameof(assemblyName));

            if(className == null)
                throw new ArgumentNullException(nameof(className));

            try
            {
                var assembly = Assembly.LoadFrom($@"{assemblyPath}\{assemblyName}.dll");
                var type = assembly.GetType($@"{assemblyName}.{className}");

                if (type == null) return;

                var instance = Activator.CreateInstance(type, ctorParams);

                Method = new Method(instance, type);
                IsLoaded = true;
            }
            catch (Exception e)
            {
                var errMsg = $@"ExtendAssembly failed: {e}";

                Log.Error(errMsg);
                throw new DllNotFoundException(errMsg);
            }
        }
    }

    public sealed class Method
    {
        private readonly object? _classInstance;

        private Type? Type { get; set; }

        public Method(object? classInstance, Type type)
        {
            _classInstance = classInstance;
            Type = type;
        }

        public object? Execute(string methodName, Type[]? types = null, object?[]? methodParams = null)
        {
            var methodInfo = types == null ? 
                Type?.GetMethod(methodName) : 
                Type?.GetMethod(methodName, types);
            
            if (methodInfo == null)
                throw new Exception("No such method exists.");

            return methodInfo.Invoke(_classInstance, methodParams);
        }
    }
}
