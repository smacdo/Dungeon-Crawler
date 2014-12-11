/// <copyright>
/// Copyright 2012-2014 Scott MacDonald
///
/// Licensed under the Apache License, Version 2.0 (the "License");
/// you may not use this file except in compliance with the License.
/// You may obtain a copy of the License at
///
/// http://www.apache.org/licenses/LICENSE-2.0
///
/// Unless required by applicable law or agreed to in writing, software
/// distributed under the License is distributed on an "AS IS" BASIS,
/// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
/// See the License for the specific language governing permissions and
/// limitations under the License.
/// </copyright>
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Scott.Forge.Core
{
    /// <summary>
    ///  A generic tuple that holds two typed values.
    /// </summary>
    [DataContract]
    public struct Pair<TFirst, TSecond>
        : IComparable,
          IComparable<Pair<TFirst, TSecond>>,
          IEquatable<Pair<TFirst, TSecond>>
    {
        private static readonly IComparer<TFirst> FirstComparer = Comparer<TFirst>.Default;
        private static readonly IComparer<TSecond> SecondComparer = Comparer<TSecond>.Default;
        private static readonly IEqualityComparer<TFirst> FirstEqualityComparer =
            EqualityComparer<TFirst>.Default;
        private static readonly IEqualityComparer<TSecond> SecondEqualityComparer =
            EqualityComparer<TSecond>.Default;

        [DataMember(Name = "First", Order = 0, IsRequired = true)]
        private TFirst mFirst;

        [DataMember(Name = "Second", Order = 1, IsRequired = true)]
        private TSecond mSecond;

        /// <summary>
        ///   Initializes a new instance of the <see cref="Scott.Forge.Core.Pair`2"/> struct.
        /// </summary>
        /// <param name="first">The first tuple value.</param>
        /// <param name="second">The second tuple value.</param>
        public Pair(TFirst first, TSecond second)
        {
            mFirst = first;
            mSecond = second;
        }

        /// <summary>
        ///  Initializes a new instance of the <see cref="Scott.Forge.Core.Pair`2"/> struct.
        /// </summary>
        /// <param name="keyValuePair">Key value pair to copy data from.</param>
        public Pair(KeyValuePair<TFirst, TSecond> keyValuePair)
        {
            mFirst = keyValuePair.Key;
            mSecond = keyValuePair.Value;
        }

        /// <summary>
        ///  Get or set the first value in the tuple.
        /// </summary>
        public TFirst First
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return mFirst; }
            [System.Diagnostics.DebuggerStepThrough]
            set { mFirst = value; }
        }

        /// <summary>
        ///  Get or set the second value in the tuple.
        /// </summary>
        public TSecond Second
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return mSecond; }
            [System.Diagnostics.DebuggerStepThrough]
            set { mSecond = value; }
        }

        /// <summary>
        ///  Compares this object to another object.
        /// </summary>
        /// <returns>
        ///  Negative one if this object precedes the other object, zero if they are equal and
        ///  positive one if the other object precedes this object.
        /// </returns>
        public int CompareTo(Pair<TFirst, TSecond> other)
        {
            int first = FirstComparer.Compare(mFirst, other.mFirst);

            if (first != 0)
            {
                return first;
            }
            else
            {
                return SecondComparer.Compare(mSecond, other.mSecond);
            }
        }

        /// <summary>
        ///  Compares this pair to another object.
        /// </summary>
        /// <returns>
        ///  Negative one if this object precedes the other object, zero if they are equal and
        ///  positive one if the other object precedes this object.
        /// </returns>
        int IComparable.CompareTo(object other)
        {
            if (other is Pair<TFirst, TSecond>)
            {
                return CompareTo((Pair<TFirst, TSecond>) other);
            }
            else
            {
                throw new ArgumentException("Cannot compare dissimiliar object types", "other");
            }
        }

        /// <summary>
        ///  Check if another Pair instance is equal to this one.
        /// </summary>
        /// <returns>True if the pairs are equal, false otherwise.</returns>
        public bool Equals(Pair<TFirst, TSecond> other)
        {
            return FirstEqualityComparer.Equals(mFirst, other.mFirst) &&
                   SecondEqualityComparer.Equals(mSecond, other.mSecond);
        }

        /// <summary>
        ///  Check if another Pair instance is equal to this one.
        /// </summary>
        /// <returns>True if the pairs are equal, false otherwise.</returns>
        public override bool Equals(object obj)
        {
            if (obj is Pair<TFirst, TSecond>)
            {
                return Equals((Pair<TFirst, TSecond>) obj);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        ///  Get this object's numeric value that can be used to insert and identify an object in a
        ///  hash based collection.
        /// </summary>
        /// <returns>This object's hash code.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;

                hash *= 23 + mFirst.GetHashCode();
                hash *= 23 + mSecond.GetHashCode();

                return hash;
            }
        }

        /// <summary>
        ///  Convert this pair object into a KeyValuePair object.
        /// </summary>
        /// <returns>This object as a key value pair.</returns>
        public KeyValuePair<TFirst, TSecond> ToKeyValuePair()
        {
            return new KeyValuePair<TFirst, TSecond>(mFirst, mSecond);
        }

        /// <summary>
        ///  Returns a string that represents this object.
        /// </summary>
        /// <returns>A string that represents this object.</returns>
        public override string ToString()
        {
            return String.Format(
                "{0} {1}",
                (mFirst == null) ? "null" : mFirst.ToString(),
                (mSecond == null) ? "null" : mSecond.ToString());
        }

        public static bool operator <(Pair<TFirst, TSecond> left,
                                        Pair<TFirst, TSecond> right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator >(Pair<TFirst, TSecond> left,
                                        Pair<TFirst, TSecond> right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator ==(Pair<TFirst, TSecond> left,
                                        Pair<TFirst, TSecond> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Pair<TFirst, TSecond> left,
                                        Pair<TFirst, TSecond> right)
        {
            return !(left.Equals(right));
        }

        public static explicit operator KeyValuePair<TFirst, TSecond>(Pair<TFirst, TSecond> pair)
        {
            return new KeyValuePair<TFirst, TSecond>(pair.mFirst, pair.mSecond);
        }

        public static explicit operator Pair<TFirst, TSecond>(
            KeyValuePair<TFirst, TSecond> keyValuePair)
        {
            return new Pair<TFirst, TSecond>(keyValuePair.Key, keyValuePair.Value);
        }
    }
}
