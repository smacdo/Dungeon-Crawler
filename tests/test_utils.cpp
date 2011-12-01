#include "common/utils.h"
#include <ostream>
#include <gtest.h>

#include <vector>

class TestDummy
{
public:
    TestDummy( size_t& totalCounter, size_t& selfCounter )
        : mTotalCounter( totalCounter ),
          mSelfCounter( selfCounter )
    {
        mTotalCounter++;
        mSelfCounter++;
    }

    ~TestDummy()
    {
        mTotalCounter--;
        mSelfCounter--;
    }

private:
    size_t& mTotalCounter;
    size_t& mSelfCounter;
};

TEST(Common,Utils_InternalTest_VerifyTestDummy)
{
    size_t total = 0;
    size_t a     = 0;
    size_t b     = 0;

    TestDummy *pA = new TestDummy( total, a );
    EXPECT_EQ( 1u, total );
    EXPECT_EQ( 1u, a );
    EXPECT_EQ( 0u, b );

    TestDummy *pB = new TestDummy( total, b );
    EXPECT_EQ( 2u, total );
    EXPECT_EQ( 1u, a );
    EXPECT_EQ( 1u, b );

    delete pA;
    EXPECT_EQ( 1u, total );
    EXPECT_EQ( 0u, a );
    EXPECT_EQ( 1u, b );

    delete pB;
    EXPECT_EQ( 0u, total );
    EXPECT_EQ( 0u, a );
    EXPECT_EQ( 0u, b );

}

TEST(Common,Utils_Delete_Pointer)
{
    size_t total = 0;
    size_t a = 0;

    TestDummy *pA = new TestDummy( total, a );
    Delete( pA );

    EXPECT_EQ( 0u, total );
    EXPECT_EQ( 0u, a );

    EXPECT_TRUE( pA == NULL );
}

TEST(Common,Utils_DeletePointerContainer)
{
    size_t total = 0;
    size_t a     = 0;
    size_t b     = 0;
    size_t c     = 0;

    std::vector<TestDummy*> v;
    v.push_back( new TestDummy( total, a ) );
    v.push_back( new TestDummy( total, b ) );
    v.push_back( new TestDummy( total, c ) );

    EXPECT_EQ( 3u, total );
    EXPECT_EQ( 1u, a );
    EXPECT_EQ( 1u, b );
    EXPECT_EQ( 1u, c );

    DeletePointerContainer( v );
    EXPECT_EQ( 0u, total );
    EXPECT_EQ( 0u, a );
    EXPECT_EQ( 0u, b );
    EXPECT_EQ( 0u, c );
}

TEST(Common,Utils_DeleteVectorPointers)
{
    size_t total = 0;
    size_t a     = 0;
    size_t b     = 0;
    size_t c     = 0;

    std::vector<TestDummy*> v;
    v.push_back( new TestDummy( total, a ) );
    v.push_back( new TestDummy( total, b ) );
    v.push_back( new TestDummy( total, c ) );

    EXPECT_EQ( 3u, total );
    EXPECT_EQ( 1u, a );
    EXPECT_EQ( 1u, b );
    EXPECT_EQ( 1u, c );

    DeleteVectorPointers( v );
    EXPECT_EQ( 0u, total );
    EXPECT_EQ( 0u, a );
    EXPECT_EQ( 0u, b );
    EXPECT_EQ( 0u, c );
}

TEST(Common,Utils_DeleteMapPointers)
{
    size_t total = 0;
    size_t a     = 0;
    size_t b     = 0;
    size_t c     = 0;

    std::map<int, TestDummy*> v;
    v.insert( std::pair<int, TestDummy*>( 0, new TestDummy( total, a ) ) );
    v.insert( std::pair<int, TestDummy*>( 1, new TestDummy( total, b ) ) );
    v.insert( std::pair<int, TestDummy*>( 2, new TestDummy( total, c ) ) );

    EXPECT_EQ( 3u, total );
    EXPECT_EQ( 1u, a );
    EXPECT_EQ( 1u, b );
    EXPECT_EQ( 1u, c );

    DeleteMapPointers( v );
    EXPECT_EQ( 0u, total );
    EXPECT_EQ( 0u, a );
    EXPECT_EQ( 0u, b );
    EXPECT_EQ( 0u, c );
}

TEST(Common,Utils_Deref_Valid)
{
    int v = 42;
    int *pV = &v;
    int& rV = deref( pV );

    EXPECT_EQ( 42, rV );

    rV = 250;
    EXPECT_EQ( 250, v );
    EXPECT_EQ( 250, *pV );
}

TEST(Common,Utils_Deref_Const_Valid)
{
    int v = 42;
    const int *pV = &v;

    EXPECT_EQ( 42, deref( pV ) );
}

TEST(Common,Utils_Deref_Null_Death)
{
    const int *pV = NULL;
    EXPECT_DEATH( deref( pV ), "ASSERTION FAILED: ptr != __null" );
}




