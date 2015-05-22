/*
 * Copyright 2012-2014 Scott MacDonald
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;

namespace Scott.Forge
{
    /// <summary>
    ///  String extension methods.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        ///  Replaces one or more format items in a specified string with the string representation of
        ///  a specified object.
        /// </summary>
        /// <param name="text">A composite format string.</param>
        /// <param name="arg0">The object to format.</param>
        /// <returns>
        ///  A copy text in which any format items are replaced by the string representation of arg0.
        /// </returns>
        public static string With(this string text, Object arg0)
        {
            return String.Format(text, arg0);
        }

        /// <summary>
        ///  Replaces one or more format items in a specified string with the string representation of
        ///  a specified object.
        /// </summary>
        /// <param name="text">A composite format string.</param>
        /// <param name="arg0">The first object to format.</param>
        /// <param name="arg1">The second object to format.</param>
        /// <returns>
        ///  A copy text in which any format items are replaced by the string representation of arg0
        ///  and arg1.
        /// </returns>
        public static string With(this string text, Object arg0, Object arg1)
        {
            return String.Format(text, arg0, arg1);
        }

        /// <summary>
        ///  Replaces one or more format items in a specified string with the string representation of
        ///  a specified object.
        /// </summary>
        /// <param name="text">A composite format string.</param>
        /// <param name="arg0">The first object to format.</param>
        /// <param name="arg1">The second object to format.</param>
        /// <param name="arg2">The third object to format.</param>
        /// <returns>
        ///  A copy text in which any format items are replaced by the string representation of arg0,
        ///  arg1 and arg2.
        /// </returns>
        public static string With(this string text, Object arg0, Object arg1, Object arg2)
        {
            return String.Format(text, arg0, arg1, arg2);
        }


        /// <summary>
        ///  Replaces one or more format items in a specified string with the string representation of
        ///  a specified object.
        /// </summary>
        /// <param name="text">A composite format string.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <returns>
        ///  A copy text in which any format items are replaced by the string representation of arg0
        ///  and arg1.
        /// </returns>
        public static string With(this string text, params Object[] args)
        {
            return String.Format(text, args);
        }
    }
}