#!/bin/bash

echo "Steam needs to be shutdown in order for Modlunky2SteamDeck to run. Shutting down Steam now.."
steam -shutdown &

while pgrep -x steam >/dev/null; do
    echo "Steam is being shut down, please wait.."
    sleep 3
done

echo "Steam has been shut down, continuing install of Modlunky2SteamDeck"

# Fetch the latest Modlunky2SteamDeck release
api_url="https://api.github.com/repos/JulianDeclercq/Modlunky2SteamDeck/releases/latest"
echo "Fetching latest release data from $api_url"

# Use curl to fetch the latest release data and jq to parse the JSON for the browser_download_url
download_url=$(curl -s $api_url | jq -r '.assets[0].browser_download_url')

if [ -z "$download_url" ]; then
    echo "Download URL could not be found."
    exit 1
fi

# Download the release
echo "Downloading latest release from $download_url"
curl -LO $download_url

echo "Download of Modlunky2SteamDeck completed, running now."

# Execute the release
chmod +x Modlunky2SteamDeck
./Modlunky2SteamDeck

# Keep terminal open to show eventual errors
exec $SHELL