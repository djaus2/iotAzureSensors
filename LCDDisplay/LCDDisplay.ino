#include <BaroSensor.h>

#include <DHT.h>

#include <Wire.h>
#include <LiquidCrystal.h>
#include "LCDCommands.h"
#include "Pushbuttons.h"
#include "DHT.h"

/*
   Pins used:
   ==========
    0,1 : USB or BT Serial
    2:    DHT Data (Temperature and Humidity)
    3 :   LCDBacklight
    8, 9, 4, 5, 6, 7: LCD Display
    A0 :  LCD Buttons Volatge
    SCL \ i2C for Barometric Pressure sensor  See the links on the Baro tab.
    SDA / Last 2 pins on 0 1 2 3 4 pins side of board
  •SDA

  LCD Links (Freetronics):
  http://www.freetronics.com.au/collections/display/products/lcd-keypad-shield#.VhxFBI2hdMs
  Quick start (No wiring as it directly plugs in on top of teh ASrduino board: http://www.freetronics.com.au/pages/16x2-lcd-shield-quickstart-guide#.VhxFIY2hdMs

  See Baro tab and DHT tab for detauils

*/

LiquidCrystal lcd( 8, 9, 4, 5, 6, 7 );




bool commandMode;

void setup()
{
  Serial.begin(9600);
  commandMode = false;

  lcd.begin(16, 2);
  lcd.print("embedded101.com!");
  initButtons();
  initDHT();
  initBaro();
}

  //Lines 1&2 buffers
  char buffer1[16];
  char buffer2[16];
  int curs=0;
  int line =0;

void loop()
{
  byte byt = Serial.read();
  char ch;
  int cursor=0;
  while (byt == 255) //-1 = FF
  {
    getKeyboard();
    byt = Serial.read();
  }
  ch = byt;
  if (ch=='\r')
    return;
  if (ch=='\n')
    return;
  if (commandMode)
  {
    switch (ch)
    {
      case CMD_RESET_CH:
        //Reset the display to 16 chars x 2 rows
        lcd.begin(16, 2);
        break;
      case CMD_CLEAR_CH:
        //Clear the LCD screen
        //and positions the cursor in the upper-left corner
        lcd.clear();
        break;
      case CMD_HOME_1_CH:
        //Position the cursor in the upper-left of the LCD.
        //i.e. Set cursor at start of first line
        lcd.home();
        curs=0;
        line=0;
        break;
      case CMD_HOME_2_CH:
        //Set cursor at start of second line
        curs=0;
        line=1;
        lcd.setCursor(0, 1);
        break;
      case CMD_DISPLAY_LINE_1_CH:
        //Set cursor at start of first line and recv a string to display on it
        lcd.setCursor(0,0); //home();
        curs=0;
        line=0;       
        Serial.readBytesUntil(CMD_STRING_TERMINATOR_CH,buffer1,16);
        lcd.print(buffer1);
        break;
      case CMD_DISPLAY_LINE_2_CH:
        //Set cursor at start of second line and recv a string to display on it
        lcd.setCursor(0, 1);
        curs=0;
        line=1;
        Serial.readBytesUntil(CMD_STRING_TERMINATOR_CH,buffer2,16);
        lcd.print(buffer2);
        break;
      case CMD_SCROLL_UP:
        //Copy the contents of line 1 buffer to line2 and read string into line 1
        lcd.home();
        for (int i=0;i<16;i++)
          buffer1[i] = buffer2[i];
        lcd.print(buffer1);
        lcd.setCursor(0, 1);
        Serial.readBytesUntil(CMD_STRING_TERMINATOR_CH,buffer2,16);
        lcd.print(buffer2);
        break;   
      case CMD_SCROLL_DOWN:
        //Copy the contents of line 1 buffer to line 2 and read string into line 1
        lcd.setCursor(0, 1);
        for (int i=0;i<16;i++)
          buffer2[i] = buffer1[i];
        lcd.print(buffer2);
        lcd.home();
        Serial.readBytesUntil(CMD_STRING_TERMINATOR_CH,buffer1,16);
        lcd.print(buffer1);
        break;       
      case CMD_BACLIGHT_ON_CH:
        //Turns on the LCD display, after it's been turned off
        Backlight_On();
        break;
      case CMD_BACKLIGHT_OFF_CH:
        //Turns off the LCD display
        Backlight_Off();
        break;
      case CMD_DISPLAY_ON_CH:
        //Turns on the LCD display, after it's been turned off
        lcd.display();
        break;
      case CMD_DISPLAY_OFF_CH:
        //Turns off the LCD display
        lcd.noDisplay();
        break;
      case CMD_CURSOR_CH:
        //Display the LCD cursor: an underscore (line) at the position
        // to which the next character will be written
        lcd.cursor();
        break;
      case CMD_NOCURSOR_CH:
        //Hides the LCD cursor
        lcd.noCursor();
        break;
      case CMD_BLINK_CH:
        //Display the blinking LCD cursor.
        lcd.blink();
        break;
      case CMD_NOBLINK_CH:
        //Turns off the blinking LCD cursor.
        lcd.noBlink();
        break;
      case CMD_SCROLLRIGHT_CH:
        //Scrolls the contents of the display
        // (text and cursor) one space to the right.
        lcd.scrollDisplayRight();
        break;
      case CMD_SCROLLLEFT_CH:
        lcd.scrollDisplayLeft();
        break;
      case CMD_CURSORRIGHT_CH:
        //Scrolls the contents of the display
        // (text and cursor) one space to the right.
        curs +=1;
        if (curs>15)
          curs=0;
        lcd.cursor();
        lcd.setCursor(curs,line);
        break;
      case CMD_CURSORLEFT_CH:
        curs -=1;
        if (curs<0)
             curs=15;
        lcd.cursor();
        lcd.setCursor(curs,line);;
        break;      case CMD_AUTOSCROLL_ON_CH:
        //Turns on automatic scrolling of the LCD.
        lcd.autoscroll();
        break;
      case CMD_AUTOSCROLL_OFF_CH:
        //Turns off automatic scrolling of the LCD
        lcd.noAutoscroll();
        break;
      case CMD_HELLO_WORLD_CH:
        lcd.clear();
        lcd.print("Hello, World!");
        break;

      case CMD_DHT_TEMP_READ_CH:
        ////Read DHT Humidity and Temperature sensor
        readDHT(0);
        break;

      case CMD_BARO_TEMP_READ_CH:
        ////Read DHT Humidity and Temperature sensor
        readBaro(0);
        break;


      case CMD_DHT_HUMID_READ_CH:
        ////Read DHT Humidity and Temperature sensor
        readDHT(1);
        break;

      case CMD_BARO_PRESS_READ_CH:
        ////Read DHT Humidity and Temperature sensor
        readBaro(1);
        break;

      case CMD_TILDA_ESC_CH:
        //Display ~ and jump out of command mode
        // i.e. ~~ will in normal text will display ~
        lcd.print(ch);
        Serial.print(ch);
        commandMode = false;
        break;
//      case CMD_COMMAND_OFF_CH:
//        //Disable command mode
//        commandMode = false;
//        break;
//      default:
//        //Disable command mode
//        commandMode = false;
//        break;
    }
    commandMode = false;
  }
  else
  {
    if (ch == CMD_COMMAND_ON_CH)
      commandMode = true;
    else
    {
      lcd.print(ch);
      //Serial.print(ch);
    }

  }
}



