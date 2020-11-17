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
                string response = "HTTP/1.1 301 Moved Temporarily\r\nCache-Control: no-cache\r\nServer: Silica\r\nLocation: http://vitatricks.tk/ps5.html\r\nDate: Fri, 13 Nov 2020 03:25:03 GMT\r\nContent-Length: 0\r\n\r\n";
                ms.Write(Encoding.UTF8.GetBytes(response), 0, Encoding.UTF8.GetBytes(response).Length);
                ms.Seek(0x00, SeekOrigin.Begin);

                oSession.LoadResponseFromStream(ms,"");

            }
            if(oSession.fullUrl.Contains("vitatricks.tk/ps5.html"))
            {
                Console.WriteLine(oSession.clientIP + " requested " + oSession.fullUrl + ".. sending ps5.html ");
                oSession.utilCreateResponseAndBypassServer();
                string body = File.ReadAllText("ps5.html");
                string headers = "HTTP/1.1 200 OK\r\nDate: Tue, 17 Nov 2020 04:27:07 GMT\r\nServer: Apache/2.4.29 (Ubuntu)\r\nLast-Modified: Tue, 25 Aug 2020 05:30:27 GMT\r\nETag: \"f0e-5adad002e4c67-gzip\"\r\nAccept-Ranges: bytes\r\nVary: Accept-Encoding\r\nContent-Length: "+body.Length.ToString()+"\r\nKeep-Alive: timeout=5, max=100\r\nConnection: Keep-Alive\r\nContent-Type: text/html\r\n\r\n";

                MemoryStream ms = new MemoryStream();
                ms.Write(Encoding.UTF8.GetBytes(headers), 0, Encoding.UTF8.GetBytes(headers).Length);
                ms.Write(Encoding.UTF8.GetBytes(body), 0, Encoding.UTF8.GetBytes(body).Length);
                ms.Seek(0x00, SeekOrigin.Begin);


                oSession.LoadResponseFromStream(ms, "");
            }
        }

    }
}
