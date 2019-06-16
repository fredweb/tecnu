using System;

namespace XNuvem.Environment.Configuration
{
    [Serializable]
    public class ConnectionSettings : ICloneable
    {
        public ConnectionSettings()
        {
        }

        public ConnectionSettings(string connectionString, string dataProvider)
        {
            DataConnectionString = connectionString;
            DataProvider = dataProvider;
        }

        public string DataConnectionString { get; set; }
        public string DataProvider { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public object Clone()
        {
            return new ConnectionSettings
            {
                DataConnectionString = DataConnectionString,
                DataProvider = DataProvider,
                CreatedAt = CreatedAt,
                UpdatedAt = UpdatedAt
            };
        }
    }
}