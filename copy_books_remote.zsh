#!/bin/zsh
set -e

source=~/Library/CloudStorage/Dropbox/calibre/
destination=root@alexandria.local:/var/www/api/bookdata

echo "Copying books to remote server..."
rsync -avz --delete --exclude=appdata --exclude=full-text-search.db --info=progress2 --info=name0 $source $destination
echo "Finished copying books to remote server..."