using Spa.Core.Models;
using Spa.Core.Services;
using Spa.Core.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace Spa.Core.Builders
{
    public class ClassDecoratorBuilder
    {
        private readonly string _name;
        private IFileSystem _fileSystem;
        private ICommandService _commandService;
        private Dictionary<string, object> _options;

        public ClassDecoratorBuilder(IFileSystem fileSystem, ICommandService commandService, string name)
        {
            _commandService = commandService;
            _fileSystem = fileSystem;
            _name = name;
            _options = new();
        }

        public ClassDecoratorBuilder AddOption(string key, object value)
        {
            _options.Add(key, value);
            return this;
        }

        public string[] Content
        {
            get
            {
                var content = new List<string>();

                if (_options.Count == 0)
                {
                    content.Add($"@{((Token)_name).PascalCase}()");

                    return content.ToArray();
                }

                content.Add($"@{((Token)_name).PascalCase}" + "({");

                var last1 = _options.Last();

                foreach (var option in _options)
                {
                    var eol1 = option.Equals(last1) ? "" : ",";

                    if (option.Value is string[] arr)
                    {
                        var last2 = arr.Last();

                        foreach(var arrItem in arr)
                        {
                            var eol2 = arrItem.Equals(last2) ? "" : ",";

                        }
                    } else
                    {
                        content.Add($"{option.Key}: {option.Value}{eol1}".Indent(1));
                    }
                }

                content.Add("})");

                return content.ToArray();
            }
        }

        public string[] Build() => Content;
    }
}
