using System;
using System.Runtime.CompilerServices;

namespace CloudRun.Common.Util
{
    /// <summary>
    /// A string whose value is inferred from the time of declaration.
    /// </summary>
    /// <example>
    /// <code>public static readonly string SomeValue = new Inferred()</code>
    /// </example>
    public class Inferred : IEquatable<string>, IEquatable<Inferred>
    {
        readonly string _memberName = null;

        public Inferred([CallerMemberName] string memberName = null)
        {
            if (string.IsNullOrEmpty(memberName))
                throw new ArgumentNullException(nameof(memberName));

            _memberName = memberName;
        }

        public bool Equals(string other)
        {
            return string.Equals(_memberName, other);
        }

        public bool Equals(Inferred other)
        {
            return string.Equals(_memberName, other._memberName);
        }

        public override string ToString()
        {
            return _memberName;
        }

        public override int GetHashCode()
        {
            return _memberName.GetHashCode();
        }

        public static implicit operator string(Inferred inferred)
        {
            return inferred._memberName;
        }
    }
}
