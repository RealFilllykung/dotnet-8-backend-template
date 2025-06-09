using dotnet_8_backend_template.interfaces.repositories;

namespace dotnet_8_backend_template.repositories;

public class IpRepository : IIpRepository
{
    
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<IpRepository> _logger;

    public IpRepository(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }
    
    public async Task<string> GetCurrentPublicIp()
    {
        try
        {
            var response = await _httpClient.GetAsync("");
            return response.Content.ReadAsStringAsync().Result;
        }
        catch (Exception error)
        {
            _logger.LogError(error, error.Message);
            throw;
        }
    }
}