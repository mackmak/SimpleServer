using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerLibrary
{
    public static class Utils
    {
        public static string SplitStringByChar(string source, string character, int timesToSplit)
        {
            StringBuilder resultString = new StringBuilder();
            var charIndex = source.LastIndexOf(character);


            for (int i = 0; i < timesToSplit; i++)
            {
                if (charIndex != -1)
                {
                    resultString.Append(source.Substring(0, charIndex - 1));
                    charIndex = resultString.ToString().LastIndexOf(character);
                }
            }

            return resultString.ToString();

        }

        /// <summary>
		/// If the object is null, performs the action and returns true.
		/// </summary>
		public static bool IfNull<T>(this T obj, Action action)
        {
            bool ret = obj == null;

            if (ret) { action(); }

            return ret;
        }



        public static bool If<T>(this T v, Func<T, bool> predicate, Action<T> action)
        {
            bool ret = predicate(v);

            if (ret)
            {
                action(v);
            }

            return ret;
        }

        /// <summary>
		/// Implements a ForEach for generic enumerators.
		/// </summary>
		public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (var item in collection)
            {
                action(item);
            }
        }

        public static string LeftOf(this String src, string s)
        {
            string ret = src;
            int idx = src.IndexOf(s);

            if (idx != -1)
            {
                ret = src.Substring(0, idx);
            }

            return ret;
        }

        public static string RightOf(this String src, string s)
        {
            string ret = String.Empty;
            int idx = src.IndexOf(s);

            if (idx != -1)
            {
                ret = src.Substring(idx + s.Length);
            }

            return ret;
        }

        public static string LeftOf(this String src, char c)
        {
            return StringHelpers.LeftOf(src, c);
        }

        public static string LeftOfRightmostOf(this String src, char c)
        {
            return StringHelpers.LeftOfRightmostOf(src, c);
        }

        public static string LeftOfRightmostOf(this String src, string s)
        {
            string ret = src;
            int idx = src.IndexOf(s);
            int idx2 = idx;

            while (idx2 != -1)
            {
                idx2 = src.IndexOf(s, idx + s.Length);

                if (idx2 != -1)
                {
                    idx = idx2;
                }
            }

            if (idx != -1)
            {
                ret = src.Substring(0, idx);
            }

            return ret;
        }

        public static string RightOf(this String src, char c)
        {
            return StringHelpers.RightOf(src, c);
        }

        public static string RightOfRightmostOf(this String src, char c)
        {
            return StringHelpers.RightOfRightmostOf(src, c);
        }

        public static class StringHelpers
        {

            /// <summary>
            /// Left of the first occurance of c
            /// </summary>
            /// <param name="src">The source string.</param>
            /// <param name="c">Return everything to the left of this character.</param>
            /// <returns>String to the left of c, or the entire string.</returns>
            public static string LeftOf(string src, char c)
            {
                string ret = src;

                int idx = src.IndexOf(c);

                if (idx != -1)
                {
                    ret = src.Substring(0, idx);
                }

                return ret;
            }

            /// <summary>
            /// Left of the n'th occurance of c.
            /// </summary>
            /// <param name="src">The source string.</param>
            /// <param name="c">Return everything to the left n'th occurance of this character.</param>
            /// <param name="n">The occurance.</param>
            /// <returns>String to the left of c, or the entire string if not found or n is 0.</returns>
            public static string LeftOf(string src, char c, int n)
            {
                string ret = src;
                int idx = -1;

                while (n > 0)
                {
                    idx = src.IndexOf(c, idx + 1);

                    if (idx == -1)
                    {
                        break;
                    }

                    --n;
                }

                if (idx != -1)
                {
                    ret = src.Substring(0, idx);
                }

                return ret;
            }

            /// <summary>
            /// Returns everything to the left of the righmost char c.
            /// </summary>
            /// <param name="src">The source string.</param>
            /// <param name="c">The search char.</param>
            /// <returns>Everything to the left of the rightmost char c, or the entire string.</returns>
            public static string LeftOfRightmostOf(string src, char c)
            {
                string ret = src;
                int idx = src.LastIndexOf(c);

                if (idx != -1)
                {
                    ret = src.Substring(0, idx);
                }

                return ret;
            }

            /// <summary>
            /// Right of the first occurance of c
            /// </summary>
            /// <param name="src">The source string.</param>
            /// <param name="c">The search char.</param>
            /// <returns>Returns everything to the right of c, or an empty string if c is not found.</returns>
            public static string RightOf(string src, char c)
            {
                string ret = String.Empty;
                int idx = src.IndexOf(c);

                if (idx != -1)
                {
                    ret = src.Substring(idx + 1);
                }

                return ret;
            }

            /// <summary>
            /// Returns all the text to the right of the specified string.
            /// Returns an empty string if the substring is not found.
            /// </summary>
            /// <param name="src"></param>
            /// <param name="substr"></param>
            /// <returns></returns>
            public static string RightOf(string src, string substr)
            {
                string ret = String.Empty;
                int idx = src.IndexOf(substr);

                if (idx != -1)
                {
                    ret = src.Substring(idx + substr.Length);
                }

                return ret;
            }

            /// <summary>
            /// Returns everything to the right of the rightmost char c.
            /// </summary>
            /// <param name="src">The source string.</param>
            /// <param name="c">The seach char.</param>
            /// <returns>Returns everything to the right of the rightmost search char, or an empty string.</returns>
            public static string RightOfRightmostOf(string src, char c)
            {
                string ret = String.Empty;
                int idx = src.LastIndexOf(c);

                if (idx != -1)
                {
                    ret = src.Substring(idx + 1);
                }

                return ret;
            }
        }

        
    }
}
