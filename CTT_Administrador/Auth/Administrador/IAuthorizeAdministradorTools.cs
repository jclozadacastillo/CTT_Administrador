namespace CTT_Administrador.Auth.Administrador
{
    public interface IAuthorizeAdministradorTools
    {
        string get(string key);

        string getUser();

        string getName();

        string login(TokenTools.userData user);

        void logoutSync();

        Task logoutAsync();

        bool validateToken();
    }
}