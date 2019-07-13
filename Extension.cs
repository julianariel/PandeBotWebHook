using System;
using System.Collections.Generic;

namespace PandeBot
{
    public static class CollectionExtension
    {
        private static Random rng = new Random();

        public static T RandomElement<T>(this IList<T> list, Queue<int> lastResults)
        {
            int result;
            int maxCount = (list.Count < 10) ? list.Count : 10;

            do {
                result = rng.Next(list.Count);   
            }         
            while (lastResults.Contains(result) && list.Count > 1);

            lastResults.Enqueue(result);

            if (lastResults.Count >= maxCount)
                lastResults.Dequeue();

            return list[result];
        }


    }
}
