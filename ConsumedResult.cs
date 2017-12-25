using NetCore.Simple.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

namespace ConsumeAPI.Simple
{
    public class ConsumedResult<TModel>
    {

        public HttpResponseMessage Response { get; private set; }

        public List<ErrorModel> Errors { get; set; }

        public TModel Data { get; internal set; }

        public string TextResponse { get; private set; }

        public HttpStatusCode StatusCode => Response.StatusCode;

        public static implicit operator TModel(ConsumedResult<TModel> model)
        {
            return model.Data;
        }

        public static implicit operator ConsumedResult<TModel>(TModel model)
        {
            return new ConsumedResult<TModel> { Data = model };
        }

        internal ConsumedResult()
        {

        }
        
        public ConsumedResult(HttpResponseMessage Response, bool DeserializeData = true)
        {
            
            this.Response = Response;
            var jsonTask = Response.Content.ReadAsStringAsync();
            if (!jsonTask.IsCompleted) jsonTask.RunSynchronously();
            TextResponse = jsonTask.Result;
            if (string.IsNullOrWhiteSpace(TextResponse)) return;
            if (StatusCode == HttpStatusCode.OK)
            {
                if (DeserializeData)
                    Data = JsonConvert.DeserializeObject<TModel>(TextResponse);
            }
            else if (StatusCode == HttpStatusCode.BadRequest)
            {
                Errors = JsonConvert.DeserializeObject<List<ErrorModel>>(TextResponse);
            }
        }

        

    }
}
