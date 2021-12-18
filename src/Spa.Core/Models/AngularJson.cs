using System.Collections.Generic;
using System.IO;

namespace Spa.Core.Models
{
    public class AngularJson
    {
        public Dictionary<string, Project> Projects { get; set; } = new Dictionary<string, Project>();
        public string DefaultProject { get; set; }
        public string RootDirectory { get; set; }
        public string AppDirectory => $"{RootDirectory}{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}app";
        public string CoreDirectory => $"{AppDirectory}{Path.DirectorySeparatorChar}@core";
        public string ApiDirectory => $"{AppDirectory}{Path.DirectorySeparatorChar}@api";
        public string SharedDirectory => $"{AppDirectory}{Path.DirectorySeparatorChar}@shared";
        public string ModelsDirectory => $"{ApiDirectory}{Path.DirectorySeparatorChar}models";
        public string ServicesDirectory => $"{ApiDirectory}{Path.DirectorySeparatorChar}services";
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
        public static AngularJson Empty => new();
    }

}
