using Amazon.CloudWatchLogs;
using Amazon.Runtime;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog.Sinks.AwsCloudWatch;

namespace SeriLogX.Logging;

public static class SerilogConfigurator
{
    public static async Task<LoggerConfiguration> BuildLoggerConfigurationAsync(IConfiguration config)
    {
        var loggerConfig = new LoggerConfiguration()
            .MinimumLevel.Information()
            .Enrich.FromLogContext()
            .WriteTo.Console(); // Always log to console

        var enableFile = config.GetValue<bool>("Serilog:LogSinks:EnableFile");
        var enableCloudWatch = config.GetValue<bool>("Serilog:LogSinks:EnableCloudWatch");

        if (enableFile)
        {
            loggerConfig.WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day);
        }

        if (enableCloudWatch)
        {
            try
            {
                var region = config["Serilog:AWS:Region"];
                var accessKey = config["Serilog:AWS:AccessKey"];
                var secretKey = config["Serilog:AWS:SecretKey"];
                var logGroup = config["Serilog:AWS:LogGroup"];

                if (string.IsNullOrWhiteSpace(region) ||
                    string.IsNullOrWhiteSpace(accessKey) ||
                    string.IsNullOrWhiteSpace(secretKey) ||
                    string.IsNullOrWhiteSpace(logGroup))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("CloudWatch logging is enabled, but AWS credentials or LogGroup are missing.");
                    Console.ResetColor();
                }
                else
                {
                    var cloudWatchClient = new AmazonCloudWatchLogsClient(
                        new BasicAWSCredentials(accessKey, secretKey),
                        new AmazonCloudWatchLogsConfig
                        {
                            RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(region)
                        });

                    await EnsureLogGroupAsync(cloudWatchClient, logGroup);

                    var cloudWatchOptions = new CloudWatchSinkOptions
                    {
                        LogGroupName = logGroup,
                        LogStreamNameProvider = new DefaultLogStreamProvider(),
                        TextFormatter = new RenderedCompactJsonFormatter(),
                        MinimumLogEventLevel = LogEventLevel.Information,
                        CreateLogGroup = false,
                    };

                    loggerConfig.WriteTo.AmazonCloudWatch(cloudWatchOptions, cloudWatchClient);
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Failed to configure CloudWatch logging. Error: " + ex.Message);
                Console.ResetColor();
            }
        }

        return loggerConfig;
    }

    private static async Task EnsureLogGroupAsync(IAmazonCloudWatchLogs client, string logGroupName)
    {
        var groups = await client.DescribeLogGroupsAsync(new Amazon.CloudWatchLogs.Model.DescribeLogGroupsRequest
        {
            LogGroupNamePrefix = logGroupName
        });

        if (!groups.LogGroups.Any(g => g.LogGroupName == logGroupName))
        {
            await client.CreateLogGroupAsync(new Amazon.CloudWatchLogs.Model.CreateLogGroupRequest
            {
                LogGroupName = logGroupName
            });
        }
    }
}
