#nullable enable
using Serilog;
using System;
using System.Reflection;

namespace Domotica.Core.Functionality
{
    public sealed class ImportAssembly
    {
        /// <summary>
        /// Execute a method in the assembly dll
        /// </summary>
        public Method? Method { get; set; }

        /// <summary>
        /// True: id assembly is loaded
        /// </summary>
        public bool IsLoaded { get; set; }

        /// <summary>
        /// Import Assembly and take the specified parameters
        /// </summary>
        /// <param name="assemblyPath">Path where assembly resides</param>
        /// <param name="assemblyName">Name of assembly</param>
        /// <param name="className">Name of class</param>
        /// <param name="ctorParams">Constructor parameters</param>
        /// <exception cref="ArgumentNullException">All parameter have to be given</exception>
        /// <exception cref="DllNotFoundException">If no assembly is found</exception>
        public ImportAssembly(
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
                // load assembly
                var assembly = Assembly.LoadFrom($@"{assemblyPath}\{assemblyName}.dll");
                var type = assembly.GetType($@"{assemblyName}.{className}");

                if (type == null) return;

                // create assembly constructor instance with constructor parameters
                var classInstance = Activator.CreateInstance(type, ctorParams);

                // instantiate method
                Method = new Method(classInstance, type);
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

        /// <summary>
        /// Fluent part for ImportAssembly: instantiate method
        /// </summary>
        /// <param name="classInstance">Instance of class</param>
        /// <param name="type">Type of class</param>
        public Method(object? classInstance, Type type)
        {
            _classInstance = classInstance;
            Type = type;
        }

        /// <summary>
        /// Execute method from imported assembly and class instance
        /// </summary>
        /// <param name="methodName">Take this method</param>
        /// <param name="types">Define method types</param>
        /// <param name="methodParams">Define method parameters</param>
        /// <returns>return value or object</returns>
        /// <exception cref="Exception"></exception>
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
