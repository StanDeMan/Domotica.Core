using Serilog;
using System;
using System.IO;
using System.Text;
using System.Dynamic;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Domotica.Core.Hardware
{
    using Functionality;

    public sealed class Command
    {
        private static ImportAssembly _assembly;
        private static readonly StreamWriter Writer;

        /// <summary>
        /// On development machine local path is set to: ./dev/pigpio file
        /// On linux the path is set to pigpio deamon: /dev/pigpio
        /// </summary>
        static Command()
        {
            var gpio = Platform.DevicePath;
            var fileStream = new FileInfo(gpio).OpenWrite();
            Writer = new StreamWriter(fileStream, Encoding.ASCII);
        }

        /// <summary>
        /// Execute command
        /// </summary>
        /// <param name="json">What should be executed is located in the json file</param>
        public static void Execute(string json)
        {
            var parameter = ReadCmd(json);
            var present = HasProperty(parameter, "External");

            var ok = present 
                ? (bool)RunExternal(parameter) 
                : (bool)RunInternal(parameter);

            Log.Debug($"Command.Execute: {ok}, {json}");
        }

        /// <summary>
        /// External execution
        /// </summary>
        /// <param name="cmdParams">Command parameter part</param>
        /// <returns>True: if went ok</returns>
        private static bool RunExternal(dynamic cmdParams)
        {
            try
            {
                var assemblyName = Convert.ToString(cmdParams.External.Assembly); 
                var className = Convert.ToString(cmdParams.External.Class);
                var methodName = Convert.ToString(cmdParams.External.Method);

                _assembly = new ImportAssembly(assemblyName, className);

                // object created from json: method execution parameters
                var param = new object[1];
                param[0] = cmdParams;            
            
                // type of parameter: dynamic -> so take object
                var type = new[] { typeof(object) };

                _assembly.Method?.Execute(methodName, type, param);
            }
            catch (Exception e)
            {
                Log.Error($"Command.Execute.RunExternal: {e}");

                return false;
            }

            return true;
        }

        /// <summary>
        /// Internal execution
        /// </summary>
        /// <param name="parameter">Command property</param>
        /// <returns>True: if went ok</returns>
        private static bool RunInternal(dynamic parameter)
        {
            try
            {
                var cmd = Convert.ToString(parameter.Command);

                Writer.Write(@$"{cmd}{Environment.NewLine}");
                Writer.Flush();
            }
            catch (Exception e)
            {
                Log.Error($"Command.Execute.RunInternal: {e}");

                return false;
            }

            return true;
        }

        /// <summary>
        /// Check if property present
        /// </summary>
        /// <param name="cmd">Search here inside</param>
        /// <param name="name">Search for this property</param>
        /// <returns>True: if found</returns>
        private static bool HasProperty(dynamic cmd, string name)
        {
            return cmd is ExpandoObject
                ? ((IDictionary<string, object>)cmd).ContainsKey(name)
                : (bool)(cmd.GetType().GetProperty(name) != null);
        }

        /// <summary>
        /// Read command section
        /// </summary>
        /// <param name="json">Read from here</param>
        /// <param name="onlyParams">Take only parameter part for the search</param>
        /// <returns>Json tree part: Params ore complete</returns>
        private static dynamic ReadCmd(string json, bool onlyParams = true)
        {
            dynamic cmd = JsonConvert.DeserializeObject<ExpandoObject>(json, new ExpandoObjectConverter());

            return onlyParams 
                ? cmd?.Params 
                : cmd;
        }
    }
}