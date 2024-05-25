#!/bin/zsh
set -e

# Default to Release mode
configuration="Release"

# Process command-line arguments
while (( "$#" )); do
  case "$1" in
    -c|--configuration)
      # Convert the argument to lowercase
      config=$(echo "$2" | tr '[:upper:]' '[:lower:]')
      if [ "$config" = "debug" ]; then
        configuration="Debug"
      elif [ "$config" = "release" ]; then
        configuration="Release"
      else
        echo "Invalid configuration: $2. Use 'debug' or 'release'."
        exit 1
      fi
      shift 2
      ;;
    *)
      echo "Unknown option: $1"
      exit 1
      ;;
  esac
done

rm -rf ./publish
echo "Removed local publish directory..."

# Build the project in the specified configuration mode
if dotnet publish -c $configuration ./alexandria_api.csproj -o ./publish; then
    echo "Build successful, proceeding with deployment..."

    # Exclude the appdata and bookdata directories
    rm -rf ./publish/appdata
    rm -rf ./publish/bookdata
    rm -rf ./publish/api-collection
    echo "Excluded local directories appdata, bookdata, api-collection..."

    ssh root@alexandria.local 'systemctl stop alexandria_api.service'
    echo "Stopped remote service..."

    echo "Deploying to remote server..."
    rsync -avz --delete --exclude=appdata --exclude=bookdata ./publish/ root@alexandria.local:/var/www/api
    echo "Deployed to remote server..."

    ssh root@alexandria.local 'systemctl start alexandria_api.service'
    echo "Started remote service..."

else
    echo "Build failed, aborting deployment."
    exit 1
fi
