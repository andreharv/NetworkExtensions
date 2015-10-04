using ICities;

namespace Transit.Addon
{
    public sealed partial class Mod : IUserMod
    {
        public string Name
        {
            get
            {
                return "Transit Addon Mod";
            }
        }

        public string Description
        {
            get
            {
                OnGameLoaded();
                return "An addition of mysterious goodies! :D";
            }
        }
    }
}
