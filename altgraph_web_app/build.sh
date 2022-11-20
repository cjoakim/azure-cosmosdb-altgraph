#!/bin/bash

# Compile this app and produce an uberJar file.
# Chris Joakim, Microsoft, November 2022

echo "clean..."
gradle clean

echo "build..."
gradle build

echo "uberJar..."
gradle uberJar

echo "dir of build\libs..."
ls -al build/libs/

echo "gradle dependencies..."
mkdir -p tmp/
gradle -q dependencies > tmp/dependencies.txt

echo "build script complete"
