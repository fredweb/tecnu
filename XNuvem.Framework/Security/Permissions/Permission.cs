namespace XNuvem.Security.Permissions
{
    public class Permission
    {
        public virtual string Name { get; set; }
        public virtual string Parent { get; set; }
        public virtual string Category { get; set; }
        public virtual int Position { get; set; }
        public virtual string Description { get; set; }
        public virtual string Summary { get; set; }
    }
}