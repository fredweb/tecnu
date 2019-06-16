namespace XNuvem.Data.Schema
{
    public interface ISchemaUpdate
    {
        /// <summary>
        ///     Verify if the database has updates
        /// </summary>
        /// <returns></returns>
        bool HasUpdates();

        /// <summary>
        ///     This method drop and create the database
        /// </summary>
        void CreateDatabase();

        /// <summary>
        ///     This method update database preservating the data on existing database
        /// </summary>
        void UpdateDatabse();
    }
}