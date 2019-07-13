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
            do {
                result = rng.Next(list.Count);   
            }         
            while (lastResults.Contains(result));

            lastResults.Enqueue(result);

            if (lastResults.Count > 5)
                lastResults.Dequeue();

            return list[result];
        }


    }
}
