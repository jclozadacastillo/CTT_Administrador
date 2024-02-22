namespace CTT_Administrador.Auth.Asesor
{
    public interface IAuthorizeAsesorTools
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