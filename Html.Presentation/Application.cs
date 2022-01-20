using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading;

namespace HtmlUiTest
{
    public class Application
    {
        List<Page> pages=new List<Page>();

        public string Name { get; set; }
        public string UrlHost { get; set; } // http://localhost:8080
        public string UrlPrefix { get; set; } // /myapp/
        public string HomepageUrl { get; set; }

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
    request(location.href+'?command=exec&argument='+method);
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

                string responceString = null;

                if (url.StartsWith(UrlPrefix + "dotnet-common.js"))
                {
                    responceString = GetCommonJs();
                }
                else
                {
                    foreach (Page page in this.pages)
                    {
                        if (url.StartsWith(UrlPrefix + page.Name))
                        {
                            responceString = page.ProcessRequest(request);
                            break;
                        }
                    }
                }

                if (responceString == null)
                {
                    //вернуть ошибку при неверном URL
                    response.StatusCode = 404;
                    response.StatusDescription = "Not found";
                    response.Close();
                    continue;
                }
                
                byte[] buffer = Encoding.UTF8.GetBytes(responceString);

                // Get a response stream and write the response to it.
                response.Headers.Add("Expires: Tue, 01 Jul 2000 06:00:00 GMT");
                response.Headers.Add("Cache-Control: max-age=0, no-cache, must-revalidate");
                response.ContentLength64 = buffer.Length;
                System.IO.Stream output = response.OutputStream;
                System.IO.BinaryWriter wr = new System.IO.BinaryWriter(output);

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
            Console.WriteLine("Listening on " + HomepageUrl);

            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = HomepageUrl;
            psi.UseShellExecute = true;
            Process.Start(psi);
        }
    }
}
