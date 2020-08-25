namespace MVCApp1.Models
{
    /// <summary>
    /// This class specifies the API client properties based on appsettings.json.
    /// </summary>
    public class APIClient
    {
        // Set a default value with defined properties specified in appsettings.json:
        private string _name = "APIClients:Application2";
        private string _baseUrl = "APIClients:App2BaseURL";

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

    } // End class.
} // End namespace.
