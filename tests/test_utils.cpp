/*
 * Copyright 2012 Scott MacDonald
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
#include "common/utils.h"
#include "common/platform.h"

#include <memory>
#include <ostream>
#include <vector>
#include <gtest.h>

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

TEST(UtilsTests,InternalTest_VerifyTestDummy)
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

TEST(UtilsTests,DeletePointer)
{
    size_t total = 0;
    size_t a = 0;

    TestDummy *pA = new TestDummy( total, a );
    Delete( pA );

    EXPECT_EQ( 0u, total );
    EXPECT_EQ( 0u, a );

    EXPECT_TRUE( pA == NULL );
}

TEST(UtilsTest,DeletePointerArray)
{
    size_t total = 0;
    size_t a     = 0;

    // TODO: Implement
    EXPECT_TRUE( true );
}

TEST(UtilsTests,DeletePointerGenericContainerWhichIsAVector)
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

    EXPECT_EQ( 3u, DeletePointerContainer( v ) );
    EXPECT_EQ( 0u, total );
    EXPECT_EQ( 0u, a );
    EXPECT_EQ( 0u, b );
    EXPECT_EQ( 0u, c );
}

TEST(UtilsTests,DeleteVectorPointers)
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

    EXPECT_EQ( 3u, DeleteVectorPointers( v ) );
    EXPECT_EQ( 0u, total );
    EXPECT_EQ( 0u, a );
    EXPECT_EQ( 0u, b );
    EXPECT_EQ( 0u, c );
}

TEST(UtilsTests,DeleteMapPointers)
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

    EXPECT_EQ( 3u, DeleteMapPointers( v ) );
    EXPECT_EQ( 0u, total );
    EXPECT_EQ( 0u, a );
    EXPECT_EQ( 0u, b );
    EXPECT_EQ( 0u, c );
}

/**
 * Tests that we can deref a non-null pointer value
 */
TEST(UtilsTests,DerefValid)
{
    int v = 42;
    int *pV = &v;
    int& rV = deref( pV );

    EXPECT_EQ( 42, rV );

    rV = 250;
    EXPECT_EQ( 250, v );
    EXPECT_EQ( 250, *pV );
}

/**
 * Tests if we can deref a non-null constant pointer value
 */
TEST(UtilsTests,DerefConst)
{
    int v = 42;
    const int *pV = &v;

    EXPECT_EQ( 42, deref( pV ) );
}

/**
 * Tests if we can deref a non-null smart pointer value
 */
TEST(UtilsTest,DerefSmartPointer)
{
    std::shared_ptr<int> p( new int(42) );
    EXPECT_EQ( 42, deref( p ) );
}

/**
 * Tests if we can deref a non-null constant smart pointer value
 */
TEST(UtilsTest,DerefConstSmartPointer)
{
    std::shared_ptr<const int> p( new int(42) );
    EXPECT_EQ( 42, deref( p ) );
}

TEST(UtilsTests,DerefNullDeath)
{
    const int *pV = NULL;

    App::setTestAssertsShouldDie( true );
    EXPECT_DEATH( deref( pV ), "ASSERTION FAILED: ptr != __null" );

    App::resetTestAssertsShouldDie();
}

TEST(UtilsTests,DerefNullSmartPointerDeath)
{
    std::shared_ptr<int> p;

    App::setTestAssertsShouldDie( true );
    EXPECT_DEATH( deref( p ), "ASSERTION FAILED: ptr != __null" );

    App::resetTestAssertsShouldDie();
}

