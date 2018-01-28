// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NuGet.Common;

namespace MicroElements.FileStorage.NuGetEngine
{
    /// <summary>
    /// NuGet logger based on Microsoft.Extensions.Logging.
    /// </summary>
    public class NuGetLogger : LoggerBase
    {
        private readonly ILogger<NuGetLogger> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="NuGetLogger"/> class.
        /// </summary>
        /// <param name="logger">Microsoft logger.</param>
        public NuGetLogger(ILogger<NuGetLogger> logger)
        {
            _logger = logger;
        }

        /// <inheritdoc />
        public override void Log(ILogMessage message)
        {
            if (message.Level == NuGet.Common.LogLevel.Information)
                _logger.LogInformation(message.Message);
            else if (message.Level == NuGet.Common.LogLevel.Debug)
                _logger.LogDebug(message.Message);
            else if (message.Level == NuGet.Common.LogLevel.Warning)
                _logger.LogWarning(message.Message);
            else if (message.Level == NuGet.Common.LogLevel.Error)
                _logger.LogError(message.Message);
            else if (message.Level == NuGet.Common.LogLevel.Minimal)
                _logger.LogInformation(message.Message);
            else if (message.Level == NuGet.Common.LogLevel.Verbose)
                _logger.LogTrace(message.Message);
        }

        /// <inheritdoc />
        public override Task LogAsync(ILogMessage message)
        {
            Log(message);
            return Task.CompletedTask;
        }
    }
}
