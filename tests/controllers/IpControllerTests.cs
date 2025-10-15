using System.Net;
using dotnet_8_backend_template.interfaces.services;
using dotnet_8_backend_template.models;
using Microsoft.AspNetCore.TestHost;
using Moq;
using Xunit;

namespace dotnet_8_backend_template.tests.controllers;

public class IpControllerTests : IDisposable
{
    private TestServer? _testServer;
    private HttpClient? _httpClient;

    public void Dispose()
    {
        _httpClient?.Dispose();
        _testServer?.Dispose();
    }

    #region Positive Test Cases

    [Fact]
    public async Task GivenValidRequest_WhenGetCurrentMachinePublicIp_ThenReturnsOkWithIpAddress()
    {
        string expectedIp = "203.0.113.42";
        var mockIpService = new Mock<IIpService>();
        mockIpService
            .Setup(service => service.GetCurrentMachinePublicIp())
            .ReturnsAsync(new IpResponseModel { ip = expectedIp });

        _testServer = new TestServer(new WebHostBuilder()
            .ConfigureServices(services =>
            {
                services.AddControllers();
                services.AddTransient(_ => mockIpService.Object);
            })
            .Configure(app =>
            {
                app.UseRouting();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });
            }));

        _httpClient = _testServer.CreateClient();

        var response = await _httpClient.GetAsync("/ip");

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<IpResponseModel>();
        Assert.NotNull(result);
        Assert.Equal(expectedIp, result.ip);

        mockIpService.Verify(service => service.GetCurrentMachinePublicIp(), Times.Once);
    }

    #endregion

    #region Negative Test Cases

    [Fact]
    public async Task GivenServiceThrowsException_WhenGetCurrentMachinePublicIp_ThenThrowsException()
    {
        var mockIpService = new Mock<IIpService>();
        mockIpService
            .Setup(service => service.GetCurrentMachinePublicIp())
            .ThrowsAsync(new HttpRequestException("Service unavailable"));

        _testServer = new TestServer(new WebHostBuilder()
            .ConfigureServices(services =>
            {
                services.AddControllers();
                services.AddTransient(_ => mockIpService.Object);
            })
            .Configure(app =>
            {
                app.UseRouting();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });
            }));

        _httpClient = _testServer.CreateClient();

        var exception = await Assert.ThrowsAsync<HttpRequestException>(
            async () => await _httpClient.GetAsync("/ip")
        );

        Assert.Equal("Service unavailable", exception.Message);

        mockIpService.Verify(service => service.GetCurrentMachinePublicIp(), Times.Once);
    }

    #endregion
}
