using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace ShiftAssignerServer.Tests.Infrastructure;

public class ClientSender
{
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

    public async Task<TResponse> PostCommandAsync<TRequest, TResponse>(string url, TRequest request)
    {
        using (HttpClient client = new HttpClient())
        {
            var sendOptions = new JsonSerializerOptions();
            //sendOptions.Converters.Add(_dateOnlyConverter);

            var content = new StringContent(JsonSerializer.Serialize(request, sendOptions), System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage response = null;

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

        return response;
    }

    protected string ConvertFileToBase64(string filePath)
    {
        byte[] fileBytes = File.ReadAllBytes(filePath);
        string base64String = Convert.ToBase64String(fileBytes);

        return base64String;
    }
}
