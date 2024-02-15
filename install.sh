#!/bin/bash

# GitHub API URL for the latest release
api_url="https://api.github.com/repos/spelunky-fyi/modlunky2/releases/latest"
echo "Fetching latest release data from $api_url"

# Use curl to fetch the latest release data and jq to parse the JSON for the browser_download_url
download_url=$(curl -s $api_url | jq -r '.assets[0].browser_download_url')

# Check if the download URL is not empty
if [ -z "$download_url" ]; then
    echo "Download URL could not be found."
    exit 1
fi

# Use wget or curl to download the latest release
echo "Downloading latest release from $download_url"
curl -LO $download_url

echo "Download of Modlunky2 completed."
