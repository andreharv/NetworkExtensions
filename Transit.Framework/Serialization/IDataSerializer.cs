using ICities;

namespace Transit.Framework.Serialization
{
    public interface IDataSerializer<TData>
    {
        TData DeserializeData(ISerializableData gameData);
        void SerializeData(ISerializableData gameData, TData data);
    }
}
