namespace Spa.Core.Services
{
    public class DetectResponseDto
    {
        public string Language { get; set; }
        public decimal Score { get; set; }
        public string IsTranslationSupported { get; set; }
        public bool IsTransliterationSupported { get; set; }

    }
}
