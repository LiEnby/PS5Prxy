using System;
using Fiddler;
using System.IO;
using System.Text;

namespace PS5Prxy
{
    class Program
    {
        static void Main(string[] args)
        {

            string UrlFilter = "manuals.playstation.net";

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
            if(oSession.fullUrl.Contains("manuals.playstation.net"))
            {
                Console.WriteLine(oSession.clientIP+" requested "+oSession.fullUrl+".. redirecting ");
                oSession.utilCreateResponseAndBypassServer();

                MemoryStream ms = new MemoryStream();
                string response = "HTTP/1.1 301 Moved Temporarily\r\nCache-Control: no-cache\r\nServer: Silica\r\nLocation: http://ps5html.com\r\nDate: Fri, 13 Nov 2020 03:25:03 GMT\r\nContent-Length: 0\r\n\r\n";
                ms.Write(Encoding.UTF8.GetBytes(response), 0, Encoding.UTF8.GetBytes(response).Length);
                ms.Seek(0x00, SeekOrigin.Begin);

                oSession.LoadResponseFromStream(ms,"");

            }
            if(oSession.fullUrl.Contains("ps5html.com"))
            {
                Console.WriteLine(oSession.clientIP + " requested " + oSession.fullUrl + ".. sending ps5.html ");
                oSession.utilCreateResponseAndBypassServer();
                oSession.utilSetResponseBody(File.ReadAllText("ps5.html"));

            }
        }

    }
}
