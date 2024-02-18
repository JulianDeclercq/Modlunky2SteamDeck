using System.Text.Json;
using Modlunky2SteamDeck.Models;

namespace Modlunky2SteamDeck.Utils;

public static class GithubUtil
{
    private static readonly HttpClient SharedClient = new()
    {
        BaseAddress = new Uri("https://api.github.com"),
        DefaultRequestHeaders =
        {
            { "User-Agent", "csharp" }
        }
    };

    public static async Task DownloadLatestRelease(string owner, string repo, string savePath)
    {
        using var httpResponse = await SharedClient.GetAsync($"/repos/{owner}/{repo}/releases/latest");
        httpResponse.EnsureSuccessStatusCode();

        var jsonResponse = await httpResponse.Content.ReadAsStringAsync();

        var response = JsonSerializer.Deserialize(jsonResponse, ReleaseResponseContext.Default.ReleaseResponse)!;
        var downloadUrl = response.assets[0].browser_download_url;

        using var executableResponse = await SharedClient.GetAsync(downloadUrl);
        await using var fs = new FileStream(savePath, FileMode.CreateNew);
        await executableResponse.Content.CopyToAsync(fs);
    }
}