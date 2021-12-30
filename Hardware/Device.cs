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

        public Apa102? Apa102;
        
        public bool IsReady { get; set; }

        public int Quantity { get; set; }

        public Color Color { get; set; } = Color.White;

        public Device()
        {
            try
            {
                _spiDevice = SpiDevice.Create(new SpiConnectionSettings(0, 0)
                {
                    ClockFrequency = 20_000_000,
                    DataFlow = DataFlow.MsbFirst,
                    Mode = SpiMode.Mode0            // ensure data is ready at clock rising edge
                });

                if (_spiDevice == null) 
                    throw new NullReferenceException("Cannot start SPI.");

                IsReady = true;
            }
            catch (Exception)
            {
                IsReady = false;
            }
        }

        public void Dimmer(dynamic parameter)
        {
            if (!IsReady) return;

            // minimal one APA102 LED possible
            var quantity = parameter.LedAmount ?? 1;

            Quantity = quantity <= 0            
                ? 1 
                : quantity;

            if(_spiDevice != null)
                Apa102 = new Apa102(_spiDevice, Quantity);

            if(Apa102 == null) return;

            // preset alpha and colors
            double a = 0;   
            var red = 0;     
            var green = 0;   
            var blue = 0;    

            try
            {
                a = Convert.ToDouble(parameter.Color.A);        // alpha 
                red   = Convert.ToInt32(parameter.Color.R);     // red
                green = Convert.ToInt32(parameter.Color.G);     // green
                blue  = Convert.ToInt32(parameter.Color.B);     // blue
            }
            catch (Exception)
            {
                // catch silently -> colors are present to 0
            }

            // check if in bounds
            var alpha = a > 1 
                ? 1 
                : (int)(a * byte.MaxValue);

            Color = Color.FromArgb(alpha, red, green, blue);
            Dim(alpha);
            Flush();
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

        private void Flush()
        {
            Apa102?.Flush();
        }

        private void Dim(int brightness)
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