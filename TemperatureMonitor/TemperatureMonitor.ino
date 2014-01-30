#include "DHT.h"

int sampleRate = 2500; //Milliseconds between sampling
float tCorrection = -14.0;

#define DHTPWR 7 //Power
#define DHTGND 4 //Ground
#define DHTPIN 6 //Data Pin (digital)

#define LM34PWR A0 //Power
#define LM34GND A2 //Ground
#define LM34PIN A1 //Data Pin (analog)

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
  pinMode(LM34PWR, OUTPUT);
  pinMode(LM34GND, OUTPUT);
  pinMode(LM34PIN, INPUT);
  
  Serial.begin(9600); 
  
  Serial.println("TemperatureMonitor");
  Serial.println("v0.2 Eric Menze 1.25.2014");
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
  
  float h = dht.readHumidity(); //Read Humidity from DHT22
  float tC = dht.readTemperature(); //Read Temperature (in Celsius) from DHT22
  //float tF = (lm34/1024.0) * (readVcc() - 500) / 10; //Convert the LM34 analog value to a temperature
  float tF = tC * 9 / 5 + 32;
  float l = 0; //Light Sensor, future use
  
  //float tArduino = readTemp()/1000.0 + tCorrection;
  float tArduino = GetTemp() + tCorrection;
  float vArduino = readVcc()/1000.0;

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
    Serial.print(vArduino);
    Serial.print("|");
    Serial.print(tArduino);
    Serial.print("|");
    Serial.print("(C,F,%,lm,Vcc,Ci)");
    Serial.println();
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

double GetTemp(void)
{
  unsigned int wADC;
  double t;

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
  return (t);
}
