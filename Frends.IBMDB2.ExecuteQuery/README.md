# Frends.IBMDB2.ExecuteQuery

Frends Task for executing queries in IBMDB2 database.

[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://opensource.org/licenses/MIT)
[![Build](https://github.com/FrendsPlatform/Frends.IBMDB2/actions/workflows/ExecuteQuery_build_and_test_on_main.yml/badge.svg)](https://github.com/FrendsPlatform/Frends.IBMDB2/actions)
![Coverage](https://app-github-custom-badges.azurewebsites.net/Badge?key=FrendsPlatform/Frends.IBMDB2/Frends.IBMDB2.ExecuteQuery|main)

## Installing

You can install the Task via frends UI Task View.

## Building

### Clone a copy of the repository

`git clone https://github.com/FrendsPlatform/Frends.IBMDB2.git`

### Build the project

`dotnet build`

### Run tests

Create a simple DB2 server to docker

`docker run -h db2server --name db2server --restart=always --detach --privileged=true -p  50000:50000 --env-file ./Frends.IBMDB2.ExecuteQuery.Tests/lib/env_list.txt -v $PWD:/database icr.io/db2_community/db2`

Run the tests

`dotnet test`

### Create a NuGet package

`dotnet pack --configuration Release`

### Third-party licenses

StyleCop.Analyzer version (unmodified version 1.1.118) used to analyze code uses Apache-2.0 license, full text and source code can be found in [StyleCopAnalyzers GitHub](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/README.md)