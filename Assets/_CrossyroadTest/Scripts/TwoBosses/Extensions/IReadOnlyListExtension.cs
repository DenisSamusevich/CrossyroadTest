using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets._CrossyroadTest.Scripts.TwoBosses.Extensions
{
    internal static class IReadOnlyListExtension
    {
        public static List<T> GetRandom<T>(this IReadOnlyList<T> list, int countRandomT)
        {
            if (countRandomT > list.Count)
            {
                throw new System.ArgumentException();
            }
            var listInt = Enumerable.Range(0, list.Count).ToList();
            var randomT = new List<T>();
            for (int i = 0; i < countRandomT; i++)
            {
                int index = Random.Range(0, listInt.Count);
                randomT.Add(list[listInt[index]]);
                listInt.RemoveAt(index);
            }
            return randomT;
        }

        public static T GetRandom<T>(this IReadOnlyList<T> list)
        {
            if (list.Count == 0)
            {
                throw new System.ArgumentException();
            }
            return list[Random.Range(0, list.Count)];
        }
    }
}
