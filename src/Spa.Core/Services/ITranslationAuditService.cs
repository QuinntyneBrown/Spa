using System.Collections.Generic;

namespace Spa.Core.Services
{
    public interface ITranslationAuditService
    {
        List<string> Audit(string path, List<string> failures = null);
        Dictionary<string, List<string>> AuditDirectory(string directory);
    }
}
