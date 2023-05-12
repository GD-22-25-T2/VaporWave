namespace UDK.API.Extensions.NonAllocLINQ
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Helper class with methods to easily interact with dictionaries.
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Iterates over the specified collection.
        /// </summary>
        /// <typeparam name="TKey">The key.</typeparam>
        /// <typeparam name="TVal">The value.</typeparam>
        /// <param name="target">The target to iterate on.</param>
        /// <param name="action">The action to be performed.</param>
        public static void ForEach<TKey, TVal>(this Dictionary<TKey, TVal> target, Action<KeyValuePair<TKey, TVal>> action)
        {
            foreach (KeyValuePair<TKey, TVal> obj in target)
                action(obj);
        }

        /// <summary>
        /// Converts an array to dictionary.
        /// </summary>
        /// <typeparam name="TKey">The key.</typeparam>
        /// <typeparam name="TArrItem">The array item type.</typeparam>
        /// <param name="target">The target to iterate on.</param>
        /// <param name="array">The array to convert.</param>
        /// <param name="selector">The function selector.</param>
        public static void FromArray<TKey, TArrItem>(this Dictionary<TKey, TArrItem> target, TArrItem[] array, Func<TArrItem, TKey> selector)
        {
            int num = array.Length;
            for (int i = 0; i < num; i++)
            {
                TArrItem tarrItem = array[i];
                target[selector(tarrItem)] = tarrItem;
            }
        }

        /// <summary>
        /// Iterates over all the keys of the specified collection.
        /// </summary>
        /// <typeparam name="TKey">Th key.</typeparam>
        /// <typeparam name="TVal">The value.</typeparam>
        /// <param name="target">The target to iterate on.</param>
        /// <param name="action">The action to be performed.</param>
        public static void ForEachKey<TKey, TVal>(this Dictionary<TKey, TVal> target, Action<TKey> action) => target.ForEach((KeyValuePair<TKey, TVal> x) => action(x.Key));

        /// <summary>
        /// Iterates over all the value of the specified collection.
        /// </summary>
        /// <typeparam name="TKey">The key.</typeparam>
        /// <typeparam name="TVal">The value.</typeparam>
        /// <param name="target">The target to iterate on.</param>
        /// <param name="action">The action to be performed.</param>
        public static void ForEachValue<TKey, TVal>(this Dictionary<TKey, TVal> target, Action<TVal> action) => target.ForEach((KeyValuePair<TKey, TVal> x) => action(x.Value));

        /// <summary>
        /// Gets the amount of elements matching the condition in the specified collection.
        /// </summary>
        /// <typeparam name="TKey">The key.</typeparam>
        /// <typeparam name="TVal">The value.</typeparam>
        /// <param name="target">The target to iterate on.</param>
        /// <param name="condition">The iteration condition.</param>
        /// <returns>The amount of elements matching the specified condition.</returns>
        public static int Count<TKey, TVal>(this Dictionary<TKey, TVal> target, Func<KeyValuePair<TKey, TVal>, bool> condition)
        {
            int num = 0;
            foreach (KeyValuePair<TKey, TVal> arg in target)
            {
                if (condition(arg))
                    num++;
            }

            return num;
        }

        /// <summary>
        /// Gets a value indicating whether the specified collection contains an element matching the condition.
        /// </summary>
        /// <typeparam name="TKey">The key.</typeparam>
        /// <typeparam name="TVal">The value.</typeparam>
        /// <param name="target">The target to iterate on.</param>
        /// <param name="condition">The iteration condition.</param>
        /// <returns><see langword="true"/> if an element matching the specified condition was found; otherwise, <see langword="false"/>.</returns>
        public static bool Any<TKey, TVal>(this Dictionary<TKey, TVal> target, Func<KeyValuePair<TKey, TVal>, bool> condition)
        {
            foreach (KeyValuePair<TKey, TVal> arg in target)
            {
                if (condition(arg))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Gets a value indicating whether all elements of a specified collection match the condition.
        /// </summary>
        /// <typeparam name="TKey">The key.</typeparam>
        /// <typeparam name="TVal">The value.</typeparam>
        /// <param name="target">The target to iterate on.</param>
        /// <param name="condition">The iteration condition.</param>
        /// <returns><see langword="true"/> if all elements match the specified condition; otherwise, <see langword="false"/>.</returns>
        public static bool All<TKey, TVal>(this Dictionary<TKey, TVal> target, Func<KeyValuePair<TKey, TVal>, bool> condition)
        {
            foreach (KeyValuePair<TKey, TVal> arg in target)
            {
                if (!condition(arg))
                    return false;
            }

            return true;
        }
    }
}
