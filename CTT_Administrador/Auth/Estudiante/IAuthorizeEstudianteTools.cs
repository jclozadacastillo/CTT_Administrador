using CTT_Administrador.Auth;

namespace CTT_Estudiante.Auth.Estudiante
{
    public interface IAuthorizeEstudianteTools
    {
        string get(string key);

        string getUser();

        string getName();

        string login(TokenTools.userData user);

        void logoutSync();

        Task logoutAsync();

        bool validateToken();
        bool inRol(string roles);
    }
}