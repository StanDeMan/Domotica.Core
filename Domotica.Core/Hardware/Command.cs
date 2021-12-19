using System;
using System.Drawing;
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

            LightDimmer(cmd);
        }

        private static void LightDimmer(string cmd)
        {
            using var apa102 = new Gadget.Device(8);

            if (!apa102.IsWorking) return;

            var dimVal = Convert.ToInt32(cmd.Split(' ')[2]);
            apa102.Dim(dimVal);
            apa102.Flush();
        }
    }
}
