using Aff.YB.DataSync.DataDomain.Api;
using Aff.YB.DataSync.DataDomain.Params;
using Aff.YB.DataSync.Service.Infrastructure.Apis;

namespace Aff.YB.DataSync.Service.Infrastructure.Helpers
{
    public class AddDefaultContentHeader : DelegatingHandler
    {
        private readonly ITokenApi _tokenApi;
        private readonly TokenRequestParam _tokenRequestParam;

        public AddDefaultContentHeader(ITokenApi tokenApi, TokenRequestParam tokenRequestParam) {
            _tokenApi = tokenApi;
            _tokenRequestParam = tokenRequestParam;
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
        {   try
            {
                var token = await _tokenApi.GetTokenAsync(_tokenRequestParam);
                return token;

            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Cronjobs can't get JwtSecurityToken for {_tokenRequestParam.Username}!, {ex.Message}");
            }
        }
    }
}
