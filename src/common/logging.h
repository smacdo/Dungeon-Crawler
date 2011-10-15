/*
 * Copyright (C) 2011 Scott MacDonald. All rights reserved.
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
#ifndef SCOTT_DUNGEON_LOGGING_H
#define SCOTT_DUNGEON_LOGGING_H

#include <ostream>
#include <fstream>
#include <string>
#include <boost/noncopyable.hpp>

#define LOG_TRACE(x)  GlobalLog::getInstance().trace(x)
#define LOG_DEBUG(x)  GlobalLog::getInstance().debug(x)
#define LOG_INFO(x)   GlobalLog::getInstance().info(x)
#define LOG_NOTICE(x) GlobalLog::getInstance().notice(x)
#define LOG_WARN(x)   GlobalLog::getInstance().warn(x)
#define LOG_ERROR(x)  GlobalLog::getInstance().error(x)

/**
 * The logging severity level for a log entry
 */
enum ELogLevel
{
    ELOG_TRACE,
    ELOG_DEBUG,
    ELOG_INFO,
    ELOG_NOTICE,
    ELOG_WARN,
    ELOG_ERROR,
    ELogLevel_Count
};

/**
 * A null stream buffer, great for redirecting C++ output streams
 * to nothingness. This functions very similiarly to the UNIX idea of /dev/null
 */
class NullStreamBuf : public std::streambuf
{
public:
    NullStreamBuf()
    {
    }

private:
    virtual int overflow( int c )
    {
        return std::char_traits<char>::not_eof(c);
    }
};

/**
 * The "magic" of the logging system. An instance of this class is returned
 * each time a call is made to GlobalLog::write(), which allows the caller
 * to stream anything interesting into the log entry for writing. As a great
 * bonus, the destructor (called at the end of that statement) will emit
 * new lines!
 */
class LogEntry
{
public:
    LogEntry( std::ostream& consoleInput,
              std::ostream& fileInput,
              ELogLevel logLevel,
              const std::string& system );
    LogEntry( std::ostream& singleInput );
    ~LogEntry();

    template<typename T>
    LogEntry& operator << ( const T& obj )
    {
        mConsoleStream << obj;
        mFileStream    << obj;
        return *this;
    }

private:
    std::ostream& mConsoleStream;
    std::ostream& mFileStream;
};

/**
 * A object that can log information to files and the console
 */
class Log : boost::noncopyable
{
public:
    Log();
    ~Log();

    LogEntry trace( const std::string& system ) const;
    LogEntry debug( const std::string& system ) const;
    LogEntry info( const std::string& system ) const;
    LogEntry notice( const std::string& system ) const;
    LogEntry warn( const std::string& system ) const;
    LogEntry error( const std::string& system ) const;

private:
    std::ofstream mOutputFile;
    mutable std::ostream  mNullStream;
    std::ostream& mConsoleStream;
    std::ostream& mFileStream;
    ELogLevel mMinimumLogLevel;
};

/**
 * Logging singleton, used in conjunction with the logging macros
 */
class GlobalLog : boost::noncopyable
{
public:
    static void start();
    static Log& getInstance();

private:
    static Log mLog;
};

#endif