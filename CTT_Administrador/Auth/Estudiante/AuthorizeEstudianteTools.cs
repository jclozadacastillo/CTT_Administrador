using CTT_Administrador.Auth;
using Dapper;
using MySql.Data.MySqlClient;

namespace CTT_Administrador.Auth.Estudiante
{
    public class AuthorizeEstudianteTools : IAuthorizeEstudianteTools
    {
        private IHttpContextAccessor _context;
        private string _token;
        private string _cookieName = ConfigurationHelper.config["JWT:cookieNames:estudiante"];

        public AuthorizeEstudianteTools(IHttpContextAccessor context)
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

        public string get(string key) => TokenTools.getTokenValue(key, _token);

        public string getUser() => TokenTools.getTokenValue("usuario", _token);

        public string getName() => TokenTools.getTokenValue("nombre", _token);

        public string login(TokenTools.userData user)
        {
            try
            {
                var token = TokenTools.generateToken(user);
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

        public bool inRol(string roles)
        {
            var dapper = new MySqlConnection(ConfigurationHelper.config.GetConnectionString("ctt"));
            try
            {
                var permitido = false;
                var usuario = getUser();
                if (string.IsNullOrEmpty(usuario)) throw new Exception("Usuario no encontrado");
                if (string.IsNullOrEmpty(roles?.Trim())) throw new Exception("Roles vacíos");
                string sql = @"select distinct(lower(rol)) 
                            from rolesusuarios ru
                            inner join roles r on r.idRol = ru.idRol
                            inner join usuarios u on u.idUsuario=ru.idUsuario
                            where ru.activo=1 and r.activo=1 and usuario = @usuario";
                var rolesSolicitados=roles.ToLower().Trim().Split(',');
                var rolesUsuario = dapper.Query<string>(sql, new {usuario});
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