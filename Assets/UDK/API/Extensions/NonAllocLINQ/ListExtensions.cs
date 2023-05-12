namespace UDK.API.Extensions.NonAllocLINQ
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Helper class with methods to easily interact with lists.
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// Iterates over the specified collection.
        /// </summary>
        /// <typeparam name="T">The box type.</typeparam>
        /// <param name="target">The target to iterate on.</param>
        /// <param name="action">The action to be performed.</param>
        public static void ForEach<T>(this List<T> target, Action<T> action)
        {
            foreach (T obj in target)
                action(obj);
        }

        /// <summary>
        /// Gets a value indicating whether the specified collection contains an element matching the condition.
        /// </summary>
        /// <typeparam name="T">The box type.</typeparam>
        /// <param name="target">The target to iterate on.</param>
        /// <param name="condition">The iteration condition.</param>
        /// <returns><see langword="true"/> if an element matching the specified condition was found; otherwise, <see langword="false"/>.</returns>
        public static bool Any<T>(this List<T> target, Func<T, bool> condition)
        {
            foreach (T arg in target)
            {
                if (condition(arg))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the amount of elements matching the condition in the specified collection.
        /// </summary>
        /// <typeparam name="T">The box type.</typeparam>
        /// <param name="target">The target to iterate on.</param>
        /// <param name="condition">The iteration condition.</param>
        /// <returns>The amount of elements matching the specified condition.</returns>
        public static int Count<T>(this List<T> target, Func<T, bool> condition)
        {
            int num = 0;
            foreach (T arg in target)
            {
                if (condition(arg))
                    num++;
            }

            return num;
        }

        /// <summary>
        /// Gets a value indicating whether all elements of a specified collection match the condition.
        /// </summary>
        /// <typeparam name="T">The box type.</typeparam>
        /// <param name="target">The target to iterate on.</param>
        /// <param name="condition">The iteration condition.</param>
        /// <param name="emptyResult">Whether empty collections are allowed.</param>
        /// <returns><see langword="true"/> if all elements match the specified condition; otherwise, <see langword="false"/>.</returns>
        public static bool All<T>(this List<T> target, Func<T, bool> condition, bool emptyResult = true)
        {
            foreach (T arg in target)
            {
                if (!condition(arg))
                    return false;

                emptyResult = true;
            }

            return emptyResult;
        }

        /// <summary>
        /// Gets the first element matching the specified condition given a collection to iterate on.
        /// </summary>
        /// <typeparam name="T">The box type.</typeparam>
        /// <param name="target">The target to iterate on.</param>
        /// <param name="condition">The iteration condition.</param>
        /// <param name="defaultRet">The default return type.</param>
        /// <returns>The first element matching the specified condition, or <see langword="default"/> if not found.</returns>
        public static T FirstOrDefault<T>(this List<T> target, Func<T, bool> condition, T defaultRet)
        {
            foreach (T t in target)
            {
                if (condition(t))
                    return t;
            }

            return defaultRet;
        }

        /// <summary>
        /// Tries to get the first element matching the specified condition given a collection to interate on.
        /// </summary>
        /// <typeparam name="T">The box type.</typeparam>
        /// <param name="target">The target to iterate on.</param>
        /// <param name="condition">The iteration condition.</param>
        /// <param name="first">The found element.</param>
        /// <returns><see langword="true"/> if an ellement matching the specified condition was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGetFirst<T>(this List<T> target, Func<T, bool> condition, out T first)
        {
            foreach (T t in target)
            {
                if (condition(t))
                {
                    first = t;
                    return true;
                }
            }

            first = default(T);
            return false;
        }
    }
}
