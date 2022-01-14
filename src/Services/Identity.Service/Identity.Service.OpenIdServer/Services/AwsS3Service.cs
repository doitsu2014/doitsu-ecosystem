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

public interface IAwsS3Service
{
    Task<PutObjectResponse> UploadFileAsync(string filename, byte[] content, string bucketPath = "");
}

public class AwsS3Service : IAwsS3Service
{
    private readonly ILogger<AwsS3Service> _logger;
    private readonly AwsS3Settings _awsS3Settings;

    public AwsS3Service(IOptions<AwsS3Settings> awsS3Settings, ILogger<AwsS3Service> logger)
    {
        _logger = logger;
        _awsS3Settings = awsS3Settings.Value;
    }

    private AmazonS3Config GetConfig()
    {
        var config = new AmazonS3Config();
        config.ServiceURL = _awsS3Settings.Url;
        config.ForcePathStyle = true;
        return config;
    }

    public async Task<PutObjectResponse> UploadFileAsync(string filename, byte[] content, string bucketPath = "")
    {
        using var amazonS3Client = new AmazonS3Client(_awsS3Settings.AccessKeyId, _awsS3Settings.AccessKeySecret, GetConfig());
        using var memoryStream = new MemoryStream(content);

        var putRequest = new PutObjectRequest
        {
            Key = filename,
            BucketName = _awsS3Settings.BucketName,
            InputStream = memoryStream
        };
        
        var result = await amazonS3Client.PutObjectAsync(putRequest);
        _logger.LogInformation($"Upload file to bucket {_awsS3Settings.BucketName} with filename {filename}, response status code: {result.HttpStatusCode}");
        return result;
    }
}