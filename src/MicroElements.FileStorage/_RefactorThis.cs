using System;
using System.Linq;

namespace MicroElements.FileStorage
{
    /// <summary>
    /// Data snapshot represents data state in time.
    /// </summary>
    public interface IDataSnapshot
    {
    }

    public class SnapshotInfo
    {
        public DateTime Timestamp { get; set; }
    }

    public interface IDataAddon
    {
    }

    public interface ISession
    {
        void Save();
    }

    public interface IDataValidator { }
    public interface IDataConvertor { }


}
