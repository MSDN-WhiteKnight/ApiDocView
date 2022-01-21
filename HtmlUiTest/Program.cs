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

            if (args.HasField("file"))
            {
                string sfile = args.GetField("file").ToString();
                txt += " " + sfile;
            }

            Console.WriteLine(txt);
            args.SendCustomResponse = true;
            args.CustomResponse = txt;
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
            app.RunInBackground();

            Console.ReadKey();
        }
    }
}
