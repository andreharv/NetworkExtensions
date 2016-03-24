using System.Collections.Generic;
using ColossalFramework.Globalization;

namespace Transit.Framework.UI
{
    public partial class MenuManager
    {
        private readonly ICollection<string> _installedLocCatKeys = new HashSet<string>();
        private readonly ICollection<string> _installedLocToolbarKeys = new HashSet<string>();

        public void InstallLocalization(Locale locale)
        {
            foreach (var kvp in _categoryBuilders)
            {
                if (!_installedLocCatKeys.Contains(kvp.Value.Name))
                {
                    locale.CreateMenuTitleLocalizedString(kvp.Value.Name, kvp.Value.DisplayName);
                    _installedLocCatKeys.Add(kvp.Value.Name);
                }
            }

            foreach (var tbItem in _toolbarItems)
            {
                if (!_installedLocToolbarKeys.Contains(tbItem.Name))
                {
                    locale.CreateMenuTitleLocalizedString(tbItem.Name, tbItem.DisplayName);
                    locale.CreateTutorialTitleLocalizedString(tbItem.Name, tbItem.DisplayName);
                    locale.CreateTutorialContentLocalizedString(tbItem.Name, tbItem.Tutorial);
                    _installedLocToolbarKeys.Add(tbItem.Name);
                }
            }
        }
    }
}
