using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CTT_Administrador.Models.ctt;

public partial class cttContext : DbContext
{
    public cttContext()
    {
    }

    public cttContext(DbContextOptions<cttContext> options)
        : base(options)
    {
    }

    public virtual DbSet<asignacionesinstructorescalificaciones> asignacionesinstructorescalificaciones { get; set; }

    public virtual DbSet<calificaciones> calificaciones { get; set; }

    public virtual DbSet<carrerasuniandes> carrerasuniandes { get; set; }

    public virtual DbSet<categorias> categorias { get; set; }

    public virtual DbSet<centrosuniandes> centrosuniandes { get; set; }

    public virtual DbSet<clientesfacturas> clientesfacturas { get; set; }

    public virtual DbSet<cursos> cursos { get; set; }

    public virtual DbSet<cursos_mallas> cursos_mallas { get; set; }

    public virtual DbSet<estudiantes> estudiantes { get; set; }

    public virtual DbSet<gruposcursos> gruposcursos { get; set; }

    public virtual DbSet<instructores> instructores { get; set; }

    public virtual DbSet<matriculas> matriculas { get; set; }

    public virtual DbSet<modalidades> modalidades { get; set; }

    public virtual DbSet<periodos> periodos { get; set; }

    public virtual DbSet<roles> roles { get; set; }

    public virtual DbSet<rolesusuarios> rolesusuarios { get; set; }

    public virtual DbSet<temas> temas { get; set; }

    public virtual DbSet<tiposcursos> tiposcursos { get; set; }

    public virtual DbSet<tiposdescuentos> tiposdescuentos { get; set; }

    public virtual DbSet<tiposdocumentos> tiposdocumentos { get; set; }

