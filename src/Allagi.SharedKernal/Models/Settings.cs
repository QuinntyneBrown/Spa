namespace Spa.Core.Models
{
    public class Settings
    { 
        public int Port { get; set; }
        public string Path { get; set; }
        public string[] Resources { get; set; }
        public string[] AppDirectories { get; set; }
        public string AppProjectDirectory { get; set; }
        public string SolutionPath { get; set; }
        public string DomainDirectory { get; set; }
        public static Settings Empty => new();
    }
}
