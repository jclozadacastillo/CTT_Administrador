using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CTT_Administrador.Auth.Administrador
{
    [AttributeUsage(AttributeTargets.All)]
    public class AuthorizeAdministrador : Attribute, IAuthorizationFilter
    {
        public string? Roles { get; set; }
        private AuthorizationFilterContext? _context;
        public static readonly string _cookieName = ConfigurationHelper.config["JWT:cookieNames:admin"];
        private string? _token;

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            _context = context;
            _token = _context?.HttpContext.Request.Cookies[_cookieName];
            if (!validateToken())
            {
                _context.Result = new UnauthorizedResult();
                return;
            }
        }

        public bool validateToken()
        {
            try
            {
                var valido = TokenTools.validateToken(_token);
                if (!valido) _context?.HttpContext.Response.Cookies.Delete(_cookieName);
                return valido;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                _context?.HttpContext.Response.Cookies.Delete(_cookieName);
                return false;
            }
        }

        public string getUser() => TokenTools.getTokenValue("usuario", _token);
    }
}