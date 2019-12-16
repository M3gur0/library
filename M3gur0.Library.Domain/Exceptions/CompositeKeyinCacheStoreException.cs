using System;

namespace M3gur0.Library.Domain.Exceptions
{
    public class CompositeKeyinCacheStoreException : Exception
    {
        public CompositeKeyinCacheStoreException() : base("Unable to store object with multiple keys in repository. Please, use projection to transform object before storing it.")
        {
        }
    }
}
