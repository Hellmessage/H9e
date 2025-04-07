using System;

namespace H9e.Core.Annotations {
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class DefaultValueAttribute : Attribute {
        public object Value { get; private set; }
        public DefaultValueAttribute(object value) {
            Value = value;
        }
    }
}
