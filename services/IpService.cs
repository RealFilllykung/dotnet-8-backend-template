using dotnet_8_backend_template.interfaces.repositories;
using dotnet_8_backend_template.interfaces.services;
using dotnet_8_backend_template.models;

namespace dotnet_8_backend_template.services;

public class IpService : IIpService
{
    private readonly IIpRepository _ipRepository;
    private readonly ILogger<IpService> _logger;

    public IpService(IIpRepository ipRepository, ILogger<IpService> logger)
    {
        _ipRepository = ipRepository;
        _logger = logger;
    }
    
    public async Task<IpResponseModel> GetCurrentMachinePublicIp()
    {
        IpResponseModel responseModel = new IpResponseModel();
        _logger.LogInformation("[IpService] Calling IP repository to get IP string");
        string ipResponseBody = await _ipRepository.GetCurrentPublicIp();
        _logger.LogInformation("[IpService] Successfully receive a response of: " + ipResponseBody);
        responseModel.ip = GetIpFromResponseBody(ipResponseBody);
        return responseModel;
    }

    private string GetIpFromResponseBody(string ipResponseBody)
    {
        string[] colonSplit = ipResponseBody.Split(':');
        string behindSubstring = colonSplit[1].Substring(1);
        string[] tagSplit = behindSubstring.Split('<');
        string ip = tagSplit[0];
        return ip;
    }
}