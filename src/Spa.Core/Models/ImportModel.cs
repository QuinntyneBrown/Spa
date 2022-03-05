namespace Spa.Core.Models
{
    public class ImportModel
    {
        public string Name { get; private set; }
        public string ModuleName { get; private set; }

        public ImportModel(string name, string moduleName)
        {
            Name = name;
            ModuleName = moduleName;
        }
    }
}
