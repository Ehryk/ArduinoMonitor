#include "DHT.h"

// Pin Definitions
#define DHTPWR 7 //Power
#define DHTPIN 6 //Data Pin (digital)
#define DHTGND 5 //Ground
#define LM34PWR A0 //Power
#define LM34GND A2 //Ground
#define LM34PIN A1 //Data Pin (analog)
#define LIGHTPIN A4 //Data Pin (analog)

// Variables
int sampleRate = 2500; //Milliseconds between sampling
bool debug = false;
float internalCorrection = +8.7;

// Uncomment whatever type you're using!
//#define DHTTYPE DHT11   // DHT 11 
//#define DHTTYPE DHT21   // DHT 21 (AM2301)
#define DHTTYPE DHT22   // DHT 22  (AM2302)

DHT dht(DHTPIN, DHTTYPE);

void setup() {
  pinMode(DHTPWR, OUTPUT);
  pinMode(DHTGND, OUTPUT);
  pinMode(DHTPIN, INPUT);
  
  pinMode(LM34PWR, OUTPUT);
  pinMode(LM34GND, OUTPUT);
  pinMode(LM34PIN, INPUT);
  
  pinMode(LIGHTPIN, INPUT);
  
  Serial.begin(57600); 
  
  Serial.println("TemperatureMonitor");
  Serial.println("v0.3 Eric Menze 2.11.2016");
  Serial.println("Powering Sensors...");
  
  digitalWrite(DHTPWR, HIGH);
  digitalWrite(DHTGND, LOW);
  
  digitalWrite(LM34PWR, HIGH);
  digitalWrite(LM34GND, LOW);
  
  delay(2000);
  dht.begin();
  
  Serial.println("Beginning Read Loop...");
}

void loop() {
  // Reading temperature or humidity takes about 250 milliseconds!
  // Sensor readings may also be up to 2 seconds 'old' (its a very slow sensor)
  
  float lm34 = analogRead(LM34PIN); //Read Analog Data from LM34
  float light = analogRead(LIGHTPIN); //Read Analog Data from Light Sensor
  
  float h = dht.readHumidity(); //Read Humidity from DHT22
  float tC = dht.readTemperature(); //Read Temperature (in Celsius) from DHT22
  //float tF = (lm34/1024.0) * (readVcc() - 500) / 10; //Convert the LM34 analog value to a temperature
  float tF = tC * 9 / 5 + 32;
  float l = (1024 - light) / 1024 * 100; //Light Sensor, future use
  
  float tArduino = readInternalTemp();
  float vArduino = readVcc()/1000.0;

  // check if returns are valid, if they are NaN (not a number) then something went wrong!
  if (isnan(tC) || isnan(h)) {
    Serial.println(" - Failed to read from Temperature Sensor");
  } else {
    if (debug) {
      //Nicely Formatted
      Serial.print("Temperature: ");
      Serial.print(tF);
      Serial.write(176);
      Serial.print("F ("); 
      Serial.print(tC);
      Serial.write(176);
      Serial.println("C)");
      
      Serial.print("Humidity: "); 
      Serial.print(h);
      Serial.println("%");
      
      Serial.print("Light: ");
      Serial.print(l);
      Serial.println("%");
      
      Serial.println();
    }
    else {
      //Delimited by |
      Serial.print(tC);
      Serial.print("|");
      Serial.print(tF);
      Serial.print("|");
      Serial.print(h);
      Serial.print("|");
      Serial.print(l);
      Serial.print("|");
      Serial.print(vArduino);
      Serial.print("|");
      Serial.print(tArduino);
      Serial.print("|");
      Serial.print("(C,F,%,lux,Vcc,Ci)");
      Serial.println();
    }
  }
  
  delay(sampleRate);
}

long readTemp() {
  long result;
  // Read temperature sensor against 1.1V reference
  ADMUX = _BV(REFS1) | _BV(REFS0) | _BV(MUX3);
  delay(2); // Wait for Vref to settle
  ADCSRA |= _BV(ADSC); // Convert
  while (bit_is_set(ADCSRA,ADSC));
  result = ADCL;
  result |= ADCH<<8;
  result = (result - 125) * 1075;
  return result;
}

long readVcc() {
  long result;
  // Read 1.1V reference against AVcc
  ADMUX = _BV(REFS0) | _BV(MUX3) | _BV(MUX2) | _BV(MUX1);
  delay(2); // Wait for Vref to settle
  ADCSRA |= _BV(ADSC); // Convert
  while (bit_is_set(ADCSRA,ADSC));
  result = ADCL;
  result |= ADCH<<8;
  result = 1126400L / result; // Back-calculate AVcc in mV
  return result;
}

float readInternalTemp()
{
  unsigned int wADC;
  float t;

  // The internal temperature has to be used
  // with the internal reference of 1.1V.
  // Channel 8 can not be selected with
  // the analogRead function yet.

  // Set the internal reference and mux.
  ADMUX = (_BV(REFS1) | _BV(REFS0) | _BV(MUX3));
  ADCSRA |= _BV(ADEN);  // enable the ADC

  delay(20);            // wait for voltages to become stable.

  ADCSRA |= _BV(ADSC);  // Start the ADC

  // Detect end-of-conversion
  while (bit_is_set(ADCSRA,ADSC));

  // Reading register "ADCW" takes care of how to read ADCL and ADCH.
  wADC = ADCW;

  // The offset of 324.31 could be wrong. It is just an indication.
  t = (wADC - 324.31 ) / 1.22;

  // The returned temperature is in degrees Celcius.
  return t + internalCorrection;
}

