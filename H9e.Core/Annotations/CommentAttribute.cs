using System;

namespace H9e.Core.Annotations {
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = true)]
    public class CommentAttribute : Attribute {
        public string Comment { get; private set; }

        public CommentAttribute(string comment) {
            Comment = comment;
        }

    }
}
