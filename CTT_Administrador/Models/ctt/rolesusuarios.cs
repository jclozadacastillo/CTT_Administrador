namespace CTT_Administrador.Models.ctt;

public partial class rolesusuarios
{
    public int idRolUsuario { get; set; }

    public int? idRol { get; set; }

    public int? idUsuario { get; set; }

    public sbyte? activo { get; set; }

    public virtual roles? idRolNavigation { get; set; }

    public virtual usuarios? idUsuarioNavigation { get; set; }
}