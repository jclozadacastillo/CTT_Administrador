namespace CTT_Administrador.Auth.Docente
{
    public interface IAuthorizeDocenteTools
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

