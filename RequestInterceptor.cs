using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using VRC.Core.BestHTTP;

namespace VRCTools
{
    class RequestInterceptor
    {
        public static HTTPRequest InterceptRequest(HTTPRequest request) // Remove Hardware address from requests
        {
            request.RemoveHeader("X-MacAddress");
            Type HTTPManagerType = typeof(HTTPRequest).Assembly.GetType("VRC.Core.BestHTTP.HTTPManager");
            //VRCToolsLogger.Info("HTTPManagerType: " + HTTPManagerType);
            MethodInfo HTTPManager_Send = HTTPManagerType.GetMethod(
                "SendRequest",
                BindingFlags.Static | BindingFlags.Public,
                null,
                new Type[] { typeof(HTTPRequest) },
                null
            );
            //VRCToolsLogger.Info("HTTPManager_SendRequest: " + HTTPManager_Send);
            HTTPRequest postProcessedRequest = HTTPManager_Send.Invoke(null, new object[] { request }) as HTTPRequest;
            return postProcessedRequest;
        }
    }
}
