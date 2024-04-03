
namespace Aff.YB.DataSync.DataDomain.Params
{
    public class TokenRequest
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public int CacheTimeOut { get; set; } = 20;
    }
}
