using ProxyInterceptorTestsClient;

using System;
using System.Management.Automation;
using System.Threading;

namespace HttpToGrpcProxy.Powershell
{
    [Cmdlet(VerbsCommon.Get, "InterceptedRequest")]
    [OutputType(typeof(RequestContext))]
    public class GetInterceptedRequest : Cmdlet
    {
        private CancellationTokenSource cancellationTokenSource;

        [Parameter(Mandatory = true)]
        public IClient ProxyClient { get; set; }

        [Parameter(Mandatory = true)]
        public string Route { get; set; }

        [Parameter]
        public TimeSpan? Timeout { get; set; }

        protected override void BeginProcessing()
        {
            base.BeginProcessing();

            cancellationTokenSource = Timeout.HasValue ? new CancellationTokenSource(Timeout.Value) : new CancellationTokenSource();
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            var interceptTask = ProxyClient.InterceptRequest(Route, cancellationTokenSource.Token);

            // seems there is no async method, hence blocking
            var requestContext = interceptTask.GetAwaiter().GetResult();

            WriteObject(requestContext);
        }

        protected override void StopProcessing()
        {
            base.StopProcessing();

            cancellationTokenSource.Cancel();
        }
    }
}
