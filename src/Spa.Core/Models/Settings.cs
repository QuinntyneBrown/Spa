namespace Spa.Core.Models
{
    public class Settings
    { 
        public int Port { get; set; }
        public string Path { get; set; }
        public string[] Resources { get; set; }
        public static Settings Empty => new();
    }
}
