using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Transit.Framework
{
    public static class ExpressionExtensions
    {
        public static string GetSelectedMemberName<T, TValue>(this Expression<Func<T, TValue>> selector)
        {
            if (selector == null)
            {
                return null;
            }
            else
            {
                if (selector.Body is MemberExpression)
                {
                    var memberExpression = selector.Body as MemberExpression;
                    return memberExpression.Member.Name;
                }
                else if (selector.Body is UnaryExpression)
                {
                    var unaryExpression = selector.Body as UnaryExpression;
                    return (unaryExpression.Operand as MemberExpression).Member.Name;
                }
                return null;

            }
        }
    }
}
