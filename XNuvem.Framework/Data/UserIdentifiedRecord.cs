using System;

namespace XNuvem.Data
{
    public class UserIdentifiedRecord
    {
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public static class UserIdentifiedClassMapExtensions
    {
        public static void MapUserIdentified<T>(this EntityMap<T> that) where T : UserIdentifiedRecord
        {
            that.Map(m => m.CreatedBy).Length(100).Not.Nullable();
            that.Map(m => m.CreatedAt).Not.Nullable().Default("(getdate())");
            that.Map(m => m.UpdatedBy).Length(100).Nullable();
            that.Map(m => m.UpdatedAt).Nullable();
        }
    }
}