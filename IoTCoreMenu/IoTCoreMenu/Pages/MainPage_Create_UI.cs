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
        public string[] ArduinoLCDMenu  = null;
        private void LayoutDesign()
        {
            TextBoxes = new Dictionary<string, TextBox>();
            ArduinoLCDMenu = new string[0];
            buttonSwitchStatement = "     switch (command) \r\n     {\r\n";

#if GENERATE_ALL_NON_MENUS_UI
            StackPanel VButtonsStackPanel = new StackPanel();
            VButtonsStackPanel.Margin = new Thickness(2);
            VButtonsStackPanel.Orientation = Orientation.Vertical;
            VButtonsStackPanel.HorizontalAlignment = HorizontalAlignment.Left;
            LayoutRoot.Children.Add(VButtonsStackPanel);

            Grid.SetColumn(VButtonsStackPanel, 1);
            Grid.SetRow(VButtonsStackPanel, 1);

            TextBlock DeptListHeading = new TextBlock();
            DeptListHeading.Text = "Commands";
            VButtonsStackPanel.Children.Add(DeptListHeading);

            Button ShowSerial = new Button();
            ShowSerial.Content = "Set up Serial";
            ShowSerial.Click += navButton_Click;
            VButtonsStackPanel.Children.Add(ShowSerial);

            //Create StackPanel for buttons
            StackPanel VButtonsStackPanel2 = new StackPanel();
            VButtonsStackPanel.Margin = new Thickness(2);
            VButtonsStackPanel.Orientation = Orientation.Vertical;
            VButtonsStackPanel.HorizontalAlignment = HorizontalAlignment.Left;
            VButtonsStackPanel.Children.Add(VButtonsStackPanel2);
#endif
            // Add buttons etc to UI
            int numRows = 1 + Commands.MaxIdTag.Row;
            for (int row = 0; row < numRows; row++)
            {
                Array.Resize(ref ArduinoLCDMenu, ArduinoLCDMenu.Length + 1);
                ArduinoLCDMenu[ArduinoLCDMenu.Length - 1] = "";
                StackPanel ButtonsStackPanel = new StackPanel();
                ButtonsStackPanel.Margin = new Thickness(2);
                ButtonsStackPanel.Orientation = Orientation.Horizontal;
                ButtonsStackPanel.HorizontalAlignment = HorizontalAlignment.Left;

                VButtonsStackPanel2.Children.Add(ButtonsStackPanel);
                int numButtons = 1 + Commands.MaxIdTag.Col;
 

                for (int column = 0; column < numButtons; column++)
                {

                    Commands cmd = Commands.GetCommand("MainMenu", column, row);
                    if (cmd == null)
                        continue;
                    if (cmd.name[0] == Commands.ElementConfigCh["cTextBlockPrefix"])
                    {
                        //Is a TextBlock
                        TextBlock textBlock = new TextBlock();
                        textBlock.Text = cmd.name.Substring(1);
                        textBlock.Tag = cmd.idTag;

                        textBlock.HorizontalAlignment = HorizontalAlignment.Left;
                        textBlock.VerticalAlignment = VerticalAlignment.Center;
                        textBlock.TextAlignment = TextAlignment.Right;
                        ArduinoLCDMenu[ArduinoLCDMenu.Length - 1] += " " + cmd.name.Substring(1) + " ";
                        textBlock.Width = Commands.ElementConfigInt["iTextBlockWidth"];
                        textBlock.Height = Commands.ElementConfigInt["iHeight"];
                        ButtonsStackPanel.Children.Add(textBlock);
                        textBlock.Margin = new Thickness(Commands.ElementConfigInt["iMargin"]);
                    }
                    else if (cmd.name[0] == Commands.ElementConfigCh["cTextBoxPrefix"])
                    {
                        //Is a TextBlock
                        TextBox textBox = new TextBox();
                        textBox.Text = cmd.name.Substring(1);
                        textBox.Tag = cmd.idTag;
                        textBox.Name = "textBox" + textBox.Text.Replace(" ", "_");
                        textBox.HorizontalAlignment = HorizontalAlignment.Left;
                        textBox.VerticalAlignment = VerticalAlignment.Center;
                        textBox.TextAlignment = TextAlignment.Right;
                        ArduinoLCDMenu[ArduinoLCDMenu.Length - 1] += " " + cmd.name.Substring(1) + " ";
                        textBox.Width = Commands.ElementConfigInt["iTextBoxWidth"];
                        textBox.Height = Commands.ElementConfigInt["iHeight"];
                        ButtonsStackPanel.Children.Add(textBox);
                        textBox.Margin = new Thickness(Commands.ElementConfigInt["iMargin"]);
                        TextBoxes.Add(textBox.Name, textBox);
                    }
                    else
                    {
                        Button button = new Button();
                        button.Content = cmd.name;
                        button.Name = cmd.name.Replace(" ", "_");
                        button.Tag = cmd.idTag;
                        ArduinoLCDMenu[ArduinoLCDMenu.Length - 1] += "[" + cmd.name + "] ";
                        buttonSwitchStatement += "        case \"" + cmd.name + "\":\r\n          break;\r\n\r\n";
                        button.Width = Commands.ElementConfigInt["iWidth"];
                        button.Height = Commands.ElementConfigInt["iHeight"];
                        ButtonsStackPanel.Children.Add(button);
                        button.Margin = new Thickness(Commands.ElementConfigInt["iMargin"]);
                        button.Click += Button_Click;
                    }
                }
            }

#if GENERATE_ALL_NON_MENUS_UI
            TextBox textBoxSerial = new TextBox();
            textBoxSerial.Text = "";
            textBoxSerial.TextWrapping = TextWrapping.Wrap;
            textBoxSerial.AcceptsReturn = true;
            textBoxSerial.Width = 400;
            textBoxSerial.Name = "textBoxSerial";
            
            
            VButtonsStackPanel.Children.Add(TextBoxSerial);

            ListBox listItems = new ListBox(); 
            VButtonsStackPanel.Children.Add(listItems);

#endif
            TextBoxSerial = textBoxSerial;
            //ListItems = listItems;

            buttonSwitchStatement += "        default:\r\n";
            buttonSwitchStatement += "          System.Diagnostics.Debug.WriteLine(\"Command: {0} not found\",command);\r\n";
            buttonSwitchStatement += "          break;\r\n }\r\n";
            System.Diagnostics.Debug.WriteLine(buttonSwitchStatement);
            //System.Diagnostics.Debug.WriteLine(buttonSwitchStatement);
            //Button CancelButton = new Button();


        }
    }
}
