using ApiCatalogoOrg.Models;

namespace ApiCatalogoOrg.Services
{
    public interface ITokenService
    {
        string GerarToken(string key, string issuer, string audience, UserModel user);
    }
}
