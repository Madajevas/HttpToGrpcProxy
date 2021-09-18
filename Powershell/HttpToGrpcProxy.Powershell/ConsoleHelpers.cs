using System;
using System.Threading;

namespace HttpToGrpcProxy.Powershell
{
    static class ConsoleHelpers
    {
        public static void CancelTokenOnConsoleCancel(CancellationTokenSource cancellationTokenSource)
        {
            Console.CancelKeyPress += (_, __) =>
            {
                Console.WriteLine("Termination command received");
                cancellationTokenSource.Cancel();
            };
        }
    }
}
