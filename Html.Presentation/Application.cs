/* Html.Presentation
 * Copyright (c) 2022,  MSDN.WhiteKnight (https://github.com/MSDN-WhiteKnight) 
 * License: MIT */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace Html.Presentation
{
    public class Application
    {
        List<Page> pages=new List<Page>();

        public Application()
        {
            this.Name = "myapp";
            this.UrlHost = "http://localhost:8080";
            this.UrlPrefix = "/myapp/";
            this.ShowIntroMessage = true;
        }

        public string Name { get; set; }
        public string UrlHost { get; set; } // http://localhost:8080
        public string UrlPrefix { get; set; } // /myapp/
        public string HomepageUrl { get; set; }
        public bool ShowIntroMessage { get; set; }

        public void AddPage(Page page)
        {
            this.pages.Add(page);
        }

        static string GetCommonJs()
        {
            return @"function reqReadyStateChange() {

    if (window.xhr.readyState == 4) {
        var status = window.xhr.status;

        if (status != 200) {
            alert('Error: '+window.xhr.statusText);
        }
    }
}

function request(url){    
    window.xhr = new XMLHttpRequest();
    xhr.open('GET', url);
    xhr.onreadystatechange = reqReadyStateChange;
    xhr.send();
}

function dotnet_execute(method) {
    request(location.href+'?dotnet-command=exec&dotnet-argument='+method);
}";
        }

        public void Run()
        {
            // Create a listener.
            HttpListener listener = new HttpListener();

            // Add the prefixes.
            listener.Prefixes.Add(UrlHost + UrlPrefix);
            listener.Start();

            while (true)
            {
                HttpListenerContext context = listener.GetContext();
                HttpListenerRequest request = context.Request;
                string url = request.RawUrl;

                HttpListenerResponse response = context.Response;
                // Construct a response.

                if (!url.StartsWith(UrlPrefix))
                {
                    //вернуть ошибку при неверном URL
                    response.StatusCode = 404;
                    response.StatusDescription = "Not found";
                    response.Close();
                    continue;
                }

                ContentData responceData = null;

                if (url.StartsWith(UrlPrefix + "dotnet-common.js"))
                {
                    responceData = ContentData.FromJS(GetCommonJs());
                }
                else
                {
                    foreach (Page page in this.pages)
                    {
                        if (url.StartsWith(UrlPrefix + page.Name))
                        {
                            responceData = page.ProcessRequest(request);
                            break;
                        }
                    }
                }

                if (responceData == null)
                {
                    //вернуть ошибку при неверном URL
                    response.StatusCode = 404;
                    response.StatusDescription = "Not found";
                    response.Close();
                    continue;
                }

                byte[] buffer = responceData.Content;

                // Get a response stream and write the response to it.
                response.Headers.Add("Expires: Tue, 01 Jul 2000 06:00:00 GMT");
                response.Headers.Add("Cache-Control: max-age=0, no-cache, must-revalidate");
                response.ContentLength64 = buffer.Length;

                if (!string.IsNullOrEmpty(responceData.ContentType))
                {
                    response.ContentType = responceData.ContentType;
                }

                Stream output = response.OutputStream;
                BinaryWriter wr = new BinaryWriter(output);

                using (wr)
                {
                    wr.Write(buffer);
                }
            }
        }

        public void RunInBackground()
        {
            Thread th = new Thread(Run);
            th.IsBackground = true;
            th.Start();

            if (!string.IsNullOrEmpty(this.HomepageUrl))
            {
                if (this.ShowIntroMessage)
                {
                    Console.WriteLine("Welcome to " + this.Name + " application!");
                    Console.WriteLine("Displaying web UI on " + HomepageUrl);
                    Console.WriteLine("(If browser does not start automatically, navigate to URL manually.)");
                }

                try
                {
                    ProcessStartInfo psi = new ProcessStartInfo();
                    psi.FileName = HomepageUrl;
                    psi.UseShellExecute = true;
                    Process.Start(psi);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to start browser");
                    Console.WriteLine(ex.ToString());
                }
            }
            else
            {
                if (this.ShowIntroMessage)
                {
                    Console.WriteLine("Welcome to " + this.Name + " application!");
                    Console.WriteLine("Displaying web UI on " + this.UrlHost + this.UrlPrefix);
                }
            }
        }
    }
}
