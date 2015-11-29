using Transit.Framework.Modularity;

namespace NetworkExtensions
{
    public sealed partial class Mod : ITransitMod
    {
        public string Name
        {
            get
            {
                OnGameLoaded();
                return NAME;
            }
        }

        public string Description
        {
            get
            {
                OnGameLoaded();
                return DESCRIPTION;
            }
        }

        public ulong WorkshopId
        {
            get { return 478820060; }
        }
    }
}
