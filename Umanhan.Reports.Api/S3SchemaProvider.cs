
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Caching.Memory;

namespace Umanhan.Reports.Api
{
    public class S3SchemaProvider : ISchemaProvider
    {
        private string BucketName = Environment.GetEnvironmentVariable("S3_BUCKET_NAME") ?? throw new InvalidOperationException("Missing environment variable: S3_BUCKET_NAME");
        private string ObjectKey = Environment.GetEnvironmentVariable("SCHEMA_FILENAME") ?? throw new InvalidOperationException("Missing environment variable: SCHEMA_FILENAME");
        private readonly IAmazonS3 _s3;
        private readonly IMemoryCache _cache;
        private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(1);

        public S3SchemaProvider(IAmazonS3 s3, IMemoryCache cache)
        {
            _s3 = s3;
            _cache = cache;
        }

        public async Task<string> GetSchemaAsync(CancellationToken ct = default)
        {
            // Try cache first
            if (_cache.TryGetValue<string>("DbSchema", out var schema))
                return schema;

            // Fetch from S3
            var response = await _s3.GetObjectAsync(new GetObjectRequest
            {
                BucketName = BucketName,
                Key = ObjectKey
            }, ct).ConfigureAwait(false);

            using var reader = new StreamReader(response.ResponseStream);
            schema = await reader.ReadToEndAsync().ConfigureAwait(false);

            // Cache it
            _cache.Set("DbSchema", schema, CacheDuration);
            return schema;
        }
    }
}
