/* ApiDocView
 * Copyright (c) 2022,  MSDN.WhiteKnight (https://github.com/MSDN-WhiteKnight) 
 * License: MIT */
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Markdig;
using Markdig.Syntax;
using Microsoft.DocAsCode.Common;
using Microsoft.DocAsCode.MarkdigEngine.Extensions;

namespace ApiDocView
{
    static class DocsViewer
    {
        static DocsViewer()
        {
            SrcDirectory = Directory.GetCurrentDirectory();
        }

        public static string SrcDirectory { get; set; }

        public static string GetLinkCallback(string path, MarkdownObject origin)
        {
            return "";
        }

        public static (string, object) ReadFileCallback(string path, MarkdownObject origin)
        {
            string path_content;
            RelativePath rp;
            string content;

            if (path.StartsWith("~/"))
            {
                string rel = path.Substring(2, path.Length - 2);
                path_content = Path.Combine(GetRepositoryRoot(SrcDirectory), rel);
                rp = RelativePath.Parse(Path.GetFileName(path));
            }
            else
            {
                if (Path.IsPathFullyQualified(path)) path_content = path;
                else path_content = Path.Combine(SrcDirectory, path);

                rp = RelativePath.Parse(path);
            }

            if (File.Exists(path_content)) content = File.ReadAllText(path_content);
            else content = "[File not found: " + path + "]";

            return (content, rp);
        }

        static string GetRepositoryRoot(string dir)
        {
            string docfx_path = Path.Combine(dir, "docfx.json");

            if (File.Exists(docfx_path)) return dir;

            DirectoryInfo parent = Directory.GetParent(dir);

            if (parent == null) return dir;
            else return GetRepositoryRoot(parent.FullName);
        }

        public static string RenderDocument(string srctext)
        {
            MarkdownContext ctx = new MarkdownContext(
                //getLink: GetLinkCallback,
                readFile: ReadFileCallback
                );

            MarkdownPipelineBuilder mpb = new MarkdownPipelineBuilder();
            MarkdownPipeline mp = mpb.UseDocfxExtensions(ctx).Build();
            return Markdown.ToHtml(srctext, mp);
        }

        static string GetRemarksText(XmlNode docs)
        {
            XmlNode nodeRemarks = docs["remarks"];
            if (nodeRemarks == null) return string.Empty;

            if (!nodeRemarks.HasChildNodes)
            {
                string ret = nodeRemarks.Value;
                if (ret == null) ret = string.Empty;
                return ret;
            }

            XmlNode nodeFormat = nodeRemarks["format"];
            if (nodeFormat == null) return string.Empty;

            XmlNode nodeCdata = nodeFormat.FirstChild;
            if (nodeCdata == null) return string.Empty;

            string remarks = nodeCdata.Value;
            if (remarks == null) remarks = string.Empty;
            return remarks;
        }

        public static string GetFromXML(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            if (!Utils.StrEquals(doc.DocumentElement.Name, "Type"))
            {
                throw new ArgumentException("Incorrect XML. Root tag must be 'Type'.");
            }

            XmlNode docs = doc.DocumentElement["Docs"];
            string remarks = GetRemarksText(docs);

            XmlNode members = doc.FirstChild["Members"];

            string typename = doc.FirstChild.Attributes["FullName"].Value;
            //Console.WriteLine(typename);

            foreach (XmlNode node in members.ChildNodes)
            {
                if (node.NodeType != XmlNodeType.Element) continue;

                string mname;
                string docid = "";
                
                //parse member node

                if (node.Name != "Member") continue;
                
                mname = node.Attributes["MemberName"].Value;

                //signature
                foreach (XmlNode x in node.ChildNodes)
                {
                    if (x.Name == "MemberSignature" && x.Attributes["Language"].Value == "DocId")
                    {
                        docid = x.Attributes["Value"].Value;
                        break;
                    }
                }

                if (string.IsNullOrEmpty(docid)) docid = mname;
                
                //Console.WriteLine(docid);

            }//end foreach

            return remarks;
        }
    }
}
