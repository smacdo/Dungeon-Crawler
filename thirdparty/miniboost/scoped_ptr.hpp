#ifndef BOOST_SMART_PTR_SCOPED_PTR_HPP_INCLUDED
#define BOOST_SMART_PTR_SCOPED_PTR_HPP_INCLUDED

//  (C) Copyright Greg Colvin and Beman Dawes 1998, 1999.
//  Copyright (c) 2001, 2002 Peter Dimov
//
//  Distributed under the Boost Software License, Version 1.0. (See
//  accompanying file LICENSE_1_0.txt or copy at
//  http://www.boost.org/LICENSE_1_0.txt)
//
//  http://www.boost.org/libs/smart_ptr/scoped_ptr.htm
//

#include <thirdparty/miniboost/assert.hpp>
#include <thirdparty/miniboost/checked_delete.hpp>

#include <memory>          // for std::auto_ptr

namespace boost
{

//  scoped_ptr mimics a built-in pointer except that it guarantees deletion
//  of the object pointed to, either on destruction of the scoped_ptr or via
//  an explicit reset(). scoped_ptr is a simple solution for simple needs;
//  use shared_ptr or std::auto_ptr if your needs are more complex.

template<class T> class scoped_ptr // noncopyable
{
private:

    T * px;

    scoped_ptr(scoped_ptr const &);
    scoped_ptr & operator=(scoped_ptr const &);

    typedef scoped_ptr<T> this_type;

    void operator==( scoped_ptr const& ) const;
    void operator!=( scoped_ptr const& ) const;

public:

    typedef T element_type;

    explicit scoped_ptr( T * p = 0 ): px( p ) // never throws
    {
    }

    explicit scoped_ptr( std::auto_ptr<T> p ): px( p.release() ) // never throws
    {
    }

    ~scoped_ptr() // never throws
    {
        boost::checked_delete( px );
    }

    void reset(T * p = 0) // never throws
    {
        BOOST_ASSERT_MSG( p == 0 || p != px, "Cannot reset non-null scoped_ptr" );
        this_type(p).swap(*this);
    }

    T & operator*() const // never throws
    {
        BOOST_ASSERT_MSG( px != 0, "Cannot dereference null scoped_ptr" );
        return *px;
    }

    T * operator->() const // never throws
    {
        BOOST_ASSERT_MSG( px != 0, "Cannot dereference null scoped_ptr" );
        return px;
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

    void swap(scoped_ptr & b) // never throws
    {
        T * tmp = b.px;
        b.px = px;
        px = tmp;
    }
};

template<class T> inline void swap(scoped_ptr<T> & a, scoped_ptr<T> & b) // never throws
{
    a.swap(b);
}

// get_pointer(p) is a generic way to say p.get()

template<class T> inline T * get_pointer(scoped_ptr<T> const & p)
{
    return p.get();
}

} // namespace boost

#endif // #ifndef BOOST_SMART_PTR_SCOPED_PTR_HPP_INCLUDED

