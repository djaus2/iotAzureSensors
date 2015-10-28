using System;
using System.Collections.Generic;
using System.Text;
using Windows.Data.Json;

namespace IoTCoreMenu
{
    //Note: To change the table from the Azure Mobile Service (ie Same service and therefore same app key) ..
    // Just change the class name here using Refactoring
    public class Telemetry2
    {

        public int Id { get; set; }

 
        public string Sensor { get; set; }


        public bool Complete { get; set; }


        public double Value { get; set; }

        public string _DateTime { get; set; }
    }

    public class Item
    {
        public string Id { get; set; }
        public string Text { get; set; }
    }
}

