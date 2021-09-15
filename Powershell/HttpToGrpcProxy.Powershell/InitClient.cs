using ProxyInterceptorTestsClient;

using System;
using System.Management.Automation;

namespace HttpToGrpcProxy.Powershell
{
    [Cmdlet("Init", "Client")]
    [OutputType(typeof(Client))]
    public class InitClient : PSCmdlet
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
