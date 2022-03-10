namespace Spa.Core.Models
{
    public class AngularComponentModel
    {
        public FileModel TypeScriptFile { get; set; }
        public FileModel ScssFile { get; set; }
        public FileModel HtmlFile { get; set; }
        public string Name { get; set; }

        public AngularComponentModel(string name)
        {
            Name = name;
        }

        public AngularComponentModel()
        {

        }
    }
}
