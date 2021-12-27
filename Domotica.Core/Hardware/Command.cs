using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Drawing;
using System.Dynamic;
using System.IO;
using System.Text;
using Gadget = Hardware;

namespace Domotica.Core.Hardware
{
    public sealed class Command
    {
        public enum EnmExecState
        {
            None = 0,
            Internal = 1,
            Command = 2
        }

        private static readonly StreamWriter Writer;

        static Command()
        {
            var gpio = Platform.DevicePath;
            var fileStream = new FileInfo(gpio).OpenWrite();
            Writer = new StreamWriter(fileStream, Encoding.ASCII);
        }

        public static void Execute(string json)
        {
            var value = ReadValues(json);
            Writer.Write($"{value.Command}\n");
            Writer.Flush();
        }

        public static void ExecuteAmbient(string cmd)
        {
            Dimmer(cmd);
        }

        private static void Dimmer(string json)
        {
            using var apa102 = new Gadget.Device(8);
            if (!apa102.IsReady) return;

            var ledColor = ReadValues(json);
            apa102.Color = Color.FromArgb(ledColor.Alpha, ledColor.Red, ledColor.Green, ledColor.Blue);
            apa102.Dim(ledColor.Alpha);
            apa102.Flush();
        }

        private static Value ReadValues(string json)
        {
            dynamic cmd = JsonConvert.DeserializeObject<ExpandoObject>(json, new ExpandoObjectConverter());

            var execute = Convert.ToString(cmd?.Params.Command != null
                ? (string)cmd?.Params.Command
                : string.Empty);

            return new Value
            {
                Command = execute,
                Alpha = Convert.ToInt32(cmd?.Params.Color.A),
                Red = Convert.ToInt32(cmd?.Params.Color.R),
                Green = Convert.ToInt32(cmd?.Params.Color.G),
                Blue = Convert.ToInt32(cmd?.Params.Color.B)
            };
        }
    }

    public sealed class Value
    {
        public string Command { get; set; } = string.Empty;
        public int Alpha { get; set; }
        public int Red { get; set; }
        public int Green { get; set; }
        public int Blue { get; set; }
    }
}