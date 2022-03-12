using System.Collections.Generic;

namespace Spa.Core.Models
{
    public class AngularComponentModel
    {
        public AngularJsonFileModel AngularJsonFile { get; set; }
        public FileModel TypeScriptFile { get; set; }
        public FileModel ScssFile { get; set; }
        public FileModel HtmlFile { get; set; }
        public string Name { get; set; }
        public List<dynamic> SharedDependencies { get; set; }
        public List<dynamic> CoreDependencies { get; set; }
        public List<dynamic> ScssDependencies { get; set; }

        public AngularComponentModel(string name, AngularJsonFileModel angularJsonFile)
            :this(name)
        {
            AngularJsonFile = angularJsonFile;
        }

        public AngularComponentModel(string name)
        {
            Name = name;
        }

        public AngularComponentModel()
        {

        }
    }
}
