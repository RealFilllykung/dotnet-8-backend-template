using dotnet_8_backend_template.interfaces.repositories;
using dotnet_8_backend_template.models;
using dotnet_8_backend_template.services;
using Moq;
using Xunit;

namespace dotnet_8_backend_template.tests.services;

public class IpServiceTests
{
    private readonly Mock<IIpRepository> _mockIpRepository;
    private readonly Mock<ILogger<IpService>> _mockLogger;
    private readonly IpService _ipService;

    public IpServiceTests()
    {
        _mockIpRepository = new Mock<IIpRepository>();
        _mockLogger = new Mock<ILogger<IpService>>();
        _ipService = new IpService(_mockIpRepository.Object, _mockLogger.Object);
    }

    #region Positive Test Cases

    [Fact]
    public async Task GivenValidIpResponse_WhenGetCurrentMachinePublicIp_ThenReturnsCorrectIpAddress()
    {
        string expectedIp = "1.2.3.4";
        string mockRepositoryResponse = $"{{\"ip\": {expectedIp}}}";

        _mockIpRepository
            .Setup(repo => repo.GetCurrentPublicIp())
            .ReturnsAsync(mockRepositoryResponse);

        IpResponseModel result = await _ipService.GetCurrentMachinePublicIp();

        Assert.NotNull(result);
        Assert.Equal(expectedIp, result.ip);

        _mockIpRepository.Verify(repo => repo.GetCurrentPublicIp(), Times.Once);

        _mockLogger.Verify(
            logger => logger.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("[IpService] Calling IP repository")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

        _mockLogger.Verify(
            logger => logger.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("[IpService] Successfully receive a response")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task GivenDifferentValidIpAddress_WhenGetCurrentMachinePublicIp_ThenReturnsCorrectIpAddress()
    {
        string expectedIp = "192.168.1.100";
        string mockRepositoryResponse = $"{{\"ip\": {expectedIp}}}";

        _mockIpRepository
            .Setup(repo => repo.GetCurrentPublicIp())
            .ReturnsAsync(mockRepositoryResponse);

        IpResponseModel result = await _ipService.GetCurrentMachinePublicIp();

        Assert.NotNull(result);
        Assert.Equal(expectedIp, result.ip);

        _mockLogger.Verify(
            logger => logger.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("[IpService] Calling IP repository")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

        _mockLogger.Verify(
            logger => logger.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("[IpService] Successfully receive a response")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task GivenSuccessfulRepositoryCall_WhenGetCurrentMachinePublicIp_ThenLogsInformationMessages()
    {
        string mockRepositoryResponse = "{\"ip\": 1.2.3.4}";

        _mockIpRepository
            .Setup(repo => repo.GetCurrentPublicIp())
            .ReturnsAsync(mockRepositoryResponse);

        await _ipService.GetCurrentMachinePublicIp();

        _mockLogger.Verify(
            logger => logger.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("[IpService] Calling IP repository")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

        _mockLogger.Verify(
            logger => logger.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("[IpService] Successfully receive a response")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    #endregion

    #region Negative Test Cases

    [Fact]
    public async Task GivenRepositoryThrowsException_WhenGetCurrentMachinePublicIp_ThenThrowsException()
    {
        var expectedException = new HttpRequestException("Unable to connect to the remote server");

        _mockIpRepository
            .Setup(repo => repo.GetCurrentPublicIp())
            .ThrowsAsync(expectedException);

        var exception = await Assert.ThrowsAsync<HttpRequestException>(
            _ipService.GetCurrentMachinePublicIp
        );

        Assert.Equal("Unable to connect to the remote server", exception.Message);

        _mockIpRepository.Verify(repo => repo.GetCurrentPublicIp(), Times.Once);

        _mockLogger.Verify(
            logger => logger.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("[IpService] Calling IP repository")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task GivenRepositoryThrowsTimeoutException_WhenGetCurrentMachinePublicIp_ThenThrowsException()
    {
        var expectedException = new TimeoutException("The operation has timed out");

        _mockIpRepository
            .Setup(repo => repo.GetCurrentPublicIp())
            .ThrowsAsync(expectedException);

        var exception = await Assert.ThrowsAsync<TimeoutException>(
            _ipService.GetCurrentMachinePublicIp
        );

        Assert.Equal("The operation has timed out", exception.Message);

        _mockLogger.Verify(
            logger => logger.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("[IpService] Calling IP repository")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task GivenRepositoryThrowsTaskCanceledException_WhenGetCurrentMachinePublicIp_ThenThrowsException()
    {
        var expectedException = new TaskCanceledException("The request was canceled");

        _mockIpRepository
            .Setup(repo => repo.GetCurrentPublicIp())
            .ThrowsAsync(expectedException);

        await Assert.ThrowsAsync<TaskCanceledException>(
            _ipService.GetCurrentMachinePublicIp
        );

        _mockLogger.Verify(
            logger => logger.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("[IpService] Calling IP repository")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task GivenMalformedResponse_WhenGetCurrentMachinePublicIp_ThenThrowsIndexOutOfRangeException()
    {
        string malformedResponse = "invalid response without colon";

        _mockIpRepository
            .Setup(repo => repo.GetCurrentPublicIp())
            .ReturnsAsync(malformedResponse);

        await Assert.ThrowsAsync<IndexOutOfRangeException>(
            _ipService.GetCurrentMachinePublicIp
        );

        _mockLogger.Verify(
            logger => logger.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("[IpService] Calling IP repository")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task GivenEmptyResponse_WhenGetCurrentMachinePublicIp_ThenThrowsException()
    {
        string emptyResponse = "";

        _mockIpRepository
            .Setup(repo => repo.GetCurrentPublicIp())
            .ReturnsAsync(emptyResponse);

        await Assert.ThrowsAsync<IndexOutOfRangeException>(
            _ipService.GetCurrentMachinePublicIp
        );

        _mockLogger.Verify(
            logger => logger.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("[IpService] Calling IP repository")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task GivenNullResponse_WhenGetCurrentMachinePublicIp_ThenThrowsNullReferenceException()
    {
        _mockIpRepository
            .Setup(repo => repo.GetCurrentPublicIp())
            .ReturnsAsync((string)null!);

        await Assert.ThrowsAsync<NullReferenceException>(
            _ipService.GetCurrentMachinePublicIp
        );

        _mockLogger.Verify(
            logger => logger.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("[IpService] Calling IP repository")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task GivenNetworkError_WhenGetCurrentMachinePublicIp_ThenPropagatesException()
    {
        var networkException = new HttpRequestException("Network is unreachable");

        _mockIpRepository
            .Setup(repo => repo.GetCurrentPublicIp())
            .ThrowsAsync(networkException);

        var exception = await Assert.ThrowsAsync<HttpRequestException>(
            _ipService.GetCurrentMachinePublicIp
        );

        Assert.Contains("Network is unreachable", exception.Message);

        _mockLogger.Verify(
            logger => logger.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("[IpService] Calling IP repository")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public async Task GivenResponseWithExtraWhitespace_WhenGetCurrentMachinePublicIp_ThenParsesCorrectly()
    {
        string expectedIp = " 1.2.3.4 ";
        string mockRepositoryResponse = $"{{\"ip\":  {expectedIp} }}";

        _mockIpRepository
            .Setup(repo => repo.GetCurrentPublicIp())
            .ReturnsAsync(mockRepositoryResponse);

        IpResponseModel result = await _ipService.GetCurrentMachinePublicIp();

        Assert.NotNull(result);
        Assert.Equal(expectedIp.Trim(), result.ip.Trim());

        _mockLogger.Verify(
            logger => logger.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("[IpService] Calling IP repository")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

        _mockLogger.Verify(
            logger => logger.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("[IpService] Successfully receive a response")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task GivenMultipleConsecutiveCalls_WhenGetCurrentMachinePublicIp_ThenEachCallSucceeds()
    {
        string expectedIp1 = "1.2.3.4";
        string expectedIp2 = "5.6.7.8";
        string mockResponse1 = $"{{\"ip\": {expectedIp1}}}";
        string mockResponse2 = $"{{\"ip\": {expectedIp2}}}";

        _mockIpRepository
            .SetupSequence(repo => repo.GetCurrentPublicIp())
            .ReturnsAsync(mockResponse1)
            .ReturnsAsync(mockResponse2);

        IpResponseModel result1 = await _ipService.GetCurrentMachinePublicIp();
        IpResponseModel result2 = await _ipService.GetCurrentMachinePublicIp();

        Assert.Equal(expectedIp1, result1.ip);
        Assert.Equal(expectedIp2, result2.ip);

        _mockIpRepository.Verify(repo => repo.GetCurrentPublicIp(), Times.Exactly(2));

        _mockLogger.Verify(
            logger => logger.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("[IpService] Calling IP repository")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Exactly(2));

        _mockLogger.Verify(
            logger => logger.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("[IpService] Successfully receive a response")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Exactly(2));
    }

    #endregion
}
