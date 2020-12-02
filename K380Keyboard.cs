using System;
using System.Linq;
using HidLibrary;
using Microsoft.Extensions.Logging;

namespace k380_func
{
    class K380Keyboard
    {
        static readonly int HID_VENDOR_ID_LOGITECH = 0x046d;
        static readonly int HID_DEVICE_ID_K380 = 0xb342;

        static readonly byte[] k380_seq_fkeys_on = {0x10, 0xff, 0x0b, 0x1e, 0x00, 0x00, 0x00};
        static readonly byte[] k380_seq_fkeys_off = {0x10, 0xff, 0x0b, 0x1e, 0x01, 0x00, 0x00};

        public HidDevice Device { get; }


        private static void SetFunctionKeys(HidDevice device, bool enable, ILogger logger)
        {
            device.OpenDevice();

            device.ReadFeatureData(out _);

            var canActivate = device.Write(enable ? k380_seq_fkeys_on : k380_seq_fkeys_off);

            device.CloseDevice();

            if (!canActivate)
            {
                logger.LogInformation($"[{DateTime.Now.ToShortTimeString()}] Could not active function keys");
            }
        }

        public static K380Keyboard[] GetConnected()
        {
            return HidDevices.Enumerate(HID_VENDOR_ID_LOGITECH, HID_DEVICE_ID_K380).Where(d => d.IsConnected).Select(d => new K380Keyboard(d)).ToArray();
        }

        private K380Keyboard(HidDevice device)
        {
            Device = device;
        }

        public void SetFunctionKeys(bool enable, ILogger<Worker> logger)
        {
            SetFunctionKeys(Device, enable, logger);
        }
    }
}