//Create a settings class to store MongoDB connection information.

namespace WebService.Settings
{
    public class MongoDBSettings
    {
        public string ConnectionString { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
    }
}
