using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmpyrionModdingFramework.Database
{
    public enum DatabaseSubComponent
    {
        Tethers,
        Events,
        Inventory
    }
    public interface IDatabaseManager
    {
        void SaveRecord<T>(string recordId, T record, bool clearExisting);
        void SaveRecords<T>(string recordId, List<T> records, bool clearExisting);
        List<T> LoadRecords<T>(string recordId);
        List<T> LoadAllRecords<T>(string groupName, Action<string> action);
        bool DeleteRecord(string recordId);
        string _databasePath { get; }

    }

    public interface IDatabaseRecord
    {
        string RecordName { get; }
    }
}