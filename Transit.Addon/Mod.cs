using ICities;
using Transit.Framework.Modularity;

namespace Transit.Addon
{
    public sealed partial class Mod : ITransitMod
    {
        public string Name
        {
            get
            {
                OnGameLoaded();
                return _name;
            }
        }

        public string Description
        {
            get
            {
                OnGameLoaded();
                return _description;
            }
        }

        public ulong WorkshopId
        {
            get { return 543703997; }
        }
    }
}
