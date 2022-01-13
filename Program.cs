using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace HtmlUiTest
{
    class IndexPage : Page
    {
        public IndexPage()
        {
            this.Name = "index.html";
            this.Html = File.ReadAllText("frontend.html");
        }

        public void OnHelloClick()
        {
            Console.WriteLine("Hello, world");
        }

        public override void OnLoad(LoadEventArgs args)
        {
            if (!args.Fields.ContainsKey("x") || !args.Fields.ContainsKey("y")) return;

            int x = Convert.ToInt32(args.Fields["x"]);
            int y = Convert.ToInt32(args.Fields["y"]);
            string txt = x.ToString() + "+" + y.ToString() + "=" + (x + y).ToString();
            Console.WriteLine(txt);
            args.SendCustomResponse = true;
            args.CustomResponse = txt;
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
            app.RunInBackground();

            Console.ReadKey();
        }
    }
}
