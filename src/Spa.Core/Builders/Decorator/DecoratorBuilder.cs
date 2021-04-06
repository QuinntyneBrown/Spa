using Spa.Core.Services;
using System.Collections.Generic;

namespace Spa.Core.Builders
{
    public class DecoratorBuilder
    {
        private readonly string _name;
        private IFileSystem _fileSystem;
        private ICommandService _commandService;

        public DecoratorBuilder(IFileSystem fileSystem, ICommandService commandService, string name)
        {
            _name = name;
        }
        public string[] Content
        {
            get
            {
                var content = new List<string>();

                return content.ToArray();
            }
        }
    }
}
