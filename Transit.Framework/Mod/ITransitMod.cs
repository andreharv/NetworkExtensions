using ICities;
using Transit.Framework.Prerequisites;

namespace Transit.Framework.Mod
{
    public interface ITransitMod : IUserMod
    {
        string AssetPath { get; }
        TransitModType Type { get; }
        PrerequisiteType Requirements { get; }
        void SaveSettings();
        void LoadSettings();
    }

    public enum TransitModType
    {
        Master,
        Standalone
    }
}
