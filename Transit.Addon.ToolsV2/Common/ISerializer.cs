using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICities;

namespace Transit.Addon.ToolsV2.Common
{
    public interface ISerializer<T>
    {
        T DeserializeData(ISerializableData gameData);
        void SerializeData(ISerializableData gameData, T data);
    }
}
