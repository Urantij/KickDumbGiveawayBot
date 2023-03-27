using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace KickDumbGiveawayBot.Randomness;

public class MyRandomService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger _logger;

    private readonly string? apiKey;

    public MyRandomService(IHttpClientFactory httpClientFactory, ILogger<MyRandomService> logger, IOptions<AppOptions> options)
    {
        apiKey = options.Value.RandomOrgApiKey;
        this._httpClientFactory = httpClientFactory;
        this._logger = logger;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <param name="forceRandomOrg"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<int> GetIntAsync(int min, int max, bool forceRandomOrg)
    {
        try
        {
            if (apiKey != null)
                return await RequestFromRandomOrg(min, max);
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, $"{nameof(GetIntAsync)}.{nameof(RequestFromRandomOrg)}");
        }

        if (forceRandomOrg)
            throw new Exception($"Форсед, но без успеха. {nameof(apiKey)} != null ({apiKey != null})");

        return RandomNumberGenerator.GetInt32(min, max + 1);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    /// <exception cref="NullReferenceException"></exception>
    async Task<int> RequestFromRandomOrg(int min, int max)
    {
        if (apiKey == null)
            throw new NullReferenceException($"{nameof(apiKey)}");

        using var client = _httpClientFactory.CreateClient();

        JsonContent requestContent = JsonContent.Create(new
        {
            jsonrpc = "2.0",
            method = "generateIntegers",
            @params = new
            {
#pragma warning disable IDE0037
                apiKey = apiKey,
                n = 1,
                min = min,
                max = max
#pragma warning restore IDE0037
            },
            id = Guid.NewGuid().ToString("N")
        });

        using var response = await client.PostAsync($"https://api.random.org/json-rpc/4/invoke", requestContent);
        response.EnsureSuccessStatusCode();

        RandomOrgModel? content = await response.Content.ReadFromJsonAsync<RandomOrgModel>();

        if (content == null)
            throw new NullReferenceException($"{nameof(content)}");

        return content.Result.Random.Data[0];
    }
}
