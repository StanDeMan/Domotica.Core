using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Domotica.Core.Execute
{
    public class BashCommand
    {
        public string Execute(string command)
        {
            command = command.Replace("\"","\"\"");

            var proc = Process(command);
            proc.Start();
            proc.WaitForExit();

            return proc.StandardOutput.ReadToEnd();
        }

        public async Task<string> ExecuteAsync(string command, double secsTimeout = 10)
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(secsTimeout));
            command = command.Replace("\"", "\"\"");
            var proc = Process(command);
            await proc.WaitForExitAsync(cts.Token);

            return await proc.StandardOutput.ReadToEndAsync();
        }

        private static Process Process(string command)
        {
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = "-c \"" + command + "\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            proc.Start();

            return proc;
        }
    }
}
