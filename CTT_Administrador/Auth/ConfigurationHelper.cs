namespace CTT_Administrador.Auth
{
    public static class ConfigurationHelper
    {
        public static IConfiguration config;
        public static string host;

        public static void Initialize(IConfiguration Configuration,string _host)
        {
            config = Configuration;
            host= _host;
        }
    }
}