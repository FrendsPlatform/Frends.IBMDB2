#!/bin/bash

docker run -h db2server --name db2server --restart=always --detach --privileged=true -p 50000:50000 --env-file ./Frends.IBMDB2.ExecuteQuery.Tests/lib/env_list.txt -v $PWD:/database icr.io/db2_community/db2

DLL = $(sudo find / -type f -name "libdb2.so")

echo "$DLL"

echo "${DLL[0]}"

export LD_LIBRARY_PATH="./Frends.IBMDB2.ExecuteQuery/clidriver/lib"