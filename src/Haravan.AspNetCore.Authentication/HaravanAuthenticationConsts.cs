
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class HaravanAuthenticationConsts
    {
        public const string Cookie = "Cookies.Haravan";
        public const string Scheme = "Haravan";
        public const string ServiceScheme = "HaravanService";
    }
}