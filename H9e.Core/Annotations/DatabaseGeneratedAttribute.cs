using System;

namespace H9e.Core.Annotations {

    public enum DatabaseGeneratedOption {
        None,
        Identity,
        Computed
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class DatabaseGeneratedAttribute :Attribute {

        public DatabaseGeneratedOption DatabaseGeneratedOption { get; private set; }
        public DatabaseGeneratedAttribute(DatabaseGeneratedOption option) {
            DatabaseGeneratedOption = option;
        }
    }
}
