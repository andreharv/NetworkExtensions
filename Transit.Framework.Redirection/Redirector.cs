using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Transit.Framework.Redirection
{
    public abstract class RedirectAttribute : Attribute
    {
        public RedirectAttribute(Type classType, string methodName, ulong bitSetOption = 0)
        {
            ClassType = classType;
            MethodName = methodName;
            BitSetRequiredOption = bitSetOption;
        }

        public RedirectAttribute(Type classType, ulong bitSetOption = 0)
            : this(classType, null, bitSetOption)
        { }

        public Type ClassType { get; set; }
        public string MethodName { get; set; }
        public ulong BitSetRequiredOption { get; set; }
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
        public RedirectFromAttribute(Type classType, string methodName, ulong bitSetOption = 0)
            : base(classType, methodName, bitSetOption)
        { }

        public RedirectFromAttribute(Type classType, ulong bitSetOption = 0)
            : base(classType, bitSetOption)
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
        public RedirectToAttribute(Type classType, string methodName, ulong bitSetOption = 0)
            : base(classType, methodName, bitSetOption)
        { }

        public RedirectToAttribute(Type classType, ulong bitSetOption = 0)
            : base(classType, bitSetOption)
        { }
    }

    public static class Redirector
    {
        internal class MethodRedirection : IDisposable
        {
            private bool _isDisposed = false;

            public MethodInfo OriginalMethod { get; private set; }
            public MethodInfo NewMethod { get; private set; }
            public Assembly RedirectionSource { get; private set; }
            public ulong BitSetRequiredOption { get; private set; }
            private readonly RedirectCallsState _callsState;

            public MethodRedirection(MethodInfo originalMethod, MethodInfo newMethod, Assembly redirectionSource, ulong bitSetOption)
            {
                OriginalMethod = originalMethod;
                NewMethod = newMethod;
                RedirectionSource = redirectionSource;
                BitSetRequiredOption = bitSetOption;
                _callsState = RedirectionHelper.RedirectCalls(OriginalMethod, NewMethod);
            }

            public void Dispose()
            {
                if (!_isDisposed)
                {
                    RedirectionHelper.RevertRedirect(OriginalMethod, _callsState);
                    OriginalMethod = null;
                    _isDisposed = true;
                }
            }
        }

        private static List<MethodRedirection> s_redirections = new List<MethodRedirection>();

        public static void PerformRedirections(ulong bitMask = 0)
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
                    if (redirectAttr.BitSetRequiredOption != 0 && (bitMask & redirectAttr.BitSetRequiredOption) == 0)
                        continue;

                    string originalName = String.IsNullOrEmpty(redirectAttr.MethodName) ? method.Name : redirectAttr.MethodName;

                    MethodInfo originalMethod = null;
                    foreach (MethodInfo m in redirectAttr.ClassType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static))
                    {
                        if (m.Name != originalName)
                            continue;

                        if (method.IsCompatibleWith(m))
                        {
                            originalMethod = m;
                            break;
                        }
                    }

                    if (originalMethod == null)
                    {
                        throw new Exception(string.Format("TFW: Original method {0} has not been found for redirection", originalName));
                    }

                    if (redirectAttr is RedirectFromAttribute)
                    {
                        if (!s_redirections.Any(r => r.OriginalMethod == originalMethod))
                        {
                            Log.Info(string.Format("TFW: Redirecting from {0}.{1} to {2}.{3}",
                                originalMethod.DeclaringType,
                                originalMethod.Name,
                                method.DeclaringType,
                                method.Name));
                            s_redirections.Add(originalMethod.RedirectTo(method, callingAssembly, redirectAttr.BitSetRequiredOption));
                        }
                    }

                    if (redirectAttr is RedirectToAttribute)
                    {
                        if (!s_redirections.Any(r => r.OriginalMethod == method))
                        {
                            Log.Info(string.Format("TFW: Redirecting from {0}.{1} to {2}.{3}",
                                method.DeclaringType,
                                method.Name,
                                originalMethod.DeclaringType,
                                originalMethod.Name));
                            s_redirections.Add(method.RedirectTo(originalMethod, callingAssembly, redirectAttr.BitSetRequiredOption));
                        }
                    }
                }
            }
        }

        public static void RevertRedirections(ulong bitMask = 0)
        {
            Assembly callingAssembly = Assembly.GetCallingAssembly();

            for (int i = s_redirections.Count - 1; i >= 0; --i)
            {
                var redirection = s_redirections[i];

                if (!Equals(redirection.RedirectionSource, callingAssembly))
                {
                    continue;
                }

                if (redirection.BitSetRequiredOption != 0 && (bitMask & redirection.BitSetRequiredOption) == 0)
                    continue;

                Log.Info(string.Format("TFW: Removing redirection from {0}.{1} to {2}.{3}",
                    redirection.OriginalMethod.DeclaringType,
                    redirection.OriginalMethod.Name,
                    redirection.NewMethod.DeclaringType,
                    redirection.NewMethod.Name));
                redirection.Dispose();
                s_redirections.RemoveAt(i);
            }
        }
    }
}
