namespace Infraestructure.Config
{
    public class AppSettings : IAppSettings
    {
        public string ConnectionString { get; set; }
    }
    
    public interface IAppSettings
    {
        string ConnectionString { get; set; }
    }
}