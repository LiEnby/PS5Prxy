using System;
using Fiddler;
using System.IO;
using System.Runtime.InteropServices;

namespace PS5Prxy
{
    class Program
    {

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleCtrlHandler(ConsoleEventDelegate callback, bool add);

        static ConsoleEventDelegate handler;   // Keeps it from getting garbage collected
        private delegate bool ConsoleEventDelegate(int eventType);
        static string UrlFilter;
        static bool ConsoleEventCallback(int eventType)
        {
            if (eventType == 2) // WM_CLOSE
            {
                if(FiddlerApplication.IsStarted())
                {
                    Console.WriteLine("Stopping proxy...");
                    FiddlerApplication.Shutdown();
                }
            }
            return false;
        }

        static void Main(string[] args)
        {
            handler = new ConsoleEventDelegate(ConsoleEventCallback);
            SetConsoleCtrlHandler(handler, true);

            UrlFilter = "manuals.playstation.net";

            Console.WriteLine("PS5Prxy running on port 8080");
            FiddlerCoreStartupSettingsBuilder builder = new FiddlerCoreStartupSettingsBuilder();
            builder.ListenOnPort(8080);
            builder.AllowRemoteClients();
            FiddlerCoreStartupSettings settings = builder.Build();
            FiddlerApplication.BeforeRequest += FiddlerApplication_BeforeRequest;
            FiddlerApplication.Startup(settings);

            while (true) { };
        }

        private static void FiddlerApplication_BeforeRequest(Session oSession)
        {
            if(oSession.fullUrl.Contains(UrlFilter))
            {
                Console.WriteLine("Sending ps5.html... to "+oSession.clientIP);
                if (oSession.HTTPMethodIs("CONNECT"))
                {
                    oSession["x-replywithtunnel"] = "Hello Sony :3";
                    return;
                }

                oSession.utilCreateResponseAndBypassServer();
                string htmlContents = File.ReadAllText("ps5.html");
                oSession.utilSetResponseBody(htmlContents);
                oSession.responseCode = 200;
            }
        }

    }
}
