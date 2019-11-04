﻿
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
                options.Authority = "https://accounts.haravan.com";
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

                if(opt.OnUserInformationReceived != null)
                {
                    options.Events = new OpenIdConnectEvents();
                    options.Events.OnUserInformationReceived = opt.OnUserInformationReceived;
                }
            });

            builder.AddOpenIdConnect(HaravanAuthenticationConsts.ServiceScheme, options =>
            {
                options.CallbackPath = $"{options.CallbackPath}_service";
                options.Authority = "https://accounts.haravan.com";
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

                if(opt.OnAppTokenInformationReceived != null)
                {
                    options.Events = new OpenIdConnectEvents();
                    options.Events.OnUserInformationReceived = opt.OnAppTokenInformationReceived;
                }
            });

            return builder;
        }
    }
}