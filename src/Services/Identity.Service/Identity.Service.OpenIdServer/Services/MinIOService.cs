using System.IO;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Identity.Service.OpenIdServer.Settings;
using LanguageExt;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;

namespace Identity.Service.OpenIdServer.Services;

public interface IMinIOService
{
    Task UploadFileAsync(string filename, byte[] content, string bucketPath = "");
}

public class MinIOService : IMinIOService
{
    private readonly ILogger<MinIOService> _logger;
    private readonly MinIOSetting _minIOSetting;

    public MinIOService(IOptions<MinIOSetting> minIOSetting, ILogger<MinIOService> logger)
    {
        _logger = logger;
        _minIOSetting = minIOSetting.Value;
    }

    private AmazonS3Config GetConfig()
    {
        var config = new AmazonS3Config();
        config.ServiceURL = _minIOSetting.Url;
        config.ForcePathStyle = true;
        config.SignatureVersion = "v4";
        return config;
    }

    public async Task UploadFileAsync(string filename, byte[] content, string bucketPath = "")
    {
        using var amazonS3Client = new AmazonS3Client(_minIOSetting.AccessKeyId, _minIOSetting.AccessKeySecret, GetConfig());
        using var memoryStream = new MemoryStream(content);

        var putRequest = new PutObjectRequest
        {
            Key = filename,
            BucketName = _minIOSetting.BucketName,
            InputStream = memoryStream
        };
        
        var result = await amazonS3Client.PutObjectAsync(putRequest);
        _logger.LogInformation($"Upload file to bucket {_minIOSetting.BucketName} with filename {filename}, response status code: {result.HttpStatusCode}");
    }
}