namespace Spa.Core.Models
{
    public enum ClientApplicationType
    {
        Default,
        Minimal
    }

    public class ClientApplicationModel
    {
        public string Prefix { get; set; }
        public ClientApplicationType Type = ClientApplicationType.Default;
    }
}
