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
#include "engine/optionsparser.h"
#include "engine/appconfig.h"
#include "common/platform.h"
#include "version.h"

#include <string>
#include <sstream>
#include <fstream>

#include <boost/program_options.hpp>
#include <boost/functional/hash.hpp>
#include <boost/lexical_cast.hpp>

namespace po = boost::program_options;

/**
 * Constructor
 */
OptionsParser::OptionsParser()
    : mConfig(),
      mHelpRequested( false ),
      mVersionRequested( false ),
      mHadErrors( false ),
      mConfigPath( "dungeon.ini" ),
      mGenericOptions( "Application Options" ),
      mCommandLineOptions( "Command Line Options" ),
      mVariables()
{
    init();
}

/**
 * Destructor
 */
OptionsParser::~OptionsParser()
{
    // empty
}

/**
 * Parses the standard c command arguments, and populates the current apppconfig
 * data structure
 *
 * \param  argc        Number of command line arguments
 * \param  argv        Pointer to an array of c-strings containing arguments
 */
bool OptionsParser::parseCommandLine( int argc, char ** argv )
{
    // Attempt to parse the command line. If any exceptions are thrown, bail out
    // and record it
    try
    {
        // Copy all relevant config description groups into one big group
        po::options_description options;

        options.add( mGenericOptions );
        options.add( mCommandLineOptions );

        // Now parse the configuration file
        po::store( po::parse_command_line( argc, argv, options ), mVariables );
        po::notify( mVariables );

        // Did the user special an additional configuration file to parse?
        //  (Or was there a built in config file to parse?)
        if (! mConfigPath.empty() )
        {
            std::cerr << "parsing: " << mConfigPath << std::endl;
            parseConfigFile( mConfigPath );
        }

        // Were any "special" options requested?
        mHelpRequested    = ( mVariables.count("help") );
        mVersionRequested = ( mVariables.count("version") );
        mLicenseRequested = ( mVariables.count("license") );

        // Load the random seed (if provided).
        if ( mVariables.count("game.randomseed") )
        {
            mConfig.randomSeed =
                parseSeed( mVariables["game.randomseed"].as<std::string>() );
        }
    }
    catch ( ... )
    {
        mHadErrors = true;
    }

    return (!mHadErrors);
}

/**
 * Takes the given config file path, parses it for configuration data and
 * populates the current appconfig data structure
 */
bool OptionsParser::parseConfigFile( const std::string& filepath )
{
    bool status = true;

    // Try to open the file path as a text stream. If it succeeds, then use
    // boost to parse everything
    std::ifstream configFile( filepath.c_str() );

    if ( configFile )
    {
        // Attempt to parse the configuration file. If anything happen, report
        // an error and refuse to start
        try
        {
            po::store( po::parse_config_file( configFile, mCommandLineOptions ),
                       mVariables );
        }
        catch ( ... )
        {
            App::raiseFatalError( "Failed to parse the application config file" );
            status = false;
        }
    }
    else if (! mHadErrors )
    {
        std::cerr << "Could not open config file '" << filepath << "'" << std::endl;
    }

    po::notify( mVariables );
    return status;
}

/**
 * Performs the standard command line validation of options, and also intercepts
 * and executes common application independent arguments such as --help and
 * --version.
 */
void OptionsParser::process()
{
    std::cout << commandLineHeader() << std::endl;

    if ( mHelpRequested )
    {
        std::cout << help() << std::endl;
        App::quit( EPROGRAM_OK );
    }
    else if ( mVersionRequested )
    {
        std::cout << version() << std::endl;
        App::quit( EPROGRAM_OK );
    }
    else if ( mLicenseRequested )
    {
        std::cout << license() << std::endl;
        App::quit( EPROGRAM_OK );
    }
    else if ( mHadErrors )
    {
        std::cout << help() << std::endl << std::endl
                  << "Unknown or invalid option(s) specified" << std::endl;
        App::quit( EPROGRAM_USER_ERROR );
    }
}

/**
 * Generates the commmand line header
 */
std::string OptionsParser::commandLineHeader() const
{
    std::ostringstream ss;
    ss << Version::TITLE     << " "   << Version::VERSION_S << "\n"
       << Version::COPYRIGHT << "\n";

    return ss.str();
}

/**
 * Generates the version string
 */
std::string OptionsParser::version() const
{
    std::ostringstream ss;

    ss << "REVISION " << Version::REVISION << "\n";
    return ss.str();
}

/**
 * Generates help text
 */
