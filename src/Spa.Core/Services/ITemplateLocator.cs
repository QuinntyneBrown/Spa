namespace Spa.Core.Services
{
    public interface ITemplateLocator
    {
        string[] Get(string filename);
    }
}