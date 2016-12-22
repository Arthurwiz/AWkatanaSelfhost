using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using System.Collections.Concurrent;

namespace AWKatanaSelfHost.OAuthProviders
{
    public class MyRefreshTokenProvider : IAuthenticationTokenProvider
    {
        private static ConcurrentDictionary<string, AuthenticationTicket> _refreshTokens = new ConcurrentDictionary<string, AuthenticationTicket>();

        public async Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            var guid = Guid.NewGuid().ToString();

            // maybe only create a handle the first time, then re-use

            _refreshTokens.TryAdd(guid, context.Ticket);

            // consider storing only the hash of the handle
             context.SetToken(guid);
            await Task.FromResult<object>(null);
        }

        public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {

            AuthenticationTicket ticket;

            if (_refreshTokens.TryRemove(context.Token, out ticket))
            {
                context.SetTicket(ticket);
            }
            await Task.FromResult<object>(null);
        }

        public void Create(AuthenticationTokenCreateContext context)
        {
            throw new NotImplementedException();
        }

     

        public void Receive(AuthenticationTokenReceiveContext context)
        {
            throw new NotImplementedException();
        }

     
    }
}
