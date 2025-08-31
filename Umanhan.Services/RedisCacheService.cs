using Amazon.Runtime.Internal.Util;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Umanhan.Services.Interfaces;
using IDatabase = StackExchange.Redis.IDatabase;

namespace Umanhan.Services
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDatabase _redis;
        private readonly ILogger<RedisCacheService> _logger;
        private readonly ConcurrentDictionary<string, (DateTimeOffset Expiry, object Data)> _localCache = new();

        private const int TTL = 30; // in minutes

        public RedisCacheService(IConnectionMultiplexer connection, 
            ILogger<RedisCacheService> logger)
        {
            _redis = connection.GetDatabase();
            _logger = logger;
            this._logger = logger;
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            try
            {
                if (_localCache.TryGetValue(key, out var entry) && entry.Expiry > DateTimeOffset.UtcNow)
                    return (T)entry.Data;

                var data = await _redis.StringGetAsync(key);
                if (data.IsNullOrEmpty) return default;

                var value = JsonSerializer.Deserialize<T>(data!);
                _localCache[key] = (DateTimeOffset.UtcNow.AddSeconds(30), value!);
                return value;
            }
            catch (RedisConnectionException ex)
            {
                _logger.LogWarning(ex, "Redis connection failed while reading key '{Key}'.", key);
                return default;
            }
            catch (RedisTimeoutException ex)
            {
                _logger.LogWarning(ex, "Redis timeout while reading key '{Key}'.", key);
                return default;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error accessing Redis for key '{Key}'.", key);
                return default;
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? ttl = null, string tag = null)
        {
            try
            {
                ttl ??= TimeSpan.FromMinutes(TTL); // Default TTL if not provided

                var json = JsonSerializer.Serialize(value);
                await _redis.StringSetAsync(key, json, ttl);
                _localCache[key] = (DateTimeOffset.UtcNow.AddSeconds(30), value!);

                if (!string.IsNullOrEmpty(tag))
                {
                    var tagKey = $"tag:{tag}";
                    await _redis.SetAddAsync(tagKey, key);
                    await _redis.KeyExpireAsync(tagKey, ttl + TimeSpan.FromMinutes(1));
                }
            }
            catch (RedisConnectionException ex)
            {
                _logger.LogWarning(ex, "Redis connection failed while writing key '{Key}'.", key);
            }
            catch (RedisTimeoutException ex)
            {
                _logger.LogWarning(ex, "Redis timeout while writing key '{Key}'.", key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error writing to Redis for key '{Key}'.", key);
            }
        }

        public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? ttl = null, string tag = null)
        {
            var fromCache = await GetAsync<T>(key);
            if (fromCache != null && !fromCache.Equals(default(T)))
                return fromCache;

            ttl ??= TimeSpan.FromMinutes(TTL); // Default TTL if not provided

            // Cache miss or Redis error → fallback to DB
            var value = await factory();
            // Try writing to Redis (optional)
            await SetAsync(key, value, ttl, tag);
            return value;
        }

        public async Task RemoveAsync(string key)
        {
            try
            {
                await _redis.KeyDeleteAsync(key);
                _localCache.TryRemove(key, out _);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to remove cache key '{Key}'", key);
            }
        }

        public async Task InvalidateTagAsync(string tag)
        {
            try
            {
                var tagKey = $"tag:{tag}";
                var members = await _redis.SetMembersAsync(tagKey);

                if (members.Length > 0)
                {
                    foreach (var key in members)
                    {
                        await _redis.KeyDeleteAsync(key.ToString());
                    }

                    await _redis.KeyDeleteAsync(tagKey);
                    _logger.LogInformation("Invalidated tag: {Tag} with {Count} keys", tag, members.Length);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Redis TAG invalidation failed for tag: {Tag}", tag);
            }
        }
    }
}