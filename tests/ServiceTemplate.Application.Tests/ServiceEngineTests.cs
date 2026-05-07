using Castle.Core.Logging;
using Microsoft.Extensions.Logging;
using Moq;
using ServiceTemplate.Application.Engines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceTemplate.Application.Tests
{
    public class ServiceEngineTests
    {
        [Fact]
        public async Task ProcessAsync_ShouldLogDebugMessage()
        {
            // 1. ARRANGE
            var mockLogger = new Mock<ILogger<ServiceEngine>>();
            var engine = new ServiceEngine(mockLogger.Object);

            var token = CancellationToken.None;

            // 2. ACT
            await engine.ProcessAsync(token);

            // 3. ASSERT
            mockLogger.Verify(x => x.Log(
                LogLevel.Debug,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }
}
