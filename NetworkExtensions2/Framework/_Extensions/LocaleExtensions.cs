using ColossalFramework.Globalization;

namespace Transit.Framework
{
    public static class LocaleExtensions
    {
        public static void CreateMenuTitleLocalizedString(this Locale locale, string key, string label)
        {

            var k = new Locale.Key()
            {
                m_Identifier = "MAIN_CATEGORY",
                m_Key = key
            };
            if (!Locale.Exists("MAIN_CATEGORY", key))
                locale.AddLocalizedString(k, label);
        }

        public static void CreateNetTitleLocalizedString(this Locale locale, string key, string label)
        {
            var k = new Locale.Key()
            {
                m_Identifier = "NET_TITLE",
                m_Key = key

            };
            if (!Locale.Exists("NET_TITLE", key))
                locale.AddLocalizedString(k, label);
        }

        public static void CreateNetDescriptionLocalizedString(this Locale locale, string key, string label)
        {
            var k = new Locale.Key()
            {
                m_Identifier = "NET_DESC",
                m_Key = key
            };
            if (!Locale.Exists("NET_DESC", key))
                locale.AddLocalizedString(k, label);
        }
    }
}
