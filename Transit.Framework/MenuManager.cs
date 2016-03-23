using System.Collections.Generic;
using ColossalFramework.Globalization;

namespace Transit.Framework
{
    public partial class MenuManager
    {
        private readonly ICollection<string> _installedKeys = new HashSet<string>();

        public void InstallLocalization(Locale locale)
        {
            foreach (var kvp in _categoryBuilders)
            {
                if (!_installedKeys.Contains(kvp.Value.Name))
                {
                    locale.CreateMenuTitleLocalizedString(kvp.Value.Name, kvp.Value.DisplayName);
                    _installedKeys.Add(kvp.Value.Name);
                }
            }
        }
    }
}
