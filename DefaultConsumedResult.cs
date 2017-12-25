using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

namespace ConsumeAPI.Simple
{
    public class DefaultConsumedResult : ConsumedResult<string>
    {

        public DefaultConsumedResult(HttpResponseMessage Response)
            : base(Response, false)
        {
            if (StatusCode == HttpStatusCode.OK)
                Data = TextResponse;
        }

    }
}