    public virtual DbSet<usuarios> usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySQL("server=localhost;user=root;password=123;database=cec_ctt;SslMode=none");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<asignacionesinstructorescalificaciones>(entity =>
        {
            entity.HasKey(e => e.idAsignacion).HasName("PRIMARY");

            entity.Property(e => e.activo).HasDefaultValueSql("'1'");
            entity.Property(e => e.atrasoNotas).HasDefaultValueSql("'0'");
            entity.Property(e => e.fechaLimiteNotas).HasColumnType("date");
            entity.Property(e => e.fechaLimiteNotasAtraso).HasColumnType("date");
            entity.Property(e => e.fechaRegistro)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.observacion).HasMaxLength(100);
            entity.Property(e => e.paralelo).HasMaxLength(1);
            entity.Property(e => e.usuarioRegistra).HasMaxLength(20);
        });

        modelBuilder.Entity<calificaciones>(entity =>
        {
            entity.HasKey(e => new { e.idMatricula, e.idGrupoCurso, e.idCurso }).HasName("PRIMARY");

            entity.Property(e => e.aprobado).HasDefaultValueSql("'0'");
            entity.Property(e => e.esExcento).HasDefaultValueSql("'0'");
            entity.Property(e => e.faltas).HasPrecision(5);
            entity.Property(e => e.nota1)
                .HasPrecision(5)
                .HasDefaultValueSql("'0.00'");
            entity.Property(e => e.nota2)
                .HasPrecision(5)
                .HasDefaultValueSql("'0.00'");
            entity.Property(e => e.nota3)
                .HasPrecision(5)
                .HasDefaultValueSql("'0.00'");
            entity.Property(e => e.nota4)
                .HasPrecision(5)
                .HasDefaultValueSql("'0.00'");
            entity.Property(e => e.nota5)
                .HasPrecision(5)
                .HasDefaultValueSql("'0.00'");
            entity.Property(e => e.observacion).HasMaxLength(100);
            entity.Property(e => e.pierdeFaltas).HasDefaultValueSql("'0'");
            entity.Property(e => e.promedioFinal)
                .HasPrecision(5)
                .HasDefaultValueSql("'0.00'");
        });

        modelBuilder.Entity<carrerasuniandes>(entity =>
        {
            entity.HasKey(e => e.idCarrera).HasName("PRIMARY");

            entity.Property(e => e.idCarrera).HasMaxLength(3);
            entity.Property(e => e.activa).HasDefaultValueSql("'1'");
            entity.Property(e => e.carrera).HasMaxLength(200);
            entity.Property(e => e.especializacion).HasMaxLength(20);
        });

        modelBuilder.Entity<categorias>(entity =>
        {
            entity.HasKey(e => e.idCategoria).HasName("PRIMARY");

            entity.Property(e => e.activo).HasDefaultValueSql("'1'");
            entity.Property(e => e.categoria).HasMaxLength(100);
            entity.Property(e => e.imagen).HasMaxLength(100);
        });

        modelBuilder.Entity<centrosuniandes>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.activo).HasDefaultValueSql("'1'");
            entity.Property(e => e.centro).HasMaxLength(20);
            entity.Property(e => e.idCentro).HasMaxLength(3);
        });

        modelBuilder.Entity<clientesfacturas>(entity =>
        {
            entity.HasKey(e => e.idCliente).HasName("PRIMARY");

            entity.HasIndex(e => e.documento, "documento").IsUnique();

            entity.Property(e => e.direccion).HasMaxLength(100);
            entity.Property(e => e.documento).HasMaxLength(13);
            entity.Property(e => e.email).HasMaxLength(100);
            entity.Property(e => e.idTipoDocumento)
                .HasMaxLength(1)
                .IsFixedLength();
            entity.Property(e => e.nombre).HasMaxLength(100);
            entity.Property(e => e.telefono).HasMaxLength(10);
        });

        modelBuilder.Entity<cursos>(entity =>
        {
            entity.HasKey(e => e.idCurso).HasName("PRIMARY");

            entity.HasIndex(e => e.idCategoria, "idCategoria");

            entity.HasIndex(e => e.idTipoCurso, "idTipoCurso");

            entity.Property(e => e.activo).HasDefaultValueSql("'1'");
            entity.Property(e => e.curso).HasMaxLength(200);
            entity.Property(e => e.idCursoPrecedencia).HasDefaultValueSql("'0'");
            entity.Property(e => e.imagen).HasMaxLength(100);
            entity.Property(e => e.objetivoPrincipal).HasMaxLength(100);
            entity.Property(e => e.objetivoSecuncdario).HasMaxLength(1000);
            entity.Property(e => e.precioCurso)
                .HasPrecision(10)
                .HasDefaultValueSql("'0.00'");
            entity.Property(e => e.tienePrecedencia).HasDefaultValueSql("'0'");

            entity.HasOne(d => d.idCategoriaNavigation).WithMany(p => p.cursos)
                .HasForeignKey(d => d.idCategoria)
                .HasConstraintName("cursos_ibfk_1");

            entity.HasOne(d => d.idTipoCursoNavigation).WithMany(p => p.cursos)
                .HasForeignKey(d => d.idTipoCurso)
                .HasConstraintName("cursos_ibfk_2");
        });

        modelBuilder.Entity<cursos_mallas>(entity =>
        {
            entity.HasKey(e => e.idCursoMalla).HasName("PRIMARY");

            entity.HasIndex(e => e.idCurso, "idCurso");

            entity.HasIndex(e => e.idCursoAsociado, "idCursoAsociado");

            entity.Property(e => e.activo).HasDefaultValueSql("'1'");
            entity.Property(e => e.valor)
                .HasPrecision(10)
                .HasDefaultValueSql("'0.00'");

            entity.HasOne(d => d.idCursoNavigation).WithMany(p => p.cursos_mallasidCursoNavigation)
                .HasForeignKey(d => d.idCurso)
                .HasConstraintName("cursos_mallas_ibfk_1");

            entity.HasOne(d => d.idCursoAsociadoNavigation).WithMany(p => p.cursos_mallasidCursoAsociadoNavigation)
                .HasForeignKey(d => d.idCursoAsociado)
                .HasConstraintName("cursos_mallas_ibfk_2");
        });

        modelBuilder.Entity<estudiantes>(entity =>
        {
            entity.HasKey(e => e.idEstudiante).HasName("PRIMARY");

            entity.HasIndex(e => e.documentoIdentidad, "documentoIdentidad").IsUnique();

            entity.Property(e => e.activo).HasDefaultValueSql("'1'");
            entity.Property(e => e.celular).HasMaxLength(10);
            entity.Property(e => e.direccion).HasMaxLength(100);
            entity.Property(e => e.documentoIdentidad).HasMaxLength(13);
            entity.Property(e => e.email).HasMaxLength(100);
            entity.Property(e => e.idTipoDocumento)
                .HasMaxLength(1)
                .IsFixedLength();
            entity.Property(e => e.observacion).HasMaxLength(200);
            entity.Property(e => e.primerApellido).HasMaxLength(40);
            entity.Property(e => e.primerNombre).HasMaxLength(40);
            entity.Property(e => e.segundoApellido).HasMaxLength(40);
            entity.Property(e => e.segundoNombre).HasMaxLength(40);
            entity.Property(e => e.sexo).HasMaxLength(10);
        });

        modelBuilder.Entity<gruposcursos>(entity =>
        {
            entity.HasKey(e => e.idGrupoCurso).HasName("PRIMARY");

            entity.Property(e => e.activo).HasDefaultValueSql("'1'");
            entity.Property(e => e.esVisible).HasDefaultValueSql("'1'");
            entity.Property(e => e.fechaFinCurso).HasColumnType("date");
            entity.Property(e => e.fechaFinMatricula).HasColumnType("date");
            entity.Property(e => e.fechaInicioCurso).HasColumnType("date");
            entity.Property(e => e.fechaInicioMatricula).HasColumnType("date");
            entity.Property(e => e.horario).HasMaxLength(100);
        });

        modelBuilder.Entity<instructores>(entity =>
        {
            entity.HasKey(e => e.idInstructor).HasName("PRIMARY");

            entity.HasIndex(e => e.documentoIdentidad, "documentoIdentidad").IsUnique();

            entity.Property(e => e.abreviaturaTitulo).HasMaxLength(10);
            entity.Property(e => e.activo).HasDefaultValueSql("'1'");
            entity.Property(e => e.celular).HasMaxLength(10);
            entity.Property(e => e.direccion).HasMaxLength(100);
            entity.Property(e => e.documentoIdentidad).HasMaxLength(13);
            entity.Property(e => e.elPassword).HasMaxLength(60);
            entity.Property(e => e.email).HasMaxLength(100);
            entity.Property(e => e.historialLaboral).HasMaxLength(200);
            entity.Property(e => e.historialTitulo).HasMaxLength(500);
            entity.Property(e => e.observacion).HasMaxLength(400);
            entity.Property(e => e.primerApellido).HasMaxLength(40);
            entity.Property(e => e.primerNombre).HasMaxLength(40);
            entity.Property(e => e.referencia).HasMaxLength(500);
            entity.Property(e => e.segundoApellido).HasMaxLength(40);
            entity.Property(e => e.segundoNombre).HasMaxLength(40);
            entity.Property(e => e.sexo).HasMaxLength(10);
            entity.Property(e => e.telefono).HasMaxLength(10);
            entity.Property(e => e.tipoDocumento).HasMaxLength(1);
        });

        modelBuilder.Entity<matriculas>(entity =>
        {
            entity.HasKey(e => e.idMatricula).HasName("PRIMARY");

            entity.Property(e => e.esUniandes).HasDefaultValueSql("'0'");
            entity.Property(e => e.fechaRegistro)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.idCarrera).HasMaxLength(3);
            entity.Property(e => e.idCentro).HasMaxLength(3);
            entity.Property(e => e.paralelo).HasMaxLength(1);
            entity.Property(e => e.usuarioRegistro).HasMaxLength(20);
        });

        modelBuilder.Entity<modalidades>(entity =>
        {
            entity.HasKey(e => e.idModalidad).HasName("PRIMARY");

            entity.Property(e => e.activa).HasDefaultValueSql("'1'");
            entity.Property(e => e.modalidad).HasMaxLength(30);
        });

        modelBuilder.Entity<periodos>(entity =>
        {
            entity.HasKey(e => e.idPeriodo).HasName("PRIMARY");

            entity.Property(e => e.activo).HasDefaultValueSql("'1'");
            entity.Property(e => e.detalle).HasMaxLength(100);
            entity.Property(e => e.fechaFin).HasColumnType("date");
            entity.Property(e => e.fechaInicio).HasColumnType("date");
        });

        modelBuilder.Entity<roles>(entity =>
        {
            entity.HasKey(e => e.idRol).HasName("PRIMARY");

            entity.Property(e => e.activo).HasDefaultValueSql("'1'");
            entity.Property(e => e.nombre).HasMaxLength(100);
            entity.Property(e => e.rol).HasMaxLength(19);
        });

        modelBuilder.Entity<rolesusuarios>(entity =>
        {
            entity.HasKey(e => e.idRolUsuario).HasName("PRIMARY");

            entity.HasIndex(e => e.idRol, "idRol");

            entity.HasIndex(e => e.idUsuario, "idUsuario");

            entity.Property(e => e.activo).HasDefaultValueSql("'1'");

            entity.HasOne(d => d.idRolNavigation).WithMany(p => p.rolesusuarios)
                .HasForeignKey(d => d.idRol)
                .HasConstraintName("rolesusuarios_ibfk_1");

            entity.HasOne(d => d.idUsuarioNavigation).WithMany(p => p.rolesusuarios)
                .HasForeignKey(d => d.idUsuario)
                .HasConstraintName("rolesusuarios_ibfk_2");
        });

        modelBuilder.Entity<temas>(entity =>
        {
            entity.HasKey(e => e.idTema).HasName("PRIMARY");

            entity.HasIndex(e => e.idCurso, "idCurso");

            entity.Property(e => e.activo).HasDefaultValueSql("'1'");
            entity.Property(e => e.orden).HasDefaultValueSql("'1'");
            entity.Property(e => e.tema).HasMaxLength(500);

            entity.HasOne(d => d.idCursoNavigation).WithMany(p => p.temas)
                .HasForeignKey(d => d.idCurso)
                .HasConstraintName("temas_ibfk_1");
        });

        modelBuilder.Entity<tiposcursos>(entity =>
        {
            entity.HasKey(e => e.idTipoCurso).HasName("PRIMARY");

            entity.Property(e => e.activo).HasDefaultValueSql("'1'");
            entity.Property(e => e.esCurso).HasDefaultValueSql("'0'");
            entity.Property(e => e.esDiplomado).HasDefaultValueSql("'0'");
            entity.Property(e => e.tipoCurso).HasMaxLength(60);
        });

        modelBuilder.Entity<tiposdescuentos>(entity =>
        {
            entity.HasKey(e => e.idTipoDescuento).HasName("PRIMARY");

            entity.Property(e => e.activo).HasDefaultValueSql("'1'");
            entity.Property(e => e.nombreDescuento).HasMaxLength(100);
            entity.Property(e => e.porcentaje)
                .HasPrecision(10)
                .HasDefaultValueSql("'0.00'");
            entity.Property(e => e.sinDescuento).HasDefaultValueSql("'0'");
        });

        modelBuilder.Entity<tiposdocumentos>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.esCedula).HasDefaultValueSql("'0'");
            entity.Property(e => e.idTipoDocumento)
                .HasMaxLength(1)
                .IsFixedLength();
            entity.Property(e => e.tipo).HasMaxLength(20);
        });

        modelBuilder.Entity<usuarios>(entity =>
        {
            entity.HasKey(e => e.idUsuario).HasName("PRIMARY");

            entity.Property(e => e.activo).HasDefaultValueSql("'1'");
            entity.Property(e => e.clave).HasMaxLength(50);
            entity.Property(e => e.nombre).HasMaxLength(100);
            entity.Property(e => e.usuario).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
