using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;

namespace EmpyrionModdingFramework.Database
{
    public class CsvManager: IDatabaseManager
    {
        private const string GitIgnoreFileName = ".gitignore";
        public string _databasePath { get; }

        public CsvManager(string databasePath)
        {
            _databasePath = databasePath;
        }

        public List<T> LoadRecords<T>(string fileName)
        {
            return LoadCsvRows<T>(fileName);
        }

        public List<T> LoadAllRecords<T>(string folderName, Action<string> action)
        {
            var allRecords = new List<T>();
            var path = FormatFilePath(folderName);
            var files = Directory.GetFiles(path).Where(name => !name.Contains(GitIgnoreFileName));

            action("Files: " + files.Count());
            foreach (var file in files)
            {
                action("file: " + file);
                var records = LoadCsvRows<T>(file);
                allRecords.AddRange(records);
            }

            return allRecords;
        }

        private List<T> LoadCsvRows<T>(string fileName)
        {
            var path = FormatFilePath(fileName);

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                NewLine = Environment.NewLine,
                Delimiter = ","
            };

            var rows = new List<T>();

            try
            {
                using (var stream = new StreamReader(path))
                using (var csv = new CsvReader(stream, config))
                    rows = csv.GetRecords<T>().ToList();
            }
            catch {
                return null;
            }

            return rows;
        }

        public void SaveRecord<T>(string fileName, T record, bool clearExisting)
        {
            var path = FormatFilePath(fileName);

            var fileExists = File.Exists(path);

            if (fileExists && clearExisting)
            {
                File.Delete(path);
            }

            var config = new CsvConfiguration(CultureInfo.InvariantCulture);
            config.NewLine = "\r\n";

            if (!clearExisting && fileExists)
            {
                config.HasHeaderRecord = true;
                using (var stream = new StreamWriter(path, true))
                using (var csv = new CsvWriter(stream, config))
                {
                    csv.NextRecord();
                    csv.WriteRecord(record);
                }
            }
            else
            {
                using (var stream = new StreamWriter(path, false, System.Text.Encoding.UTF8))
                using (var csv = new CsvWriter(stream, config))
                {
                    csv.WriteHeader(typeof(T));
                    csv.NextRecord();
                    csv.WriteRecord(record);
                }
            }      
        }

        private string FormatFilePath(string filename)
        {
            return Path.Combine(_databasePath, filename);
        }

        public void SaveRecords<T>(string fileName, List<T> records, bool clearExisting)
        {
            var path = FormatFilePath(fileName);
            if (clearExisting && File.Exists(path))
                File.Delete(path);

            foreach (T record in records)
                SaveRecord<T>(fileName, record, false);
        }

        public bool DeleteRecord(string fileName)
        {
            var path = FormatFilePath(fileName);

            if (File.Exists(path))
            {
                File.Delete(path);
                return true;
            }

            return false;
        }
    }
}
