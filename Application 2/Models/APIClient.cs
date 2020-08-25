namespace App2.Models
{
    /// <summary>
    /// This class specifies the API client property.
    /// </summary>
    public class APIClient
    {
        // Set a default value based on defined properties in appsettings.json:
        private string _name = "APIClients:Application1";
        private string _baseUrl = "APIClients:App1BaseURL";

        public string Name
        {
            get => _name;
            set => _name = value;
        }

        public string BaseURL
        {
            get => _baseUrl;
            set => _baseUrl = value;
        }
    }
}
