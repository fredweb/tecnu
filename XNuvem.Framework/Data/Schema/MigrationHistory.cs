using System;

namespace XNuvem.Data.Schema
{
    [Serializable]
    public class MigrationHistory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public int LongVersion { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class MigrationHistoryMap : EntityMap<MigrationHistory>
    {
        public MigrationHistoryMap()
        {
            Id(x => x.Id).GeneratedBy.Identity();
            Map(X => X.Name).Length(100).Not.Nullable();
            Map(x => x.Version).Length(30).Not.Nullable();
            Map(x => x.LongVersion).Not.Nullable();
            Map(x => x.CreatedAt);
            Map(x => x.UpdatedAt);
        }
    }
}