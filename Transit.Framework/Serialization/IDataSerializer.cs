using ICities;

namespace Transit.Framework.Serialization
{
    public interface IDataSerializer<TData>
    {
        TData DeserializeData(ISerializableData gameData, string dataId);
        void SerializeData(ISerializableData gameData, string dataId, TData data);
    }
}
