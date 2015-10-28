using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTCoreMenu
{
    public static class ArduinoLCDDisplay
    {
        public static class LCD
        {
            public const char CMD_COMMAND_ON_CH = '~';
            //Enter command mode

            //Commands are now a single char so exit out of command mode once done:
            //        public const char CMD_COMMAND_OFF_CH  =  '~';
            //        //Exit command mode

            public const char CMD_TILDA_ESC_CH = '~';
            //Display ~ and jump out of command mode
            // i.e. ~~ will in normal text will display ~

            public const char CMD_RESET_CH = 'R';
            //Reset the display to 16 chars x 2 rows

            public const char CMD_CLEAR_CH = 'C';
            //Clear the LCD screen
            //and positions the cursor in the upper-left corner

            public const char CMD_HOME_1_CH = 'H';
            //Position the cursor in the upper-left of the LCD
            //i.e. Set cursor at start of first line

            public const char CMD_HOME_2_CH = 'h';
            //Set cursor at start of second line   

            public const char CMD_SCROLL_UP = '^';
            public const char CMD_SCROLL_DOWN = 'V';

            public const char CMD_STRING_TERMINATOR_CH = '#';
            public const char CMD_STRING_NULL_TERMINATOR_CH = '\0';

            public const char CMD_DISPLAY_LINE_1_CH = '1';
            //Set cursor at start of first line and recv a string to display on it
            //String neeads to be # terminated (# isn';t relaced)
            // Or should be 16 chars as line isn';t cleared

            public const char CMD_DISPLAY_LINE_2_CH = '2';
            //Set cursor at start of second line and recv a string to display on it

            public const char CMD_BACLIGHT_ON_CH = 'L';
            //Turns on the LCD display, after it';s been turned off

            public const char CMD_BACKLIGHT_OFF_CH = 'l';
            //Turns off the LCD display

            public const char CMD_DISPLAY_ON_CH = 'D';
            //Turns on the LCD display, after it';s been turned off

            public const char CMD_DISPLAY_OFF_CH = 'd';
            //Turns off the LCD display



            public const char CMD_CURSOR_CH = '_';
            //Display the LCD cursor an underscore (line) at the position

            public const char CMD_NOCURSOR_CH = '|';
            //Hides the LCD cursor

            public const char CMD_BLINK_CH = 'B';
            //Display the blinking LCD cursor.

            public const char CMD_NOBLINK_CH = 'b';
            //Turns off the blinking LCD cursor.

            public const char CMD_SCROLLRIGHT_CH = '>';
            //Scrolls the contents of the display
            // (text and cursor) one space to the right.

            public const char CMD_SCROLLLEFT_CH = '<';
            //Scrolls the contents of the display
            // (text and cursor) one space to the left.

            public const char CMD_CURSORLEFT_CH = '-';
            public const char CMD_CURSORRIGHT_CH = '+';

            public const char CMD_AUTOSCROLL_ON_CH = 'A';
            //Turns on automatic scrolling of the LCD.

            public const char CMD_AUTOSCROLL_OFF_CH = 'a';
            //Turns off automatic scrolling of the LCD

            public const char CMD_HELLO_WORLD_CH = 'W';

            //Sensors
            public const char CMD_DHT_TEMP_READ_CH = 'T';
            //Read DHT Humidity sensor Temperature only

            public const char CMD_BARO_TEMP_READ_CH = 't';
            //Read Barometric pressure sensor temperature only

            public const char CMD_DHT_HUMID_READ_CH = 'U';
            //Read DHT Humidity sensor

            public const char CMD_BARO_PRESS_READ_CH = 'P';
            //Read Barometric pressure sensor.

            public const char CMD_DATA_TERMINATOR_CH = '|';



        }

        public static char[] SensorCommands = new char[] {
                LCD.CMD_DHT_TEMP_READ_CH,
                LCD.CMD_BARO_TEMP_READ_CH,
                LCD.CMD_DHT_HUMID_READ_CH,
                LCD.CMD_BARO_PRESS_READ_CH
       };


        public static class keypad
        {
            public const char BUTTON_LEFT_CHAR  =  '<';
            public const char BUTTON_RIGHT_CHAR =  '>';
            public const char BUTTON_UP_CHAR    =  '^';
            public const char BUTTON_DOWN_CHAR  =  'V';
            public const char BUTTON_SELECT_CHAR=  '#';

            //This is sent:
            public const char BUTTON_NONE_CHAR = ' ';
        }
    }
}
