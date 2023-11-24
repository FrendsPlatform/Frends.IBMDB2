#!/bin/bash

# docker run -h db2server --name db2server --restart=always --detach --privileged=true -p 50000:50000 --env-file ./Frends.IBMDB2.ExecuteQuery.Tests/lib/env_list.txt -v $PWD:/database icr.io/db2_community/db2

sudo find / -type f -name "libdb2.so"

# Use find to locate the file and store the result in an array
mapfile -t myvariable=$(sudo find / -type f -name "libdb2.so")

# Check if the array is not empty
if [ "${#myvariable[@]}" -gt 0 ]; then
    # Print all paths
    echo "Paths found:"
    for path in "${myvariable[@]}"; do
        echo "$path"
    done

    # Set LD_LIBRARY_PATH to the first path
    export LD_LIBRARY_PATH="${myvariable[0]}"
    echo "LD_LIBRARY_PATH set to: $LD_LIBRARY_PATH"
else
    echo "File not found"
fi