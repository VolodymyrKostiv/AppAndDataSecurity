using System;
using System.Linq;

namespace Lab_2.Helpers
{
    public static class ArraysHelper
    {
        public static T[] ConcatArrays<T>(params T[][] arrays)
        {
            var position = 0;
            var outputArray = new T[arrays.Sum(a => a.Length)];

            foreach (var a in arrays)
            {
                Array.Copy(a, 0, outputArray, position, a.Length);
                position += a.Length;
            }

            return outputArray;
        }
    }
}
