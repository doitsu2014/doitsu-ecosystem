using System;
using Amazon.S3;
using Serilog;
using Serilog.Configuration;
using Serilog.Formatting.Compact;
using Shared.LanguageExt.Common;

namespace Identity.Service.OpenIdServer;

public static class Extensions
{
    public static LoggerConfiguration MinIO(this LoggerSinkConfiguration sinkConf, Action<SerilogMinIOSetting> bind)
    {
        var setting = new SerilogMinIOSetting();
        bind(setting);

        if (setting.Url.IsNullOrEmpty())
        {
            throw new ArgumentException("MinIO Url is missing");
        }

        if (setting.AccessKeyId.IsNullOrEmpty())
        {
            throw new ArgumentException("MinIO AccessKeyId is missing");
        }


        if (setting.AccessKeySecret.IsNullOrEmpty())
        {
            throw new ArgumentException("MinIO AccessKeySecret is missing");
        }

        var config = new AmazonS3Config();
        config.ServiceURL = setting.Url;
        config.ForcePathStyle = true;
        config.SignatureVersion = "v4";
        var amazonClient = new AmazonS3Client(setting.AccessKeyId, setting.AccessKeySecret, config);

        return
            setting.IsJsonFormatter
                ? sinkConf.AmazonS3(amazonClient
                    , setting.Path
                    , setting.BucketName
                    , setting.LogEventLevel
                    , queueSizeLimit: setting.QueueSizeLimit
                    , batchingPeriod: setting.BatchingPeriod
                    , batchSizeLimit: setting.BatchSizeLimit
                    , eagerlyEmitFirstEvent: setting.EagerlyEmitFirstEvent
                    , formatter: new CompactJsonFormatter()
                    , rollingInterval: setting.RollingInterval
                    , bucketPath: setting.BucketPath)
                : sinkConf.AmazonS3(amazonClient
                    , setting.Path
                    , setting.BucketName
                    , setting.LogEventLevel
                    , queueSizeLimit: setting.QueueSizeLimit
                    , batchingPeriod: setting.BatchingPeriod
                    , batchSizeLimit: setting.BatchSizeLimit
                    , eagerlyEmitFirstEvent: setting.EagerlyEmitFirstEvent
                    , outputTemplate: setting.OutputTemplate
                    , rollingInterval: setting.RollingInterval
                    , bucketPath: setting.BucketPath);
    }
}