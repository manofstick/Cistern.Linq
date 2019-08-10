namespace Playground.jamesko.SystemLinq
{
    using System.Collections.Generic;
    using System.Linq;

    public class MessageBuilder
    {
        private readonly Dictionary<string, string> _messages;

        public MessageBuilder()
        {
            _messages = new Dictionary<string, string>();
        }

        public void Clear() => _messages.Clear();

        public object this[string key]
        {
            set { _messages[key] = value.ToString(); }
        }

        public override string ToString()
        {
            // Note: As long as we don't remove entries from the hashtable,
            // I believe we should get the keys/values in order of insertion.
            return string.Join(", ", _messages.Select(pair => $"{pair.Key}: {pair.Value}"));
        }

        public string ToStringAndClear()
        {
            string r = ToString();
            Clear();
            return r;
        }
    }
}

namespace Playground.jamesko.CisternLinq
{
    using System.Collections.Generic;
    using Cistern.Linq;

    public class MessageBuilder
    {
        private readonly Dictionary<string, string> _messages;

        public MessageBuilder()
        {
            _messages = new Dictionary<string, string>();
        }

        public void Clear() => _messages.Clear();

        public object this[string key]
        {
            set { _messages[key] = value.ToString(); }
        }

        public override string ToString()
        {
            // Note: As long as we don't remove entries from the hashtable,
            // I believe we should get the keys/values in order of insertion.
            return string.Join(", ", _messages.Select(pair => $"{pair.Key}: {pair.Value}"));
        }

        public string ToStringAndClear()
        {
            string r = ToString();
            Clear();
            return r;
        }
    }
}

