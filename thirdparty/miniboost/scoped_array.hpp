#ifndef BOOST_SMART_PTR_SCOPED_ARRAY_HPP_INCLUDED
#define BOOST_SMART_PTR_SCOPED_ARRAY_HPP_INCLUDED

//  (C) Copyright Greg Colvin and Beman Dawes 1998, 1999.
//  Copyright (c) 2001, 2002 Peter Dimov
//
//  Distributed under the Boost Software License, Version 1.0. (See
//  accompanying file LICENSE_1_0.txt or copy at
//  http://www.boost.org/LICENSE_1_0.txt)
//
//  http://www.boost.org/libs/smart_ptr/scoped_array.htm
//

#include <thirdparty/miniboost/assert.hpp>
#include <thirdparty/miniboost/checked_delete.hpp>

#include <cstddef>            // for std::ptrdiff_t

namespace boost
{

//  scoped_array extends scoped_ptr to arrays. Deletion of the array pointed to
//  is guaranteed, either on destruction of the scoped_array or via an explicit
//  reset(). Use shared_array or std::vector if your needs are more complex.

template<class T> class scoped_array // noncopyable
{
private:

    T * px;

    scoped_array(scoped_array const &);
    scoped_array & operator=(scoped_array const &);

    typedef scoped_array<T> this_type;

    void operator==( scoped_array const& ) const;
    void operator!=( scoped_array const& ) const;

public:

    typedef T element_type;

    explicit scoped_array( T * p = 0 ) : px( p ) // never throws
    {
    }

    ~scoped_array() // never throws
    {
        boost::checked_array_delete( px );
    }

    void reset(T * p = 0) // never throws
    {
        BOOST_ASSERT_MSG( p == 0 || p != px, "Cannot reset non-null scoped_array" ); // catch self-reset errors
        this_type(p).swap(*this);
    }

    T & operator[](std::ptrdiff_t i) const // never throws
    {
        BOOST_ASSERT_MSG( px != 0 && "Cannot index null scoped_array" );
        BOOST_ASSERT_MSG( i >= 0 && "Cannot use negative array index" );
        return px[i];
    }

    T * get() const // never throws
    {
        return px;
    }

// XXX BEGIN MOD: Manually included relevent code from operator_bool.hpp
    typedef T * this_type::*unspecified_bool_type;

    operator unspecified_bool_type() const // never throws
    {
        return px == 0? 0: &this_type::px;
    }

    // operator! is redundant, but some compilers need it
    bool operator! () const // never throws
    {
        return px == 0;
    }
// XXX END MOD

    void swap(scoped_array & b) // never throws
    {
        T * tmp = b.px;
        b.px = px;
        px = tmp;
    }
};

template<class T> inline void swap(scoped_array<T> & a, scoped_array<T> & b) // never throws
{
    a.swap(b);
}

} // namespace boost

#endif  // #ifndef BOOST_SMART_PTR_SCOPED_ARRAY_HPP_INCLUDED

