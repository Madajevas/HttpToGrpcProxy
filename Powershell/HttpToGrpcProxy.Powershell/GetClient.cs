using ProxyInterceptorTestsClient;

using System;
using System.Management.Automation;

namespace HttpToGrpcProxy.Powershell
{
    [Cmdlet(VerbsCommon.Get, "Client")]
    [OutputType(typeof(Client))]
    public class GetClient : PSCmdlet
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
