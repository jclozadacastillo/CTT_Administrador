using Microsoft.AspNetCore.Authentication;
using static CTT_Administrador.Auth.TokenTools;

namespace CTT_Administrador.Auth.Docente
{
    public class AuthorizeDocenteTools : IAuthorizeDocenteTools
    {
        private IHttpContextAccessor _context;
        private string _token;
        private string _cookieName=ConfigurationHelper.config["JWT:cookieNames:docente"];
        public AuthorizeDocenteTools(IHttpContextAccessor context)
        {
            _context = context;
            _token = context.HttpContext.Request.Cookies[_cookieName];
        }

        public bool validateToken()
        {
            var valid = TokenTools.validateToken(_token);
            if (!valid) logoutSync();
            return valid;
        }

        public string get(string key) => getTokenValue(key, _token);

        public string getUser() => getTokenValue("usuario", _token);

        public string getName() => getTokenValue("nombre", _token);

        public string login(userData user)
        {
            try
            {
                var token = generateToken(user);
                if (string.IsNullOrEmpty(token)) return null;
                _context?.HttpContext.Response.Cookies.Append(_cookieName, token, new CookieOptions
                {
                    Expires = DateTime.Now.AddYears(10),

                });
                return token;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public void logoutSync()
        {
            try
            {
                _context.HttpContext.Response.Cookies.Delete(_cookieName);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task logoutAsync()
        {
            try
            {
                _context.HttpContext.Response.Cookies.Delete(_cookieName);
                await Task.Delay(1900);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}