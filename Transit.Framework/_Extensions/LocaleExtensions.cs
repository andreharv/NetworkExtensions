using ColossalFramework.Globalization;

namespace Transit.Framework
{
    public static class LocaleExtensions
    {
        public static void CreateMenuTitleLocalizedString(this Locale locale, string key, string label)
        {
            locale.AddLocalizedString(new Locale.Key()
            {
                m_Identifier = "MAIN_CATEGORY",
                m_Key = key
            }, label);
        }

        public static void CreateNetTitleLocalizedString(this Locale locale, string key, string label)
        {
            locale.AddLocalizedString(new Locale.Key
            {
                m_Identifier = "NET_TITLE",
                m_Key = key
            }, label);
        }

        public static void CreateTutorialTitleLocalizedString(this Locale locale, string key, string label)
        {
            locale.AddLocalizedString(new Locale.Key
            {
                m_Identifier = "TUTORIAL_ADVISER_TITLE",
                m_Key = key
            }, label);
        }

        public static void CreateTutorialContentLocalizedString(this Locale locale, string key, string label)
        {
            locale.AddLocalizedString(new Locale.Key
            {
                m_Identifier = "TUTORIAL_ADVISER",
                m_Key = key
            }, label);
        }

        public static void CreateNetDescriptionLocalizedString(this Locale locale, string key, string label)
        {
            locale.AddLocalizedString(new Locale.Key
            {
                m_Identifier = "NET_DESC",
                m_Key = key
            }, label);
        }
    }
}
