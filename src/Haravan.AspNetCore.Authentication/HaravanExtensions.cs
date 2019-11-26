
using System;
using System.Linq;
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
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            builder
            .AddOpenIdConnect(HaravanAuthenticationConsts.Scheme, options =>
            {
                options.SignInScheme = opt.SignInScheme;
                options.Authority = opt.Authority;
                options.ClientId = opt.ClientId;
                options.ClientSecret = opt.ClientSecret;
                options.ResponseType = "code id_token";
                options.SaveTokens = opt.SaveUserToken;
                options.GetClaimsFromUserInfoEndpoint = true;
                options.Scope.Add("org");
                options.Scope.Add("userinfo");
                options.Scope.Add("email");

                if(opt.Scopes != null)
                foreach(var scope in opt.Scopes)
                    options.Scope.Add(scope);

                options.Events = new OpenIdConnectEvents();

                options.Events.OnRedirectToIdentityProvider = OnRedirectToIdentityProvider;

                if(opt.OnUserInformationReceived != null)
                {
                    options.Events.OnUserInformationReceived = opt.OnUserInformationReceived;
                }
            });

            builder.AddOpenIdConnect(HaravanAuthenticationConsts.ServiceScheme, options =>
            {
                options.CallbackPath = $"{options.CallbackPath}_service";
                options.Authority = opt.Authority;
                options.ClientId = opt.ClientId;
                options.ClientSecret = opt.ClientSecret;
                options.ResponseType = "code id_token";
                options.GetClaimsFromUserInfoEndpoint = true;
                options.Scope.Add("org");
                options.Scope.Add("userinfo");
                options.Scope.Add("email");

                if(opt.ServiceScopes != null)
                foreach(var scope in opt.ServiceScopes)
                    options.Scope.Add(scope);

                options.Scope.Add("grant_service");

                options.Events = new OpenIdConnectEvents();

                options.Events.OnRedirectToIdentityProvider = OnRedirectToIdentityProvider;

                if(opt.OnAppTokenInformationReceived != null)
                {
                    options.Events.OnUserInformationReceived = opt.OnAppTokenInformationReceived;
                }
            });

            return builder;
        }

        private static Task OnRedirectToIdentityProvider(RedirectContext ctx)
        {
            var parameters = ctx.Properties?.Parameters;
            if (parameters != null && parameters.Any())
            {
                foreach (var pr in parameters)
                {
                    if (pr.Key == "scope")
                    {
                        if (ctx.ProtocolMessage.Scope == null)
                            ctx.ProtocolMessage.Scope = pr.Value.ToString();
                        else
                            ctx.ProtocolMessage.Scope += " " + pr.Value.ToString();
                        continue;
                    }

                    ctx.ProtocolMessage.SetParameter(pr.Key, pr.Value.ToString());
                }
            }

            return Task.CompletedTask;
        }

    }
}