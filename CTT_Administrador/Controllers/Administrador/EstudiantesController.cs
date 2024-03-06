using CTT_Administrador.Models.ctt;
using CTT_Administrador.Utilities;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CTT_Administrador.Controllers.Administrador
{
    public class EstudiantesController : Controller
    {
        private readonly cttContext _context;
        private readonly IDbConnection _dapper;

        public EstudiantesController(cttContext context)
        {
            _context = context;
            _dapper = _context.Database.GetDbConnection();
        }

        [HttpPost]
        public async Task<IActionResult> listar([FromBody] Tools.DataTableModel _params)
        {
            try
            {
                string sql = @"SELECT idEstudiante,documentoIdentidad,
                            REPLACE(concat(primerApellido,' ',
                            CASE WHEN segundoApellido IS NULL THEN '' ELSE segundoApellido END,' ',
                            primerNombre,' ',
                            CASE WHEN segundoNombre IS NULL THEN '' ELSE segundoNombre END),'  ',' ') as estudiante,
                            e.activo, celular,email,ciudad,provincia
                            FROM estudiantes e
                            INNER JOIN ciudades c on c.idCiudad=e.idCiudad
                            INNER JOIN provincias p on p.idProvincia=c.idProvincia
                                ";
                return Ok(await Tools.DataTableMySql(new Tools.DataTableParams
                {
                    query = sql,
                    dapperConnection = _context.Database.GetDbConnection(),
                    dataTableModel = _params
                }));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> listaCiudades()
        {
            try
            {
                return Ok(await _context.ciudades.AsNoTracking().Where(x => x.activo == 1).ToListAsync());
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> listaProvincias()
        {
            try
            {
                return Ok(await _context.provincias.AsNoTracking().Where(x => x.activo == 1).ToListAsync());
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> comboTiposDocumentos()
        {
            try
            {
                return Ok(await _context.tiposdocumentos.ToListAsync());
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> unDato(int idEstudiante)
        {
            try
            {
                string sql = @"SELECT e.*,idProvincia
                               FROM estudiantes e
                               LEFT JOIN ciudades c ON c.idCiudad=e.idCiudad
                               WHERE e.idEstudiante=@idEstudiante;";
                return Ok(await _dapper.QueryFirstOrDefaultAsync(sql, new { idEstudiante }));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> guardar(estudiantes _data)
        {
            try
            {
                if (_context.estudiantes.Where(x => x.documentoIdentidad == _data.documentoIdentidad && _data.idEstudiante != x.idEstudiante).Count() > 0) throw new Exception("Lo sentimos esa documento de identidad ya se encuentra registrado");
                if (_data.idEstudiante > 0)
                {
                    var anterior = await _context.estudiantes.AsNoTracking().Where(x => x.idEstudiante == _data.idEstudiante).FirstOrDefaultAsync();
                    _data.activo = anterior.activo;
                    _data.clave = anterior.clave;
                    _context.estudiantes.Update(_data);
                }
                else
                {
                    _data.activo = 1;
                    _data.clave = _data.documentoIdentidad;
                    _context.estudiantes.Add(_data);
                }
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> activar(int idEstudiante)
        {
            try
            {
                var data = await _context.estudiantes.FindAsync(idEstudiante);
                if (data == null) throw new Exception("Elemento no encontrado");
                data.activo = Convert.ToBoolean(data.activo) == true ? Convert.ToSByte(false) : Convert.ToSByte(true);
                _context.Update(data);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
    }
}