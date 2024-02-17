using Newtonsoft.Json;

namespace Modlunky2SteamDeck;

public static class GithubService
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
        using var response = await SharedClient.GetAsync($"/repos/{owner}/{repo}/releases/latest");
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var dynamicObject = JsonConvert.DeserializeObject<dynamic>(jsonResponse)!;
        var downloadUrl = dynamicObject.assets[0].browser_download_url.ToString();

        using var executableResponse = await SharedClient.GetAsync(downloadUrl);
        await using var fs = new FileStream(savePath, FileMode.CreateNew);
        await executableResponse.Content.CopyToAsync(fs);
    }
}