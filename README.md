# Arduino Monitor
This is a set of tools to output sensor (or other) data from an arduino, graph it with Processing, and log it with a C# Console Application to a file, a database, or both. The Console application is TopShelf enabled so it can be installed as a Windows Service.

### General Requirements

 - To upload the Arduino Sketch, [Arduino IDE](http://arduino.cc/en/main/software)
 - To run the Processing Sketch, [Processing](https://processing.org/download/)
 - To run the console application, Windows with .NET
 - To log to a database, a SQL Server install (Express would work), or a database to connect to

### Sample Output

![Serial Output](https://raw2.github.com/Ehryk/ArduinoMonitor/master/Documentation/SampleSerial.png)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
![Graph](https://raw2.github.com/Ehryk/ArduinoMonitor/master/Documentation/SampleGraph.png)
![Console Application](https://raw2.github.com/Ehryk/ArduinoMonitor/master/Documentation/SampleOutput.png)

### Setup

1. Add sensors to your Arduino
1. Modify the sketch to read from them and output the data over the serial port
1. Upload the Arduino Sketch
1. Launch the Graph processing sketch to verify operation
1. Create the Database and tables from the scripts (optional)
1. Set up the Database connection string (optional)
1. Run the ArduinoMonitor console application to start logging
1. Install the ArduinoMonitor as a windows service (optional)

### Known issues
