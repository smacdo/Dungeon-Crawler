README 
======
This is the README file for the Dungeon Crawler game. Dungeon Crawler
is a simple graphical dungeon crawler inspired by historical roguelikes
mixed with modern and unqiue new game design. Explore procedurally
generated worlds filled with random dungeons housing epic loot, stake
out a portion of the unforgiving wild wildness to call your own and live
to tell the tales of your exploits! Just try not to die :)

![Dungeon Crawler Screenshot](http://i.imgur.com/8ODmQ.png)

TABLE OF CONTENTS
=================

1.   Prerequisites
2.   Building
3.   Installing
4.   Contributing
5.   Attributions and Credits
6.   Contact and Reporting Details


Prerequisites
=====================
 
  This project requires on of the following platforms to build:
    1. Windows
        * Visual Studio 2010
        * Boost libraries (exported via env var BOOST_ROOT)
    2. Linux
        * Cross-platform Make (CMake) v2.8+
        * GNU Make or equivalent
        * GCC 4.4 or newer (must support majority of C++0x standard)
        * Boost C++ libraries, v1.37+
        * SDL 2.0
        * SDL_image library, v1.2+

Building
========

Depending on your build system, you will need to build this project
in different ways. On Windows, simply open the 'dungeongame.sln'
project file. In Linux, you will either need to do the standard CMake
install path or use our conveniently provided ./configure wrapper
script followed by make.

Once built, the binary and all distributable files for the game should
be found in the game/ directory

INSTALLING
==========

Installing is not supported at the moment, since the project is very much
a work in progress. Contributions are welcome, otherwise once the project
becomes stable (ie, actually fun to play) I'll write scripts to generate
self installers along with linux packages.

CONTRIBUTIONS
=============

Contributions to this project are highly encouraged! At the moment the
project is in a heavy state of flux (seeing as it isn't even a playable
game yet), so it might be difficult to contribute new code right now.

Not a programmer? Fear not! I would *love* for any of the following
contributions:

1. Bug reports... let me know if something breaks or isn't right
2. Artwork for the game
3. Additional content for the game
4. Sound and music
5. Documentation
6. Play testing and balancing
7. Comments and feedback!! Even if it was just "meh", I would like to
    know someone tried it out :)

Forward all contributions, inquiries, comments and flames to:
scott@whitespaceconsideredharmful.com

Attributions and Credits
========================

Nobody yet! Why don't contribute something and be the first person on
this list :)

See thirdparty/LICENSES.txt for a list of thirdparty software used by this
project and for their respective licenses.

Contact and Reporting Info
==========================
Homepage: http://www.whitespaceconsideredharmful.com/dungeoncrawler

Bug reports:
Please include enough information for the developers to reproduce the
problem. Send all reports of bugs, crashes and other issues to Scott

