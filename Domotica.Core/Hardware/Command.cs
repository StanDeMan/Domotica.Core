using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Text;
using Hardware;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

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
            var parameter = ReadCmdParams(json);
            var present = HasProperty(parameter, "Command");

            if (present)
            {
                var cmd = Convert.ToString(parameter.Command);

                Writer.Write(@$"{cmd}{Environment.NewLine}");
                Writer.Flush();
            }
            else
            {
                using var device = new Device();
                if (!device.IsReady) return;

                device.Dimmer(parameter);
            }
        }
        
        private static bool HasProperty(dynamic cmd, string name)
        {
            return cmd is ExpandoObject
                ? ((IDictionary<string, object>)cmd).ContainsKey(name)
                : (bool)(cmd.GetType().GetProperty(name) != null);
        }

        private static dynamic ReadCmdParams(string json)
        {
            dynamic cmd = JsonConvert.DeserializeObject<ExpandoObject>(json, new ExpandoObjectConverter());

            return cmd?.Params;   // return only parameters part
        }
    }
}