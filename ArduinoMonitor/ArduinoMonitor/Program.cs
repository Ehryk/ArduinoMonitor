using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Topshelf;
using ArduinoMonitor.Utilities;

namespace ArduinoMonitor
{
    class Program
    {
        static void Main(string[] args)
        {
            // Set the current directory so that things work as expected when running as a service.
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            try
            {
                //Expand Buffer to review history
                Console.BufferHeight = 8000;
                Console.Title = String.Format("{0} v{1}", ApplicationInfo.ProductName, ApplicationInfo.Version);
            }
            catch (Exception e)
            {
                //Probably not running as a console application
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e.Message);
            }
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(" --- {0} v{1}, {2} ---", ApplicationInfo.ProductName, ApplicationInfo.Version, ApplicationInfo.CompanyName);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
 
            //var configuration = Configuration.FromFile("Configuration.xml");
            
            //// Configure log4net to use our decrypted database connection.
            //XmlConfigurator.Configure();
            //var loggingConnection = configuration.Connections.First(c => c.Id == "AKGEOSPT");
 
            //var hierarchy = (Hierarchy)LogManager.GetRepository();
            //var appender = (AdoNetAppender)hierarchy.Root.GetAppender("AdoNetAppender_Oracle");
            //appender.ConnectionString = loggingConnection.OracleConnectionString;
            //appender.ActivateOptions();
 
            //// Set the global properties for logging across the application.
            //GlobalContext.Properties["context"] = "MarineExchangeFeedService";
            //GlobalContext.Properties["user"] = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
 
            // Set up the topshelf configuration.
            HostFactory.Run(hf =>
            {
                hf.Service<Monitor>(s =>
                {
                    s.ConstructUsing(name => new Monitor());
                    s.WhenStarted(me => me.Start());
                    s.WhenContinued(me => me.Continue());
                    s.WhenPaused(me => me.Pause());
                    s.WhenStopped(me => me.Stop());
                });
 
                hf.RunAsLocalSystem();

                hf.EnablePauseAndContinue();

                hf.StartAutomatically();
 
                hf.SetServiceName("ArduinoMonitor");
                hf.SetDisplayName("Arduino Monitor");

                hf.SetDescription("Monitors the output of an Arduino via a serial port.");
            });

            #if DEBUG
            try
            {
                Console.ResetColor();
                Console.WriteLine();
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
            catch (Exception e)
            {
                //Probably not running as a console application
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e.Message);
            }
            #endif
        }
    }
}
