using Spa.Core.Models;

namespace Spa.Core.Builders.Project
{
    public class ProjectBuilder
    {
        private Project _project;

        public static Project WithDefaults()
        {
            return new Project();
        }

        public ProjectBuilder()
        {
            _project = WithDefaults();
        }

        public Project Build()
        {
            return _project;
        }
    }
}
