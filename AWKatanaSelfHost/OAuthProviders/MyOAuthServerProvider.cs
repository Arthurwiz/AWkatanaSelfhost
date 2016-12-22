using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Security.OAuth;
using System.Security.Claims;
using Microsoft.Owin.Security;

namespace AWKatanaSelfHost.OAuthProviders
{
    public class MyOAuthServerProvider : OAuthAuthorizationServerProvider
    {
        //Hardcoded value for demo purpose
        private string MySecretWord = "secret";
        private string MyUserName = "arthur@startrek.com";
        private string MyPassword = "enterprise";

        //Regardless the grant_type, this function will be receiving the call first
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            //Need this to obtain clientId

            string clientId = string.Empty;
            string clientSecret = string.Empty;

            //There are two ways to obtain clientId & clientSecret from a POST (from requestor)
            if (!context.TryGetBasicCredentials(out clientId, out clientSecret)) //getting from Headers
            {
                context.TryGetFormCredentials(out clientId, out clientSecret);  //getting from the Body
            }

            Console.Write(clientId);
            Console.Write(clientSecret);

            #region Preparing for Refresh Token process, so it will work later

            if (clientSecret == MySecretWord)
            {
                // need to make the client_id available for later security checks
                context.OwinContext.Set<string>("as:client_id", clientId); //OwinContext contains the current client
                await Task.FromResult(context.Validated());
            }
            else
            {
                context.Rejected();
                context.SetError("invalid_grant", "The client secret is incorrect.");
                await Task.FromResult<object>(null);
            }
            #endregion

        }

        //if the POST's grant_type is password then GrantResourceOwnerCredentials() will receive the call
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {

            // Validate user/password:
            if (context.UserName == MyUserName && context.Password == MyPassword)
            {
                // Add claims associated with this user to the ClaimsIdentity object:
                var identity = new ClaimsIdentity(context.Options.AuthenticationType);

                #region Preparing for Refresh Token process, so it will be checked in the GrantRefreshToken() process

                var props = new AuthenticationProperties(new Dictionary<string, string>
                {
                    { "as:client_id", context.ClientId }
                });

                #endregion

                var ticket = new AuthenticationTicket(identity, props);

                context.Validated(ticket);
            }
            else
            {
                context.Rejected();
                context.SetError("invalid_grant", "The user name or password is incorrect.");

                await Task.FromResult<object>(null);
            }

        }

        //if the POST's grant_type is refresh_token then GrantRefreshToken() will receive the call
        public override async Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        {
            //Now we will compare the client_id key against the current ticket and original ticket
            var originalClient = context.Ticket.Properties.Dictionary["as:client_id"];
            var currentClient = context.OwinContext.Get<string>("as:client_id");

            // Comparing the Client
            if (originalClient != currentClient)
            {
                context.SetError("invalid_clientId", "Refresh token is issued to a different clientId.");
                await Task.FromResult<object>(null); 
            }
            else
            {
                //Preparing a new ticket and send it to the requestor

                var newIdentity = new ClaimsIdentity(context.Ticket.Identity);
                newIdentity.AddClaim(new Claim("newClaim", "refreshToken"));

                var newTicket = new AuthenticationTicket(newIdentity, context.Ticket.Properties);
                context.Validated(newTicket);

                await Task.FromResult<object>(null); 
            }
        }


        //Optional function so that the requestor can read the Token details from the TokenResponse
        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }


    }
}
