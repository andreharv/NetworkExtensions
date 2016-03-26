using System;
using System.Linq;
using Transit.Framework;

namespace Transit.Addon.TM.Data
{
    [Serializable]
    public class TAMLaneRoute
    {
        public uint LaneId { get; set; }
        public ushort NodeId { get; set; }
        public uint[] Connections { get; set; }

        public TAMLaneRoute()
        {
            Connections = new uint[0];
        }

        /// <summary>
        /// Adds a connection from m_laneId to laneId.
        /// </summary>
        /// <param name="laneId"></param>
        /// <returns></returns>
        public bool AddConnection(uint laneId)
        {
            while (true)
            {
                try
                {
                    if (Connections.Contains(laneId))
                        return false; // already connected

                    // expand the array & add the lane
                    var editableConnections = Connections.ToList();
                    editableConnections.Add(laneId);
                    Connections = editableConnections.ToArray();
                    return true;
                }
                catch
                {
                    // we might get an IndexOutOfBounds here since we are not locking
                }
            }
        }

        /// <summary>
        /// Removes a connection from m_laneId to laneId.
        /// </summary>
        /// <param name="laneId"></param>
        /// <returns></returns>
        public bool RemoveConnection(uint laneId)
        {
            while (true)
            {
                try
                {
                    if (!Connections.Contains(laneId))
                        return false; // already not connected

                    // shrink the array
                    var editableConnections = Connections.ToList();
                    editableConnections.RemoveIfAny(laneId);
                    Connections = editableConnections.ToArray();
                    return true;
                }
                catch
                {
                    // we might get an IndexOutOfBounds here since we are not locking
                }
            }
        }
    }
}
