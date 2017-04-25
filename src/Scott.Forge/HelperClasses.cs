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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Scott.Forge
{
    /// <summary>
    ///  Simple utility functions for strings.
    /// </summary>
    public static class StringUtils
    {
        public static string RemovePrefix(string text, string prefix)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            if (prefix == null)
            {
                throw new ArgumentNullException(nameof(prefix));
            }

            if (text.StartsWith(prefix))
            {
                return text.Substring(prefix.Length, text.Length - prefix.Length);
            }
            else
            {
                return text;
            }
        }

        public static string RemoveSuffix(string text, string suffix)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            if (suffix == null)
            {
                throw new ArgumentNullException(nameof(suffix));
            }

            if (text.EndsWith(suffix))
            {
                return text.Substring(0, text.Length - suffix.Length);
            }
            else
            {
                return text;
            }
        }
    }

    /// <summary>
    ///  Attribute helper methods.
    /// </summary>
    public static class AttributeUtils
    {
        public static IEnumerable<AttributeType> GetAttributeInstances<AttributeType>(IEnumerable<Assembly> assemblies)
            where AttributeType : Attribute
        {
            if (assemblies == null)
            {
                throw new ArgumentNullException(nameof(assemblies));
            }

            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.DefinedTypes)
                {
                    var matchingAttributes = type.GetCustomAttributes(typeof(AttributeType), true);

                    foreach (var attribute in matchingAttributes)
                    {
                        var typedAttribute = attribute as AttributeType;
                        Debug.Assert(typedAttribute != null);

                        yield return typedAttribute;
                    }
                }
            }
        }

        public static IEnumerable<TypeInfo> GetTypesWithAttribute<AttributeType>(IEnumerable<Assembly> assemblies)
            where AttributeType : Attribute
        {
            if (assemblies == null)
            {
                throw new ArgumentNullException(nameof(assemblies));
            }
            
            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.DefinedTypes)
                {
                    var matchingAttributes = type.GetCustomAttributes(typeof(AttributeType), true);

                    if (matchingAttributes.Any())
                    {
                        yield return type;
                    }
                }
            }
        }
    }

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