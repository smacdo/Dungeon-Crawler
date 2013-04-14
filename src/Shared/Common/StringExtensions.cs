using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class StringExtensions
{
    public static string With( this string text, Object arg0 )
    {
        return String.Format( text, arg0 );
    }

    public static string With( this string text, Object arg0, Object arg1 )
    {
        return String.Format( text, arg0, arg1 );
    }

    public static string With( this string text, Object arg0, Object arg1, Object arg2 )
    {
        return String.Format( text, arg0, arg1, arg2 );
    }

    public static string With( this string text, params Object[] args )
    {
        return String.Format( text, args );
    }
}
