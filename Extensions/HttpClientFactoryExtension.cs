using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using DAF.AirplaneTrafficData.Extensions.Interfaces;
using DAF.AirplaneTrafficData.HelperClasses;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace DAF.AirplaneTrafficData.Extensions
{
    public class HttpClientFactoryExtension: IHttpClientFactoryExtension
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpClientFactoryExtension(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<T> GetResponseFromEndpointTask<T>(HttpEndpointTypes httpEndpointTypes)
        {
            var serviceEndpoint = httpEndpointTypes.ServiceEndPoint;
            var urlParameters = httpEndpointTypes.UrlParameters;
            var baseAddress = $"{serviceEndpoint}{urlParameters}";
            using var client = _httpClientFactory.CreateClient(httpEndpointTypes.ClientEndPoint);

            // List data response.
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response =
                await client.GetAsync(baseAddress)
                    .ConfigureAwait(
                        false); // Blocking call! Program will wait here until a response is received or a timeout occurs.
            if (!response.IsSuccessStatusCode)
            {
                // Parse the response body.
                var errorValidation = await response.Content.ReadAsStringAsync();
                if (errorValidation != null)
                    throw new Exception(errorValidation);

                var errorCustom =
                    JsonConvert.DeserializeObject<CustomModel>(await response.Content.ReadAsStringAsync());

                if (errorCustom != null && !string.IsNullOrEmpty(errorCustom.ErrorCode))
                    throw new Exception(errorCustom.Message);
            }

            //Dispose once all HttpClient calls are complete. This is not necessary if the containing object will be disposed of; for example in this case the HttpClient instance will be disposed automatically when the application terminates so the following call is superfluous.
            client.Dispose();
            return await response.Content.ReadAsAsync<T>();
        }

        /// <summary>
        ///     CreateEndpointTask
        /// </summary>
        /// <param name="httpEndpointTypes">HttpEndpointTypes</param>
        public async Task<T> CreateEndpointTask<T>(HttpEndpointTypes httpEndpointTypes)
        {
            var serviceEndpoint = httpEndpointTypes.ServiceEndPoint;
            var urlParameters = httpEndpointTypes.UrlParameters;
            var baseAddress = $"{serviceEndpoint}{urlParameters}";
            HttpContent httpContent =
                new StringContent(JsonConvert.SerializeObject(httpEndpointTypes.Content), Encoding.UTF8);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            using var client = _httpClientFactory.CreateClient(httpEndpointTypes.ClientEndPoint);
            
            // List data response.
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = await client.PostAsync(baseAddress, httpContent).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                // Parse the response body.
                var errorValidation = await response.Content.ReadAsStringAsync();
                if (errorValidation!= null)
                    throw new Exception(errorValidation);

                var errorCustom =
                    JsonConvert.DeserializeObject<CustomModel>(await response.Content.ReadAsStringAsync());

                if (errorCustom != null && !string.IsNullOrEmpty(errorCustom.ErrorCode))
                    throw new Exception(errorCustom.ErrorCode);
            }

            //Dispose once all HttpClient calls are complete. This is not necessary if the containing object will be disposed of; for example in this case the HttpClient instance will be disposed automatically when the application terminates so the following call is superfluous.
            client.Dispose();
            return await response.Content.ReadAsAsync<T>();
        }

        /// <summary>
        ///     UpdateEndpointTask
        /// </summary>
        /// <param name="httpEndpointTypes">HttpEndpointTypes</param>
        public async Task<T> UpdateEndpointTask<T>(HttpEndpointTypes httpEndpointTypes)
        {
            var serviceEndpoint = httpEndpointTypes.ServiceEndPoint;
            var urlParameters = httpEndpointTypes.UrlParameters;
            var baseAddress = $"{serviceEndpoint}{urlParameters}";
            HttpContent httpContent =
                new StringContent(JsonConvert.SerializeObject(httpEndpointTypes.Content), Encoding.UTF8);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            using var client = _httpClientFactory.CreateClient(httpEndpointTypes.ClientEndPoint);

            // List data response.
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = await client.PutAsync(baseAddress, httpContent).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                // Parse the response body.
                var errorValidation = await response.Content.ReadAsStringAsync();
                if (errorValidation != null)
                    throw new Exception(errorValidation);

                var errorCustom =
                    JsonConvert.DeserializeObject<CustomModel>(await response.Content.ReadAsStringAsync());

                if (errorCustom != null && !string.IsNullOrEmpty(errorCustom.Message))
                    throw new Exception(errorCustom.Message);
            }

            //Dispose once all HttpClient calls are complete. This is not necessary if the containing object will be disposed of; for example in this case the HttpClient instance will be disposed automatically when the application terminates so the following call is superfluous.
            client.Dispose();
            return await response.Content.ReadAsAsync<T>();
        }

        public class CustomModel
        {
            public string Message { get; set; }
            public string ErrorCode { get; set; }
        }
    }
}