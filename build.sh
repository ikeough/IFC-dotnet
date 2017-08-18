#!/bin/bash
dir=$(pwd)
cd src/IFC-dotnet
java -jar /usr/local/lib/antlr-4.7-complete.jar -Dlanguage=CSharp -package STEP -o ./antlr STEP.g4
cd $dir
dotnet build -c Release
cd tests/IFC-dotnet-test
dotnet xunit -c Release
cd $dir