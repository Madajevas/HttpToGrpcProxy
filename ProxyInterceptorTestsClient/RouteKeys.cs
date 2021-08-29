using HttpToGrpcProxy.Commons;

using System;
using System.Collections.Generic;

namespace HttpToGrpcProxy
{
    public partial class Request : IRoute
    {
        public string GetRoute() => Route;
    }

    public partial class Response : IRoute
    {
        public string GetRoute() => Route;

        public Dictionary<string, string> HeadersDictionary {
            get => throw new NotImplementedException("Currently there is no need to get headers");
            set {
                Headers = new Headers();
                Headers.Values.Add(value);
            }
        }
    }
}
