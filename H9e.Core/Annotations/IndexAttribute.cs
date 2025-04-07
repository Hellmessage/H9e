using System;

namespace H9e.Core.Annotations {
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class IndexAttribute : Attribute {
        public string Name { get; private set; }
        public bool Unique { get; set; }
        public int Order { get; set; }

        public IndexAttribute(string name, bool unique = false, int order = 0) {
            Name = name;
            Unique = unique;
            Order = order;
        }

        public IndexAttribute(int order) {
            Order = order;
        }

    }
}
