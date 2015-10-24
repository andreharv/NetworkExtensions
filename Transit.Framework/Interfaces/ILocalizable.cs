namespace Transit.Framework.Interfaces
{
    public interface ILocalizable : IIdentifiable, IDisplayable
    {
        string Description { get; }
    }
}
