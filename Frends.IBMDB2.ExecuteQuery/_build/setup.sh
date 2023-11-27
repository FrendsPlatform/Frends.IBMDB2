#!/bin/bash

docker run -h db2server --name db2server --restart=always --detach --privileged=true -p 50000:50000 --env-file ./Frends.IBMDB2.ExecuteQuery.Tests/lib/env_list.txt -v $PWD:/database icr.io/db2_community/db2

# Use find to locate the file and store the result in an array

nuget=$(sudo find / -name ".nuget")

echo "Nuget: $nuget"

basePath=$(sudo find $HOME -name "net.ibm.data.db2-lnx")

clidriver=$(sudo find ${basePath[0]} -name "clidriver")

lib=$(sudo find ${clidriver[0]} -name "lib")

echo $lib

# Set LD_LIBRARY_PATH to the first path

export LD_LIBRARY_PATH="${lib[0]}"

echo "LD_LIBRARY_PATH set to: $LD_LIBRARY_PATH"

