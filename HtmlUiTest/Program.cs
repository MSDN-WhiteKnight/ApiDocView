using System;
using System.Collections.Generic;
using System.IO;
using Html.Presentation;

namespace HtmlUiTest
{
    class IndexPage : Page
    {
        public IndexPage()
        {
            this.Name = "index.html";
            this.Html = ReadFromFile(this.Name);
        }

        public override void OnLoad(LoadEventArgs args)
        {
            if (!args.HasField("x") || !args.HasField("y")) return;

            int x = Convert.ToInt32(args.GetField("x"));
            int y = Convert.ToInt32(args.GetField("y"));
            string txt = x.ToString() + "+" + y.ToString() + "=" + (x + y).ToString();
            
            Console.WriteLine(txt);
            args.SendCustomResponse = true;
            args.CustomResponse = ContentData.FromText(txt);
        }
    }

    class HelloPage : Page
    {
        public HelloPage()
        {
            this.Name = "hello.html";
            this.Html = ReadFromResource(this.Name);
        }

        public void OnHelloClick()
        {
            Console.WriteLine("Hello, world");
        }

        public override void OnLoad(LoadEventArgs args)
        {
            
        }
    }

    class ViewPage : Page
    {
        public ViewPage()
        {
            this.Name = "view.html";
            this.Html = ReadFromResource(this.Name);
        }

        public override void OnLoad(LoadEventArgs args)
        {
            if (!args.HasField("file")) return;

            object file = args.GetField("file");

            if (file is ContentData)
            {
                ContentData data = (ContentData)file;

                if (data.IsText)
                {
                    args.SendCustomResponse = true;
                    args.CustomResponse = data;
                    return;
                }
            }
            else if (file is byte[])
            {
                byte[] arr = (byte[])file;
                args.SendCustomResponse = true;
                args.CustomResponse = ContentData.FromText("Binary file (" + arr.Length + " bytes)");
                return;
            }
            
            string sfile = args.GetField("file").ToString();
            Console.WriteLine(sfile);
            args.SendCustomResponse = true;
            args.CustomResponse = ContentData.FromText(sfile);
        }
    }

    class Program
    {        
        public static void Print(string text)
        {
            if (text == null) text = "";
            Console.WriteLine(text);
        }

        public static void CloseWindow()
        {
            Console.WriteLine("Closed");
        }

        static void Main(string[] args)
        {
            Application app = new Application();
            app.UrlHost = "http://localhost:8080";
            app.UrlPrefix = "/myapp/";
            app.HomepageUrl = "http://localhost:8080/myapp/index.html";
            app.AddPage(new IndexPage());
            app.AddPage(new HelloPage());
            app.AddPage(new ViewPage());
            app.RunInBackground();

            Console.ReadKey();
        }
    }
}
