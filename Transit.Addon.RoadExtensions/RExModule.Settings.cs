using ICities;
using Transit.Framework.Interfaces;

namespace Transit.Addon.RoadExtensions
{
    public partial class RExModule
    {
        public override void OnSettingsUI(UIHelperBase helper)
        {
            var uIHelperBase = helper.AddGroup("Road Extensions Options");
            var optionsChanged = false;

            foreach (var part in ActivableParts)
            {
                var partLocal = part;
                var partName = part.GetSerializableName();

                if (!Options.Instance.PartsEnabled.ContainsKey(partName))
                {
                    Options.Instance.PartsEnabled[partName] = true;
                    optionsChanged = true;
                }

                uIHelperBase.AddCheckbox(
                    part.DisplayName, 
                    Options.Instance.PartsEnabled[partName], 
                    isChecked =>
                    {
                        Options.Instance.PartsEnabled[partName] = partLocal.IsEnabled = isChecked;
                        Options.Instance.Save();
                    });
            }

            if (optionsChanged)
            {
                Options.Instance.Save();
            }
        }
    }
}
