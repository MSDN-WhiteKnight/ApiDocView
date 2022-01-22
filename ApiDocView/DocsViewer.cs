/* ApiDocView
 * Copyright (c) 2022,  MSDN.WhiteKnight (https://github.com/MSDN-WhiteKnight) 
 * License: MIT */
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Markdig;

namespace ApiDocView
{
    static class DocsViewer
    {
        public static string RenderDocument(string srctext)
        {
            return Markdown.ToHtml(srctext);
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
