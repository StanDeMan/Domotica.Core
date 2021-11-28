using System.IO;
using System.Text;

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
    }
}
