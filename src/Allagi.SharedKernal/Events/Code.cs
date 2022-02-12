using Allagi.SharedKernal.Services;
using Spa.Core.Models;

namespace Spa.Core.Events
{
    public class CodeRequest: IEvent
    {
        public string TemplateName { get; private set; }
        public string Prefix { get; private set; }

        public CodeRequest(string templateName, string prefix)
        {
            TemplateName = templateName;
            Prefix = prefix;
        }
    }

    public class CodeResponse: IEvent
    {
        public AngularCodeResult Code { get; private set; }

        public CodeResponse(AngularCodeResult code)
        {
            Code = code;
        }
    }
}
