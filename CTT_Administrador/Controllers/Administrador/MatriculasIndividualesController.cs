using CTT_Administrador.Auth;
using CTT_Administrador.Auth.Administrador;
using CTT_Administrador.Models.ctt;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;

namespace CTT_Administrador.Controllers.Administrador
{
    public class MatriculasIndividualesController : Controller
    {
        private readonly cttContext _context;
        private readonly IAuthorizeAdministradorTools _auth;
        private readonly string _path;

        public MatriculasIndividualesController(cttContext context, IAuthorizeAdministradorTools auth, IWebHostEnvironment _env)
        {
            _context = context;
            _auth = auth;
            _path = _env.WebRootPath;
        }

        [AuthorizeAdministrador]
        [HttpGet]
        public async Task<IActionResult> comboPeriodos()
        {
            var dapper = new MySqlConnection(ConfigurationHelper.config.GetConnectionString("ctt"));
            try
            {
                string sql = @"
                SELECT distinct(p.idPeriodo),p.detalle
                FROM gruposcursos g
                inner join cursos c on c.idCurso = g.idCurso
                inner join periodos p on p.idPeriodo = g.idPeriodo
                inner join modalidades m on m.idModalidad = g.idModalidad
                inner join tiposcursos t on t.idTipoCurso = c.idTipoCurso
                where
                (c.idCurso in(
                select idCurso
                from cursos_mallas cm
                where cm.idCursoAsociado = c.idCurso)
                or
                c.idCurso not in(
                select idCursoAsociado
                from cursos_mallas cm))
                and c.activo =1 and g.activo=1
                and p.activo = 1 and m.activa = 1
                and datediff(current_date(),p.fechaInicio)>=0
                and datediff(p.fechaFin,current_date())>=0
                                and (c.esVisible = 0 or c.esVisible is null)
                ";
                return Ok(await dapper.QueryAsync(sql));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
            finally
            {
                dapper.Dispose();
            }
        }

        [AuthorizeAdministrador]
        [HttpPost]
        public async Task<IActionResult> comboTiposCursos(int idPeriodo)
        {
            var dapper = new MySqlConnection(ConfigurationHelper.config.GetConnectionString("ctt"));
            try
            {
                string sql = @"
                SELECT distinct(t.idTipoCurso),t.tipoCurso
                FROM gruposcursos g
                inner join cursos c on c.idCurso = g.idCurso
                inner join periodos p on p.idPeriodo = g.idPeriodo
                inner join modalidades m on m.idModalidad = g.idModalidad
                inner join tiposcursos t on t.idTipoCurso = c.idTipoCurso
                where
                (c.idCurso in(
                select idCurso
                from cursos_mallas cm
                where cm.idCursoAsociado = c.idCurso)
                or
                c.idCurso not in(
                select idCursoAsociado
                from cursos_mallas cm))
                and c.activo =1 and g.activo=1
                and p.activo = 1 and m.activa = 1
                and datediff(current_date(),p.fechaInicio)>=0
                and datediff(p.fechaFin,current_date())>=0
                                and (c.esVisible = 0 or c.esVisible is null)
                and p.idPeriodo=@idPeriodo
                ";
                return Ok(await dapper.QueryAsync(sql, new { idPeriodo }));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
            finally
            {
                dapper.Dispose();
            }
        }

        [AuthorizeAdministrador]
        [HttpPost]
        public async Task<IActionResult> comboCursos(int idTipoCurso, int idPeriodo)
        {
            var dapper = new MySqlConnection(ConfigurationHelper.config.GetConnectionString("ctt"));
            try
            {
                string sql = @"
                            SELECT distinct(g.idGrupoCurso),concat(c.curso,' (',m.modalidad,')') as curso,
                            c.curso as cursoSolo
                            FROM gruposcursos g
                            inner join cursos c on c.idCurso = g.idCurso
                            inner join periodos p on p.idPeriodo = g.idPeriodo
                            inner join modalidades m on m.idModalidad = g.idModalidad
                            inner join tiposcursos t on t.idTipoCurso = c.idTipoCurso
                            where
                            (c.idCurso in(
                            select idCurso
                            from cursos_mallas cm
                            where cm.idCursoAsociado = c.idCurso)
                            or
                            c.idCurso not in(
                            select idCursoAsociado
                            from cursos_mallas cm))
                            and c.activo =1 and g.activo=1
                            and p.activo = 1 and m.activa = 1
                            and datediff(current_date(),p.fechaInicio)>=0
                            and datediff(p.fechaFin,current_date())>=0
                                            and (c.esVisible = 0 or c.esVisible is null)
                and p.idPeriodo=@idPeriodo and t.idTipoCurso=@idTipoCurso
                ";
                return Ok(await dapper.QueryAsync(sql, new { idTipoCurso, idPeriodo }));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
            finally
            {
                dapper.Dispose();
            }
        }

        [AuthorizeAdministrador]
        [HttpPost]
        public async Task<IActionResult> comboEstudiantes(int idGrupoCurso)
        {
            var dapper = new MySqlConnection(ConfigurationHelper.config.GetConnectionString("ctt"));
            try
            {
                string sql = @"
                                select e.idEstudiante,e.documentoIdentidad,e.primerNombre,e.segundoNombre,e.primerApellido,e.segundoApellido,
                                (select idMatricula
                                from matriculas m
                                where m.idGrupoCurso = @idGrupoCurso and m.idEstudiante=e.idEstudiante limit 1) as idMatricula,
                                (select paralelo
                                from matriculas m
                                where m.idGrupoCurso = @idGrupoCurso and m.idEstudiante=e.idEstudiante limit 1) as paralelo
                                from estudiantes e
                                WHERE activo= 1
                                order by primerApellido,segundoApellido,primerNombre,segundoNombre
                                ";
                return Ok(await dapper.QueryAsync(sql, new { idGrupoCurso }));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [AuthorizeAdministrador]
        [HttpPost]
        public async Task<IActionResult> cargarModulos(int idGrupoCurso, int idMatricula)
        {
            var dapper = new MySqlConnection(ConfigurationHelper.config.GetConnectionString("ctt"));
            try
            {
                string sql = @"SELECT m.idCursoAsociado,c.curso,c.precioCurso,
                                (select idMatricula from calificaciones ca
                                where idMatricula=@idMatricula AND idGrupoCurso=@idGrupoCurso
                                AND ca.idCurso=m.idCursoAsociado) as idMatricula,
                                (select valorPendiente from creditos cr
                                inner join detalleCreditos dt on dt.idCredito=cr.idCredito
                                and idMatricula=(select idMatricula from calificaciones ca
                                where idMatricula=@idMatricula AND idGrupoCurso=@idGrupoCurso
                                AND ca.idCurso=m.idCursoAsociado) and dt.idCurso=c.idCurso) as deuda
                                FROM gruposcursos g
                                inner join cursos_mallas m on m.idCurso = g.idCurso
                                inner join cursos c on c.idCurso = m.idCursoAsociado
                                where g.idGrupoCurso = @idGrupoCurso
                               ";
                return Ok(await dapper.QueryAsync(sql, new { idGrupoCurso, idMatricula }));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
            finally
            {
                dapper.Dispose();
            }
        }

        [AuthorizeAdministrador]
        [HttpPost]
        public async Task<IActionResult> generarMatricula(matriculas _data, string _modulos, int idMatricula)
        {
            var dapper = new MySqlConnection(ConfigurationHelper.config.GetConnectionString("ctt"));
            try
            {
                var usuario = _auth.getUser();

                var estudiante = await _context.estudiantes.FindAsync(_data.idEstudiante);
                var clienteExistente = await _context.clientesfacturas.Where(x => x.documento == estudiante.documentoIdentidad).FirstOrDefaultAsync();
                if (clienteExistente == null)
                {
                    _context.clientesfacturas.Add(new clientesfacturas
                    {
                        documento = estudiante?.documentoIdentidad,
                        nombre = $"{estudiante.primerApellido} {estudiante.segundoApellido} {estudiante.primerNombre} {estudiante.segundoNombre}",
                        direccion = estudiante.direccion,
                        telefono = estudiante.celular
                    }); ;
                    await _context.SaveChangesAsync();
                }
                string sql = "";
                if (idMatricula == 0)
                {
                    sql = $@"
                        INSERT INTO matriculas
                        (idEstudiante, idCliente, idGrupoCurso, idTipoDescuento, paralelo,
                        fechaRegistro, esUniandes, idCarrera, idCentro, usuarioRegistro,legalizado)
                        select distinct(e.idEstudiante),
                        (select cf.idCliente FROM clientesfacturas cf
                        WHERE  cf.documento=e.documentoIdentidad limit 1) as idCliente,
                        @idGrupoCurso,1 as idTipoDescuento,@paralelo,
                        current_timestamp(),1,null,null,@usuario,1
                        from estudiantes e
                        where e.idEstudiante=@idEstudiante
                        and e.idEstudiante not in(select m.idEstudiante
                        from matriculas m
                        inner join gruposcursos g on m.idGrupoCurso = @idGrupoCurso
                        );
                        SELECT LAST_INSERT_ID();
                        ";
                    idMatricula = await dapper.ExecuteScalarAsync<int>(sql, new { _data.idGrupoCurso, usuario, _data.paralelo, estudiante.idEstudiante });
                }
                if (!(idMatricula > 0)) throw new Exception("La matricula no sé generó exitosamente vuelva a intentarlo");
                sql = $@"
                        insert into calificaciones (idMatricula,idGrupoCurso,idCurso)
                        select
                        idMatricula,g.idGrupoCurso,cm.idCursoAsociado
                        from matriculas m
                        inner join gruposcursos g on g.idGrupoCurso = m.idGrupoCurso
                        inner join estudiantes e on e.idEstudiante = m.idEstudiante
                        inner join cursos c on c.idCurso=g.idCurso
                        inner join tiposcursos t on t.idTipoCurso = c.idTipoCurso
                        inner join cursos_mallas cm on cm.idCurso = g.idCurso
                        where c.activo = 1
                        and cm.idCursoAsociado in(
                        {_modulos}
                        )
                        and concat(cast(m.idMatricula as char),'_',
                        cast(m.idGrupoCurso as char),'_',cast(cm.idCursoAsociado as char))
                        not in(
                        select concat(cast(idMatricula as char),'_',
                        cast(idGrupoCurso as char),'_',cast(idCurso as char))
                        from calificaciones ca where ca.idMatricula=m.idMatricula
                        ) and m.idMatricula = @idMatricula
                    ";
                await dapper.ExecuteAsync(sql, new { idMatricula });

                #region credito

                //sql = $@"SELECT sum(precioCurso)
                //        FROM cursos
                //        WHERE idCurso in({_modulos})";
                //var valor = await dapper.ExecuteScalarAsync<decimal>(sql);
                //valor = valor == null ? 0 : valor;
                //if (valor > 0)
                //{
                //    sql = @"SELECT idCredito FROM creditos WHERE idMatricula=@idMatricula";
                //    int idCredito = await dapper.ExecuteScalarAsync<int>(sql, new { idMatricula });
                //    idCredito = idCredito == null ? 0 : idCredito;
                //    if (idCredito == 0)
                //    {
                //        sql = @"INSERT INTO creditos (fechaCredito, idMatricula, activo, fechaDesactivacion)
                //        VALUES(current_timestamp(),@idMatricula, 1, null);
                //        SELECT LAST_INSERT_ID();";
                //        idCredito = await dapper.ExecuteScalarAsync<int>(sql, new { idMatricula });
                //    }
                //    if (idCredito > 0)
                //    {
                //        sql = $@"INSERT INTO detallecreditos (idCredito,idCurso,valor,valorPendiente,cancelado,activo,fechaDesactivacion,fechaRegistro)
                //            SELECT @idCredito,idCurso,precioCurso,precioCurso,0,1,null,current_timestamp() FROM cursos
                //            WHERE idCurso in({_modulos})";
                //        await dapper.ExecuteAsync(sql, new { idCredito });
                //    }
                //}

                #endregion credito

                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
            finally
            {
                dapper.Dispose();
            }
        }
    }
}