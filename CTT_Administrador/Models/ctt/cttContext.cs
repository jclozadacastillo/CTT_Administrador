using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CTT_Administrador.Models.ctt;

public partial class cttContext : DbContext
{
    public cttContext(DbContextOptions<cttContext> options)
        : base(options)
    {
    }

    public virtual DbSet<__efmigrationshistory> __efmigrationshistory { get; set; }

    public virtual DbSet<api_logs> api_logs { get; set; }

    public virtual DbSet<api_usuarios> api_usuarios { get; set; }

    public virtual DbSet<asignacionesinstructorescalificaciones> asignacionesinstructorescalificaciones { get; set; }

    public virtual DbSet<bancos> bancos { get; set; }

    public virtual DbSet<calificaciones> calificaciones { get; set; }

    public virtual DbSet<carrerasuniandes> carrerasuniandes { get; set; }

    public virtual DbSet<categorias> categorias { get; set; }

    public virtual DbSet<centrosuniandes> centrosuniandes { get; set; }

    public virtual DbSet<ciudades> ciudades { get; set; }

    public virtual DbSet<clientesfacturas> clientesfacturas { get; set; }

    public virtual DbSet<convenios> convenios { get; set; }

    public virtual DbSet<creditos> creditos { get; set; }

    public virtual DbSet<cuentasbancos> cuentasbancos { get; set; }

    public virtual DbSet<cursos> cursos { get; set; }

    public virtual DbSet<cursos_mallas> cursos_mallas { get; set; }

    public virtual DbSet<detallecreditos> detallecreditos { get; set; }

    public virtual DbSet<estadospagos> estadospagos { get; set; }

    public virtual DbSet<estudiantes> estudiantes { get; set; }

    public virtual DbSet<formaspagos> formaspagos { get; set; }

    public virtual DbSet<gruposcursos> gruposcursos { get; set; }

    public virtual DbSet<gruposinhouse> gruposinhouse { get; set; }

    public virtual DbSet<gruposinhousemodulos> gruposinhousemodulos { get; set; }

    public virtual DbSet<instructores> instructores { get; set; }

    public virtual DbSet<matriculas> matriculas { get; set; }

    public virtual DbSet<modalidades> modalidades { get; set; }

    public virtual DbSet<pagosinhouse> pagosinhouse { get; set; }

    public virtual DbSet<pagosmatriculas> pagosmatriculas { get; set; }

    public virtual DbSet<paises> paises { get; set; }

    public virtual DbSet<periodos> periodos { get; set; }

    public virtual DbSet<provincias> provincias { get; set; }

    public virtual DbSet<roles> roles { get; set; }

    public virtual DbSet<rolesusuarios> rolesusuarios { get; set; }

    public virtual DbSet<temas> temas { get; set; }

    public virtual DbSet<tiposcuentasbancos> tiposcuentasbancos { get; set; }

    public virtual DbSet<tiposcursos> tiposcursos { get; set; }

    public virtual DbSet<tiposdescuentos> tiposdescuentos { get; set; }

    public virtual DbSet<tiposdocumentos> tiposdocumentos { get; set; }

    public virtual DbSet<usuarios> usuarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<__efmigrationshistory>(entity =>
        {
            entity.HasKey(e => e.MigrationId).HasName("PRIMARY");
        });

