using System.Collections.Generic;
using System.IO;

namespace Spa.Core.Models
{    
    public class AngularJsonFileModel
    {

        public Dictionary<string, Project> Projects { get; set; } = new Dictionary<string, Project>();
        public string DefaultProject { get; set; }
        public string RootDirectory { get; set; }
        public string TranslationsDirectory => $"{AssetsDirectory}{Path.DirectorySeparatorChar}i18n";
        public string SrcDirectory => $"{RootDirectory}{Path.DirectorySeparatorChar}src";
        public string AssetsDirectory => $"{SrcDirectory}{Path.DirectorySeparatorChar}assets";        
        public string ScssDirectory => $"{SrcDirectory}{Path.DirectorySeparatorChar}scss";
        public string AppDirectory => $"{SrcDirectory}{Path.DirectorySeparatorChar}app";
        public string CoreDirectory => $"{AppDirectory}{Path.DirectorySeparatorChar}@core";
        public string ApiDirectory => $"{AppDirectory}{Path.DirectorySeparatorChar}@api";
        public string SharedDirectory => $"{AppDirectory}{Path.DirectorySeparatorChar}@shared";
        public string ModelsDirectory => $"{ApiDirectory}{Path.DirectorySeparatorChar}models";
        public string ServicesDirectory => $"{ApiDirectory}{Path.DirectorySeparatorChar}services";
        public string SharedComponentsDirectory => $"{SharedDirectory}{Path.DirectorySeparatorChar}components";

        public string Styles => $"{SrcDirectory}{Path.DirectorySeparatorChar}styles.scss";

        public string Prefix
        {
            get
            {
                Projects.TryGetValue(DefaultProject, out Project project);
                return project?.Prefix;
            }
        }

        public class Project
        {
            public string Prefix { get; set; }
        }
        public static AngularJsonFileModel Empty => new();
    }

}
