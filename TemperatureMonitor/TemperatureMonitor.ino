#include "DHT.h"

int sampleRate = 3500; //Milliseconds between sampling

#define DHTPWR 7
#define DHTGND 4
#define DHTPIN 6     // what pin we're connected to

// Uncomment whatever type you're using!
//#define DHTTYPE DHT11   // MDHT 11 
#define DHTTYPE DHT22   // DHT 22  (AM2302)
//#define DHTTYPE DHT21   // DHT 21 (AM2301)

// Connect pin 1 (on the left) of the sensor to +5V
// Connect pin 2 of the sensor to whatever your DHTPIN is
// Connect pin 4 (on the right) of the sensor to GROUND
// Connect a 10K resistor from pin 2 (data) to pin 1 (power) of the sensor

DHT dht(DHTPIN, DHTTYPE);

void setup() {
  pinMode(DHTPWR, OUTPUT);
  pinMode(DHTGND, OUTPUT);
  digitalWrite(DHTPWR, HIGH);
  digitalWrite(DHTGND, LOW);
  
  Serial.begin(9600); 
  
  Serial.println("TemperatureMonitor");
  Serial.println("v0.2 Eric Menze 1.25.2014");
  Serial.println("Powering Sensor...");
  delay(2000);
  dht.begin();
  Serial.println("Beginning Read Loop...");
}

void loop() {
  // Reading temperature or humidity takes about 250 milliseconds!
  // Sensor readings may also be up to 2 seconds 'old' (its a very slow sensor)
  float h = dht.readHumidity();
  float tC = dht.readTemperature();
  float tF = tC * 9 / 5 + 32;
  float l = 0;

  // check if returns are valid, if they are NaN (not a number) then something went wrong!
  if (isnan(tC) || isnan(h)) {
    Serial.println(" - Failed to read from Sensor");
  } else {
    //Nicely Formatted
    //Serial.print("Humidity: "); 
    //Serial.print(h);
    //Serial.print("%\t");
    //Serial.print("Temperature: ");
    //Serial.print(tF);
    //Serial.write(176);
    //Serial.print("F ("); 
    //Serial.print(tC);
    //Serial.write(176);
    //Serial.print("C)");
    
    //Delimited by |
    Serial.print(tC);
    Serial.print("|");
    Serial.print(tF);
    Serial.print("|");
    Serial.print(h);
    Serial.print("|");
    Serial.print(l);
    Serial.print("|");
    Serial.print("(Celsius,Fahrenheit,% Humidity,Light)");
    Serial.println();
  }
  
  delay(sampleRate);
}
