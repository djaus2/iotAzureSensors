#define CMD_COMMAND_ON_CH   '~'
        //Enter command mode
        //Commands are now a single char so exit out of command mode once done:
//#define CMD_COMMAND_OFF_CH  '~'
//        //Exit command mode

#define CMD_TILDA_ESC_CH '~'
        //Display ~ and jump out of command mode
        // i.e. ~~ will in normal text will display ~

#define CMD_RESET_CH 'R'
        //Reset the display to 16 chars x 2 rows

#define CMD_CLEAR_CH 'C'
        //Clear the LCD screen
        //and positions the cursor in the upper-left corner

#define CMD_HOME_1_CH 'H'
        //Position the cursor in the upper-left of the LCD
        //i.e. Set cursor at start of first line

#define CMD_HOME_2_CH 'h'
        //Set cursor at start of second line   

#define CMD_SCROLL_UP '^'
#define CMD_SCROLL_DOWN 'V'

#define CMD_STRING_TERMINATOR_CH  '#'
#define CMD_STRING_NULL_TERMINATOR_CH  '\0'

#define CMD_DISPLAY_LINE_1_CH    '1'
       //Set cursor at start of first line and recv a string to display on it
       //String neeads to be # terminated (# isn't relaced)
       // Or should be 16 chars as line isn't cleared

#define CMD_DISPLAY_LINE_2_CH    '2'
       //Set cursor at start of second line and recv a string to display on it

#define CMD_BACLIGHT_ON_CH 'L'
        //Turns on the LCD display, after it's been turned off

#define CMD_BACKLIGHT_OFF_CH 'l'
        //Turns off the LCD display

#define CMD_DISPLAY_ON_CH 'D'
        //Turns on the LCD display, after it's been turned off

#define CMD_DISPLAY_OFF_CH 'd'
        //Turns off the LCD display



#define CMD_CURSOR_CH '_'
        //Display the LCD cursor an underscore (line) at the position

#define CMD_NOCURSOR_CH '|'
        //Hides the LCD cursor

#define CMD_BLINK_CH 'B'
        //Display the blinking LCD cursor.

#define CMD_NOBLINK_CH 'b'
        //Turns off the blinking LCD cursor.

#define CMD_SCROLLRIGHT_CH '>'
        //Scrolls the contents of the display
        // (text and cursor) one space to the right.

#define CMD_SCROLLLEFT_CH '<'
        //Scrolls the contents of the display
        // (text and cursor) one space to the left.

#define CMD_CURSORLEFT_CH  '-'
#define CMD_CURSORRIGHT_CH '+'

#define CMD_AUTOSCROLL_ON_CH 'A'
        //Turns on automatic scrolling of the LCD.

#define CMD_AUTOSCROLL_OFF_CH 'a'
        //Turns off automatic scrolling of the LCD

#define CMD_HELLO_WORLD_CH 'W'

//Sensors
#define CMD_DHT_TEMP_READ_CH  'T'
        //Read DHT Humidity and Temperature sensor
        
#define CMD_BARO_TEMP_READ_CH  't'
        //Read Barometric pressure and temperature

#define CMD_DHT_HUMID_READ_CH  'U'
        //Read DHT Humidity and Temperature sensor
        
#define CMD_BARO_PRESS_READ_CH  'P'
        //Read Barometric pressure and temperature

#define  CMD_DATA_TERMINATOR_CH '|'


