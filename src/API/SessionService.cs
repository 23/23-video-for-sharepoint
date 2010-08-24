using System.Collections.Generic;
using System.Xml.XPath;
using DotNetOpenAuth.Messaging;
using System.Web;

namespace Twentythree
{
    public class SessionService
    {
        private IAPIProvider Provider;

        public SessionService(IAPIProvider Provider)
        {
            this.Provider = Provider;
        }

        public Domain.Session GetToken()
        {
            return GetToken("/");
        }

        public Domain.Session GetToken(string ReturnURL)
        {
            // Build request URL
            List<string> RequestURLParameters = new List<string>();

            RequestURLParameters.Add("return_url=" + HttpUtility.UrlEncode(ReturnURL));

            // Do the request
            MessageReceivingEndpoint RequestMessage = new MessageReceivingEndpoint(this.Provider.GetRequestURL("/api/session/get-token", RequestURLParameters), HttpDeliveryMethods.GetRequest);

            XPathNavigator ResponseMessage = this.Provider.DoRequest(RequestMessage);
            if (ResponseMessage == null) return null;

            // Create result
            Domain.Session Result = new Domain.Session();

            // Get the access token
            XPathNodeIterator AccessTokenNode = ResponseMessage.Select("/response/access_token");
            if (!AccessTokenNode.MoveNext()) return null;

            Result.AccessToken = AccessTokenNode.Current.Value;

            // Get the local return URL
            XPathNodeIterator ReturnURLNode = ResponseMessage.Select("/response/return_url");
            if (!ReturnURLNode.MoveNext()) return null;

            string LocalReturnURL = ReturnURLNode.Current.Value;

            // Build the return URL
            List<string> ReturnURLParameters = new List<string>();

            ReturnURLParameters.Add("session_token=" + Result.AccessToken);
            ReturnURLParameters.Add("return_url=" + LocalReturnURL);

            Result.ReturnURL = this.Provider.GetRequestURL("/api/session/redeem-token", ReturnURLParameters);

            // Return the object
            return Result;
        }
    }
}
