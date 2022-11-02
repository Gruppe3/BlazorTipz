using Blazored.LocalStorage;
using Microsoft.JSInterop;
using System.Security.Claims;
using System.Text.Json;

namespace BlazorTipz.Components
{
    public class TokenServerAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;

        public TokenServerAuthenticationStateProvider(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;

        }
        
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _localStorage.GetItemAsStringAsync("token");

            var identity = new ClaimsIdentity();
            
            if (!string.IsNullOrEmpty(token))
            {
                identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
            }

            var user = new ClaimsPrincipal(identity);
            var state = new AuthenticationState(user);

            //check if token is expired
            if (identity.IsAuthenticated)
            {
                var expiresAt = long.Parse(identity.FindFirst("exp").Value);
                var expiresAtDateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(expiresAt);
                if (expiresAtDateTimeOffset < DateTime.Now)
                {
                    await _localStorage.RemoveItemAsync("token");
                    state = new AuthenticationState(new ClaimsPrincipal());
                }
            }

            NotifyAuthenticationStateChanged(Task.FromResult(state));

            return state;
        }
        public static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
            return keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()));
        }

        private static byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
    }
}
