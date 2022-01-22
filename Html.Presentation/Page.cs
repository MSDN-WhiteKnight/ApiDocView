/* Html.Presentation
 * Copyright (c) 2022,  MSDN.WhiteKnight (https://github.com/MSDN-WhiteKnight) 
 * License: MIT */
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using HttpMultipartParser;

namespace Html.Presentation
{
    public abstract class Page
    {
        public string Name { get; set; }
        public string Html { get; set; }
        public Application Owner { get; set; }

        public abstract void OnLoad(LoadEventArgs args);

        public static string ReadFromFile(string path)
        {
            return File.ReadAllText(path);
        }

        static string ReadFromResourceImpl(Assembly assembly, string nspace, string name)
        {
            using (Stream stream = assembly.GetManifestResourceStream(nspace + "." + name))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public string ReadFromResource(string name)
        {
            Type t = this.GetType();

            using (Stream stream = t.Assembly.GetManifestResourceStream(t.Namespace + "." + name))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        internal ContentData ProcessRequest(HttpListenerRequest request)
        {
            Dictionary<string, object> fields = new Dictionary<string, object>();

            if (Utils.StrEquals(request.HttpMethod, "GET"))
            {
                //разбираем параметры запроса
                string command = request.QueryString["dotnet-command"];
                string argument = request.QueryString["dotnet-argument"];

                //обработать команду

                if (argument == null) argument = "";

                switch (command)
                {
                    case "exec":
                        var method = this.GetType().GetMethod(argument);
                        method.Invoke(this, new object[] { });
                        return ContentData.FromHTML(this.Html);

                    default:                        
                        foreach (object x in request.QueryString.Keys)
                        {
                            string name = x.ToString();
                            string val = request.QueryString[name];
                            fields[name] = val;
                        }

                        break;
                }
            }
            else if(Utils.StrEquals(request.HttpMethod,"POST") && request.ContentType.StartsWith("multipart/form-data"))
            {
                MultipartFormDataParser parser = MultipartFormDataParser.Parse(request.InputStream);
                
                for (int i = 0; i < parser.Parameters.Count; i++)
                {
                    ParameterPart x = parser.Parameters[i];
                    string name = x.Name;
                    fields[name] = x.Data;
                }

                for (int i = 0; i < parser.Files.Count; i++)
                {
                    FilePart x = parser.Files[i];
                    string name = x.Name;
                    MemoryStream msData = new MemoryStream();
                    x.Data.CopyTo(msData);

                    if (ContentTypes.IsTextContentType(x.ContentType))
                    {
                        ContentData data = new ContentData();
                        data.Content = msData.ToArray();
                        data.ContentType = x.ContentType;
                        fields[name] = data;
                    }
                    else 
                    { 
                        fields[name] = msData.ToArray(); 
                    }
                }
            }
            else return ContentData.FromHTML(this.Html);

            LoadEventArgs args = new LoadEventArgs(fields);
            this.OnLoad(args);

            if (args.SendCustomResponse) return args.CustomResponse;
            else return ContentData.FromHTML(this.Html);
        }
    }

    public class LoadEventArgs : EventArgs
    {
        Dictionary<string, object> _fields;

        internal LoadEventArgs(Dictionary<string, object> fields)
        {
            this._fields = fields;
        }
                
        public object GetField(string name)
        {
            object ret;

            if (this._fields.TryGetValue(name, out ret)) return ret;
            else return null;
        }

        public bool HasField(string name)
        {
            return this._fields.ContainsKey(name);
        }

        public bool SendCustomResponse { get; set; }

        public ContentData CustomResponse { get; set; }
    }
}
