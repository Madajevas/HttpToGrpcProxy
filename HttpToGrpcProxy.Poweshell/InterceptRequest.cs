using ProxyInterceptorTestsClient;

using System;
using System.Management.Automation;

namespace HttpToGrpcProxy.Poweshell
{
    [Cmdlet("Init", "Client")]
    [OutputType(typeof(Client))]
    public class InterceptRequest : PSCmdlet
    {
        [Parameter(Mandatory = true)]
        public Uri ProxyAddress { get; set; }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            var client = new Client(ProxyAddress);

            WriteObject(client);
        }
    }
}
