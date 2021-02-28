using System;
using System.Collections.Immutable;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Blazor.FileConversion.Client.Models;
using Microsoft.Extensions.Logging;

namespace Blazor.FileConversion.Client.HttpClients.FileConversion
{
    public class FileConversionHttpClient : IFileConversionHttpClient
    {
        public const string FileConversionHttpClientSettings = "FileConversionHttpClientSettings";
        public const string HttpClientKey = "FileConversionApi";
        private readonly ILogger<FileConversionHttpClient> _logger;
        private readonly HttpClient _client;

        public FileConversionHttpClient(IHttpClientFactory clientFactory, ILogger<FileConversionHttpClient> logger)
        {
            _logger = logger;
            _client = clientFactory.CreateClient(HttpClientKey);
            if (_client == null)
            {
                throw new ArgumentNullException($"Could not resolve HttpClient {HttpClientKey}");
            }
            else
            {
            }
        }

        public async Task<ImmutableList<PackagingDocument>> ParsePackagingDocumentAsync(
            MultipartFormDataContent content)
        {
            var postResult = await _client.PostAsync("api/parser/LuanApplicationExcel/PackagingDocument", content);
            var postContent = await postResult.Content.ReadAsStringAsync();

            if (!postResult.IsSuccessStatusCode)
            {
                throw new ApplicationException(postContent);
            }
            else
            {
                var json = JsonSerializer.Deserialize<ImmutableList<PackagingDocument>>(postContent, new JsonSerializerOptions()
                {
                   PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
                return json;
            }
        }
    }
}