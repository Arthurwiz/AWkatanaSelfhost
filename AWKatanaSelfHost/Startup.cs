using AWKatanaSelfHost.OAuthProviders;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Owin;

namespace AWKatanaSelfHost
{

    public class Startup
    {
        //Building OWIN with Katana component architecture
        //Server: HttpListener ; Host: OwinHost.exe ; Middleware: Authentication/ ASP.NET Web API

        //Building the OWIN HTTP pipelines with Microsoft's OWIN IAppBuilder
        public void Configuration(IAppBuilder app)
        {
            //Configure Authentication Pipeline
            ConfigureAuth(app);

            var webApiConfiguration = ConfigureWebApi();
            app.UseWebApi(webApiConfiguration);
        }

        private void ConfigureAuth(IAppBuilder app)
        {
            #region Setup OAuthOptions
            var OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/Token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(14),
#if DEBUG
                AllowInsecureHttp = true, //Insecure HTTP is allowed in debug mode
#endif
                Provider = new MyOAuthServerProvider(),
                RefreshTokenProvider = new MyRefreshTokenProvider()
            };
            #endregion

            //Configure other Pipelines: Authorization and Token Processing
            app.UseOAuthAuthorizationServer(OAuthOptions);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
        }


        private HttpConfiguration ConfigureWebApi()
        {
            var config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
                "DefaultApi",
                "api/{controller}/{id}",
                new { id = RouteParameter.Optional });

            //var corsAttr = new EnableCorsAttribute("http://localhost:8000", "*", "*");
            //var corsAttr = new EnableCorsAttribute("*", "*", "*");
            //config.EnableCors(corsAttr);

            return config;
        }

    }
}
