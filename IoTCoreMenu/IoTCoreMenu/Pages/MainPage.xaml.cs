using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace IoTCoreMenu
{

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        /// <summary>
        /// The dynamically created (at startup) list of commands.
        /// Generated from a JSon file (menus.json) and turned into the UI.
        /// </summary>
        List<Commands> MainMenu;
        
        /// <summary>
        /// When the UI is generated this placed in the UI so that  UpdateText() can use it 
        /// </summary>
        TextBox TextBoxSerial;

        /// Need a collection of TextBoxes to write to as can't programamtically refer to them.
        /// Use TextBox name as key
        /// UpdateText(string msg) works out which TextBox (by its key) from the first character in msg
        /// This is the command character sent to the Arduino board which prefixes the returned string with this,
        Dictionary<string, TextBox> TextBoxes = null;

        DispatcherTimer Debounce = null;

        //ListBox ListItems;

        TelemetryAPI Telemetry;
        private bool UseSerial = true;


        public MainPage()
        {
            this.NavigationCacheMode =
                    Windows.UI.Xaml.Navigation.NavigationCacheMode.Required;
            this.InitializeComponent();
           // bt bt = new bt();
           // bt.BTConnect();
            Telemetry = new TelemetryAPI();
            Commands.Init();
            DoCommands(null, null);
            UseSerial = (Commands.ElementConfigStr["sUseSerial"] != "BT");
            LayoutDesign();
            Globals.MP = this;
            this.Loaded += (sender, e) =>
            {
                //ShowProgress(true);
                //this.Frame.Navigate(typeof(SerialTerminalPage), null);
            };
            datum = "";
            Debounce = new DispatcherTimer() {
                Interval = new TimeSpan(1000),
            };
            Debounce.Tick += Debounce_Tick;
            
        }

        private void Debounce_Tick(object sender, object e)
        {
            Debounce.Stop();
        }

        private void DoCommands(object sender, RoutedEventArgs e)
        {
            GetCommands("ElementConfig");
            //Following settings are mandatory
            bool res = Commands.CheckKeys();
            //Next two are optional settings
            bool res2 = Commands.CheckComportIdSettingExists();
            res2 = Commands.CheckcIfComportConnectDeviceNoKeySettingExists();
            GetCommands("MainMenu");
            MainMenu = Commands.GetMenu("MainMenu");

        }






        private int iSensor=1;
        private int iCmd = 1;

        private string datum = "";
        /// <summary>
        /// Called by SerialTerminalPage when message is received from Arduino device
        /// </summary>
        /// <param name="msg">The received value prefixed by the command character</param>
        public async Task UpdateText(string msg)
        {
            // Get the command character
            string msgg = msg.Substring(1);
            //Get the received value
            string cmd = msg.Substring(0, 1);
            char cmdCh = msg[0];
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                //Action the keypress or Display the received value
                if ((new List<char> { '<', '>', 'V', '^', '#', '@' }).Contains(cmdCh))
                {
                    if (Debounce.IsEnabled)
                        return;
                    datum = "";
                    switch (cmdCh)
                    {
                        case '@':
                            iSensor = 1;
                            iCmd = 1;
                            break;
                        case '<':
                            iSensor -= 1;
                            if (iSensor < 1)
                                iSensor = Commands.Sensors.Length;
                            break;
                        case '>':
                            iSensor += 1;
                            if (iSensor > Commands.Sensors.Length)
                                iSensor = 1;
                            break;
                        case '^':
                            iCmd += 1;
                            if (iCmd == Commands.ElementConfigInt[Commands.cValuesRowIndexKey])
                                iCmd += 1;
                            if (iCmd > Commands.CommandActions.Length - 1)
                                iCmd = 1;
                            break;
                        case 'V':
                            iCmd -= 1;
                            if (iCmd == Commands.ElementConfigInt[Commands.cValuesRowIndexKey])
                                iCmd -= 1;
                            if (iCmd < 1)
                                iCmd = Commands.CommandActions.Length - 1;
                            break;
                        case '#':
                            Button_Click(new Button { Name = "VirtualButton" + Commands.CommandActions[iSensor - 1], Content = Commands.Sensors[iSensor - 1], Tag = new Point(iSensor, iCmd) }, null);
                            break;
                    }

                    if ((cmdCh == '<') || (cmdCh == '>') || (cmdCh == '^') || (cmdCh == 'V'))
                    {
                        TextBoxes["textBoxSensor"].Text = Commands.CommandActions[iCmd] + Commands.Sensors[iSensor - 1];
                        string lcdMsg = "~C" + Commands.Sensors[iSensor - 1];
                        lcdMsg += "~" + ArduinoLCDDisplay.LCD.CMD_DISPLAY_LINE_2_CH + Commands.CommandActions[iCmd] + Globals.EndBlanks;
                        if (UseSerial)
                            Globals.SerialTerminalPage.Send(lcdMsg);
                        else
                            Globals.BluetoothSerialTerminalPage.Send(lcdMsg);
                    }
                    Debounce.Start();
                }
                else
                {
                    if ((new List<char> {
                        ArduinoLCDDisplay.LCD.CMD_DHT_TEMP_READ_CH,
                        ArduinoLCDDisplay.LCD.CMD_BARO_TEMP_READ_CH,
                        ArduinoLCDDisplay.LCD.CMD_DHT_HUMID_READ_CH,
                        ArduinoLCDDisplay.LCD.CMD_BARO_PRESS_READ_CH
                    }).Contains(msg[0]))
                    {
                        datum = "";
                    }
                    datum += msg;
                    if (datum[datum.Length-1] == ArduinoLCDDisplay.LCD.CMD_DATA_TERMINATOR_CH)
                    {
                        msg = datum.Substring(0,datum.Length-1);
                        datum = "";
                        msgg = msg.Substring(1);
                        //Get the received value
                        cmd = msg.Substring(0, 1);
                        cmdCh = msg[0];
                        TextBoxSerial.Text = cmd + ":" + msgg + "\r\n" + TextBoxSerial.Text;

                        //Direct the value to its textbox
                        string tb = "";
                        switch (msg[0])
                        {
                            case ArduinoLCDDisplay.LCD.CMD_DHT_TEMP_READ_CH:
                                tb = "DHTemp";
                                break;
                            case ArduinoLCDDisplay.LCD.CMD_BARO_TEMP_READ_CH:
                                tb = "BaroTemp";
                                break;
                            case ArduinoLCDDisplay.LCD.CMD_DHT_HUMID_READ_CH:
                                tb = "Humidity";
                                break;
                            case ArduinoLCDDisplay.LCD.CMD_BARO_PRESS_READ_CH:
                                tb = "Pressure";
                                break;
                        }
                        if (tb == "")
                            return;
                        tb = "textBox" + tb;
                        TextBoxes[tb].Text = msgg;
                        string lcdMsg = "~C" + Commands.Sensors[iSensor - 1];
                        lcdMsg += "~" + ArduinoLCDDisplay.LCD.CMD_DISPLAY_LINE_2_CH + Commands.CommandActions[iCmd] + " " + msgg + Globals.EndBlanks;
                        if (UseSerial)
                            Globals.SerialTerminalPage.Send(lcdMsg);
                        else
                            Globals.BluetoothSerialTerminalPage.Send(lcdMsg);
                    }
                }
            });
       }

        private void ShowProgress(bool show)
        {
            if (show)
            {
                Progress1.IsActive = true;
                Progress1.Visibility = Visibility.Visible;
            }

            else
            {
                Progress1.IsActive = false;
                Progress1.Visibility = Visibility.Collapsed;
            }
        }
            

        //Generates a switch statement as text and outputs to Debug Window
        private string buttonSwitchStatement;

   
    }  
}
