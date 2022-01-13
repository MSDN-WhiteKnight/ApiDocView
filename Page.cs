using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace HtmlUiTest
{
    public abstract class Page
    {
        public string Name { get; set; }
        public string Html { get; set; }
        public Application Owner { get; set; }

        public abstract void OnLoad(LoadEventArgs args);
        
        public string ProcessRequest(HttpListenerRequest request)
        {
            //разбираем параметры запроса
            string command = request.QueryString["command"];
            string argument = request.QueryString["argument"];
            
            //обработать команду
            
            if (argument == null) argument = "";

            switch (command)
            {
                case "exec":
                    var method = this.GetType().GetMethod(argument);
                    method.Invoke(this, new object[] { });
                    return this.Html;
                
                default:
                    Dictionary<string, object> fields = new Dictionary<string, object>();

                    foreach (var x in request.QueryString.Keys)
                    {
                        string name = x.ToString();
                        string val = request.QueryString[name];
                        fields[name] = val;
                    }

                    LoadEventArgs args = new LoadEventArgs(fields);
                    this.OnLoad(args);

                    if (args.SendCustomResponse) return args.CustomResponse;
                    else return this.Html;
            }
        }
    }

    public class LoadEventArgs : EventArgs
    {
        Dictionary<string, object> _fields;

        internal LoadEventArgs(Dictionary<string, object> fields)
        {
            this._fields = fields;
        }

        public Dictionary<string, object> Fields { get { return this._fields; } }

        public bool SendCustomResponse { get; set; }

        public string CustomResponse { get; set; }
    }
}
