/* ApiDocView
 * Copyright (c) 2022,  MSDN.WhiteKnight (https://github.com/MSDN-WhiteKnight) 
 * License: MIT */
using System;
using System.IO;
using Html.Presentation;

namespace ApiDocView
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length >= 1)
            {
                DocsViewer.SrcDirectory = Path.GetFullPath(args[0]);
            }
            else
            {
                DocsViewer.SrcDirectory = Directory.GetCurrentDirectory();
            }

            Console.WriteLine("*** .NET API docs viewer ***");
            Console.WriteLine("Source directory: " + DocsViewer.SrcDirectory);
            Console.WriteLine();

            Application app = new Application();
            app.Name = "apidocs";
            app.UrlHost = "http://localhost:8080";
            app.UrlPrefix = "/apidocs/";
            app.HomepageUrl = "http://localhost:8080/apidocs/index.html";
            app.AddPage(new IndexPage());
            app.RunInBackground();

            Console.ReadKey();
        }
    }
}
