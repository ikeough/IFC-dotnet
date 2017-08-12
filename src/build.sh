#!/bin/bash
cd IFC-dotnet
antlr4 -Dlanguage=CSharp -package STEP -o ./antlr STEP.g4
cd ..
dotnet build
cd IFC-dotnet-test
dotnet xunit
cd ..