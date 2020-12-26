namespace Plando.Models
{
    public class ApplicationContextConfig
    {
        public string Server { get; set; }
        public string Database { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string ConnectionString => $"Server={Server};Database={Database};User={User};Password={Password};";

    }
}