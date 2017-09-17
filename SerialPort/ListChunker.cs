using System.Collections.Generic;
using System.Linq;

namespace SerialPort
{
    /// <summary>
    /// Addition class to list
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// Srinks lits to smaller lists
        /// </summary>
        /// <typeparam name="T">Тypе оf elements</typeparam>
        /// <param name="source">List to chunk</param>
        /// <param name="chunkSize">Elements in new lists</param>
        /// <returns>List of new lists</returns>
        public static List<List<T>> ChunkBy<T>(this List<T> source, int chunkSize) => source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
    }
}