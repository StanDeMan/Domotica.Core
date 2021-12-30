using System;
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
            var cmd = Convert.ToString(parameter.Command);

            Writer.Write(@$"{cmd}{Environment.NewLine}");
            Writer.Flush();
        }

        public static void ExecuteAmbient(string json)
        {
            var parameter = ReadCmdParams(json);

            using var device = new Device();
            device.Dimmer(parameter);
        }

        private static dynamic ReadCmdParams(string json)
        {
            dynamic cmd = JsonConvert.DeserializeObject<ExpandoObject>(json, new ExpandoObjectConverter());

            return cmd?.Params;   // return only parameters part
        }
    }
}