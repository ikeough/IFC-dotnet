#!/bin/bash
antlr4 -Dlanguage=CSharp -package STEP -o ./antlr STEP.g4
#dotnet build ./csharp/IFC-gen.csproj
#dotnet run -p ./csharp/IFC-gen.csproj IFC4.exp ../../../IFC-dotnet/src/IFC-dotnet