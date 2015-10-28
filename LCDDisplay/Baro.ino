#include <Wire.h>
#include <BaroSensor.h>
#include "baro.h"
#include "LCDCommands.h"

// Freetronics unit used:
// http://www.freetronics.com.au/products/barometric-pressure-sensor-module#.VhxDqI2hdMs
// Connectivity: http://www.freetronics.com.au/pages/barometric-pressure-sensor-module-quickstart-guide#.VhxD242hdMs

void initBaro()
{
  BaroSensor.begin();
}

void readBaro(int ver)
{
  if(!BaroSensor.isOK()) {
    Serial.print("Sensor not Found/OK. Error: "); 
    Serial.print(BaroSensor.getError());
    BaroSensor.begin(); // Try to reinitialise the sensor if we can
  }
  else {
    //Serial.print("Temperature: "); 
    if (ver==0)
    {
      Serial.print(CMD_BARO_TEMP_READ_CH);
      Serial.print(BaroSensor.getTemperature());
      Serial.print(CMD_DATA_TERMINATOR_CH);
    }
    //Serial.print (',');
    //Serial.print(" %\t");
    //Serial.print("Pressure:    ");
    else if (ver==1)
    {
      Serial.print(CMD_BARO_PRESS_READ_CH);
      Serial.print(BaroSensor.getPressure());
      Serial.print(CMD_DATA_TERMINATOR_CH);
    }
  }
}
