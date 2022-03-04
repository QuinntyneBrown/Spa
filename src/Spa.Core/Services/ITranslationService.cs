namespace Spa.Core.Services
{
    public interface ITranslationService
    {
        string Translate(string textToTranslate);
        string Detect(string textToDetect);
        void Sanitize(string path, string targetLanguage);
    }
}
