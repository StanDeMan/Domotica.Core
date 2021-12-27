using System;
using System.Drawing;
using System.Dynamic;
using System.IO;
using System.Text;
using Gadget = Hardware;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Domotica.Core.Hardware
{
    public sealed class Command
    {
        private static readonly StreamWriter Writer;

        static Command()
        {
            var gpio = Platform.DevicePath;
            var fileStream = new FileInfo(gpio).OpenWrite();
            Writer = new StreamWriter(fileStream, Encoding.ASCII);
        }

        public static void Execute(string json)
        {
            var param = Read(json);
            var cmd = Convert.ToString(param.Command);

            Writer.Write(@$"{cmd}{Environment.NewLine}");
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
            
            // preset alpha and colors
            double a = 0;   
            var red = 0;     
            var green = 0;   
            var blue = 0;    

            try
            {
                var param = Read(json);

                a = Convert.ToDouble(param.Color.A);        // alpha 
                red   = Convert.ToInt32(param.Color.R);     // red
                green = Convert.ToInt32(param.Color.G);     // green
                blue  = Convert.ToInt32(param.Color.B);     // blue
            }
            catch (Exception)
            {
                // catch silently -> colors are present to 0
            }

            var alpha = a > 1 
                ? 1 
                : (int)(a * byte.MaxValue);

            apa102.Color = Color.FromArgb(alpha, red, green, blue);
            apa102.Dim(alpha);
            apa102.Flush();
        }

        private static dynamic Read(string json)
        {
            dynamic cmd = JsonConvert.DeserializeObject<ExpandoObject>(json, new ExpandoObjectConverter());

            return cmd?.Params;   // return only parameters part
        }
    }
}