        modelBuilder.Entity<api_logs>(entity =>
        {
            entity.HasKey(e => e.idLog).HasName("PRIMARY");

            entity.Property(e => e.error).HasDefaultValueSql("'0'");
            entity.Property(e => e.fechaRegistro).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<api_usuarios>(entity =>
        {
            entity.HasKey(e => e.idUsuario).HasName("PRIMARY");

            entity.Property(e => e.activo).HasDefaultValueSql("'1'");
            entity.Property(e => e.fechaRegistro).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<asignacionesinstructorescalificaciones>(entity =>
        {
            entity.HasKey(e => e.idAsignacion).HasName("PRIMARY");

            entity.Property(e => e.activo).HasDefaultValueSql("'1'");
            entity.Property(e => e.atrasoNotas).HasDefaultValueSql("'0'");
            entity.Property(e => e.fechaRegistro).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.pasaFaltas).HasDefaultValueSql("'0'");
        });

        modelBuilder.Entity<bancos>(entity =>
        {
            entity.HasKey(e => e.idBanco).HasName("PRIMARY");

            entity.Property(e => e.activo).HasDefaultValueSql("'1'");
        });

        modelBuilder.Entity<calificaciones>(entity =>
        {
            entity.HasKey(e => new { e.idMatricula, e.idGrupoCurso, e.idCurso }).HasName("PRIMARY");

            entity.Property(e => e.aprobado).HasDefaultValueSql("'0'");
            entity.Property(e => e.esExcento).HasDefaultValueSql("'0'");
            entity.Property(e => e.faltas).HasDefaultValueSql("'0.00'");
            entity.Property(e => e.fechaRegistro).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.justificaFaltas).HasDefaultValueSql("'0'");
            entity.Property(e => e.nota1).HasDefaultValueSql("'0.00'");
            entity.Property(e => e.nota2).HasDefaultValueSql("'0.00'");
            entity.Property(e => e.nota3).HasDefaultValueSql("'0.00'");
            entity.Property(e => e.nota4).HasDefaultValueSql("'0.00'");
            entity.Property(e => e.nota5).HasDefaultValueSql("'0.00'");
            entity.Property(e => e.pierdeFaltas).HasDefaultValueSql("'0'");
            entity.Property(e => e.promedioFinal).HasDefaultValueSql("'0.00'");
            entity.Property(e => e.suspendido).HasDefaultValueSql("'0'");
        });

        modelBuilder.Entity<carrerasuniandes>(entity =>
        {
            entity.HasKey(e => e.idCarrera).HasName("PRIMARY");

            entity.Property(e => e.activa).HasDefaultValueSql("'1'");
        });

        modelBuilder.Entity<categorias>(entity =>
        {
            entity.HasKey(e => e.idCategoria).HasName("PRIMARY");

            entity.Property(e => e.activo).HasDefaultValueSql("'1'");
        });

        modelBuilder.Entity<centrosuniandes>(entity =>
        {
            entity.Property(e => e.activo).HasDefaultValueSql("'1'");
        });

        modelBuilder.Entity<ciudades>(entity =>
        {
            entity.HasKey(e => e.idCiudad).HasName("PRIMARY");

            entity.Property(e => e.activo).HasDefaultValueSql("'1'");

            entity.HasOne(d => d.idProvinciaNavigation).WithMany(p => p.ciudades).HasConstraintName("ciudades_ibfk_1");
        });

        modelBuilder.Entity<clientesfacturas>(entity =>
        {
            entity.HasKey(e => e.idCliente).HasName("PRIMARY");

            entity.Property(e => e.idTipoDocumento).IsFixedLength();
        });

        modelBuilder.Entity<convenios>(entity =>
        {
            entity.HasKey(e => e.idConvenio).HasName("PRIMARY");

            entity.Property(e => e.activo).HasDefaultValueSql("'1'");
            entity.Property(e => e.permanete).HasDefaultValueSql("'0'");
            entity.Property(e => e.sinConvenio).HasDefaultValueSql("'0'");

            entity.HasOne(d => d.idClienteNavigation).WithMany(p => p.convenios).HasConstraintName("convenios_ibfk_1");
        });

        modelBuilder.Entity<creditos>(entity =>
        {
            entity.HasKey(e => e.idCredito).HasName("PRIMARY");

            entity.Property(e => e.activo).HasDefaultValueSql("'1'");
            entity.Property(e => e.fechaCredito).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.fechaDesactivacion).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<cuentasbancos>(entity =>
        {
            entity.HasKey(e => e.idCuenta).HasName("PRIMARY");

            entity.Property(e => e.activo).HasDefaultValueSql("'1'");

            entity.HasOne(d => d.idBancoNavigation).WithMany(p => p.cuentasbancos).HasConstraintName("cuentasbancos_ibfk_1");

            entity.HasOne(d => d.idTipoCuentaBancoNavigation).WithMany(p => p.cuentasbancos).HasConstraintName("cuentasbancos_ibfk_2");
        });

        modelBuilder.Entity<cursos>(entity =>
        {
            entity.HasKey(e => e.idCurso).HasName("PRIMARY");

            entity.Property(e => e.activo).HasDefaultValueSql("'1'");
            entity.Property(e => e.fechaRegistro).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.idCursoPrecedencia).HasDefaultValueSql("'0'");
            entity.Property(e => e.numeroModulo).HasDefaultValueSql("'0'");
            entity.Property(e => e.precioCurso).HasDefaultValueSql("'0.00'");
            entity.Property(e => e.tienePrecedencia).HasDefaultValueSql("'0'");

            entity.HasOne(d => d.idCategoriaNavigation).WithMany(p => p.cursos).HasConstraintName("cursos_ibfk_1");

            entity.HasOne(d => d.idTipoCursoNavigation).WithMany(p => p.cursos).HasConstraintName("cursos_ibfk_2");
        });

        modelBuilder.Entity<cursos_mallas>(entity =>
        {
            entity.HasKey(e => e.idCursoMalla).HasName("PRIMARY");

            entity.Property(e => e.activo).HasDefaultValueSql("'1'");
            entity.Property(e => e.valor).HasDefaultValueSql("'0.00'");

            entity.HasOne(d => d.idCursoNavigation).WithMany(p => p.cursos_mallasidCursoNavigation).HasConstraintName("cursos_mallas_ibfk_1");

            entity.HasOne(d => d.idCursoAsociadoNavigation).WithMany(p => p.cursos_mallasidCursoAsociadoNavigation).HasConstraintName("cursos_mallas_ibfk_2");
        });

        modelBuilder.Entity<detallecreditos>(entity =>
        {
            entity.HasKey(e => e.idDetalleCredito).HasName("PRIMARY");

            entity.Property(e => e.activo).HasDefaultValueSql("'1'");
            entity.Property(e => e.cancelado).HasDefaultValueSql("'0'");
            entity.Property(e => e.fechaDesactivacion).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.fechaRegistro).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<estadospagos>(entity =>
        {
            entity.HasKey(e => e.idEstadoPago).HasName("PRIMARY");

            entity.Property(e => e.activo).HasDefaultValueSql("'1'");
        });

        modelBuilder.Entity<estudiantes>(entity =>
        {
            entity.HasKey(e => e.idEstudiante).HasName("PRIMARY");

            entity.Property(e => e.activo).HasDefaultValueSql("'1'");
            entity.Property(e => e.confirmado).HasDefaultValueSql("'0'");
            entity.Property(e => e.idTipoDocumento).IsFixedLength();
        });

        modelBuilder.Entity<formaspagos>(entity =>
        {
            entity.HasKey(e => e.idFormaPago).HasName("PRIMARY");

            entity.Property(e => e.activo).HasDefaultValueSql("'1'");
        });

        modelBuilder.Entity<gruposcursos>(entity =>
        {
            entity.HasKey(e => e.idGrupoCurso).HasName("PRIMARY");

            entity.Property(e => e.activo).HasDefaultValueSql("'1'");
            entity.Property(e => e.esVisible).HasDefaultValueSql("'1'");
        });

        modelBuilder.Entity<gruposinhouse>(entity =>
        {
            entity.HasKey(e => e.idGrupoInHouse).HasName("PRIMARY");

            entity.Property(e => e.fechaRegistro).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<gruposinhousemodulos>(entity =>
        {
            entity.HasKey(e => e.idGrupoInHouseModulo).HasName("PRIMARY");

            entity.HasOne(d => d.idGrupoInHouseNavigation).WithMany(p => p.gruposinhousemodulos).HasConstraintName("gruposinhousemodulos_ibfk_1");
        });

        modelBuilder.Entity<instructores>(entity =>
        {
            entity.HasKey(e => e.idInstructor).HasName("PRIMARY");

            entity.Property(e => e.activo).HasDefaultValueSql("'1'");
        });

        modelBuilder.Entity<matriculas>(entity =>
        {
            entity.HasKey(e => e.idMatricula).HasName("PRIMARY");

            entity.Property(e => e.esUniandes).HasDefaultValueSql("'0'");
            entity.Property(e => e.fechaRegistro).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.inHouse).HasDefaultValueSql("'0'");
            entity.Property(e => e.legalizado).HasDefaultValueSql("'0'");
        });

        modelBuilder.Entity<modalidades>(entity =>
        {
            entity.HasKey(e => e.idModalidad).HasName("PRIMARY");

            entity.Property(e => e.activa).HasDefaultValueSql("'1'");
        });

        modelBuilder.Entity<pagosinhouse>(entity =>
        {
            entity.HasKey(e => e.idPago).HasName("PRIMARY");

            entity.HasOne(d => d.idCuentaNavigation).WithMany(p => p.pagosinhouse).HasConstraintName("pagosinhouse_ibfk_4");

            entity.HasOne(d => d.idEstadoNavigation).WithMany(p => p.pagosinhouse).HasConstraintName("pagosinhouse_ibfk_1");

            entity.HasOne(d => d.idFormaPagoNavigation).WithMany(p => p.pagosinhouse).HasConstraintName("pagosinhouse_ibfk_3");

            entity.HasOne(d => d.idGrupoInHouseNavigation).WithMany(p => p.pagosinhouse).HasConstraintName("pagosinhouse_ibfk_2");
        });

        modelBuilder.Entity<pagosmatriculas>(entity =>
        {
            entity.HasKey(e => e.idPagoMatricula).HasName("PRIMARY");

            entity.HasOne(d => d.idClienteNavigation).WithMany(p => p.pagosmatriculas).HasConstraintName("pagosmatriculas_ibfk_4");

            entity.HasOne(d => d.idCuentaNavigation).WithMany(p => p.pagosmatriculas).HasConstraintName("pagosmatriculas_ibfk_3");

            entity.HasOne(d => d.idFormaPagoNavigation).WithMany(p => p.pagosmatriculas).HasConstraintName("pagosmatriculas_ibfk_2");

            entity.HasOne(d => d.idMatriculaNavigation).WithMany(p => p.pagosmatriculas).HasConstraintName("pagosmatriculas_ibfk_1");
        });

        modelBuilder.Entity<paises>(entity =>
        {
            entity.HasKey(e => e.idPais).HasName("PRIMARY");

            entity.Property(e => e.activo).HasDefaultValueSql("'1'");
        });

        modelBuilder.Entity<periodos>(entity =>
        {
            entity.HasKey(e => e.idPeriodo).HasName("PRIMARY");

            entity.Property(e => e.activo).HasDefaultValueSql("'1'");
        });

        modelBuilder.Entity<provincias>(entity =>
        {
            entity.HasKey(e => e.idProvincia).HasName("PRIMARY");

            entity.Property(e => e.activo).HasDefaultValueSql("'1'");

            entity.HasOne(d => d.idPaisNavigation).WithMany(p => p.provincias).HasConstraintName("provincias_ibfk_1");
        });

        modelBuilder.Entity<roles>(entity =>
        {
            entity.HasKey(e => e.idRol).HasName("PRIMARY");

            entity.Property(e => e.activo).HasDefaultValueSql("'1'");
        });

        modelBuilder.Entity<rolesusuarios>(entity =>
        {
            entity.HasKey(e => e.idRolUsuario).HasName("PRIMARY");

            entity.Property(e => e.activo).HasDefaultValueSql("'1'");

            entity.HasOne(d => d.idRolNavigation).WithMany(p => p.rolesusuarios).HasConstraintName("rolesusuarios_ibfk_1");

            entity.HasOne(d => d.idUsuarioNavigation).WithMany(p => p.rolesusuarios).HasConstraintName("rolesusuarios_ibfk_2");
        });

        modelBuilder.Entity<temas>(entity =>
        {
            entity.HasKey(e => e.idTema).HasName("PRIMARY");

            entity.Property(e => e.activo).HasDefaultValueSql("'1'");
            entity.Property(e => e.orden).HasDefaultValueSql("'1'");

            entity.HasOne(d => d.idCursoNavigation).WithMany(p => p.temas).HasConstraintName("temas_ibfk_1");
        });

        modelBuilder.Entity<tiposcuentasbancos>(entity =>
        {
            entity.HasKey(e => e.idTipoCuentaBanco).HasName("PRIMARY");

            entity.Property(e => e.activo).HasDefaultValueSql("'1'");
        });

        modelBuilder.Entity<tiposcursos>(entity =>
        {
            entity.HasKey(e => e.idTipoCurso).HasName("PRIMARY");

            entity.Property(e => e.activo).HasDefaultValueSql("'1'");
            entity.Property(e => e.esCurso).HasDefaultValueSql("'0'");
            entity.Property(e => e.esDiplomado).HasDefaultValueSql("'0'");
        });

        modelBuilder.Entity<tiposdescuentos>(entity =>
        {
            entity.HasKey(e => e.idTipoDescuento).HasName("PRIMARY");

            entity.Property(e => e.activo).HasDefaultValueSql("'1'");
            entity.Property(e => e.porcentaje).HasDefaultValueSql("'0.00'");
            entity.Property(e => e.sinDescuento).HasDefaultValueSql("'0'");
        });

        modelBuilder.Entity<tiposdocumentos>(entity =>
        {
            entity.Property(e => e.esCedula).HasDefaultValueSql("'0'");
            entity.Property(e => e.idTipoDocumento).IsFixedLength();
        });

        modelBuilder.Entity<usuarios>(entity =>
        {
            entity.HasKey(e => e.idUsuario).HasName("PRIMARY");

            entity.Property(e => e.activo).HasDefaultValueSql("'1'");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
