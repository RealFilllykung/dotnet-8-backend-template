using dotnet_8_backend_template.interfaces.services;
using dotnet_8_backend_template.models;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_8_backend_template.controllers;

[ApiController]
[Route("ip")]
public class IpController : ControllerBase
{
    private readonly ILogger<IpController> _logger;
    private readonly IIpService _ipService;

    public IpController(ILogger<IpController> logger, IIpService ipService)
    {
        _logger = logger;
        _ipService = ipService;
    }

    [HttpGet]
    public async Task<IpResponseModel> GetCurrentMachinePublicIp()
    {
        return await _ipService.GetCurrentMachinePublicIp();
    }
    
}