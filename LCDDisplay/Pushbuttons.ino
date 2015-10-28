#include "Pushbuttons.h"

/* The following code is derived from:
   Example code for the Freetronics LCD & Keypad Shield:

     http://www.freetronics.com/products/lcd-keypad-shield

   by Marc Alexander, 7 September 2011
http://www.freetronics.com.au/pages/16x2-lcd-shield-quickstart-guide#.VciFhDYVjIU
*/

// Pins in use
#define BUTTON_ADC_PIN           A0  // A0 is the button ADC input
#define LCD_BACKLIGHT_PIN         3  // D3 controls LCD backlight

// ADC readings expected for the 5 buttons on the ADC input
#define RIGHT_10BIT_ADC           0  // right
#define UP_10BIT_ADC            145  // up
#define DOWN_10BIT_ADC          329  // down
#define LEFT_10BIT_ADC          505  // left
#define SELECT_10BIT_ADC        741  // right
#define BUTTONHYSTERESIS         10  // hysteresis for valid button sensing window //return values for ReadButtons()

#define BUTTON_NONE               0  // 
#define BUTTON_RIGHT              1  // 
#define BUTTON_UP                 2  // 
#define BUTTON_DOWN               3  // 
#define BUTTON_LEFT               4  // 
#define BUTTON_SELECT             5  // 

byte buttonJustPressed  = false;         //this will be true after a ReadButtons() call if triggered
byte buttonJustReleased = false;         //this will be true after a ReadButtons() call if triggered
byte buttonWas          = BUTTON_NONE;   //used by ReadButtons() for detection of button events

#define LCD_BACKLIGHT_OFF()     digitalWrite( LCD_BACKLIGHT_PIN, LOW )
#define LCD_BACKLIGHT_ON()      digitalWrite( LCD_BACKLIGHT_PIN, HIGH )
#define LCD_BACKLIGHT(state)    { if( state ){digitalWrite( LCD_BACKLIGHT_PIN, HIGH );}else{digitalWrite( LCD_BACKLIGHT_PIN, LOW );} }

void Backlight_On()
{
  LCD_BACKLIGHT_ON();
}

void Backlight_Off()
{
  LCD_BACKLIGHT_OFF();
}


char key = BUTTON_NONE_CHAR;

//The algoithm here was superceded by the ReadButtons() algorithm
void getKeyboard()
{
  byte button;
  char newKey;
  button = ReadButtons();
  switch ( button )
  {
    case BUTTON_NONE:
        key = BUTTON_NONE_CHAR;
        return;
        break;
    case BUTTON_RIGHT:
        newKey = BUTTON_RIGHT_CHAR;
        break;
    case BUTTON_UP:
        newKey = BUTTON_UP_CHAR;
        break;
    case BUTTON_DOWN:
        newKey = BUTTON_DOWN_CHAR;
        break;
    case BUTTON_LEFT:
        newKey = BUTTON_LEFT_CHAR;
        break;
    case BUTTON_SELECT:
        newKey = BUTTON_SELECT_CHAR;
        break;
  }
  if (newKey != key)
  {
    key = newKey;
    Serial.print(key);
  }

  //clear the buttonJustPressed or buttonJustReleased flags, they've already done their job now.
  if ( buttonJustPressed )
    buttonJustPressed = false;
  if ( buttonJustReleased )
    buttonJustReleased = false;

}

void initButtons()
{
  //button adc input
  pinMode( BUTTON_ADC_PIN, INPUT );         //ensure A0 is an input
  digitalWrite( BUTTON_ADC_PIN, LOW );      //ensure pullup is off on A0
  //lcd backlight control
  digitalWrite( LCD_BACKLIGHT_PIN, HIGH );  //backlight control pin D3 is high (on)
  pinMode( LCD_BACKLIGHT_PIN, OUTPUT );     //D3 is an output
}


/*--------------------------------------------------------------------------------------
  ReadButtons()
  Detect the button pressed and return the value
  Uses global values buttonWas, buttonJustPressed, buttonJustReleased.
  --------------------------------------------------------------------------------------*/
byte ReadButtons()
{
  unsigned int buttonVoltage;
  byte button = BUTTON_NONE;   // return no button pressed if the below checks don't write to btn

  //read the button ADC pin voltage
  buttonVoltage = analogRead( BUTTON_ADC_PIN );
  if (buttonVoltage<1000)
   {

   
  //sense if the voltage falls within valid voltage windows
  if ( buttonVoltage < ( RIGHT_10BIT_ADC + BUTTONHYSTERESIS ) )
  {
    button = BUTTON_RIGHT;
  }
  else if (   buttonVoltage >= ( UP_10BIT_ADC - BUTTONHYSTERESIS )
              && buttonVoltage <= ( UP_10BIT_ADC + BUTTONHYSTERESIS ) )
  {
    button = BUTTON_UP;
  }
  else if (   buttonVoltage >= ( DOWN_10BIT_ADC - BUTTONHYSTERESIS )
              && buttonVoltage <= ( DOWN_10BIT_ADC + BUTTONHYSTERESIS ) )
  {
    button = BUTTON_DOWN;
  }
  else if (   buttonVoltage >= ( LEFT_10BIT_ADC - BUTTONHYSTERESIS )
              && buttonVoltage <= ( LEFT_10BIT_ADC + BUTTONHYSTERESIS ) )
  {
    button = BUTTON_LEFT;
  }
  else if (   buttonVoltage >= ( SELECT_10BIT_ADC - BUTTONHYSTERESIS )
              && buttonVoltage <= ( SELECT_10BIT_ADC + BUTTONHYSTERESIS ) )
  {
    button = BUTTON_SELECT;
  }
}
  //handle button flags for just pressed and just released events
  if ( ( buttonWas == BUTTON_NONE ) && ( button != BUTTON_NONE ) )
  {
    //the button was just pressed, set buttonJustPressed, this can optionally be used to trigger a once-off action for a button press event
    //it's the duty of the receiver to clear these flags if it wants to detect a new button change event
    buttonJustPressed  = true;
    buttonJustReleased = false;
  }
  if ( ( buttonWas != BUTTON_NONE ) && ( button == BUTTON_NONE ) )
  {
    buttonJustPressed  = false;
    buttonJustReleased = true;
  }

  //save the latest button value, for change event detection next time round
  buttonWas = button;

  return ( button );
}
