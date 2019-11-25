using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

public static partial class Program
{
    internal sealed class RedistList
    {
        private readonly HashSet<string> assemblyNames = new HashSet<string>();

        public RedistList(string relativePath, XDocument document)
        {
            RelativePath = relativePath ?? throw new ArgumentNullException(nameof(relativePath));
            Document = document ?? throw new ArgumentNullException(nameof(document));

            foreach (var element in document.Root.Elements("File").ToList())
            {
                if (!assemblyNames.Add(element.Attribute("AssemblyName").Value))
                {
                    element.Remove();
                }
            }
        }

        public string RelativePath { get; }

        public XDocument Document { get; }

        public IReadOnlyCollection<string> AssemblyNames => assemblyNames;

        public void CombineDocument(XDocument otherDocument)
        {
            foreach (var node in otherDocument.Root.Nodes())
            {
                if (node is XElement element && element.Name == "File")
                {
                    if (!assemblyNames.Add(element.Attribute("AssemblyName").Value)) continue;
                }

                Document.Root.Add(node);
            }
        }

        public void RemoveAssemblies(IReadOnlyCollection<string> assemblyNames)
        {
            foreach (var node in Document.Root.Nodes().ToList())
            {
                if (node is XElement element && element.Name == "File")
                {
                    if (assemblyNames.Contains(element.Attribute("AssemblyName").Value))
                    {
                        element.Remove();
                    }
                }
            }
        }
    }
}
