using System.Text.Json;
using Microsoft.Extensions.Options;
using Serilog;
using Service.Config;
using Service.Models;

namespace Service.Client;

public class FundaClient : IFundaClient
{
    private readonly HttpClient _httpClient;
    private readonly FundaPartnerApiConfig _options;

    public FundaClient(HttpClient httpClient, IOptions<FundaPartnerApiConfig> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<FundaRealEstateAgentsOffers?> GetFundaRealEstateAgentsOffers(string type, string[] filterParam, int page = 1, int pageSize = 25, CancellationToken cancellationToken = default)
    {
        try
        {
            var url = ComposeUrl(type, filterParam, page, pageSize);
            Log.Verbose("Getting data from Funda API for page : {Page}", page);

            var response = await _httpClient.GetAsync(url, cancellationToken);
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                Log.Error("Getting data from Funda API for Url : {Url}, returned StatusCode {StatusCode} and content {Content}", url, response.StatusCode, content);
            }

            var fundaRealEstateAgentsOffers = JsonSerializer.Deserialize<FundaRealEstateAgentsOffers>(content);
            Log.Verbose("Succesfully retrieved data from Funda API for page : {Page}", page);

            return fundaRealEstateAgentsOffers;
        }
        catch (Exception e)
        {
            Log.Error(e, "Error fetching and deserializing data from Funda API.");
            return null;
        }
    }

    private string ComposeUrl(string type, string[] filterParam, int page, int pageSize)
    {
        var url = $"{_options.BaseUrl}/{_options.Key}/?type={type}";
        if (filterParam.Any())
        {
            url += $"&zo=/{string.Join("/", filterParam)}";
        }
        url += $"/&page={page}&pagesize={pageSize}";
        return url;
    }
}