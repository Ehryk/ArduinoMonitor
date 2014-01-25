using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Ports;
using System.Threading;
using ArduinoMonitor.DataAccess;

namespace ArduinoMonitor
{
    class Monitor
    {
        #region Properties

        //Private
        private SerialPort serialPort; //Port for Arduino Communication
        private StreamWriter logFile;  //File to write to
        private SQLServer database;    //Database to connect to
        
        private Timer logTimer;   //Writes to the enabled logs (file, db)
        private Timer checkTimer; //Checks that the temperature is within bounds

        //Public
        public int ArduinoID; //Database ID of the connected Arduino

        public float TempCelsius;    //Last Celsius Reading
        public float TempFahrenheit; //Last Fahrenheit Reading
        public float Humidity;       //Last Humidity Sensor Reading
        public float Light;          //Last Light Sensor Reading

        public DateTime LastUpdate;   //Last Update from the Arduino
        public DateTime LastLog;      //Last Log Write
        public DateTime? OutOfBounds; //Time when the temperature crossed the threshold

        public bool RetryEmailOnFailure; //If sending fails, retry until success or return to bounds?
        public bool LogToFile;           //Enables writing to the log file
        public bool LogToDatabase;       //Enables logging to the database
        public bool UseConfigurationFile = true; //Enables the configuration file to overwrite application defaults
        public bool SendDTR = true;      //DTR Resets the Arduino when the Application starts listening

        public TimeSpan InitializeWait = new TimeSpan(0, 0, 10); //Delay before first log write after initialization
        public TimeSpan LogInterval    = new TimeSpan(0, 1, 0);  //Time in between log writes
        public TimeSpan CheckInterval  = new TimeSpan(0, 0, 10); //Time in between bounds checks
        
        public float LowThreshold;  //Temperature considered 'low', in Fahrenheit
        public float HighThreshold; //Temperature considered 'high', in Fahrenheit
        public TimeSpan EmailDelay; //How Long it must be low before an email will be sent, in minutes
        public bool HasSentEmail;   //If an email has already been sent for current out-of-bounds
        public string Recipients = "";
        
        //Computed
        public bool IsLow
        {
            get { return DateTime.Now - LastUpdate < new TimeSpan(0, 1, 0) && TempFahrenheit < LowThreshold; }
        }

        public bool IsHigh
        {
            get { return DateTime.Now - LastUpdate < new TimeSpan(0, 1, 0) && TempFahrenheit > HighThreshold; }
        }

        #endregion

        #region Constructors

        public Monitor(bool pUseConfigurationFile = true)
        {
            UseConfigurationFile = pUseConfigurationFile;
        }

        #endregion

        #region Service Events

        public bool Start()
        {
            Initialize();

            //Log Start
            if (LogToFile)
                logFile.WriteLine("ArduinoMonitor Started {0:G}", DateTime.Now);
            if (LogToDatabase)
                database.InsertEvent(ArduinoID, "ArduinoMonitor Started.", EventType.ApplicationStart);
            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("ArduinoMonitor Started.");
            Console.ResetColor();
            
            logTimer = new Timer(LogSensorData, null, InitializeWait, LogInterval);
            checkTimer = new Timer(CheckLow, null, InitializeWait, CheckInterval);

            return true;
        }

        public bool Continue()
        {
            //Log Continue
            if (LogToFile)
                logFile.WriteLine("ArduinoMonitor Continued {0:G}", DateTime.Now);
            if (LogToDatabase)
                database.InsertEvent(ArduinoID, "ArduinoMonitor Continued.", EventType.ApplicationContinue);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("ArduinoMonitor Continued.");
            Console.ResetColor();
            
            //logTimer = new Timer(LogToFile, null, initializeWait, logInterval);
            checkTimer = new Timer(CheckLow, null, InitializeWait, CheckInterval);

            return true;
        }

        public bool Pause()
        {
            //Log Pause
            if (LogToFile)
                logFile.WriteLine("ArduinoMonitor Paused {0:G}", DateTime.Now);
            if (LogToDatabase)
                database.InsertEvent(ArduinoID, "ArduinoMonitor Paused.", EventType.ApplicationPause);

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine();
            Console.WriteLine("ArduinoMonitor Paused.");
            Console.ResetColor();

            //logTimer.Dispose();
            checkTimer.Dispose();

            return true;
        }

        public bool Stop()
        {
            //Log Stop
            if (LogToFile)
                logFile.WriteLine("ArduinoMonitor Stopped {0:G}", DateTime.Now);
            if (LogToDatabase)
                database.InsertEvent(ArduinoID, "ArduinoMonitor Stopped.", EventType.ApplicationStop);

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine();
            Console.WriteLine("ArduinoMonitor Stopped.");
            Console.ResetColor();

            logTimer.Dispose();
            checkTimer.Dispose();

            serialPort.Dispose();
            logFile.Dispose();
            database.Dispose();

            return true;
        }

        #endregion

        #region Methods

        public void Initialize()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Initialization Starting...");
            Console.ResetColor();

            database = new SQLServer();

