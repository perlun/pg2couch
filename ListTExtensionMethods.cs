using System.Collections.Generic;
using System.Linq;

namespace Pg2Couch
{
    public static class ListTExtensionMethods
    {
        // Inspired by https://stackoverflow.com/a/6362642/227779. I deliberately changed this to operate on List<T>
        // instead of IEnumerable<T> since the former has O(n) performance while the latter can have O(n^2)
        // characteristics.
    
        /// <summary>
        /// Break a list of items into chunks of a specific size.
        /// </summary>
        public static IEnumerable<IEnumerable<T>> Chunk<T>(this List<T> source, int chunksize)
        {
            var remainingSource = (IEnumerable<T>)source;
        
            while (remainingSource.Any())
            {
                yield return remainingSource.Take(chunksize);
                remainingSource = remainingSource.Skip(chunksize);
            }
        }
    }
}
