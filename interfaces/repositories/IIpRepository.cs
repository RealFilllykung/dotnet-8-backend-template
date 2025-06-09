namespace dotnet_8_backend_template.interfaces.repositories;

public interface IIpRepository
{
    public Task<string> GetCurrentPublicIp();
}