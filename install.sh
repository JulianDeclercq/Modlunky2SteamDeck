#!/bin/bash

echo "Steam needs to be shutdown in order for Modlunky2SteamDeck to run. Shutting down Steam now.."
steam -shutdown

# GitHub API URL for the latest release
api_url="https://api.github.com/repos/JulianDeclercq/Modlunky2SteamDeck/releases/latest"
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

echo "Download of Modlunky2SteamDeck completed, running now."

chmod +x Modlunky2SteamDeck
./Modlunky2SteamDeck

echo "Restarting Steam.."
steam

exit