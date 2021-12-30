using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Text;
using Hardware;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Serilog;

namespace Domotica.Core.Hardware
{
    public sealed class Command
    {
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

        public static void Execute(string json)
        {
            var parameter = ReadCmd(json);
            var present = HasProperty(parameter, "Command");

            var ok = present 
                ? (bool)RunInternal(parameter) 
                : (bool)RunExternal(parameter);

            Log.Debug($"Command.Execute: {ok}, {json}");

        }

        private static bool RunExternal(dynamic parameter)
        {
            try
            {
                using var device = new Device();
                if (!device.IsReady) return true;

                device.Dimmer(parameter);
            }
            catch (Exception e)
            {
                Log.Error($"Command.Execute.RunExternal: {e}");

                return false;
            }

            return true;
        }

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

        private static bool HasProperty(dynamic cmd, string name)
        {
            return cmd is ExpandoObject
                ? ((IDictionary<string, object>)cmd).ContainsKey(name)
                : (bool)(cmd.GetType().GetProperty(name) != null);
        }

        private static dynamic ReadCmd(string json, bool onlyParams = true)
        {
            dynamic cmd = JsonConvert.DeserializeObject<ExpandoObject>(json, new ExpandoObjectConverter());

            return onlyParams 
                ? cmd?.Params 
                : cmd;
        }
    }
}