            //Set Application Defaults
            ArduinoID = 1;
            LogToFile = true;
            LogToDatabase = true;
            RetryEmailOnFailure = true;
            database.ConnectionString = "Data Source=localhost;Initial Catalog=ArduinoMonitor;User ID=ArduinoMonitor;Password=password;";

            LowThreshold = 60;
            HighThreshold = 80;
            EmailDelay = new TimeSpan(0, 1, 0);

            string port = "4";
            string portName = "COM" + port;
            int portBaud = 9600;
            
            if (UseConfigurationFile)
            {
                
            }

            if (LogToFile)
            {
                //Open and Configure Log File
                logFile = File.AppendText("ArduinoMonitor.log");
                logFile.AutoFlush = true;
            }

            //Configure and Open Serial Port
            System.ComponentModel.IContainer components = new System.ComponentModel.Container();
            serialPort = new SerialPort(components) {PortName = portName, BaudRate = portBaud};

            serialPort.Open();
            if (!serialPort.IsOpen)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Cannot open Serial Port COM" + port);
                Console.ResetColor();

                if (LogToFile)
                    logFile.WriteLine("Error - Cannot open Serial Port COM" + port);
                if (LogToDatabase)
                    database.InsertEvent(ArduinoID, "Cannot open Serial Port on COM" + port, EventType.Error);

                return;
            }

            //When true, resets the Arduino when application begins.
            serialPort.DtrEnable = SendDTR;
            //Callback for data from the Arduino
            serialPort.DataReceived += DataReceived;

            //Successful Initialization
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Initialization Success!");
            Console.ResetColor();
            if (LogToFile)
                logFile.WriteLine("Initialization Successful {0:G}", DateTime.Now);
            if (LogToDatabase)
                database.InsertEvent(ArduinoID, "Initialization Successful.", EventType.Initialized);
        }

        public void LogSensorData(object state)
        {
            if (LogToFile)
            {
                logFile.Write(String.Format("{0:G}\t{1}°C\t{2}°F\t{3}% Humidity", DateTime.Now, TempCelsius, TempFahrenheit, Humidity));
                logFile.Write(IsLow ? " - LOW" : "");
                logFile.Write(IsHigh ? " - HIGH" : "");
                logFile.WriteLine();
            }
            if (LogToDatabase)
            {
                database.InsertSensorData(ArduinoID, TempCelsius, TempFahrenheit, Humidity);
                if (IsLow) database.InsertEvent(ArduinoID, String.Format("Temperature Below Threshold. Temperature: {0}°F, Threshold {1}°F", TempFahrenheit, LowThreshold), EventType.LowThresholdCrossed);
                if (IsHigh) database.InsertEvent(ArduinoID, String.Format("Temperature Above Threshold. Temperature: {0}°F, Threshold {1}°F", TempCelsius, HighThreshold), EventType.HighThresholdCrossed);
            }

            LastLog = DateTime.Now;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Wrote Sensor Data: {0:G}", DateTime.Now);
            Console.ResetColor();
        }

        public void CheckLow(object state)
        {
            if (!IsLow && !IsHigh)
            {
                OutOfBounds = null;
                HasSentEmail = false;
            }
            else
            {
                OutOfBounds = OutOfBounds ?? DateTime.Now;

                if (!HasSentEmail && DateTime.Now - OutOfBounds > EmailDelay)
                {
                    if (Email.SendEmail(TempFahrenheit))
                    {
                        HasSentEmail = true;

                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine("Email Sent, {1}°F - {0:G}", DateTime.Now, TempFahrenheit);
                        Console.ResetColor();
                        
                        if (LogToFile)
                            logFile.WriteLine("Email Sent, {1}°F - {0:G}", DateTime.Now, TempFahrenheit);
                        if (LogToDatabase)
                            database.InsertEvent(ArduinoID, String.Format("Email Sent, {1}°F - {0:G}", DateTime.Now, TempFahrenheit), EventType.EmailSent);
                    }
                    else if (!RetryEmailOnFailure)
                    {
                        HasSentEmail = true;
                    }
                }
            }
        }

        private bool ProcessLine(string line)
        {
            bool containsData = false;

            Console.Write(line.Trim(new [] {'\r', '\n'}));
            
            if (line.Contains("|"))
            {
                containsData = true;

                String[] values = line.Split('|');
                TempCelsius = float.Parse(values[0]);
                TempFahrenheit = float.Parse(values[1]);
                Humidity = float.Parse(values[2]);
                Light = float.Parse(values[3]);

                LastUpdate = DateTime.Now;

                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(IsLow ? " - LOW" : "");
                Console.Write(IsHigh ? " - HIGH" : "");
                Console.WriteLine();
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine();
            }

            return containsData;
        }

        #endregion

        #region Events

        private void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string line = serialPort.ReadLine();

            try
            {
                ProcessLine(line);
            }
            catch (Exception ex)
            {
                if (LogToFile)
                    logFile.WriteLine("Error - Could not process line '{0}'", line);
                if (LogToDatabase)
                    database.InsertEvent(ArduinoID, String.Format("Could Not Process Input: <{0}>", line), EventType.Error, true, ex.Message, ex.StackTrace);
            }
        }

        #endregion
    }
}