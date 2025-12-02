using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace ShiftAssignerServer.Tests.Infrastructure;

public class ClientSender
{
    private string _baseUrl;

    public ClientSender(string baseUrl)
    {
        _baseUrl = baseUrl;
    }

    public async Task<TDto> GetAsync<TDto>(string relativePath, string? token = null, Dictionary<string, string> parameters = null) where TDto : class
    {
        var url = PathLocator.Combine(_baseUrl, relativePath);

        using (HttpClient client = new HttpClient())
        {

            if (token is not null)
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            if (parameters is not null)
            {
                var encoder = new FormUrlEncodedContent(parameters);
                var queryString = await encoder.ReadAsStringAsync();
                url = $"{url}?{queryString}";
            }


            HttpResponseMessage response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var result = default(TDto);

                if (typeof(TDto) == typeof(byte[]))
                {
                    var buffer = await response.Content.ReadAsByteArrayAsync();
                    if (response.Content.Headers.ContentDisposition != null)
                    {
                        var fileName = response.Content.Headers.ContentDisposition.FileName?.Trim('\"');
                    }
                    else
                    {
                        // Handle cases where Content-Disposition header is missing or malformed
                        var fileName = "unknown_filename";
                    }

                    result = buffer as TDto;
                }
                else
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    if (responseContent.IsEmpty() == false)
                    {
                        var options = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        };

                        result = JsonSerializer.Deserialize<TDto>(responseContent, options);
                    }
                }

                return result;
            }

            throw new Exception("xxx");

            // var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
            // throw new ErrorResponseException { ErrorResponse = errorResponse };
        }
    }

    protected async Task<TResponse> DeleteCommand<TResponse>(string url) where TResponse : class
    {
        using (HttpClient client = new HttpClient())
        {
            var response = await client.DeleteAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var responseData = JsonSerializer.Deserialize<TResponse>(responseContent, options);
                return responseData;
            }

            throw new Exception($"Failed to perform DELETE request to {url}");
        }

    }

    protected async Task<TResponse> RunPostCommand<TRequest, TResponse>(string url, TRequest request) where TRequest : class
    {
        return await RunPutOrPostCommand<TRequest, TResponse>(url, request, true);

    }

    protected async Task<TResponse> RunPutCommand<TRequest, TResponse>(string url, TRequest request) where TRequest : class
    {
        return await RunPutOrPostCommand<TRequest, TResponse>(url, request, false);
    }

    public async Task<TResponse> UploadFiles<TResponse>(string url, string token, params string[] filePaths)
    {
        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using var content = new MultipartFormDataContent();

            for (int i = 0; i < filePaths.Length; i++)
            {
                var fileStream = File.OpenRead(filePaths[i]);
                var fileContent = new StreamContent(fileStream);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue("multipart/form-data");
                content.Add(fileContent, $"documents", Path.GetFileName(filePaths[i]));
            }

            var response = await client.PostAsync(url, content);
            return await EnsureSuccess<TResponse>(response) ?? throw new Exception($"Failed Populate in {url}");

        }
    }

    public async Task<byte[]> DownloadImageAsync(string url, string token)
    {
        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage httpResponse = await client.GetAsync(url);
            var result = default(byte[]);

            if (httpResponse.IsSuccessStatusCode)
            {
                byte[] imageBytes = await httpResponse.Content.ReadAsByteArrayAsync();
                result = imageBytes;
            }

            return result;
        }
    }

    public async Task<TResponse> RunPutOrPostCommand<TRequest, TResponse>(string url, TRequest request, bool isPostRequest = true)
    {
        using (HttpClient client = new HttpClient())
        {
            var sendOptions = new JsonSerializerOptions();
            //sendOptions.Converters.Add(_dateOnlyConverter);

            var content = new StringContent(JsonSerializer.Serialize(request, sendOptions), System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage response = null;

            if (isPostRequest)
            {
                response = await client.PostAsync(url, content);
            }
            else
            {
                response = await client.PutAsync(url, content);
            }

            return await EnsureSuccess<TResponse>(response) ?? throw new Exception($"Failed Populate in {url}");
        }
    }

    public async Task<TResponse> PostCommandAsync<TRequest, TResponse>( string relativePath,TRequest request)
    {
        var url = PathLocator.Combine(_baseUrl, relativePath);

        using (HttpClient client = new HttpClient())
        {
            var sendOptions = new JsonSerializerOptions();
            //sendOptions.Converters.Add(_dateOnlyConverter);

            var content = new StringContent(JsonSerializer.Serialize(request, sendOptions), System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage response = null;
//https://localhost:7083/api/v1/Auth/register-boss-tenant'
//http://localhost:7083/api/v1/Auth/register-boss-tenant
            response = await client.PostAsync(url, content);

            return await EnsureSuccess<TResponse>(response) ?? throw new Exception($"Failed Populate in {url}");
        }
    }



    private async Task<TResponse> EnsureSuccess<TResponse>(HttpResponseMessage message)
    {
        var response = default(TResponse);
        if (message.IsSuccessStatusCode)
        {
            string responseContent = await message.Content.ReadAsStringAsync();
            var recieveOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            response = JsonSerializer.Deserialize<TResponse>(responseContent, recieveOptions);
        }

        if (message.IsSuccessStatusCode)
        {
            return response;
        }

        // Read response content (if any) and throw a more descriptive exception so tests can surface server errors.
        var errorContent = string.Empty;
        try
        {
            errorContent = await message.Content.ReadAsStringAsync();
        }
        catch { /* ignore read errors */ }

        throw new HttpRequestException($"Request failed with status {(int)message.StatusCode} ({message.ReasonPhrase}). Response: {errorContent}");
    }

    protected string ConvertFileToBase64(string filePath)
    {
        byte[] fileBytes = File.ReadAllBytes(filePath);
        string base64String = Convert.ToBase64String(fileBytes);

        return base64String;
    }

    /// <summary>
    /// Gets an HttpClient configured with the base URL for direct HTTP calls.
    /// Useful for testing error responses and status codes.
    /// </summary>
    public HttpClient GetHttpClient()
    {
        var client = new HttpClient
        {
            BaseAddress = new Uri(_baseUrl)
        };
        return client;
    }
}
