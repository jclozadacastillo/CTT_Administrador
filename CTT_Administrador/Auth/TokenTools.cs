using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CTT_Administrador.Auth
{
    public class TokenTools
    {
        public static bool validateToken(string? token)
        {
            try
            {
                if (string.IsNullOrEmpty(token)) return false;
                var validador = new JwtSecurityTokenHandler();
                SecurityToken validatedToken;
                var validToken = validador.ValidateToken(token, new TokenValidationParameters()
                {
                    ValidateLifetime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ConfigurationHelper.config["JWT:key"]))
                }, out validatedToken);
                return DateTime.Now < validatedToken.ValidTo.AddHours(-5);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public static string getTokenValue(string _claim, string? _token)
        {
            try
            {
                if (string.IsNullOrEmpty(_token)) return "";
                var validador = new JwtSecurityTokenHandler();
                SecurityToken validatedToken;
                var validToken = validador.ValidateToken(_token, new TokenValidationParameters()
                {
                    ValidateLifetime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ConfigurationHelper.config["JWT:key"]))
                }, out validatedToken);
                var identities = validToken.Identities.FirstOrDefault();
                _claim = _claim.ToLower();
                if (_claim == "user") _claim = "usuario";
                if (_claim == "mail") _claim = "correo";
                if (_claim == "email") _claim = "correo";
                if (_claim == "name") _claim = "nombre";
                return identities?.FindFirst(_claim)?.Value; ;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "";
            }
        }

        public class userData
        {
            public string usuario { get; set; }
            public string clave { get; set; }
            public string nombre { get; set; }
            public string email { get; set; }
            public string idcentro { get; set; }
            public bool? persistent { get; set; }
        }

        public static string generateToken(userData _user)
        {
            try
            {
                var claims = new[]{
                new Claim("nombre", _user.nombre),
                new Claim("correo", string.IsNullOrEmpty(_user.email)?"":_user.email),
                new Claim("usuario",_user.usuario),
                new Claim("idcentro",string.IsNullOrEmpty(_user.idcentro)?"":_user.idcentro)
                };
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ConfigurationHelper.config["JWT:key"]));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
                var durationHours = _user.persistent == true ?
                    Convert.ToDouble(ConfigurationHelper.config["JWT:times:persistent"]?.Replace(",", ".")) :
                    Convert.ToDouble(ConfigurationHelper.config["JWT:times:default"]?.Replace(",", "."));
                var securityToken = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddHours(durationHours),
                    signingCredentials: credentials
                    );
                var expira = DateTime.Now.AddHours(durationHours);
                return new JwtSecurityTokenHandler().WriteToken(securityToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "";
            }
        }
    }
}