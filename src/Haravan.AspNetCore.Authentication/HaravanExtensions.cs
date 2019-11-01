
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class HaravanExtensions
    {
        public static AuthenticationBuilder AddHaravan(this AuthenticationBuilder builder,
            Action<HaravanOptions> funcOpt)
        {
            return builder.AddHaravan("Haravan", funcOpt);
        }
        public static AuthenticationBuilder AddHaravan(this AuthenticationBuilder builder, 
            string scheme, 
            Action<HaravanOptions> funcOpt)
        {
            if(scheme == null)
                throw new ArgumentNullException(nameof(scheme));
            
            if(funcOpt == null)
                throw new ArgumentNullException(nameof(funcOpt));

            var opt = new HaravanOptions();
            funcOpt(opt);

            if(opt.ClientId == null)
                throw new ArgumentNullException(nameof(opt.ClientId));

            if(opt.Scopes == null)
                opt.Scopes = new string[] {};

            builder.Services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            builder.AddCookie($"Cookies.{scheme}")
            .AddOpenIdConnect("Haravan", options =>
            {
                options.SignInScheme = $"Cookies.{scheme}";
                options.Authority = "https://accounts.hara.vn";
                options.ClientId = opt.ClientId;
                options.ClientSecret = opt.ClientSecret;
                options.ResponseType = "code id_token";
                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;
                options.Scope.Add("org");
                options.Scope.Add("userinfo");
                options.Scope.Add("email");

                foreach(var scope in opt.Scopes)
                    options.Scope.Add(scope);

                options.Events = new OpenIdConnectEvents();
                options.Events.OnRedirectToIdentityProvider = (ctx) =>
                {
                    var grantService = ctx.Properties.GetParameter<bool?>("grant_service");
                    if(grantService != null)
                    {
                        ctx.ProtocolMessage.Scope += " grant_service";
                    }

                    return Task.CompletedTask;
                };
            });

            return builder;
        }
    }
}