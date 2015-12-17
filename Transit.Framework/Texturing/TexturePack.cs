using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Transit.Framework.Texturing
{
    public abstract class TexturePack
    {
        private readonly Dictionary<string, object> _lazyValues = new Dictionary<string, object>();

        protected T GetValue<T>(Expression<Func<T>> selector, Func<T> factory)
        {
            object value;

            var memberName = selector.GetSelectedMemberName();
            if (!_lazyValues.TryGetValue(memberName, out value))
            {
                // First access, create the dictionary entry
                value = factory();
                _lazyValues.Add(memberName, value);
            }

            return (T)value;
        }
    }
}
