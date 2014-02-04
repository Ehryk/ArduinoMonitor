using System;
using System.Configuration;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Xml;
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

        private static readonly System.Reflection.Assembly currentAssembly = Assembly.GetAssembly(typeof(Monitor));
        private static readonly string assemblyFile = currentAssembly.Location;
        private static readonly string assemblyDirectory = Path.GetDirectoryName(currentAssembly.Location);
        private string[] configPaths = new string[] { String.Format("{0}.config", assemblyFile), String.Format("{0}{1}app.config", assemblyDirectory, Path.DirectorySeparatorChar), "app.config" };
        private string configFile;

        private XmlDocument configXML = new XmlDocument();
        private ConnectionStringSettingsCollection connectionStrings = new ConnectionStringSettingsCollection();
        private KeyValueConfigurationCollection appSettings = new KeyValueConfigurationCollection();

        //Public
        public int ArduinoID; //Database ID of the connected Arduino

        public float TempCelsius;    //Last Celsius Reading
        public float TempFahrenheit; //Last Fahrenheit Reading
        public float Humidity;       //Last Humidity Sensor Reading
        public float Light;          //Last Light Sensor Reading

        public DateTime LastUpdate;   //Last Update from the Arduino
        public DateTime LastLog;      //Last Log Write
        public DateTime? OutOfBounds; //Time when the temperature crossed the threshold

        public bool EnableEmail;         //Enable or Disable sending of Email
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
        public TimeSpan EmailHysteresis; //How Long it must be low before an email will be sent, in minutes
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
            LogStatus(String.Format("ArduinoMonitor Started: Arduino {0}.", ArduinoID), EventType.ApplicationStart);
            
            logTimer = new Timer(LogSensorData, null, InitializeWait, LogInterval);
            checkTimer = new Timer(CheckThreshold, null, InitializeWait, CheckInterval);

            return true;
        }

        public bool Continue()
        {
            //Log Continue
            LogStatus(String.Format("ArduinoMonitor Continued: Arduino {0}.", ArduinoID), EventType.ApplicationContinue);
            
            //logTimer = new Timer(LogToFile, null, initializeWait, logInterval);
            checkTimer = new Timer(CheckThreshold, null, InitializeWait, CheckInterval);

            return true;
        }

        public bool Pause()
        {
            //Log Pause
            LogStatus(String.Format("ArduinoMonitor Paused: Arduino {0}.", ArduinoID), EventType.ApplicationPause);

            //logTimer.Dispose();
            checkTimer.Dispose();

            return true;
        }

        public bool Stop()
        {
            //Log Stop
            LogStatus(String.Format("ArduinoMonitor Stopped: Arduino {0}.", ArduinoID), EventType.ApplicationStop);

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
            ConsoleStatus("Initialization Starting...");

            database = new SQLServer();
            string logPath = "ArduinoMonitor.log";

            //Set Application Defaults
            ArduinoID = 1;
            LogToFile = true;
            LogToDatabase = true;
            RetryEmailOnFailure = true;
            database.ConnectionString = "Data Source=MSP-SQLSERVER;Initial Catalog=ArduinoMonitor;User ID=ArduinoMonitor;Password=password;";

            LowThreshold = 62;
            HighThreshold = 80;
            EmailHysteresis = new TimeSpan(0, 5, 0);

            string portName = "COM4";
            int portBaud = 9600;
            
            if (UseConfigurationFile && LoadConfigurationFile())
            {
                try
                {
                    ArduinoID = int.Parse(appSettings["Arduino_ID"].Value);
                    database.ConnectionString = connectionStrings["ArduinoMonitor"].ConnectionString;

                    //COM Port
                    portName = appSettings["COM_Port"].Value;
                    portBaud = int.Parse(appSettings["COM_BaudRate"].Value);
                    SendDTR = bool.Parse(appSettings["COM_SendDTR"].Value);

                    //Logging
                    LogToDatabase = bool.Parse(appSettings["Log_ToDatabase"].Value);
                    LogToFile = bool.Parse(appSettings["Log_ToFile"].Value);
                    logPath = appSettings["Log_FileName"].Value;
                    LogInterval = new TimeSpan(0, 0, int.Parse(appSettings["Log_Interval_s"].Value));
                    InitializeWait = new TimeSpan(0, 0, int.Parse(appSettings["Log_Initialize_Wait_s"].Value));

                    //Threshold
                    CheckInterval = new TimeSpan(0, 0, int.Parse(appSettings["Check_Interval_s"].Value));
                    LowThreshold = float.Parse(appSettings["Low_Threshold"].Value);
                    HighThreshold = float.Parse(appSettings["High_Threshold"].Value);

                    //Email
                    EnableEmail = bool.Parse(appSettings["Email_Enable"].Value);
                    Email.FromAddress = appSettings["Email_From"].Value;
                    Email.SMTPServer = appSettings["Email_SMTP_Server"].Value;
                    Email.SMTPPort = int.Parse(appSettings["Email_SMTP_Port"].Value);
                    Recipients = appSettings["Email_Recipients"].Value;
                    EmailHysteresis = new TimeSpan(0, int.Parse(appSettings["Email_Hysteresis_m"].Value), 0);
                    RetryEmailOnFailure = bool.Parse(appSettings["Email_RetryOnFailure"].Value);
                }
                catch (Exception ex)
                {
                    ConsoleError("Could not load configuration settings: {0}", ex.Message);
                }
            }

            if (LogToFile)
            {
                //Open and Configure Log File
                logFile = File.AppendText(logPath);
                logFile.AutoFlush = true;
            }

            //Configure and Open Serial Port
            System.ComponentModel.IContainer components = new System.ComponentModel.Container();
            serialPort = new SerialPort(components) {PortName = portName, BaudRate = portBaud};

            serialPort.Open();
            if (!serialPort.IsOpen)
            {
                LogError(String.Format("Cannot open Serial Port {0}", portName));

                return;
            }

            //When true, resets the Arduino when application begins.
            serialPort.DtrEnable = SendDTR;
            //Callback for data from the Arduino
            serialPort.DataReceived += DataReceived;

            //Successful Initialization
            LogStatus("Initialization Successful", EventType.Initialized);
        }

        public bool LoadConfigurationFile()
        {
            foreach (string path in configPaths.Where(File.Exists))
            {
                configFile = path;
                break;
            }

            FileStream fs = new FileStream(configFile, FileMode.Open);
            try
            {
                configXML.Load(fs);
            }
            catch (Exception ex)
            {
                ConsoleError("Could Not Read Configuration File: {0} ({1})", configFile, ex.Message);
                return false;
            }
            finally
            {
                fs.Close();
            }

            try
            {
                XmlNode node = configXML.GetElementsByTagName("appSettings")[0];
                XmlNodeList nodeList = node.SelectNodes("add");

                foreach (XmlNode row in nodeList)
                {
                    appSettings.Add(new KeyValueConfigurationElement(row.Attributes["key"].Value, row.Attributes["value"].Value));
                }

                node = configXML.GetElementsByTagName("connectionStrings")[0];
                nodeList = node.SelectNodes("add");

                foreach (XmlNode row in nodeList)
                {
                    connectionStrings.Add(new ConnectionStringSettings(row.Attributes["name"].Value, row.Attributes["connectionString"].Value));
                }
            }
            catch (Exception ex)
            {
                ConsoleError("Configuration File {0} could not be parsed to XML", configFile);
                return false;
            }

            return true;
        }

        public void LogSensorData(object state)
        {
            try
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

                ConsoleMessage("Wrote Sensor Data: {0:G}", DateTime.Now);
            }
            catch (Exception ex)
            {
                LogError(String.Format("Could not Log Sensor Data: {0}", ex.Message), ex);
            }
        }

        public void CheckThreshold(object state)
        {
            try
            {
                if (!IsLow && !IsHigh)
                {
                    if (EnableEmail && HasSentEmail)
                    {
                        if (Email.SendEmail("Temperature Restored - " + TempFahrenheit + "°F", EmailBody(), "RDI Twin Cities <RDI_TwinCities@resdat.com>", "Eric Menze <emenze@resdat.com>"))
                        {
                            Console.ForegroundColor = ConsoleColor.Cyan;

                            LogEmailSent(String.Format("Restored Email Sent, {0}°F", TempFahrenheit));
                        }
                    }

                    OutOfBounds = null;
                    HasSentEmail = false;
                }
                else if (IsLow)
                {
                    OutOfBounds = OutOfBounds ?? DateTime.Now;

                    if (EnableEmail && HasSentEmail && DateTime.Now - OutOfBounds > EmailHysteresis)
                    {
                        if (Email.SendEmail("It's cold - " + TempFahrenheit + "°F!", EmailBody(), "RDI Twin Cities <RDI_TwinCities@resdat.com>", "Eric Menze <emenze@resdat.com>"))
                        {
                            HasSentEmail = true;

                            LogEmailSent(String.Format("Low Email Sent, {0}°F", TempFahrenheit));
                        }
                        else if (!RetryEmailOnFailure)
                        {
                            HasSentEmail = true;
                        }
                    }
                }
                else if (IsHigh)
                {
                    OutOfBounds = OutOfBounds ?? DateTime.Now;

                    if (EnableEmail && !HasSentEmail && DateTime.Now - OutOfBounds > EmailHysteresis)
                    {
                        if (Email.SendEmail("It's hot - " + TempFahrenheit + "°F!", EmailBody(), "RDI Twin Cities <RDI_TwinCities@resdat.com>", "Eric Menze <emenze@resdat.com>"))
                        {
                            HasSentEmail = true;

                            LogEmailSent(String.Format("High Email Sent, {0}°F", TempFahrenheit));
                        }
                        else if (!RetryEmailOnFailure)
                        {
                            HasSentEmail = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(String.Format("Checking Threshold: {0}", ex.Message), ex);
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

        private string EmailBody()
        {
            string body = "This email was autogenerated from an <a href='https://github.com/Ehryk/ArduinoMonitor'>ArduinoMonitor</a>.<br/><br/>";
            body += String.Format("Low Threshold: {0}°F<br/>", LowThreshold);
            body += String.Format("High Threshold: {0}°F<br/>", HighThreshold);

            return body;
        }

        #endregion

        #region Events

        private void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string line = null;

            try
            {
                line = serialPort.ReadLine();
            }
            catch (Exception ex)
            {
                LogError(String.Format("Could not read from Serial Port: {0}", ex.Message), ex);
            }

            if (line == null) return;

            try
            {
                bool containedData = ProcessLine(line);
            }
            catch (Exception ex)
            {
                LogError(String.Format("Could not process line <{0}>: {1}", line, ex.Message), ex);
            }
        }

        #endregion

        #region Console and Logging

        private void ConsoleDefault(string pMessage, params Object[] args)
        {
            ConsoleWrite(null, pMessage, args);
        }

        private void ConsoleMessage(string pMessage, params Object[] args)
        {
            ConsoleWrite(ConsoleColor.Green, pMessage, args);
        }

        private void ConsoleStatus(string pMessage, params Object[] args)
        {
            ConsoleWrite(ConsoleColor.Yellow, pMessage, args);
        }

        private void ConsoleError(string pMessage, params Object[] args)
        {
            ConsoleWrite(ConsoleColor.Red, pMessage, args);
        }

        private void ConsoleWrite(ConsoleColor? color, string pMessage, params Object[] args)
        {
            if (color == null)
                Console.ResetColor();
            else
                Console.ForegroundColor = color.Value;
            Console.WriteLine(pMessage, args);
            Console.ResetColor();
        }

        private void LogError(string pMessage, Exception pException = null, int? pArduinoID = null)
        {
            Log(pMessage, EventType.Error, pArduinoID, ConsoleColor.Red, pException);
        }

        private void LogEmailSent(string pMessage, int? pArduinoID = null)
        {
            Log(pMessage, EventType.EmailSent, pArduinoID, ConsoleColor.Cyan);
        }

        private void LogEmailFailed(string pMessage, Exception pException = null, int? pArduinoID = null)
        {
            Log(pMessage, EventType.EmailFailure, pArduinoID, ConsoleColor.Red, pException);
        }

        private void LogStatus(string pMessage, EventType pEventType, int? pArduinoID = null)
        {
            Log(pMessage, pEventType, pArduinoID, ConsoleColor.Yellow);
        }

        private void Log(string pMessage, EventType pEventType, int? pArduinoID = null, ConsoleColor color = ConsoleColor.White, Exception pException = null, DateTime? pDate = null, string pSource = "ArduinoMonitor")
        {
            try
            {
                Console.ForegroundColor = color;
                Console.WriteLine(pMessage);
                Console.ResetColor();

                if (LogToFile)
                    logFile.WriteLine("{0}{1} - {2:G}", pException == null ? "" : "Error - ", pMessage, pDate ?? DateTime.Now);
                if (LogToDatabase)
                {
                    if (pException == null)
                        database.InsertEvent(pArduinoID ?? ArduinoID, pMessage, pEventType, false, null, null, pDate ?? DateTime.Now, pSource);
                    else
                        database.InsertEvent(pArduinoID ?? ArduinoID, pMessage, pEventType, true, pException.Message, pException.StackTrace, pDate ?? DateTime.Now, pSource);
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Could not log error: {0}", ex.Message);
                Console.ResetColor();
            }
        }

        #endregion
    }
}