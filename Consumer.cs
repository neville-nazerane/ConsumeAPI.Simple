using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ConsumeAPI.Simple
{
    public class Consumer : IDisposable
    {
        private string _bearerToken;
        private string _baseURL;

        public HttpClient Client { get; private set; }

        public string BaseURL
        {
            get => _baseURL;
            set
            {
                Client.BaseAddress = new Uri(_baseURL);
                _baseURL = value;
            }
        }

        public string EndingURL { get; set; }

        public string BearerToken
        {
            get => _bearerToken;
            set
            {
                Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", value);
                _bearerToken = value;
            }
        }

        public Consumer(string BaseURL, long MaxResponseContentBufferSize = 256000)
        {
            Client = new HttpClient
            {
                MaxResponseContentBufferSize = MaxResponseContentBufferSize
            };
            this.BaseURL = BaseURL;
            EndingURL = string.Empty;
        }

        public static implicit operator Consumer(string URL)
        {
            return new Consumer(URL);
        }

        //#region basic calls

        //    public Task<HttpResponseMessage> PostAsync(string path, object obj, string BaseURL = null)
        //        => DoAsync((u, c) => Client.PostAsync(u, c), path, obj, BaseURL);

        //    public Task<HttpResponseMessage> PutAsync(string path, object obj, string BaseURL = null)
        //        => DoAsync((u, c) => Client.PutAsync(u, c), path, obj, BaseURL);

        //    public Task<HttpResponseMessage> GetAsync(string path, string BaseURL = null)
        //        => DoAsync((u, c) => Client.GetAsync(u), path, null, BaseURL);

        //    public Task<HttpResponseMessage> DeleteAsync(string path, string BaseURL = null)
        //        => DoAsync((u, c) => Client.DeleteAsync(u), path, null, BaseURL);

        async Task<HttpResponseMessage> DoAsync(Func<string, HttpContent, Task<HttpResponseMessage>> func,
                                                string path, object obj, string BaseURL)
        {
            if (BaseURL == null) BaseURL = this.BaseURL;
            var json = JsonConvert.SerializeObject(obj);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            //if (BearerToken != null)
            //    content.Headers.Add("Authorization", $"Bearer {BearerToken}");
            var result = await func(BaseURL + path + EndingURL, content);
            return result;
        }

        //    #endregion

        #region string calls

        public Task<DefaultConsumedResult> PostAsync(string path, object obj, string BaseURL = null)
            => DoStringAsync((u, c) => Client.PostAsync(u, c), path, obj, BaseURL);

        public Task<DefaultConsumedResult> PutAsync(string path, object obj, string BaseURL = null)
            => DoStringAsync((u, c) => Client.PutAsync(u, c), path, obj, BaseURL);

        public Task<DefaultConsumedResult> GetAsync(string path, string BaseURL = null)
            => DoStringAsync((u, c) => Client.GetAsync(u), path, null, BaseURL);

        public Task<DefaultConsumedResult> DeleteAsync(string path, string BaseURL = null)
            => DoStringAsync((u, c) => Client.DeleteAsync(u), path, null, BaseURL);

        async Task<DefaultConsumedResult> DoStringAsync(Func<string, HttpContent, Task<HttpResponseMessage>> func,
                                                string path, object obj, string BaseURL)
        {
            var result = await DoAsync(func, path, obj, BaseURL);
            var cr = new DefaultConsumedResult(result);
            return cr;
        }

        #endregion

        #region generic calls

        public Task<ConsumedResult<TModel>> PostAsync<TModel>(string path, object obj, string BaseURL = null)
            => DoAsync<TModel>((u, c) => Client.PostAsync(u, c), path, obj, BaseURL);

        public Task<ConsumedResult<TModel>> PutAsync<TModel>(string path, object obj, string BaseURL = null)
            => DoAsync<TModel>((u, c) => Client.PutAsync(u, c), path, obj, BaseURL);

        public Task<ConsumedResult<TModel>> GetAsync<TModel>(string path, string BaseURL = null)
            => DoAsync<TModel>((u, c) => Client.GetAsync(u), path, null, BaseURL);

        public Task<ConsumedResult<TModel>> DeleteAsync<TModel>(string path, string BaseURL = null)
            => DoAsync<TModel>((u, c) => Client.DeleteAsync(u), path, null, BaseURL);

        async Task<ConsumedResult<TModel>> DoAsync<TModel>(Func<string, HttpContent, Task<HttpResponseMessage>> func,
                                                string path, object obj, string BaseURL)
        {
            var result = await DoAsync(func, path, obj, BaseURL);
            return new ConsumedResult<TModel>(result);
        }

        public void Dispose()
        {
            Client.Dispose();
        }

        #endregion

    }
}
