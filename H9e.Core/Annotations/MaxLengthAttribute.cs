using System;

namespace H9e.Core.Annotations {
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class MaxLengthAttribute : Attribute {
        public int Length { get; private set; }

        public MaxLengthAttribute(int Length) {
            this.Length = Length;
        }
    }
}
