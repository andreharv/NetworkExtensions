using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

namespace Transit.Framework.Texturing
{
    public abstract class TexturePack
    {
        private readonly Dictionary<string, Texture2D> _lazyValues = new Dictionary<string, Texture2D>();

        protected Texture2D GetValue(Expression<Func<Texture2D>> selector, Func<Texture2D> factory)
        {
            Texture2D value;

            var memberName = selector.GetSelectedMemberName();
            if (!_lazyValues.TryGetValue(memberName, out value))
            {
                // First access, create the dictionary entry
                value = factory();
                _lazyValues.Add(memberName, value);
            }

            return value;
        }
    }
}
