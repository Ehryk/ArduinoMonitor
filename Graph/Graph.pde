//import Serial communication library
import processing.serial.*;

//init variables
Serial commPort;
float tempC;
float tempF;
float humidity;
float light;
int lf = 10;

int yDist;
PFont font12;
PFont font24;
float[] tempHistory = new float[100];

float graphMax = 32.4;
float graphMin = 10;

void setup()
{
  //setup fonts for use throughout the application
  font12 = loadFont("Verdana-12.vlw"); 
  font24 = loadFont("Verdana-24.vlw"); 
  
  //set the size of the window
  size(210, 200);
  
  //init serial communication port
  commPort = new Serial(this, "COM4", 9600);
  
  //fill tempHistory with default temps
  for(int index = 0; index<100; index++)
    tempHistory[index] = 0;
}

void draw()
{
  //get the temp from the serial port
  boolean refreshed = false;
  while (!refreshed) 
  {
    if (commPort.available() > 0)
    {
      String input = commPort.readStringUntil(lf);
      if (input != null && input.contains("|"))
      {
        String[] data = input.split("\\|");
        tempC = parseFloat(data[0]);
        tempF = parseFloat(data[1]);
        humidity = parseFloat(data[2]);
        light = parseFloat(data[3]);
        refreshed = true;
      }
    }
  }
  
  //refresh the background to clear old data
  background(123);
  
  //draw the temp rectangle
  colorMode(RGB, 160);  //use color mode sized for fading
  stroke (0);
  rect (9,19,22,162);
  //fade red and blue within the rectangle
  for (int colorIndex = 0; colorIndex <= 160; colorIndex++) 
  {
    stroke(160 - colorIndex, 0, colorIndex);
    line(10, colorIndex + 20, 30, colorIndex + 20);
  }
  
  //draw triangle pointer
  yDist = int(160 - (160 * (tempC * 0.01)));
  stroke(0);
  triangle(20, yDist + 20, 50, yDist + 15, 50, yDist + 25);
  
  //write reference values
  fill(0,0,0);
  textFont(font12); 
  textAlign(LEFT);
  text("212°F", 35, 25); 
  text("32°F", 35, 187);
  
  //draw graph
  stroke(0);
  fill(255,255,255);
  rect(100,80,100,100);
  stroke(0,60,0);
  float scale = 100/(graphMax-graphMin);
  float shift = scale*graphMin;
  for (int index = 0; index<100; index++)
  {  
    if(index == 99)
      tempHistory[index] = tempC;
    else
      tempHistory[index] = tempHistory[index + 1];
    
    float y = 180 - scale*tempHistory[index] + shift;
    y = min(180, y);
    y = max(80, y);
    point(100 + index, y); 
  }
  
  //write reference values
  fill(0,0,0);
  textFont(font12);
  textAlign(RIGHT);
  text(str(int(graphMax*9/5+32)) + "°F", 95, 85);
  textAlign(CENTER);
  text(str(int(graphMin*9/5+32)) + "°F", 155, 195);
  
  //write the temp in C and F
  stroke(0);
  fill(0,0,0);
  textFont(font24); 
  textAlign(LEFT);
  text(String.format("%.1f°C", tempC), 105, 65);
  //tempF = ((tempC*9)/5) + 32;
  text(String.format("%.1f°F", tempF), 105, 37);
}

void delay(int delay)
{
  int time = millis();
  while(millis() - time <= delay);
}
