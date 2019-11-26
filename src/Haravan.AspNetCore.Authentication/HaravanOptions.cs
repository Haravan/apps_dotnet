
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace Microsoft.Extensions.DependencyInjection
{
    public class HaravanOptions
    {
        public string Authority { get;set; } = "https://accounts.haravan.com";
        public string SignInScheme { get; set; } = CookieAuthenticationDefaults.AuthenticationScheme;
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string[] Scopes { get; set; }
        public string[] ServiceScopes { get; set; }
        public bool SaveUserToken { get; set; } = true;
        public Func<UserInformationReceivedContext, Task> OnUserInformationReceived { get; set; }
        public Func<UserInformationReceivedContext, Task> OnAppTokenInformationReceived { get; set; }
    }
}