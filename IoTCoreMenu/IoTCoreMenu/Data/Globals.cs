using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTCoreMenu
{
    public static class Globals
    {
        public static SerialTerminalPage SerialTerminalPage { get; set; } = null;

        public static BluetoothSerialTerminalPage BluetoothSerialTerminalPage { get; set; } = null;

        public static MainPage MP { get; set; } = null;

        public static ListTelemetryPage LTP { get; set; } = null;

        public static TelemetryAPI TelemetryApi { get; set; } = null;

        public static string EndBlanks { get; set; } = "           ";
    }
}
