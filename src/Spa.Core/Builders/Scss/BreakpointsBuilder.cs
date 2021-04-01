using Spa.Core.Models;

namespace Spa.Core.Builders.Scss
{
    public class BreakpointsBuilder
    {
        private Breakpoints _breakpoints;

        public static Breakpoints WithDefaults()
        {
            return new Breakpoints();
        }

        public BreakpointsBuilder()
        {
            _breakpoints = WithDefaults();
        }

        public Breakpoints Build()
        {
            return _breakpoints;
        }
    }
}
