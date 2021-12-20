using System.Device.Spi;
using System.Drawing;
using Iot.Device.Apa102;

namespace Hardware
{
    public class Device : IDisposable
    {
        private readonly SpiDevice? _spiDevice;

        public enum EnmState
        {
            Off = 0,
            On = 1,
            None = 3
        }

        public enum EnmBrightness
        {
            None = 0,
            Middle = 127,
            Full = 255
        }

        public readonly Apa102? Apa102;
        
        public bool IsReady { get; set; }

        public int Quantity { get; set; }

        public Color Color { get; set; } = Color.White;

        public EnmBrightness Brightness { get; set; }

        public Device(int quantity)
        {
            try
            {
                _spiDevice = SpiDevice.Create(new SpiConnectionSettings(0, 0)
                {
                    ClockFrequency = 20_000_000,
                    DataFlow = DataFlow.MsbFirst,
                    Mode = SpiMode.Mode0            // ensure data is ready at clock rising edge
                });

                Quantity = quantity <= 0            // minimal one APA102 LED possible
                    ? 1 
                    : quantity;

                Apa102 = new Apa102(_spiDevice, Quantity);
                IsReady = true;
            }
            catch (Exception)
            {
                IsReady = false;
            }
        }

        public void Flush()
        {
            Apa102?.Flush();
        }

        public void Switch(EnmState state)
        {
            switch (state)
            {
                case EnmState.Off:
                    Off();
                    break;

                case EnmState.On:
                    On();
                    break;

                case EnmState.None:
                default:
                    Off();
                    break;
            }
        }

        public void Dim(int brightness)
        {
            for (var i = 0; i < Apa102?.Pixels.Length; i++)
            {
                Apa102.Pixels[i] = Color.FromArgb(brightness, Color.R, Color.G, Color.B);
            }

            Apa102?.Flush();
        }

        private void Off(int brightness = 0)
        {
            for (var i = 0; i < Apa102?.Pixels.Length; i++)
            {
                Apa102.Pixels[i] = Color.FromArgb(brightness, 0, 0, 0);
            }

            Apa102?.Flush();
        }

        private void On(int brightness = 255)
        {
            for (var i = 0; i < Apa102?.Pixels.Length; i++)
            {
                Apa102.Pixels[i] = Color.FromArgb(brightness, 255, 255, 255);
            }

            Apa102?.Flush();
        }

        public void Dispose()
        {
            Apa102?.Dispose();
            _spiDevice?.Dispose();
        }
    }
}