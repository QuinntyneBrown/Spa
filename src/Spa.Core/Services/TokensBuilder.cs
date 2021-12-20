using Spa.Core.Models;
using System.Collections.Generic;

namespace Spa.Core.Services
{
    public class TokensBuilder
    {
        private Dictionary<string, object> _value { get; set; } = new();

        public TokensBuilder()
        {
            _value = new();

            if (!_value.ContainsKey("openDoubleSquilly"))
            {
                _value.Add("openDoubleSquilly", "{{");
            }

            if (!_value.ContainsKey("closeDoubleSquilly"))
            {
                _value.Add("closeDoubleSquilly", "}}");
            }

            if (!_value.ContainsKey("openSquilly"))
            {
                _value.Add("openSquilly", "{");
            }

            if (!_value.ContainsKey("closeSquilly"))
            {
                _value.Add("closeSquilly", "}");
            }
        }
        public TokensBuilder With(string propertyName, Token token)
        {
            var tokens = token == null ? new Token("").ToTokens(propertyName) : token.ToTokens(propertyName);

            foreach (var t in tokens)
            {
                if (!_value.ContainsKey(t.Key))
                {
                    _value.Add(t.Key, t.Value);
                }
            }
            return this;
        }

        public Dictionary<string, object> Build() => this._value;

    }
}
