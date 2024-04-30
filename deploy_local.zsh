#!/bin/zsh

SERVICENAME="alexandria-api"

git fetch

FORCEUPDATE=false

if [[ $1 =~ ^(-f|--force) ]]; then
    FORCEUPDATE=true
fi

if $FORCEUPDATE || [ $(git rev-parse HEAD) != $(git rev-parse @{u}) ]; then
    DATESTAMP=$(date +%Y-%m-%d)
    
    git pull
    git reset --hard origin/main

    docker build . --tag $SERVICENAME --tag $SERVICENAME:v$DATESTAMP
    
    docker-compose down
    docker-compose up -d
else
    echo "$SERVICENAME already up-to-date"
fi
