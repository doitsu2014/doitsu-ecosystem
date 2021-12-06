using System;
using Serilog.Events;

namespace Identity.Service.OpenIdServer;

public class AppSetting
{
    public MinIOSetting MinIOSetting { get; set; }
    public SerilogMinIOSetting SerilogMinIOSetting { get; set; }
    public InitialSetting InitialSetting { get; set; }
}

public class InitialSetting
{
    public InitialApplication Administrator { get; set; }
    public InitialUser AdminUser { get; set; }
    
    public class InitialApplication
    {
        public string ClientId { get; set; }
        public string DisplayName { get; set; }
        public string Uri { get; set; }
    }

    public class InitialUser
    {
        public string EmailAddress { get; set; }
        public string Password { get; set; }
    }
}

public class MinIOSetting
{
    /// <summary>
    /// Example: http://minio.doitsu.tech
    /// </summary>
    public string Url { get; set; }

    public string AccessKeyId { get; set; }
    public string AccessKeySecret { get; set; }
    public string BucketName { get; set; }
}

public class SerilogMinIOSetting : MinIOSetting
{
    public bool IsJsonFormatter { get; set; } = false;

    /// <summary>
    /// The time to wait between checking for unemitted events.
    /// If there are any unemitted events, they will then be uploaded to S3 in a batch of maximum size batchSizeLimit.
    /// Check: https://github.com/serilog/serilog-sinks-periodicbatching
    /// </summary>
    public TimeSpan BatchingPeriod { get; set; } = TimeSpan.FromSeconds(2);

    /// <summary>
    /// The maximum number of events to include in a single batch.
    /// This means an upload of events as a file to S3 will contain at most this number of events.
    /// Check: https://github.com/serilog/serilog-sinks-periodicbatching
    /// </summary>
    public int BatchSizeLimit { get; set; } = 100;

    /// <summary>
    /// The queue size limit meaning the limit until the last not emitted events are discarded (Standard mechanims to stop queue overflows).
    /// </summary>
    public int QueueSizeLimit { get; set; } = 10000;

    /// <summary>
    /// A value indicating whether the first event should be emitted immediately or not.	
    /// </summary>
    public bool EagerlyEmitFirstEvent { get; set; } = false;

    public string Path { get; set; }
    public string BucketPath { get; set; } = "";
    public LogEventLevel LogEventLevel { get; set; } = Serilog.Events.LogEventLevel.Verbose;
    public string OutputTemplate { get; set; } = "[{Timestamp:HH:mm:ss}-{Level}-{ThreadId}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}";
    public Serilog.Sinks.AmazonS3.RollingInterval RollingInterval { get; set; } = Serilog.Sinks.AmazonS3.RollingInterval.Day;
}