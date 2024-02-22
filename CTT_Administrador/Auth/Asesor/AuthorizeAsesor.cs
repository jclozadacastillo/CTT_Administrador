using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MySql.Data.MySqlClient;

namespace CTT_Administrador.Auth.Asesor
{
    [AttributeUsage(AttributeTargets.All)]
    public class AuthorizeAsesor : Attribute, IAuthorizationFilter
    {
        public string? Roles { get; set; }
        private AuthorizationFilterContext? _context;
        public static readonly string _cookieName = ConfigurationHelper.config["JWT:cookieNames:asesor"];
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
            else
            {
                if (!string.IsNullOrEmpty(Roles))
                {
                    if (!inRol(Roles))
                    {
                        _context.Result = new UnauthorizedResult();
                        return;
                    }
                }
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

        public bool inRol(string roles)
        {
            var dapper = new MySqlConnection(ConfigurationHelper.config.GetConnectionString("ctt"));
            try
            {
                var permitido = false;
                var usuario = getUser();
                if (string.IsNullOrEmpty(roles?.Trim())) throw new Exception("Roles vacíos");
                string sql = @"select distinct(lower(rol))
                            from rolesusuarios ru
                            inner join roles r on r.idRol = ru.idRol
                            inner join usuarios u on u.idUsuario=ru.idUsuario
                            where ru.activo=1 and r.activo=1 and usuario = @usuario";
                var rolesSolicitados = roles.ToLower().Trim().Split(',');
                var rolesUsuario = dapper.Query<string>(sql, new { usuario });
                foreach (var item in rolesUsuario)
                {
                    if (rolesSolicitados.Contains(item))
                    {
                        permitido = true;
                        break;
                    }
                }

                return permitido;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            finally
            {
                dapper.Dispose();
            }
        }
    }
}