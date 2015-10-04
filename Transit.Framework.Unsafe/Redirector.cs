using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Transit.Framework.Unsafe
{
    public abstract class RedirectAttribute : Attribute
    {
        public RedirectAttribute(Type classType, string methodName = null)
        {
            ClassType = classType;
            MethodName = methodName;
        }

        public Type ClassType { get; set; }
        public string MethodName { get; set; }
    }

    /// <summary>
    /// Marks a method for redirection. All marked methods are redirected by calling
    /// <see cref="Redirector.PerformRedirections"/> and reverted by <see cref="Redirector.RevertRedirections"/>
    /// <para>NOTE: only the methods belonging to the same assembly that calls Perform/RevertRedirections are redirected.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class RedirectFromAttribute : RedirectAttribute
    {
        /// <param name="classType">The class of the method that will be redirected</param>
        /// <param name="methodName">The name of the method that will be redirected. If null,
        /// the name of the attribute's target method will be used.</param>
        public RedirectFromAttribute(Type classType, string methodName = null)
            : base(classType, methodName)
        { }
    }

    /// <summary>
    /// Marks a method for redirection. All marked methods are redirected by calling
    /// <see cref="Redirector.PerformRedirections"/> and reverted by <see cref="Redirector.RevertRedirections"/>
    /// <para>NOTE: only the methods belonging to the same assembly that calls Perform/RevertRedirections are redirected.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class RedirectToAttribute : RedirectAttribute
    {
        /// <param name="classType">The class of the target method</param>
        /// <param name="methodName">The name of the target method. If null,
        /// the name of the attribute's target method will be used.</param>
        public RedirectToAttribute(Type classType, string methodName = null)
            : base(classType, methodName)
        { }
    }

    public static class Redirector
    {
        internal class MethodRedirection : IDisposable
        {
            private bool _isDisposed = false;

            private MethodInfo _originalMethod;
            private readonly RedirectCallsState _callsState;

            public MethodRedirection(MethodInfo originalMethod, MethodInfo newMethod)
            {
                _originalMethod = originalMethod;
                _callsState = RedirectionHelper.RedirectCalls(_originalMethod, newMethod);
            }

            public void Dispose()
            {
                if (!_isDisposed)
                {
                    RedirectionHelper.RevertRedirect(_originalMethod, _callsState);
                    _originalMethod = null;
                    _isDisposed = true;
                }
            }

            public MethodInfo OriginalMethod
            {
                get
                {
                    return _originalMethod;
                }
            }
        }

        private static List<MethodRedirection> s_redirections = new List<MethodRedirection>();

        public static void PerformRedirections()
        {
            Assembly callingAssembly = Assembly.GetCallingAssembly();

            IEnumerable<MethodInfo> methods = from type in callingAssembly.GetTypes()
                                              from method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                                              where method.GetCustomAttributes(typeof(RedirectAttribute), false).Length > 0
                                              select method;

            foreach (MethodInfo method in methods)
            {
                foreach (RedirectAttribute redirectAttr in method.GetCustomAttributes(typeof(RedirectAttribute), false))
                {
                    // TODO: check for options

                    string originalName = String.IsNullOrEmpty(redirectAttr.MethodName) ? method.Name : redirectAttr.MethodName;

                    MethodInfo originalMethod = null;
                    foreach (MethodInfo m in redirectAttr.ClassType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static))
                    {
                        if (m.Name != originalName)
                            continue;

                        if (method.IsCompatibleWith(originalMethod))
                        {
                            originalMethod = m;
                            break;
                        }
                    }

                    if (originalMethod != null)
                    {
                        if (redirectAttr.GetType() == typeof(RedirectFromAttribute))
                        {
                            if (s_redirections.Where(r => r.OriginalMethod == originalMethod).Count() == 0)
                                s_redirections.Add(originalMethod.RedirectTo(method));
                        }
                        else
                        {
                            if (s_redirections.Where(r => r.OriginalMethod == method).Count() == 0)
                                s_redirections.Add(method.RedirectTo(originalMethod));
                        }
                    }
                }
            }
        }

        public static void RevertRedirections()
        {
            Assembly callingAssembly = Assembly.GetCallingAssembly();

            for (int i = s_redirections.Count - 1; i >= 0; --i)
            {
                if (s_redirections[i].OriginalMethod.DeclaringType.Assembly == callingAssembly)
                {
                    s_redirections[i].Dispose();
                    s_redirections.RemoveAt(i);
                }
            }
        }
    }
}
