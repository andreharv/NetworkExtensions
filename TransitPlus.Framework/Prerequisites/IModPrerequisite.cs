
namespace TransitPlus.Framework.Prerequisites
{
    public interface IModPrerequisite
    {
        void Install();

        void Uninstall();
    }
}
