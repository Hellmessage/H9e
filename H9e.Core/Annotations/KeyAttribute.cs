using System;

namespace H9e.Core.Annotations {
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class KeyAttribute : Attribute {
    }
}
