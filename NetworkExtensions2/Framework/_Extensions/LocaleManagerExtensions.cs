using System.Reflection;
using ColossalFramework.Globalization;

namespace Transit.Framework
{
    public static class LocaleManagerExtensions
    {
        private static readonly FieldInfo s_localeField = typeof(LocaleManager).GetFieldByName("m_Locale");

        public static Locale GetLocale(this LocaleManager localeManager)
        {
            return (Locale)s_localeField.GetValue(localeManager);
        }
    }
}
