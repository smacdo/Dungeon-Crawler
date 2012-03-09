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
#ifndef SCOTT_DUNGEON_OPTIONS_PARSER_H
#define SCOTT_DUNGEON_OPTIONS_PARSER_H

#include "engine/appconfig.h"

#include <string>
#include <vector>

#include <boost/utility.hpp>
#include <boost/program_options.hpp>

/**
 * Responsible for loading stored application configuration data, both from the
 * command line and from disk.
 */
class OptionsParser : boost::noncopyable
{
public:
    // Constructor
    OptionsParser();

    // Destructor
    ~OptionsParser();

    // Parse the command line
    bool parseCommandLine( int argc, char** args );

    // Parse a config file
    bool parseConfigFile( const std::string& path );

    // Does default command line processing. This handles the standard commands
    // like --help, --version and error checking.
    void process();

    // Generates a command line header
    std::string commandLineHeader() const;

    // Generates extra version information
    std::string version() const;

    // Generates help documentation
    std::string help() const;

    // Generates license text
    std::string license() const;

    // Returns a copy containing parsed options
    AppConfig appConfig() const;

    // Checks if the help command was requested
    bool helpRequested() const;

    // Checks if the version command was requested
    bool versionRequested() const;

    // Checks if the license command was requested
    bool licenseRequested() const;

    // Checks if the command line options were invalid
    bool hadErrors() const;

private:
    // Initializes the options parser by passing set up information to boost
    // program_options
    void init();

private:
    AppConfig mConfig;
    bool mHelpRequested;
    bool mVersionRequested;
    bool mLicenseRequested;
    bool mHadErrors;
    boost::program_options::options_description mGenericOptions;
    boost::program_options::options_description mCommandLineOptions;
    boost::program_options::variables_map       mVariables;
};

#endif
