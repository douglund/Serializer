using System;

namespace Nova.Serializer
{
    public class TypeModel
    {
        public TypeModel(Type type)
        {
            Type = type;
        }

        public Type Type { get; set; }

        public string DisplayName
        {
            get { return $"{Type.Name} ({Type.Namespace})"; }
        }

        public override string ToString()
        {
            return DisplayName;
        }
    }
}