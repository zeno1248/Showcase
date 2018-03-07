using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Auth.OAuth2.Web;
using System.Threading;
using System.Threading.Tasks;
using Showcase.Util;

namespace Showcase.Controllers
{
    public class AuthCallbackController : Google.Apis.Auth.OAuth2.Mvc.Controllers.AuthCallbackController
    {
        //protected static readonly ILogger Logger = ApplicationContext.Logger.ForType<AuthCallbackController>();

        protected override Google.Apis.Auth.OAuth2.Mvc.FlowMetadata FlowData
        {
            get { return new ZenoFlowMetadata(); }
        }

        public override async Task<ActionResult> IndexAsync(AuthorizationCodeResponseUrl authorizationCode, CancellationToken taskCancellationToken)
        {
            if (string.IsNullOrEmpty(authorizationCode.Code))
            {
                var errorResponse = new TokenErrorResponse(authorizationCode);
                return OnTokenError(errorResponse);
            }

            var returnUrl = Request.Url.ToString();
            returnUrl = returnUrl.Substring(0, returnUrl.IndexOf("?"));

            var token = await Flow.ExchangeCodeForTokenAsync(UserId, authorizationCode.Code, returnUrl,
                taskCancellationToken).ConfigureAwait(false);

            // Extract the right state.
            var oauthState = await AuthWebUtility.ExtracRedirectFromState(Flow.DataStore, UserId,
                authorizationCode.State).ConfigureAwait(false);

            return new RedirectResult(oauthState);
        }

        protected override ActionResult OnTokenError(TokenErrorResponse errorResponse)
        {
            return Redirect("~/Home/AccessDenied");
            //throw new TokenResponseException(errorResponse);
        }
    }
}