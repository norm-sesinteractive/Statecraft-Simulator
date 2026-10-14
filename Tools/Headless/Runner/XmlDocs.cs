using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Statecraft.SimRunner
{
    /// <summary>
    /// Reads Statecraft.Sim.xml (the compiler's XML documentation output) so generated docs quote
    /// the code's own summaries instead of restating them.
    /// </summary>
    internal sealed class XmlDocs
    {
        readonly Dictionary<string, string> summaries = new Dictionary<string, string>(StringComparer.Ordinal);

        public static XmlDocs Load()
        {
            var docs = new XmlDocs();
            string path = Path.Combine(AppContext.BaseDirectory, "Statecraft.Sim.xml");
            if (!File.Exists(path))
            {
                Console.Error.WriteLine($"warning: {path} not found; parameter meanings will be blank.");
                return docs;
            }

            foreach (var member in XDocument.Load(path).Descendants("member"))
            {
                string name = (string)member.Attribute("name");
                var summary = member.Element("summary");
                if (name != null && summary != null) docs.summaries[name] = Flatten(summary);
            }
            return docs;
        }

        public string Summary(Type type) => Get("T:" + type.FullName);

        public string Summary(FieldInfo field) => Get("F:" + field.DeclaringType.FullName + "." + field.Name);

        string Get(string key) => summaries.TryGetValue(key, out var summary) ? summary : "";

        static string Flatten(XElement element)
        {
            var text = new StringBuilder();
            foreach (var node in element.Nodes())
            {
                if (node is XText plain)
                {
                    text.Append(plain.Value);
                }
                else if (node is XElement child)
                {
                    string tag = child.Name.LocalName;
                    if (tag == "see" || tag == "seealso")
                        text.Append(ShortCref((string)child.Attribute("cref") ?? (string)child.Attribute("langword") ?? child.Value));
                    else if (tag == "paramref" || tag == "typeparamref")
                        text.Append((string)child.Attribute("name"));
                    else
                        text.Append(Flatten(child));
                }
            }
            return Regex.Replace(text.ToString(), @"\s+", " ").Trim();
        }

        static string ShortCref(string cref)
        {
            int paren = cref.IndexOf('(');
            if (paren >= 0) cref = cref.Substring(0, paren);
            int colon = cref.IndexOf(':');
            if (colon >= 0) cref = cref.Substring(colon + 1);
            int dot = cref.LastIndexOf('.');
            return dot >= 0 ? cref.Substring(dot + 1) : cref;
        }
    }
}
