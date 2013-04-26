using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

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
    public static string With( this string text, Object arg0 )
    {
        return String.Format( text, arg0 );
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
    public static string With( this string text, Object arg0, Object arg1 )
    {
        return String.Format( text, arg0, arg1 );
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
    public static string With( this string text, Object arg0, Object arg1, Object arg2 )
    {
        return String.Format( text, arg0, arg1, arg2 );
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
    public static string With( this string text, params Object[] args )
    {
        return String.Format( text, args );
    }
}

/*
public static class StreamExtensions
{
    public static byte[] ReadToEnd( this System.IO.Stream stream )
    {

    }
}
 */

/// <summary>
///  System.Type extension methods.
/// </summary>
public static class TypeExtensions
{
    /// <summary>
    ///  Check if type has a default constructor that takes no arguments.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns>True if the type has a constructor that takes no arguments.</returns>
    public static bool HasDefaultConstructor( this Type type )
    {
        // NOTE: WinRT has type.GetTypeInfo()
        ConstructorInfo[] constructors = type.GetConstructors();

        for ( int index = 0; index < constructors.Length; ++index )
        {
            ConstructorInfo constructor = constructors[index];

            if ( constructor.IsPublic && ( constructor.GetParameters().Length == 0 ) )
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    ///  Check if a type has one or more of the given attribute type.
    /// </summary>
    /// <typeparam name="T">The attribute type to check.</typeparam>
    /// <param name="type">The type to check.</param>
    /// <returns>True if the type has the requested attribute type.</returns>
    public static bool HasAttribute<T>( this Type type )
        where T : System.Attribute
    {
        object[] attributes = type.GetCustomAttributes( typeof(T), true );
        return ( attributes != null ) && ( attributes.Length > 0 );
    }

    /// <summary>
    ///  Check if a type has one or more of the given attribute type.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <param name="attributeType">The attribute type to check.</param>
    /// <returns>True if the type has the requested attribute type.</returns>
    public static bool HasAttribute( this Type type, Type attributeType )
	{
		object[] attributes = type.GetCustomAttributes( attributeType, true );
		return ( attributes != null ) && ( attributes.Length > 0 );
	}

    /// <summary>
    ///  Takes a type and returns all attributes that match the given attribute type.
    /// </summary>
    /// <typeparam name="T">The type to check for attributes.</typeparam>
    /// <param name="type">The attribute type.</param>
    /// <param name="attribute">List to populate with found attribute values.</param>
    /// <returns>True if there was at least one attribute on the type.</returns>
    public static bool TryFindAttribute<T>( this Type type, ref T attribute )
        where T : System.Attribute
    {
        object[] objAttributes = type.GetCustomAttributes( typeof( T ), true );

        if ( objAttributes != null && objAttributes.Length > 0 )
        {
            attribute = (T) objAttributes[0];
            return true;
        }

        return false;
    }

    /// <summary>
    ///  Takes a type and returns the first attribute that match the given attribute type.
    /// </summary>
    /// <typeparam name="T">The type to check for attributes.</typeparam>
    /// <param name="type">The attribute type.</param>
    /// <param name="attribute">Reference to set if an attribute is found.</param>
    /// <returns>True if there was at least one attribute on the type.</returns>	
	public static bool TryFindAttributes<T>( this Type type, ref T[] attributes )
        where T : System.Attribute
	{
		object[] objAttributes = type.GetCustomAttributes( typeof(T), true );
			
		if ( objAttributes != null && objAttributes.Length > 0 )
		{
			attributes = (T[]) objAttributes;
			return true;
		}
			
		return false;
	}
    
    /// <summary>
    ///  Takes a type and returns a "prettier" version of it's actual system name.
    /// </summary>
    /// <remarks>
    ///  This method uses StringBuilder to build the type name, so it will allocate memory.
    ///  Credit: http://stackoverflow.com/a/401824
    /// <param name="type">The type to get a name for.</param>
    /// <returns>Type name.</returns>
    public static string GetPrettyName( this Type type )
    {
        bool isArray = false;

        if ( type.IsArray )
        {
            type = type.GetElementType();
            isArray = true;
        }

        if ( type.IsGenericParameter )
        {
            return type.Name;
        }

        if ( !type.IsGenericType )
        {
            return type.FullName;
        }

        System.Text.StringBuilder builder = new System.Text.StringBuilder();

        string name = type.Name;
        int index = name.IndexOf( "`" );

        builder.AppendFormat( "{0}.{1}", type.Namespace, name.Substring( 0, index ) );
        builder.Append( '<' );

        bool  first = true;

        foreach ( Type typeArgument in type.GetGenericArguments() )
        {
            if ( first )
            {
                first = false;
            }
            else
            {
                builder.Append( ", " );
            }

            builder.Append( typeArgument.GetPrettyName() );
        }

        builder.Append( '>' );

        if ( isArray )
        {
            builder.Append( "[]" );
        }

        return builder.ToString();
    }
}