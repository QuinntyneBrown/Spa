using HtmlAgilityPack;
using System;
using System.Collections.Generic;

namespace Spa.Core.Services
{
    public class TranslationAuditService : ITranslationAuditService
    {
        public List<string> Audit(string html, List<string> failures = null)
        {
            failures ??= new List<string>();

            HtmlDocument htmlDocument = new();

            htmlDocument.LoadHtml(html);
           
            foreach (var node in htmlDocument.DocumentNode.ChildNodes)
            {
                if(node is HtmlTextNode)
                {
                    var text = node.InnerText.Trim(new Char[] { ' ', '\r', '\n', '\t' });

                    if(!string.IsNullOrEmpty(text) && !text.Contains("{{") && !text.Contains("| translate"))
                    {
                        failures.Add(text);
                    }
                }

                if(node.HasChildNodes)
                {
                    Audit(node.InnerHtml, failures);
                }
            }

            return failures;
        }

        public Dictionary<string, List<string>> AuditDirectory(string directory)
        {
            var failures = new Dictionary<string, List<string>>();

            foreach(var file in System.IO.Directory.GetFiles(directory,"*.html", System.IO.SearchOption.AllDirectories))
            {
                failures.Add(file, Audit(System.IO.File.ReadAllText(file)));
            }

            return failures;
        }
    }
}
