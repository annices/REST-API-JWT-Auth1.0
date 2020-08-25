using App2.DBAccess;
using Microsoft.Extensions.Configuration;

namespace App2.Models
{
    /// <summary>
    /// This class specifies the database properties to be used in the database layer (DBAccess).
    /// </summary>
    public class DBContext
    {
        // Set default values based on defined properties in appsettings.json:
        private string _connection = "DBSettings:ConnectionString";
        private string _table = "DBSettings:Table";

        public string Connection
        {
            get => _connection;
            set => _connection = value;
        }

        public string Table
        {
            get => _table;
            set => _table = value;
        }

        /// <summary>
        /// This function forwards user data to be updated in the database via the database layer.
        /// </summary>
        /// <param name="_iconfig">Configuration dependency to attach db connection.</param>
        /// <param name="item">User object.</param>
        public void SaveUser(IConfiguration _iconfig, User item)
        {
            new DBConnect(_iconfig).SaveUser(item);
        }

        /// <summary>
        /// This function forwards a request to the database layer to get user details based on a user email.
        /// </summary>
        /// <param name="_iconfig">Configuration dependency to attach db connection.</param>
        /// <param name="userEmail">User email as input.</param>
        /// <returns>User object.</returns>
        public User GetUser(IConfiguration _iconfig, string userEmail)
        {
            User item = new DBConnect(_iconfig).GetUser(userEmail);
            return item;
        }

    } // End class.
} // End namespace.
