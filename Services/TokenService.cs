using Microsoft.IdentityModel.Tokens;
using ShoppingApp.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ShoppingApp.Services
{
    public interface ITokenService
    {
        string CreateToken(User user);
    }

    public partial class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _key;

        public TokenService(IConfiguration config)
        {
            _config = config;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"] ?? "BuyNext_SuperSecret_Security_Key_For_Premium_Shopping_2026_Experience_Longer_Key_For_SHA512"));
        }
    }
}
