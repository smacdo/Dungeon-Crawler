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

namespace Scott.Forge
{
    public class ChildChangedEventArgs : EventArgs
    {
        public Object Child { get; set; }
    }

    public class ParentChangedEventArgs : EventArgs
    {
        public Object PreviousOwner { get; set; }
        public Object Owner { get; set; }
    }

    /// <summary>
    /// MAKE SURE TO IMPLEMENT EQUALS PROPERLY
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ParentableNode<T> where T : class
    {
        /// <summary>
        ///  Gets the parent of this object.
        /// </summary>
        public ParentableNode<T> Parent
        {
            get
            {
                return mParent;
            }
            set
            {
                if ( value == null )
                {
                    RemoveParent();
                }
                else
                {
                    SetParent( value );
                }
            }
        }

        /// <summary>
        ///  Gets the sibling object immediately following this object.
        /// </summary>
        public ParentableNode<T> NextSibling
        {
            get
            {
                return mNextSibling;
            }
        }

        /// <summary>
        ///  Gets the sibling object immediately prior to this object.
        /// </summary>
        public ParentableNode<T> PreviousSibling
        {
            get
            {
                return mPreviousSibling;
            }
        }

        /// <summary>
        ///  Gets the first child of this object.
        /// </summary>
        public ParentableNode<T> FirstChild
        {
            get
            {
                return mFirstChild;
            }
        }

        /// <summary>
        ///  Gets the last child of this object.
        /// </summary>
        public ParentableNode<T> LastChild
        {
            get
            {
                return mLastChild;
            }
        }

        /// <summary>
        ///  Gets an enumerable list of this object's children.
        /// </summary>
        public IEnumerable<T> Children
        {
            get
            {
                ParentableNode<T> node = mFirstChild;

                while ( node != null )
                {
                    yield return node.mValue;
                    node = mFirstChild.mNextSibling;
                }
            }
        }

        /// <summary>
        ///  Gets the number of children belonging to this object.
        /// </summary>
        public int ChildrenCount
        {
            get
            {
                return mChildCount;
            }
        }

        /// <summary>
        ///  Gets a value indicating whether this object has a parent.
        /// </summary>
        public bool HasParent
        {
            get
            {
                return ( mParent != null );
            }
        }

        /// <summary>
        ///  Gets a value indicating whether this object has any children.
        /// </summary>
        public bool HasChildren
        {
            get
            {
                return ( mFirstChild != null );
            }
        }

        private ParentableNode<T> mParent = null;
        private ParentableNode<T> mNextSibling = null;
        private ParentableNode<T> mPreviousSibling = null;
        private ParentableNode<T> mFirstChild = null;
        private ParentableNode<T> mLastChild = null;
        private T mValue = default( T );

        private int mChildCount = 0;

        /// <summary>
        ///  Default constructor.
        /// </summary>
        public ParentableNode()
            : this( null )
        {
        }

        /// <summary>
        ///  ParentableObject constructor.
        /// </summary>
        public ParentableNode( ParentableNode<T> parent )
        {
            if ( parent != null )
            {
                SetParent( parent );
            }
        }

        /// <summary>
        ///  Checks if the child exists in this object's list of children.
        /// </summary>
        public bool ContainsChild( ParentableNode<T> childObject )
        {
            // Search for the child in our list of children
            ParentableNode<T> node = mFirstChild;

            while ( node != null )
            {
                if ( node == childObject )
                {
                    break;
                }
                else
                {
                    node = node.mNextSibling;
                }
            }

            // Were we able to locate it?
            return ( node != null );
        }

        /// <summary>
        ///  Checks if this object is a child of the parent object.
        /// </summary>
        public bool IsChildOf( ParentableNode<T> parent, bool searchParentsRecursively = false )
        {
            if ( mParent == parent )
            {
                return true;
            }
            else if ( mParent != null && searchParentsRecursively )
            {
                return mParent.IsChildOf( parent, true );
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        ///  Sets this object's parent.
        /// </summary>
        public void SetParent( ParentableNode<T> newParent )
        {
            // Ensure that the caller is not trying to add this object as its parent, which will lead
            // quite happily into an infinte loop.
            if ( ReferenceEquals( this, newParent ) )
            {
                throw new ParentingException( "Cannot parent object to itself" );
            }

            // Detach the object from it's current parent, and sever all child / sibling links first.
            RemoveParent();

            // Now attach ourself to the new parent. If the new parent is null, well our work is done
            // :)
            if ( newParent != null )
            {
                mParent = newParent;
                mParent.mChildCount++;

                // Place ourself into the parent's list of children by making the parent's last child
                // point to us as the next sibling.
                if ( newParent.mLastChild != null )
                {
                    mPreviousSibling = newParent.mLastChild;
                    mPreviousSibling.mNextSibling = this;
                }
                else
                {
                    // The parent's last child reference is null, which means this parent has no
                    // children. Configure the FirstChild reference to point to us.
                    newParent.mFirstChild = this;
                }

                // Set this object as the last sibling.
                newParent.mLastChild = this;
            }
        }

        /// <summary>
        ///  Disconnect this object from it's parent by setting the parent to null.
        /// </summary>
        private void RemoveParent()
        {
            // If this object has a parent, go ahead and remove this object from the parent's linked
            // list of children.
            if ( mParent != null )
            {
                if ( mParent.mFirstChild != null && ReferenceEquals( mParent.mFirstChild, this ) )
                {
                    mParent.mFirstChild = mParent.mFirstChild.mNextSibling;
                }

                if ( mParent.mLastChild != null && ReferenceEquals( mParent.mLastChild, this ) )
                {
                    mParent.mLastChild = mParent.mLastChild.mPreviousSibling;
                }

                mParent.mChildCount--;
                mParent = null;
            }

            // If this object has siblings, we will need to remove ourself from that linked list as
            // well.
            if ( mNextSibling != null )
            {
                mNextSibling.mPreviousSibling = mPreviousSibling;
            }

            if ( mPreviousSibling != null )
            {
                mPreviousSibling.mNextSibling = mNextSibling;
            }

            mNextSibling = null;
            mPreviousSibling = null;
        }

        /// <summary>
        ///  Adds the child to the end of this object's list of children.
        /// </summary>
        public void AddChild( ParentableNode<T> child )
        {
            // Refuse to add a null child
            if ( child == null )
            {
                throw new ParentingException( "Cannot add a null child" );
            }

            // Refuse to add this object as it's child
            if ( ReferenceEquals( child, this ) )
            {
                throw new ParentingException( "Cannot add self as a child" );
            }

            child.SetParent( this );
        }

        /// <summary>
        ///  Removes the child from this object's list of children which will cause
        ///  the child to become parentless and sibling-less.
        /// </summary>
        public void RemoveChild( ParentableNode<T> child )
	    {
		    if ( child == null )
		    {
			    throw new ParentingException( "Cannot remove a null child" );
		    }

            if ( ReferenceEquals( child, this ) )
            {
                throw new ParentingException( "Cannot remove self as a child" );
            }

            // Find the child requested
            ParentableNode<T> node = mFirstChild;

            while ( node != null && !ReferenceEquals( node, child ) )
            {
                node = node.mNextSibling;
            }

		    // Did we find the child node? If so, unparent it from this object
            if ( node != null )
            {
                node.SetParent( null );
            }
	    }
    }
}
