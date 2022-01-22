/* ApiDocView
 * Copyright (c) 2022,  MSDN.WhiteKnight (https://github.com/MSDN-WhiteKnight) 
 * License: MIT */
using System;
using Html.Presentation;

namespace ApiDocView
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("*** .NET API docs viewer ***");

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
