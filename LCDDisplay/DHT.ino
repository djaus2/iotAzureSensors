// Example testing sketch for various DHT humidity/temperature sensors
// Written by ladyada, public domain

#include "DHT.h"

#define DHTPIN 2     // what pin we're connected to

// Humidity Temp Links (Freetronics):
// http://www.freetronics.com.au/products/humidity-and-temperature-sensor-module#.VhxESI2hdMs
// Connectivity: http://www.freetronics.com.au/pages/humid-humidity-temperature-sensor-module-quickstart-guide#.VhxEb42hdMs



DHT dht(DHTPIN, DHTTYPE);

void initDHT() {
  dht.begin();
}

void readDHT(int ver) {
  // Reading temperature or humidity takes about 250 milliseconds!
  // Sensor readings may also be up to 2 seconds 'old' (its a very slow sensor)
  float h = dht.readHumidity();
  float t = dht.readTemperature();

  // check if returns are valid, if they are NaN (not a number) then something went wrong!
  if (isnan(t) || isnan(h)) {
    Serial.println("Failed to read from DHT");
  } else {
    //Serial.print("Humidity: ");
    if (ver==1)
    {
      Serial.print(CMD_DHT_HUMID_READ_CH);
      Serial.print(h);
      Serial.print(CMD_DATA_TERMINATOR_CH);
    }
    //Serial.print(" %\t");
    //Serial.print(',');
    //Serial.print("Temperature: ");
    else if (ver==0)
    {
      Serial.print(CMD_DHT_TEMP_READ_CH);
      Serial.print(t);
      Serial.print(CMD_DATA_TERMINATOR_CH);
    }
    //Serial.println(" *C");
  }
}
