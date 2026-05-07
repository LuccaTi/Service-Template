using Castle.Core.Logging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using ServiceTemplate.Application.Interfaces;
using ServiceTemplate.Application.Orchestrators;

namespace ServiceTemplate.Application.Tests;

public class ServiceOrchestratorTests
{
    // To run a test use the 'test manager' in the Test tab on the upper visual studio menu.

    [Fact] // Annotation that indicates the method below is a test.
    public async Task EventHandlerAsync_WhenCancelled_ShouldNotProcess()
    {
        // 1. ARRANGE

        var mockLogger = new Mock<ILogger<ServiceOrchestrator>>();
        var mockEngine = new Mock<IServiceEngine>();

        var fakeSettings = Options.Create(new ServiceSettings { Interval = 10 });

        var orchestrator = new ServiceOrchestrator(mockLogger.Object, mockEngine.Object, fakeSettings);

        var tokenSource = new CancellationTokenSource();
        tokenSource.Cancel();

        // 2. ACT
        await orchestrator.EventHandlerAsync(tokenSource.Token);

        // 3. ASSERT
        mockEngine.Verify(x => x.ProcessAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task EventHandlerAsync_WhenNotCancelled_ShouldProcess()
    {
        // 1. ARRANGE
        var mockLogger = new Mock<ILogger<ServiceOrchestrator>>();
        var mockEngine = new Mock<IServiceEngine>();
        var fakeSettings = Options.Create(new ServiceSettings { Interval = 10 });
        var orchestrator = new ServiceOrchestrator(mockLogger.Object, mockEngine.Object, fakeSettings);

        var tokenSource = new CancellationTokenSource();

        mockEngine
            .Setup(x => x.ProcessAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Callback(() => tokenSource.Cancel());

        // 2. ACT
        await orchestrator.EventHandlerAsync(tokenSource.Token);

        // 3. ASSERT
        mockEngine.Verify(x => x.ProcessAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task EventHandlerAsync_WhenEngineThrowsException_ShouldLogErrorAndContinue()
    {
        // 1. ARRANGE
        var mockLogger = new Mock<ILogger<ServiceOrchestrator>>();
        var mockEngine = new Mock<IServiceEngine>();
        var fakeSettings = Options.Create(new ServiceSettings { Interval = 10 });
        var orchestrator = new ServiceOrchestrator(mockLogger.Object, mockEngine.Object, fakeSettings);

        var tokenSource = new CancellationTokenSource();
        var simulatedException = new Exception("Database connection failed!");

        mockEngine
            .Setup(x => x.ProcessAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(simulatedException)
            .Callback(() => tokenSource.Cancel());

        // 2. ACT
        await orchestrator.EventHandlerAsync(tokenSource.Token);

        // 3. ASSERT
        mockLogger.Verify(x => x.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => true),
            simulatedException,
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}