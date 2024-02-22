using Dapper;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CTT_Administrador.Utilities
{
    public static class Tools
    {
        public static IConfiguration? config;
        public static string? rootPath;

        public static void Initialize(IConfiguration Configuration, string _host)
        {
            config = Configuration!;
            rootPath = _host!;
        }

        public class userData
        {
            public string? usuario { get; set; }
            public string? clave { get; set; }
            public string? nombre { get; set; }
            public string? email { get; set; }
            public string? idcentro { get; set; }
            public string? centro_detalle { get; set; }
            public bool? persistent { get; set; }
            public string? roles { get; set; }
        }

        public class Column
        {
            public string data { get; set; }
            public string name { get; set; }
            public bool? searchable { get; set; }
            public bool? orderable { get; set; }
            public Search search { get; set; }
        }

        public class DataTableModel
        {
            public int draw { get; set; }

            public int start { get; set; }
            public int length { get; set; }
            public List<Column> columns { get; set; }
            public Search search { get; set; }
            public List<Order> order { get; set; }
            public string? parametros { get; set; }
        }

        public class Order
        {
            public int column { get; set; }
            public string dir { get; set; }
        }

        public class Search
        {
            public string value { get; set; }
            public bool? regex { get; set; }
        }

        public class DataTableResponse
        {
            public int draw { get; set; }
            public int recordsTotal { get; set; }
            public int recordsFiltered { get; set; }
            public IEnumerable<dynamic>? data { get; set; }
        }

        public static string getIdEmpresa(HttpContext _httpContext)
        {
            try
            {
                return getClaim(_httpContext, "idEmpresa");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "";
            }
        }

        public static string getIdUsuario(HttpContext _httpContext)
        {
            try
            {
                return getClaim(_httpContext, "idUsuario");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "";
            }
        }

        public static string getClaim(HttpContext _httpContext, string claim)
        {
            try
            {
                var identity = (ClaimsIdentity)_httpContext.User.Identity;
                return identity?.FindFirst(claim)?.Value;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "";
            }
        }

        public static bool validateToken(string? token)
        {
            try
            {
                if (string.IsNullOrEmpty(token)) return false;
                token = token.Replace("Bearer ", "").TrimStart().TrimEnd();
                var validador = new JwtSecurityTokenHandler();
                SecurityToken validatedToken;
                var validToken = validador.ValidateToken(token, new TokenValidationParameters()
                {
                    ValidateLifetime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:key"]))
                }, out validatedToken);
                return DateTime.Now < validatedToken.ValidTo.AddHours(-5);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public static string validateTokenApi(string? token)
        {
            try
            {
                if (string.IsNullOrEmpty(token)) return "empty";
                token = token.Replace("Bearer ", "").TrimStart().TrimEnd();
                var validador = new JwtSecurityTokenHandler();
                SecurityToken validatedToken;
                var validToken = validador.ValidateToken(token, new TokenValidationParameters()
                {
                    ValidateLifetime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:key"]))
                }, out validatedToken);
                return (DateTime.Now < validatedToken.ValidTo.AddHours(-5)) ? "" : "expired";
            }
            catch (Exception ex)
            {
                handleError(ex);
                return (ex.Message.Contains("Lifetime")) ? "expired" : "El Token proporcionado no es válido";
            }
        }

        public static string getTokenValue(string _claim, string? _token)
        {
            try
            {
                if (string.IsNullOrEmpty(_token)) return "";
                _token = _token.Replace("Bearer ", "").TrimStart().TrimEnd();
                var validador = new JwtSecurityTokenHandler();
                SecurityToken validatedToken;
                var validToken = validador.ValidateToken(_token, new TokenValidationParameters()
                {
                    ValidateLifetime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:key"]))
                }, out validatedToken);
                var identities = validToken.Identities.FirstOrDefault();
                _claim = _claim.ToLower();
                if (_claim == "user") _claim = "usuario";
                if (_claim == "mail") _claim = "correo";
                if (_claim == "email") _claim = "correo";
                if (_claim == "name") _claim = "nombre";
                return identities?.FindFirst(_claim)?.Value;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "";
            }
        }

        public static string generateToken(userData _user)
        {
            try
            {
                var claims = new[]{
                new Claim("nombre", string.IsNullOrEmpty(_user.nombre)?"":_user.nombre),
                new Claim("correo", string.IsNullOrEmpty(_user.email)?"":_user.email),
                new Claim("centro_detalle", string.IsNullOrEmpty(_user.centro_detalle)?"":_user.centro_detalle),
                new Claim("roles", string.IsNullOrEmpty(_user.roles)?"":_user.roles),
                new Claim("usuario",_user.usuario),
                new Claim("idcentro",string.IsNullOrEmpty(_user.idcentro)?"":_user.idcentro)
                };
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:key"]));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
                var durationHours = _user.persistent == true ?
                    Convert.ToDouble(config["JWT:times:persistent"]?.Replace(",", ".")) :
                    Convert.ToDouble(config["JWT:times:default"]?.Replace(",", "."));
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

        public static IActionResult handleError(Exception error)
        {
            var result = new ObjectResult("");
            try
            {
                result.StatusCode = 500;
                if (error.Source == "modelError") result.StatusCode = 422;
                if (error.Source.ToLower().Contains("data")) result.StatusCode = 424;
                result.Value = error.Message;
                logError(error);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                result.StatusCode = 500;
                logError(ex);
                return result;
            }
        }

        public static void logError(Exception _error)
        {
            try
            {
                IDbConnection dapper = new MySqlConnection(config.GetConnectionString("SG_DISTRIBUTIVO"));
                var path = $@"{rootPath}/_errors.txt";
                var error = new
                {
                    error = $"Error: {_error.Message}  - Source: {_error.StackTrace}",
                    fechaRegistro = DateTime.Now
                };
                string sql = @"INSERT INTO logs (fechaRegistro, error) VALUES(@fechaRegistro, @error);";
                dapper.Execute(sql, error);
                //var path = $@"{rootPath}/_errors.txt";
                //if (!File.Exists(path))
                //{
                //    using (var sw = new StreamWriter(path, true))
                //    {
                //        sw.WriteLine($"{DateTime.Now} - Error: {_error.Message}  - Source: {_error.StackTrace}");
                //    }
                //}
                //else
                //{
                //    File.AppendAllLines(path, new[] { $"{DateTime.Now} - Error: {_error.Message} - Source: {_error.StackTrace}" });
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static Exception handleModelError(ModelStateDictionary modelState)
        {
            var _ex = new Exception();
            try
            {
                var errores = modelState?.Where(x => x.Value.ValidationState == ModelValidationState.Invalid);
                var elementos = new List<string>();
                var mensaje = "";
                foreach (var item in errores)
                {
                    mensaje += mensaje == "" ? $"- {item.Value?.Errors?.FirstOrDefault()?.ErrorMessage}" : $"<br>- {item.Value?.Errors?.FirstOrDefault()?.ErrorMessage}";
                    elementos.Add(item.Key);
                }

                _ex = new Exception(JsonConvert.SerializeObject(new { mensaje, elementos }));
                _ex.Source = "modelError";
                return _ex;
            }
            catch (Exception ex)
            {
                ex.Source = "modelError";
                return _ex = new Exception(JsonConvert.SerializeObject(new { mensaje = ex.Message, elementos = new List<string>() }));
            }
        }

        public class DataTableParams
        {
            public dynamic? parameters { get; set; }
            public string query { get; set; }
            public IDbConnection dapperConnection { get; set; }
            public DataTableModel? dataTableModel { get; set; }
        }

        public static async Task<DataTableResponse> DataTableSql(DataTableParams _dataParams)
        {
            IDbConnection _dapper = _dataParams.dapperConnection;
            SqlMapper.Settings.CommandTimeout = 1900;
            try
            {
                var _params = _dataParams.dataTableModel;
                var busqueda = _params.search.value != null ? _params.search.value : "";
                var filtro = "";
                IEnumerable<string> queryParams = _dataParams.dataTableModel.columns.Select(x => x.data).ToList();
                var orderBy = queryParams.FirstOrDefault();
                var orderDirection = "";
                if (_params.order.Count > 0)
                {
                    orderBy = _params.columns[_params.order[0].column].data;
                    orderDirection = _params.order[0].dir;
                }
                if (!string.IsNullOrEmpty(busqueda.Trim()))
                {
                    busqueda = $"%{busqueda}%";
                    foreach (var item in queryParams)
                    {
                        filtro += filtro == "" ? $" WHERE cast({item} as varchar) COLLATE Latin1_general_CI_AI LIKE @busqueda COLLATE Latin1_general_CI_AI" : $" OR cast({item} as varchar) COLLATE Latin1_general_CI_AI LIKE @busqueda COLLATE Latin1_general_CI_AI ";
                    }
                }
                var parameters = new DynamicParameters();
                if (!string.IsNullOrEmpty(_dataParams.dataTableModel.parametros))
                {
                    foreach (var item in JsonConvert.DeserializeObject<dynamic>(_dataParams.dataTableModel.parametros)) parameters.Add(item.Name, item.Value.Value);
                }

                parameters.Add("busqueda", busqueda);
                if (_dataParams.parameters != null)
                {
                    var properties = _dataParams.parameters.GetType().GetProperties();
                    foreach (var property in properties)
                    {
                        var key = property.Name;
                        var value = _dataParams.parameters.GetType().GetProperty(property.Name).GetValue(_dataParams.parameters, null);
                        parameters.Add(key, value);
                    }
                }
                string sql = $@"
                            SELECT * FROM (
                            SELECT ROW_NUMBER() over(order by {orderBy} {orderDirection}) as row,* from(
                            {_dataParams.query}
                            ) as t_t_t_jclc {filtro}
                            ) as t_t_t_jclc_tf
                            WHERE row BETWEEN {_params.start} AND {_params.start + _params.length}
                            ";
                var lista = await _dapper.QueryAsync(sql, parameters);
                //Total Global
                sql = $@"SELECT COUNT(*)
                         FROM ({_dataParams.query}) t_t_t_jclc";
                var recordsTotal = await _dapper.ExecuteScalarAsync<int>(sql, parameters);
                //Total Filtrado
                sql = $@" SELECT COUNT(*) from(
                            {_dataParams.query}
                            ) as t_t_t_jclc_tf {filtro}
                        ";
                var recordsFiltered = await _dapper.ExecuteScalarAsync<int>(sql, parameters);
                //Modelo DataTable Server Side
                return new DataTableResponse
                {
                    draw = Convert.ToInt32(_params.draw),
                    recordsTotal = recordsTotal,
                    data = lista,
                    recordsFiltered = recordsFiltered,
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static async Task<DynamicParameters> getParametersFromJsonString(string json)
        {
            try
            {
                var parameters = new DynamicParameters();
                if (!string.IsNullOrEmpty(json))
                {
                    foreach (var item in JsonConvert.DeserializeObject<dynamic>(json)) parameters.Add(item.Name, item.Value.Value);
                }
                return parameters;
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
                return new DynamicParameters();
            }
        }

        public static async Task<IEnumerable<dynamic>> DataTableSqlReport(DataTableParams _dataParams)
        {
            IDbConnection _dapper = _dataParams.dapperConnection;
            try
            {
                var _params = _dataParams.dataTableModel;
                var busqueda = _params.search.value != null ? _params.search.value : "";
                var filtro = "";
                IEnumerable<string> queryParams = _dataParams.dataTableModel.columns.Select(x => x.data).ToList();
                var orderBy = queryParams.FirstOrDefault();
                var orderDirection = "";
                if (_params.order.Count > 0)
                {
                    orderBy = _params.columns[_params.order[0].column].data;
                    orderDirection = _params.order[0].dir;
                }
                if (!string.IsNullOrEmpty(busqueda.Trim()))
                {
                    busqueda = $"%{busqueda}%";
                    foreach (var item in queryParams)
                    {
                        filtro += filtro == "" ? $" WHERE cast({item} as varchar) COLLATE Latin1_general_CI_AI LIKE @busqueda COLLATE Latin1_general_CI_AI" : $" OR cast({item} as varchar) COLLATE Latin1_general_CI_AI LIKE @busqueda COLLATE Latin1_general_CI_AI ";
                    }
                }
                var parameters = new DynamicParameters();
                if (!string.IsNullOrEmpty(_dataParams.dataTableModel.parametros))
                {
                    foreach (var item in JsonConvert.DeserializeObject<dynamic>(_dataParams.dataTableModel.parametros)) parameters.Add(item.Name, item.Value.Value);
                }

                parameters.Add("busqueda", busqueda);
                if (_dataParams.parameters != null)
                {
                    var properties = _dataParams.parameters.GetType().GetProperties();
                    foreach (var property in properties)
                    {
                        var key = property.Name;
                        var value = _dataParams.parameters.GetType().GetProperty(property.Name).GetValue(_dataParams.parameters, null);
                        parameters.Add(key, value);
                    }
                }
                string sql = $@"
                            SELECT * FROM (
                            SELECT * FROM(
                            {_dataParams.query}
                            ) as t_t_t_jclc {filtro}
                            ) as t_t_t_jclc_tf
                            ";
                return await _dapper.QueryAsync(sql, parameters);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static async Task<DataTableResponse> DataTableMySql(DataTableParams _dataParams)
        {
            var _params = _dataParams.dataTableModel;
            IDbConnection _dapper = _dataParams.dapperConnection;
            var busqueda = _params.search.value != null ? _params.search.value : "";
            var filtro = "";
            //string sql = $@"SELECT TOP 1 * FROM({_dataParams.query}) as t_t_jclc_f";
            //var _fila = _dapper.QueryFirstOrDefault(sql);
            IEnumerable<string> queryParams = _dataParams.dataTableModel.columns.Select(x => x.data).ToList();
            //if (_fila != null)
            //{
            //    IDictionary<string, object> columnas = (IDictionary<string, object>)_fila;
            //    queryParams= columnas.Keys;
            //}

            var orderBy = queryParams.FirstOrDefault();
            var orderDirection = "";
            if (_params.order.Count > 0)
            {
                orderBy = _params.columns[_params.order[0].column].data;
                orderDirection = _params.order[0].dir;
            }
            if (!string.IsNullOrEmpty(busqueda.Trim()))
            {
                busqueda = $"%{busqueda}%";
                foreach (var item in queryParams)
                {
                    filtro += filtro == "" ? $@" WHERE upper(cast({item} as char)) LIKE upper(@busqueda) " : $@" OR upper(cast({item} as char)) LIKE upper(@busqueda) ";
                }
            }
            var parameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(_dataParams.dataTableModel.parametros))
            {
                foreach (var item in JsonConvert.DeserializeObject<dynamic>(_dataParams.dataTableModel.parametros)) parameters.Add(item.Name, item.Value.Value);
            }
            parameters.Add("busqueda", busqueda);
            if (_dataParams.parameters != null)
            {
                var properties = _dataParams.parameters.GetType().GetProperties();
                foreach (var property in properties)
                {
                    var key = property.Name;
                    var value = _dataParams.parameters.GetType().GetProperty(property.Name).GetValue(_dataParams.parameters, null);
                    parameters.Add(key, value);
                }
            }
            string sql = $@"
                            SELECT * FROM (
                            SELECT *,@rownum := @rownum + 1 as fila from(
                            {_dataParams.query}
                            order by {orderBy} {orderDirection}
                            ) as t_t_t_jclc,(SELECT @rownum := 0) r
                            {filtro}
                            ) as t_t_t_jclc_tf
                            WHERE fila >{_params.start}
                            LIMIT {_params.length}
                            ";
            var lista = await _dapper.QueryAsync(sql, parameters);
            //Total Global
            sql = $@"SELECT COUNT(*)
                         FROM ({_dataParams.query}) t_t_t_jclc";
            var recordsTotal = await _dapper.ExecuteScalarAsync<int>(sql, parameters);
            //Total Filtrado
            sql = $@" SELECT COUNT(*) FROM (
                            SELECT *,@rownum := @rownum + 1 as fila from(
                            {_dataParams.query}
                            order by {orderBy} {orderDirection}
                            ) as t_t_t_jclc,(SELECT @rownum := 0) r
                            {filtro}
                            ) as t_t_t_jclc_tf
                        ";
            var recordsFiltered = await _dapper.ExecuteScalarAsync<int>(sql, parameters);
            //Modelo DataTable Server Side
            return new DataTableResponse
            {
                draw = Convert.ToInt32(_params.draw),
                recordsTotal = recordsTotal,
                data = lista,
                recordsFiltered = recordsFiltered,
            };
        }

        public static string getToken(HttpContext _httpContext) => _httpContext.Request.Headers[HeaderNames.Authorization].ToString();

        public static string getUser(HttpContext _httpContext) => getContextClaim(_httpContext, "usuario");

        public static string getContextClaim(HttpContext _httpContext, string claim) => getTokenValue(claim, getToken(_httpContext));

        public static string? carnetTipo1(string cadena)
        {
            try
            {
                var vector = Convert.ToString(cadena)?.ToUpper().Split(new string[] { "ADDR" }, StringSplitOptions.None);
                if (!(vector?.Length > 1)) throw new Exception("Incompatible carnetTipo1");
                vector = vector[1].Split(new string[] { "TEL" }, StringSplitOptions.None);
                vector = vector[0].Split(new string[] { "Ñ" }, StringSplitOptions.None);
                string cedula = vector[1];
                return (cedula.Length >= 5 && cedula.Length <= 14) ? cedula : null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public static string? carnetTipo2(string cadena)
        {
            try
            {
                var vector = Convert.ToString(cadena)?.ToUpper().Split(new string[] { "ADDR" }, StringSplitOptions.None);
                if (!(vector?.Length > 1)) throw new Exception("Incompatible carnetTipo2");
                vector = vector[1].Split(new string[] { "TEL" }, StringSplitOptions.None);
                vector = vector[1].Split(new string[] { "Ñ" }, StringSplitOptions.None);
                vector = vector[1].Split(new string[] { "EMAIL" }, StringSplitOptions.None);
                string cedula = vector[0];
                return (cedula.Length >= 5 && cedula.Length <= 14) ? cedula : null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public static Guid toGuid(Guid? guid)
        {
            return guid ?? Guid.Empty;
        }

        public static string Base64Decode(string base64EncodedData)
        {
            if (string.IsNullOrEmpty(base64EncodedData)) return null;
            string decoded = Encoding.UTF8.GetString(Convert.FromBase64String(base64EncodedData));
            return System.Uri.UnescapeDataString(decoded);
        }

        public static double ConvertirFloatDouble(float _num)
        {
            string _floatString = _num.ToString();
            try
            {
                var array = _floatString.Split(',');
                var decimales = array[1] == null ? "00" : array[1].Substring(0, 2);
                _floatString = $"{array[0]},{decimales}";
                return Convert.ToDouble(_floatString);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Convert.ToDouble(_floatString);
            }
        }
    }
}