#include <iostream>
#include <ostream>
#include <fstream>
#include <string>

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

const char* LOG_LEVEL_NAMES[ELogLevel_Count] =
{
    "TRACE",
    "DEBUG",
    "INFO",
    "NOTICE",
    "WARN",
    "ERROR"
};

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
 * Log entry constructor. Takes two output streams and assigns them to
 * be the console output, and file output streams. Will also print a short
 * entry header consisting of the log level and component before returning
 *
 * \param  consoleStream  Output stream to use for console output
 * \param  fileStream     Output stream to use for logfile output
 * \param  logLevel       The level at which this entry was emitted
 * \param  system         Name of the component or subsystem
 */
LogEntry::LogEntry( std::ostream& consoleStream,
                    std::ostream& fileStream,
                    ELogLevel logLevel,
                    const std::string& system )
    : mConsoleStream(consoleStream),
      mFileStream(fileStream)
      
{
    // Write console output header
    mConsoleStream
        << "["  << LOG_LEVEL_NAMES[logLevel]
        << "; " << system << "] ";
}

/**
 * Null stream constructor. Takes in a reference to a (hopefully)
 * nullstream object, and doesn't print anything useful
 *
 * \param  nullStream  Reference to a null stream object
 */
LogEntry::LogEntry( std::ostream& nullStream )
    : mConsoleStream( nullStream ),
      mFileStream( nullStream )
{
}

/**
 * LogEntry destructor. Cleans up and emits end of line notices to the
 * log entry's streams
 */
LogEntry::~LogEntry()
{
    mConsoleStream << std::endl;
}

class Logger
{
public:
    Logger();
    ~Logger();

    static std::string findAcceptableLogName( const std::string& path );

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

Logger::Logger()
    : mOutputFile(),
      mNullStream( new NullStreamBuf ),
      mConsoleStream( std::cout ),
      mFileStream( mNullStream ),
      mMinimumLogLevel( ELOG_DEBUG )
{
}

std::string findNewLogFileName( const std::string& directory )
{
    DIR *dir;
    dirent *pDir;

    // Open the directory
    dir = opendir( directory.c_str() );

    if ( dir == NULL )
    {
        // oops
        return std::string("");
    }

    // Iterate through the list of directories
    while ( ( pDir = readdir(dir) ) != NULL )
    {
        // Examine the filename
        char * filename = pDir->d_name;
        std::cout << filename << std::endl;
    }

    closedir(dir);
    return std::string("ohhai");
}

Logger::~Logger()
{
    if ( mOutputFile.is_open() )
    {
        mOutputFile.close();
    }
}

/**
 * Writes a trace entry to the program's log, and a stream that can be
 * used to append additional information to the entry
 *
 * \param  system  The name of the system or component writing the entry
 * \return A LogEntry object that can be used to append additional
 *         information to the log entry
 */
LogEntry Logger::trace( const std::string& system ) const
{
    if ( mMinimumLogLevel <= ELOG_TRACE )
    {
        return LogEntry( mConsoleStream, mFileStream, ELOG_TRACE, system );
    }
    else
    {
        return LogEntry( mNullStream );
    }
}

/**
 * Writes a debug entry to the program's log, and a stream that can be
 * used to append additional information to the entry
 *
 * \param  system  The name of the system or component writing the entry
 * \return A LogEntry object that can be used to append additional
 *         information to the log entry
 */
LogEntry Logger::debug( const std::string& system ) const
{
    if ( mMinimumLogLevel <= ELOG_DEBUG )
    {
        return LogEntry( mConsoleStream, mFileStream, ELOG_DEBUG, system );
    }
    else
    {
        return LogEntry( mNullStream );
    }
}

/**
 * Writes an info entry to the program's log, and a stream that can be
 * used to append additional information to the entry
 *
 * \param  system  The name of the system or component writing the entry
 * \return A LogEntry object that can be used to append additional
 *         information to the log entry
 */
LogEntry Logger::info( const std::string& system ) const
{
    if ( mMinimumLogLevel <= ELOG_INFO )
    {
        return LogEntry( mConsoleStream, mFileStream, ELOG_INFO, system );
    }
    else
    {
        return LogEntry( mNullStream );
    }
}

/**
 * Writes a notice entry to the program's log, and a stream that can be
 * used to append additional information to the entry
 *
 * \param  system  The name of the system or component writing the entry
 * \return A LogEntry object that can be used to append additional
 *         information to the log entry
 */
LogEntry Logger::notice( const std::string& system ) const
{
    if ( mMinimumLogLevel <= ELOG_NOTICE )
    {
        return LogEntry( mConsoleStream, mFileStream, ELOG_NOTICE, system );
    }
    else
    {
        return LogEntry( mNullStream );
    }
}

/**
 * Writes a warning entry to the program's log, and a stream that can be
 * used to append additional information to the entry
 *
 * \param  system  The name of the system or component writing the entry
 * \return A LogEntry object that can be used to append additional
 *         information to the log entry
 */
LogEntry Logger::warn( const std::string& system ) const
{
    if ( mMinimumLogLevel <= ELOG_WARN )
    {
        return LogEntry( mConsoleStream, mFileStream, ELOG_WARN, system );
    }
    else
    {
        return LogEntry( mNullStream );
    }
}

/**
 * Writes an error entry to the program's log, and a stream that can be
 * used to append additional information to the entry
 *
 * \param  system  The name of the system or component writing the entry
 * \return A LogEntry object that can be used to append additional
 *         information to the log entry
 */
LogEntry Logger::error( const std::string& system ) const
{
    if ( mMinimumLogLevel <= ELOG_ERROR )
    {
        return LogEntry( mConsoleStream, mFileStream, ELOG_ERROR, system );
    }
    else
    {
        return LogEntry( mNullStream );
    }
}

int main( int argc, char* argv[] )
{
    Logger logger;
    logger.info("core") << "Hello World";
}
