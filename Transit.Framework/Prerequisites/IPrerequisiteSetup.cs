
namespace Transit.Framework.Prerequisites
{
    public interface IPrerequisiteSetup
    {
        void Install(PrerequisiteType type);

        void Uninstall(PrerequisiteType type);
    }
}
