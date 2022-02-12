using System.Collections.Generic;
using System.Linq;

namespace Spa.Core.Models
{
    public class RootScssFileModel
    {
        public List<string> Lines { get; private set; } = new List<string>();

        public RootScssFileModel(List<string> lines)
        {
            Lines = lines;
        }

        public void EnsureUseStatementsBeginFile()
        {
            Lines = Lines.Where(x => x.StartsWith("@use")).Concat(Lines.Where(x => !x.StartsWith("@use"))).ToList();
        }
    }
}
