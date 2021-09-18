using ProxyInterceptorTestsClient;

using System;
using System.Management.Automation;
using System.Threading;

using static HttpToGrpcProxy.Powershell.ConsoleHelpers;

namespace HttpToGrpcProxy.Powershell
{
    [Cmdlet(VerbsCommon.Get, "InterceptedRequest")]
    [OutputType(typeof(RequestContext))]
    public class GetInterceptedRequest : PSCmdlet
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

            var cancellationTokenSource = Timeout.HasValue ? new CancellationTokenSource(Timeout.Value) : new CancellationTokenSource();
            CancelTokenOnConsoleCancel(cancellationTokenSource);

            var interceptTask = ProxyClient.InterceptRequest(Route, cancellationTokenSource.Token);

            // seems there is no async method, hence blocking
            var requestContext = interceptTask.GetAwaiter().GetResult();

            WriteObject(requestContext);
        }
    }
}
