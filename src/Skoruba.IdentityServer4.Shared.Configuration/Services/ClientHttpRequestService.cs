using Skoruba.IdentityServer4.Shared.Configuration.Configuration.HttpRequestService;
using Skoruba.IdentityServer4.Shared.Configuration.Services.Interfaces;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.Shared.Configuration.Services
{
    public class ClientHttpRequestService : IHttpRequestService
    {
        public async Task<LabsoftHttpResponse> GetAsync(string requestUri, string token)
        {
            HttpResponseMessage httpResponseMessage;
            string content = string.Empty;

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                using (httpResponseMessage = await client.GetAsync(
                           requestUri,
                           new CancellationToken()))
                {
                    content = await httpResponseMessage.Content.ReadAsStringAsync();
                }
            }

            var labsoftHttpResponse = new LabsoftHttpResponse(
                statusCode: (int)httpResponseMessage.StatusCode,
                message: httpResponseMessage.ReasonPhrase!,
                isSuccessStatusCode: httpResponseMessage.IsSuccessStatusCode,
                requestPayload: string.Empty,
                content);

            return labsoftHttpResponse;
        }

        public async Task<LabsoftHttpResponse> PostAsync(string requestUri, string objectBody)
        {
            return await PostAsync(requestUri, objectBody, string.Empty);
        }

        public async Task<LabsoftHttpResponse> PostAsync(
            string requestUri,
            string objectBody, 
            string accessToken, 
            string company = "")
        {
            HttpResponseMessage httpResponseMessage;
            var requestContent = GetStringContent(objectBody);
            string content = string.Empty;


            using (var handler = new HttpClientHandler())
            {
                // Ignore SSL certificate validation errors
                handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

                using (var client = new HttpClient(handler))
                {
                    if (string.IsNullOrEmpty(accessToken) is false)
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    }

                    using (var request = new HttpRequestMessage(HttpMethod.Post, requestUri))
                    {
                        if (string.IsNullOrEmpty(company) is false)
                        {
                            var origin = $"https://{company}.mylimsportal.cloud";
                            request.Headers.Add("Origin", origin);
                        }

                        request.Content = requestContent;

                        using (httpResponseMessage = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead))
                        {
                            content = await httpResponseMessage.Content.ReadAsStringAsync();
                        }
                    }
                }
            }

            var labsoftHttpResponse = new LabsoftHttpResponse(
                statusCode: (int)httpResponseMessage.StatusCode,
                message: httpResponseMessage.ReasonPhrase!,
                isSuccessStatusCode: httpResponseMessage.IsSuccessStatusCode,
                requestPayload: objectBody,
                content);

            return labsoftHttpResponse;
        }

        public async Task<LabsoftHttpResponse> PostFormAsync(string requestUri,
            string clientId,
            string scope,
            string clientSecret,
            string grantType,
            string resource)
        {
            var encodedContent = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "client_id", clientId },
                    { "scope", scope },
                    { "client_secret", clientSecret },
                    { "grant_type", grantType },
                    { "resource", resource },
                });
            HttpResponseMessage httpResponseMessage;
            string content = string.Empty;
            using (var client = new HttpClient())
            {
                using (httpResponseMessage = await client.PostAsync(requestUri, encodedContent))
                {
                    content = await httpResponseMessage.Content.ReadAsStringAsync();
                }
            }
            var labsoftHttpResponse = new LabsoftHttpResponse(
                statusCode: (int)httpResponseMessage.StatusCode,
                message: httpResponseMessage.ReasonPhrase!,
                isSuccessStatusCode: httpResponseMessage.IsSuccessStatusCode,
                requestPayload: string.Empty,
                content);

            return labsoftHttpResponse;
        }

        private static StringContent GetStringContent(string objectBody)
        {
            return new StringContent(
                content: objectBody,
                encoding: Encoding.UTF8,
                mediaType: "application/json");
        }
    }
}
