
namespace Transit.Framework.Prerequisites
{
    public interface IModPrerequisite
    {
        void Install();

        void Uninstall();
    }
}
