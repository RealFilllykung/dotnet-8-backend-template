using dotnet_8_backend_template.models;

namespace dotnet_8_backend_template.interfaces.services;

public interface IIpService
{
    public Task<IpResponseModel> GetCurrentMachinePublicIp();
}