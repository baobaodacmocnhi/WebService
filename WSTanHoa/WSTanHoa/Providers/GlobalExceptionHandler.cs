using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Results;

namespace WSTanHoa.Providers
{
    public class GlobalExceptionHandler : ExceptionHandler
    {
        public async override Task HandleAsync(ExceptionHandlerContext context, CancellationToken cancellationToken)
        {
            if (context.Exception != null)
            {
                Exception filteredException = context.Exception;

                while ((filteredException != null) && (filteredException.GetType() != typeof(HttpResponseException)))
                {
                    filteredException = filteredException.InnerException;
                }

                if ((filteredException != null) && (filteredException != context.Exception))
                {
                    var httpResponseException = (HttpResponseException)filteredException;
                    var response = context.Request.CreateErrorResponse(
                        httpResponseException.Response.StatusCode,
                        httpResponseException
                    );

                    context.Result = new ResponseMessageResult(response);
                }
            }
        }
    }
}