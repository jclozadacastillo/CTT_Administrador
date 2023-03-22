namespace CTT_Administrador.Auth
{
    public static class ConfigurationHelper
    {
        public static IConfiguration config;

        public static void Initialize(IConfiguration Configuration) => config = Configuration;
    }
}