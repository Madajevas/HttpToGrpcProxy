using ProxyInterceptorTestsClient;

using System;
using System.Management.Automation;
using System.Threading.Tasks;

namespace HttpToGrpcProxy.Poweshell
{
    [Cmdlet("Intercept", "Request")]
    [OutputType(typeof(RequestContext))]
    public class InterceptRequest : PSCmdlet
    {
        [Parameter(Mandatory = true)]
        public Client ProxyClient { get; set; }

        [Parameter(Mandatory = true)]
        public string Route { get; set; }

        [Parameter]
        public TimeSpan? Timeout { get; set; }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            Task<RequestContext> interceptTask;
            if (Timeout.HasValue)
            {
                interceptTask = ProxyClient.InterceptRequest(Route, Timeout.Value);
            }
            else
            {
                interceptTask = ProxyClient.InterceptRequest(Route);
            }

            // seems there is no async method, hence blocking
            var requestContext = interceptTask.GetAwaiter().GetResult();

            WriteObject(requestContext);
        }
    }
}
