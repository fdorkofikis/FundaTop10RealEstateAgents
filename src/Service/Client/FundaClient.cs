using System.Text.Json;
using Microsoft.Extensions.Options;
using Serilog;
using Service.Config;
using Service.Objects;

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
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                return JsonSerializer.Deserialize<FundaRealEstateAgentsOffers>(content);
            }
            
            Log.Error("Getting data from Funda API for page : {Page}, returned StatusCode {StatusCode}", page, response.StatusCode);
        }
        catch (Exception e)
        {
            Log.Error(e, "Error fetching and deserializing data from Funda API.");
            return null;
        }
        
        return null;
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