#!/bin/bash

docker run -h db2server --name db2server --restart=always --detach --privileged=true -p 50000:50000 --env-file ./Frends.IBMDB2.ExecuteQuery.Tests/lib/env_list.txt -v $PWD:/database icr.io/db2_community/db2

# Set LD_LIBRARY_PATH to the first path

export LD_LIBRARY_PATH="$PWD/_build/net.ibm.data.db2-lnx/7.0.0.200/buildTransitive/clidriver/lib"

echo "LD_LIBRARY_PATH set to: $LD_LIBRARY_PATH"

ls $LD_LIBRARY_PATH

