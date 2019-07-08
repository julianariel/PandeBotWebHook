using System;
using System.Collections.Generic;
using System.Text;

namespace PandeBot
{
    public static class CollectionExtension
    {
        private static Random rng = new Random();

        public static T RandomElement<T>(this IList<T> list)
        {
            return list[rng.Next(list.Count)];
        }


    }
}