std::string OptionsParser::help() const
{
    std::ostringstream ss;
    ss << mCommandLineOptions << "\n"
       << mGenericOptions     << "\n"
       << "Report bugs to: "  << Version::EMAIL   << "\n"
       << "Homepage:       "  << Version::WEBSITE << "\n";

    return ss.str();
}

/**
 * Shows application license information
 */
std::string OptionsParser::license() const
{
    std::ostringstream ss;
    ss << "Licensed under the Apache License, Version 2.0 (the \"License\");\n"
       << "you may not use this program except in compliance with the License.\n"
       << "You may obtain a copy of the License at\n"
       << "\n"
       << "http://www.apache.org/licenses/LICENSE-2.0\n"
       << "\n"
       << "Unless required by applicable law or agreed to in writing, software\n"
       << "distributed under the License is distributed on an \"AS IS\" BASIS,\n"
       << "WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.\n"
       << "See the License for the specific language governing permissions and\n"
       << "limitations under the License.\n";

    return ss.str();
}

/**
 * Returns an appconfig structure populated with information from a previously
 * called parse()
 */
AppConfig OptionsParser::appConfig() const
{
    return mConfig;
}

/**
 * Checks if the user asked for help
 */
bool OptionsParser::helpRequested() const
{
    return mHelpRequested;
}

/**
 * Checks if the user asked for the application's version information
 */
bool OptionsParser::versionRequested() const
{
    return mVersionRequested;
}

/**
 * Checks if the user asked for the application's license information
 */
bool OptionsParser::licenseRequested() const
{
    return mLicenseRequested;
}

/**
 * Checks if there were errors when parsing the command line
 */
bool OptionsParser::hadErrors() const
{
    return mHadErrors;
}

/**
 * Initializes the options parser by generating the configuration structures and
 * passing them into boost's program_options.
 *
 * Call this once from the constructor
 */
void OptionsParser::init()
{
    // Declare a group of options that will only be allowed from the command
    // line
    mCommandLineOptions.add_options()
        (
            "version,v",
            "Print version information"
        )
        (
            "help,h",
            "Show command line options"
        )
        (
            "license",
            "Show licensing information"
        )
        (
            "debug",
            po::value<bool>( &mConfig.debug )->default_value( true ),
            "Enables extra debug options in the game"
        )
        (
            "quiet",
            po::value<bool>( &mConfig.quiet )->default_value( false ),
            "Greatly reduces the amount of information sent to the console"
        )
        (
            "datadir",
            po::value<std::string>( &mConfig.contentPath ),
            "Directory containing game content files"
        )
        (
            "config",
            po::value<std::string>( &mConfigPath ),
            "Path to an additional configuration file"
        )
        ;

    // Declare a group of options that will be allowed from both the command
    // line and from a configuration file
    mGenericOptions.add_options()
        (
            "renderer.width,w",
            po::value<int>( &mConfig.rwWidth ),
            "Width of the main game window"
        )
        (
            "renderer.height,h",
            po::value<int>( &mConfig.rwHeight ),
            "Height of the main game window"
        )
        (
            "renderer.x",
            po::value<int>( &mConfig.rwX ),
            "X position to create window at"
        )
        (
            "renderer.y",
            po::value<int>( &mConfig.rwY ),
            "Y position to create window at"
        )
        (
            "renderer.fullscreen,f",
            po::value<bool>( &mConfig.rwFullscreen ),
            "Launch in full screen or windowed mode"
        )
        (
            "game.randomseed,s",
            po::value<unsigned int>( &mConfig.randomSeed ),
            "Value to seed the random number generator with"
        )
    ;
}

/**
 * Takes a user inputted string random seed and attempts to convert it into
 * an unsigned integer. If the string is a numeric value, this function will
 * merely convert it from a string. However, if the string is not numeric then
 * this function will hash it and generate a value from that.
 */
unsigned int OptionsParser::parseSeed( const std::string& value )
{
    unsigned int seed = 0u;

    // Try to cast it to an integer. If this fails... well it wasn't a numeric
    // value!
    try
    {
        seed = boost::lexical_cast<unsigned int>( value );
    }
    catch ( boost::bad_lexical_cast& )
    {
        // Failed to cast. Hash the string into a numeric value instead
        try
        {
            boost::hash<std::string> hasher;
            seed = hasher( value );
        }
        catch ( ... )
        {
            std::cerr << "Failed to parse random seed '" << value << "'"
                      << std::endl;
            mHadErrors = true;
        }
    }
    catch ( ... )
    {
        std::cerr << "Failed to lexically cast random seed '" << value << "'"
                  << std::endl;
        mHadErrors = true;
    }

    return seed;
}
