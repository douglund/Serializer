using System.Collections.Generic;

namespace Nova.Serializer.Models
{
    public class ListModel<T> : List<T>
    {
        public ListModel(T item)
        {
            this.Add(item);
        }
    }
}