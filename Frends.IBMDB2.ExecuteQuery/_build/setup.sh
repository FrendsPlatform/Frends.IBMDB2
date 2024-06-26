#!/bin/bash

# dotnet build

# # Use find to locate the file and store the result in an array

# basePath=$(sudo find / -name "net.ibm.data.db2-lnx")
# echo "Base path: $basePath"

# clidriver=$(sudo find $basePath -name "clidriver")
# echo "Clidriver: $clidriver"

# lib=$(sudo find $clidriver -name "lib")
# echo "Lib: $lib"

# # Set LD_LIBRARY_PATH to the first path
# export LD_LIBRARY_PATH="$lib"
# echo "LD_LIBRARY_PATH set to: $LD_LIBRARY_PATH"
# echo "DAYOFWEEK set to: $DAY_OF_WEEK"

# docker run -h db2server --name db2server --restart=always --detach --privileged=true -p 50000:50000 --env-file ./Frends.IBMDB2.ExecuteQuery.Tests/lib/env_list.txt -v $PWD:/database icr.io/db2_community/db2
#local sudo