#!/bin/bash
testbin=game/test-dungeoncrawler

valgrind --leak-check=yes --track-origins=yes --leak-resolution=high $testbin
