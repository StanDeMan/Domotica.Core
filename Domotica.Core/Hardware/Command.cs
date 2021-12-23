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
    public static class Command
    {
        private static readonly StreamWriter Writer;

        static Command()
        {
            var gpio = Platform.DevicePath;
            var fileStream = new FileInfo(gpio).OpenWrite();
            Writer = new StreamWriter(fileStream, Encoding.ASCII);
        }

        public static void Execute(string cmd)
        {
            var command = $"{cmd}\n";

            Writer.Write(command);
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

            dynamic cmd = JsonConvert.DeserializeObject<ExpandoObject>(json, new ExpandoObjectConverter());
            
            int alpha = Convert.ToInt32(cmd.LedRGBStripe.Color.A * 255);
            int red = Convert.ToInt32(cmd.LedRGBStripe.Color.R);
            int green = Convert.ToInt32(cmd.LedRGBStripe.Color.G);
            int blue = Convert.ToInt32(cmd.LedRGBStripe.Color.B);

            apa102.Color = Color.FromArgb(alpha, red, green, blue);
            apa102.Dim(alpha);
            apa102.Flush();
        }
    }
}