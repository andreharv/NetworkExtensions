using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Transit.Framework.Texturing;

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

        public static string GetSelectedMemberName<T>(this Expression<Func<T>> selector)
        {
            var member = selector.FindMember();

            return member == null ? null : member.Name;
        }

        public static MemberInfo FindMember<T>(this Expression<Func<T>> selector)
        {
            var memberExpression = selector.Body as MemberExpression;

            if (memberExpression == null)
            {
                selector.Body.Maybe<UnaryExpression>(u => memberExpression = u.Operand as MemberExpression);
            }

            if (memberExpression == null)
            {
                return null;
            }
            else
            {
                return memberExpression.Member;
            }
        }
    }
}
