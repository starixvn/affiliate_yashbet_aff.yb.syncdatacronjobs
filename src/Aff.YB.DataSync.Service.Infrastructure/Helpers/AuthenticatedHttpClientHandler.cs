using Aff.YB.DataSync.DataDomain.Api;
using Aff.YB.DataSync.DataDomain.Params;
using Aff.YB.DataSync.Service.Infrastructure.Apis;
using Refit;

namespace Aff.YB.DataSync.Service.Infrastructure.Helpers
{
    public class AuthenticatedHttpClientHandler : HttpClientHandler
    {
        private readonly TokenRequest _tokenRequest;
        private readonly ITokenApi _tokenApi;

        public AuthenticatedHttpClientHandler(string memberSiteApiUrl, TokenRequest tokenRequest)
        {
            _tokenApi = TokenApiInstance(memberSiteApiUrl);
            _tokenRequest = tokenRequest;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            await SetJwtToken(request);

            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }

        private async Task SetJwtToken(HttpRequestMessage request)
        {
            var tokenInfo = await GetJwtToken();

            // Use the UriBuilder to parse and modify the url
            var uriBuilder = new UriBuilder(request.RequestUri);

            uriBuilder.Query = string.IsNullOrEmpty(uriBuilder.Query)
                ? $"access_token={tokenInfo.AccessToken}"
                : $"{uriBuilder.Query.Trim('?')}&access_token={tokenInfo.AccessToken}";

            request.RequestUri = uriBuilder.Uri;
        }

        private async Task<TokenInfo> GetJwtToken()
        {
            try
            {
                var token = await _tokenApi.GetTokenAsync(_tokenRequest);
                return token;

            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Can't get JwtSecurityToken for {_tokenRequest.Username}!/n {ex.Message}");
            }
        }

        private static ITokenApi TokenApiInstance(string memberSiteApiUrl)
        {
            var handler = new HttpClientHandler();

            return RestService.For<ITokenApi>(
                new HttpClient(handler)
                {
                    BaseAddress = new Uri(memberSiteApiUrl)
                });
        }
    }
}
