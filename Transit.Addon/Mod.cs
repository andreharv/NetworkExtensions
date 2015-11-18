using ICities;

namespace Transit.Addon
{
    public sealed partial class Mod : IUserMod
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
    }
}
