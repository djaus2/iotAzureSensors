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



namespace IoTCoreMenu
{
    public sealed partial class MainPage : Page
    {

        private async void Button_Click(object sender, RoutedEventArgs e)
        {

            if (UseSerial)
            { 
                if (Globals.SerialTerminalPage == null)
                {
                    this.Frame.Navigate(typeof(SerialTerminalPage), null);
                    return;
                }
            }
            else
            {
                if (Globals.BluetoothSerialTerminalPage == null)
                {
                    this.Frame.Navigate(typeof(BluetoothSerialTerminalPage), null);
                    return;
                }
            }

            Button button = (Button)sender;
            if (button != null)
            {
                ShowProgress(true);
                Point coords = (Point)button.Tag;
                //System.Diagnostics.Debug.WriteLine(command);
                //Paste your button switch statement here from Debug output
                IMobileServiceTable<Telemetry2> telemetryTable = App.MobileService.GetTable<Telemetry2>();
                string sCoords = coords.ToPt();
                string cmd = ((string)button.Content).Replace(" ", "_");
                string val;
                string lcdMsg;
                if (coords.Row != 0)
                {
                    iSensor = coords.Col;
                    iCmd = coords.Row;
                }
                    

                double value;
                try
                {
                    switch (sCoords)
                    {
                        // Main commands
                        case "(0,0)":
                        case "(1,0)":
                        case "(2,0)":
                        case "(3,0)":
                            navButton_Click(sender, e);
                            break;
                        //case "(4,0)":
                        //    try
                        //    {
                        //        Globals.SerialTerminalPage.Send("~C");
                        //        //Globals.STP.Send("0123456789a123456789b123456789c123456789");
                        //        string str = "~" + ArduinoLCDDisplay.LCD.CMD_HOME_1_CH.ToString();
                        //        str += "Read: A B C D   ";

                        //        str += "~" + ArduinoLCDDisplay.LCD.CMD_HOME_2_CH.ToString();
                                

                        //        str+= "DT BT HUMID PRES";

                        //        str += "~" + ArduinoLCDDisplay.LCD.CMD_HOME_1_CH;
                        //        str += "~" + ArduinoLCDDisplay.LCD.CMD_BLINK_CH;
                        //        for (int i = 0; i < 9; i++)
                        //        {
                        //            str += "~" + ArduinoLCDDisplay.LCD.CMD_CURSORRIGHT_CH;
                        //        }
                        //        Globals.SerialTerminalPage.Send(str);
                        //    }
                        //    catch (Exception ex)
                        //    {
                        //    }
                        //    break;
                        // Read sensors
                        case "(1,1)":
                        case "(2,1)":
                        case "(3,1)":
                        case "(4,1)":
                            System.Diagnostics.Debug.WriteLine(sCoords);
                            if (UseSerial)
                                Globals.SerialTerminalPage.Send(coords.Col - 1);
                            else
                                Globals.BluetoothSerialTerminalPage.Send(coords.Col - 1);
                            break;
                        // Post to Azure
                        case "(1,3)":
                        case "(2,3)":
                        case "(3,3)":
                        case "(4,3)":
                            val = TextBoxes["textBox" + cmd].Text;
                            if (!double.TryParse(val, out value))
                                return;
                            await Telemetry.Post(cmd, value);
                            lcdMsg = "~C" + Commands.Sensors[iSensor - 1];
                            lcdMsg += "~" + ArduinoLCDDisplay.LCD.CMD_DISPLAY_LINE_2_CH + Commands.CommandActions[iCmd] + " Done" + Globals.EndBlanks;
                            if (UseSerial)
                                Globals.SerialTerminalPage.Send(lcdMsg); 
                            else
                                Globals.BluetoothSerialTerminalPage.Send(lcdMsg);
                            break;
                        // Get from Azure
                        case "(1,4)":
                        case "(2,4)":
                        case "(3,4)":
                        case "(4,4)":
                            value = await Telemetry.RefreshTelemetryItemValue(cmd);
                            string valueStr = value.ToString();
                            //The database returns -1 if the record isn't found
                            if (value == -1)
                                valueStr = "Failed";
                            TextBoxes["textBox" + cmd].Text = valueStr;
                            lcdMsg = "~C" + Commands.Sensors[iSensor - 1];
                            lcdMsg += "~" + ArduinoLCDDisplay.LCD.CMD_DISPLAY_LINE_2_CH + Commands.CommandActions[iCmd] + " " + valueStr + Globals.EndBlanks;
                            if (UseSerial)
                                Globals.SerialTerminalPage.Send(lcdMsg);
                            else
                                Globals.BluetoothSerialTerminalPage.Send(lcdMsg);
                            break;
                        // Get full history for sensor
                        case "(1,5)":
                        case "(2,5)":
                        case "(3,5)":
                        case "(4,5)":
                            this.Frame.Navigate(typeof(ListTelemetryPage), coords.Col - 1);
                            break;
                        // Update current sensor value in Azure
                        case "(1,6)":
                        case "(2,6)":
                        case "(3,6)":
                        case "(4,6)":
                            val = TextBoxes["textBox" + cmd].Text;
                            if (!double.TryParse(val, out value))
                                return;
                            lcdMsg = "~C" + Commands.Sensors[iSensor - 1];
                            lcdMsg += "~" + ArduinoLCDDisplay.LCD.CMD_DISPLAY_LINE_2_CH + Commands.CommandActions[iCmd];
                            if (await Telemetry.UpdateTelemetryItem(cmd, value))
                            {
                                lcdMsg += " Done"+Globals.EndBlanks;
                                if (UseSerial)
                                    Globals.SerialTerminalPage.Send(lcdMsg);
                                else
                                    Globals.BluetoothSerialTerminalPage.Send(lcdMsg);
                            }
                            else
                            {
                                lcdMsg += " Failed"+Globals.EndBlanks;
                                if (UseSerial)
                                    Globals.SerialTerminalPage.Send(lcdMsg);
                                else
                                    Globals.BluetoothSerialTerminalPage.Send(lcdMsg);
                            }
                            break;
                        // Clear current sensor value from Azure
                        case "(1,7)":
                        case "(2,7)":
                        case "(3,7)":
                        case "(4,7)":
                            lcdMsg = "~C" + Commands.Sensors[iSensor - 1];
                            lcdMsg += "~" + ArduinoLCDDisplay.LCD.CMD_DISPLAY_LINE_2_CH + Commands.CommandActions[iCmd];
                            if (  await Telemetry.DeleteTelemetryItem(cmd))
                            {
                                lcdMsg += " Done" + Globals.EndBlanks;
                                if (UseSerial)
                                    Globals.SerialTerminalPage.Send(lcdMsg);
                                else
                                    Globals.BluetoothSerialTerminalPage.Send(lcdMsg);
                            }
                            else
                            {
                                lcdMsg += " Failed" + Globals.EndBlanks;
                                if (UseSerial)
                                    Globals.SerialTerminalPage.Send(lcdMsg);
                                else
                                    Globals.BluetoothSerialTerminalPage.Send(lcdMsg);
                            }
                               
                            break;
                        // Clear all sensor values from Azure
                        case "(1,8)":
                        case "(2,8)":
                        case "(3,8)":
                        case "(4,8)":
                            TextBoxes["textBox" + cmd].Text = "Todo";
                            //this.Frame.Navigate(typeof(ListTelemetryPage), coords.Col - 1);
                            break;
                       
                        default:
                            System.Diagnostics.Debug.WriteLine("Command: {0} not found", coords);
                            break;
                    }
                    ShowProgress(false);
                }
                catch (Exception ex)
                {
                    ShowProgress(false);
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
        }
        private void navButton_Click(object sender, RoutedEventArgs e)
        {
            //Frame.GoBack();
            Button button = (Button)sender;
            string buttonContent = (string)button.Content;
            if (buttonContent == "Setup Serial")
                this.Frame.Navigate(typeof(SerialTerminalPage), null);
            else if (buttonContent == "Setup BT")
                this.Frame.Navigate(typeof(BluetoothSerialTerminalPage), null);
            else if (buttonContent == "Show sensor list")
            {
                this.Frame.Navigate(typeof(ListTelemetryPage), "Current");
            }
            else if (buttonContent == "Back to sensor list")
            {
                this.Frame.Navigate(typeof(ListTelemetryPage), "");
            }
        }
    }
}
