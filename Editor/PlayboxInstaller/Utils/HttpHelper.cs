#if UNITY_EDITOR

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace PlayboxInstaller
{
    public static class HttpHelper
    {
        private static HttpClient _client;

        private static string _baseUrl = "";

        private static void InitService()
        {
            var handler = new HttpClientHandler
            {
            };

            _client = new HttpClient(handler);
            
            _client.DefaultRequestHeaders.Accept.Clear();
            
            ProductInfoHeaderValue product = new ProductInfoHeaderValue("Playbox", "1.0");
            
            _client.DefaultRequestHeaders.UserAgent.Add(product);
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.raw"));
            
        }

        public static Task<HttpResult> GetAsync(string url, CancellationToken ct = default)
            => SendAsync(HttpMethod.Get, url, bodyJson: null, ct);

        public static Task<HttpResult> DeleteAsync(string url, CancellationToken ct = default)
            => SendAsync(HttpMethod.Delete, url, bodyJson: null, ct);

        public static Task<HttpResult> PostJsonAsync(string url, string bodyJson, CancellationToken ct = default)
            => SendAsync(HttpMethod.Post, url, bodyJson, ct);

        public static Task<HttpResult> PutJsonAsync(string url, string bodyJson, CancellationToken ct = default)
            => SendAsync(HttpMethod.Put, url, bodyJson, ct);

        public static async Task<HttpResult> SendAsync(HttpMethod method, string url, string bodyJson,
            CancellationToken ct = default)
        {
            if (_client == null)
                InitService();
            
            if(_client == null)
                Debug.Log("Client is not initialized");
            
            using var req = new HttpRequestMessage(method, url);

            if (bodyJson != null)
            {
                req.Content = new StringContent(bodyJson, Encoding.UTF8, "application/json");
            }

            var startedAt = DateTime.UtcNow;

            try
            {
                using var resp = await _client.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, ct)
                    .ConfigureAwait(false);

                var responseText = resp.Content != null
                    ? await resp.Content.ReadAsStringAsync().ConfigureAwait(false)
                    : null;

                return new HttpResult
                {
                    IsSuccess = resp.IsSuccessStatusCode,
                    StatusCode = (int)resp.StatusCode,
                    Body = responseText,
                    Error = resp.IsSuccessStatusCode ? null : $"{(int)resp.StatusCode} {resp.ReasonPhrase}",
                    DurationMs = (long)(DateTime.UtcNow - startedAt).TotalMilliseconds
                };
            }
            catch (OperationCanceledException oce) when (!ct.IsCancellationRequested)
            {
                return new HttpResult
                {
                    IsSuccess = false,
                    StatusCode = 0,
                    Body = null,
                    Error = $"Timeout: {oce.Message}",
                    DurationMs = (long)(DateTime.UtcNow - startedAt).TotalMilliseconds
                };
            }
            catch (OperationCanceledException oce)
            {
                return new HttpResult
                {
                    IsSuccess = false,
                    StatusCode = 0,
                    Body = null,
                    Error = $"Canceled: {oce.Message}",
                    DurationMs = (long)(DateTime.UtcNow - startedAt).TotalMilliseconds
                };
            }
            catch (HttpRequestException hre)
            {
                return new HttpResult
                {
                    IsSuccess = false,
                    StatusCode = 0,
                    Body = null,
                    Error = $"HttpRequestException: {hre.Message}",
                    DurationMs = (long)(DateTime.UtcNow - startedAt).TotalMilliseconds
                };
            }
            catch (Exception ex)
            {
                return new HttpResult
                {
                    IsSuccess = false,
                    StatusCode = 0,
                    Body = null,
                    Error = $"Unexpected: {ex.GetType().Name}: {ex.Message}",
                    DurationMs = (long)(DateTime.UtcNow - startedAt).TotalMilliseconds
                };
            }
        }
    }


    public sealed class HttpResult
    {
        public bool IsSuccess;
        public int StatusCode;
        public string Body;
        public string Error;
        public long DurationMs;
    }
}

#endif