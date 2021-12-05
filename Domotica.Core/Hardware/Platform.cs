using System;
using System.IO;
using Net = System.Net; 
//using System.Configuration;

namespace Domotica.Core.Hardware
{
    internal class Platform
    {
        private const string GpioFile = "/dev/pigpio";

        public static string DevicePath { get; set; }
        public static string Dns { get; set; }

        static Platform()
        {
            // If not running on pi set environment for windows platform
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
            {
                DevicePath = GpioFile;

                //Dns = ConfigurationManager.AppSettings["DNS.RasPi"];                  // Raspberry Pi is the preference
                Dns = $"{Net.Dns.GetHostName()}.local";

                return;
            }

            DevicePath = Directory.GetCurrentDirectory() + GpioFile;
            SetPath(DevicePath);
        }

        /// <summary>
        /// Rebuild the path to windows convention:
        /// This is for debugging and simulating purpose 
        /// of the gpio if run under windows
        /// </summary>
        /// <param name="path">Linux path convetion</param>
        private static void SetPath(string path)
        {
            var currentPath = Path.GetFullPath(@"..\..\");  
            path = path.TrimStart('/').Replace('/', '\\');
            DevicePath = Path.Combine(currentPath, path);
        }
    }
}
