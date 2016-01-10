using Transit.Framework.Interfaces;

namespace Transit.Framework
{
    public abstract class Activable : IActivable
    {
        public bool IsEnabled { get; set; }
    }
}
