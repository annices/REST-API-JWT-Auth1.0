using System;
using Microsoft.Extensions.Configuration;
using App2.Models;
using System.Data.SqlClient;

/*
Copyright (c) 2020 Annice Strömberg – Annice.se

This script is MIT (Massachusetts Institute of Technology) licensed, which means that
permission is granted, free of charge, to any person obtaining a copy of this software
and associated documentation files to deal in the software without restriction. This
includes, without limitation, the rights to use, copy, modify, merge, publish, distribute,
sublicense, and/or sell copies of the software, and to permit persons to whom the software
is furnished to do so subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or
substantial portions of the software.
*/
namespace App2.DBAccess
{
    /// <summary>
    /// This class handles the interaction with the App2 database where the user details are stored.
    /// </summary>
    public class DBConnect
    {
        // Instantiate necessary configuration variables:
        private readonly IConfiguration _iconfig;
        private string Connect => _iconfig[new DBContext().Connection];
        private string Table => _iconfig[new DBContext().Table];

        /// <summary>
        /// This constructor injects a configuration dependency to reach the database connection.
        /// </summary>
        /// <param name="iconfig">Configuration variable.</param>
        public DBConnect(IConfiguration iconfig)
        {
            _iconfig = iconfig;
        }

        /// <summary>
        /// This function saves user details into the database.
        /// </summary>
        /// <param name="item">User object as input.</param>
        public void SaveUser(User item)
        {
            SqlConnection dbConnection = new SqlConnection(Connect);

            // Prepare sql query to be executed to db:
            string sql = "IF EXISTS(Select * From " + Table + " Where ID = @id) " +
                "BEGIN UPDATE " + Table + " " +
                       "SET Firstname = @firstname, Lastname = @lastname, Email = @email, " +
                       "Password = CASE WHEN(Password = @password Or @password = '') THEN Password ELSE @password END " +
                       "WHERE ID = @id END " +
                "ELSE INSERT INTO " + Table + " " +
                       "VALUES(@firstname, @lastname, @email, @password)";

            SqlCommand dbCommand = new SqlCommand(sql, dbConnection);

            // Handle fields on empty input (to prevent null conflicts):
            item.Firstname ??= "";
            item.Lastname ??= "";
            item.Password ??= "";

            // Use SqlParameters to prevent SQL injections:
            dbCommand.Parameters.Add(new SqlParameter("@id", item.ID));
            dbCommand.Parameters.Add(new SqlParameter("@firstname", item.Firstname));
            dbCommand.Parameters.Add(new SqlParameter("@lastname", item.Lastname));
            dbCommand.Parameters.Add(new SqlParameter("@email", item.Email));
            dbCommand.Parameters.Add(new SqlParameter("@password", item.Password));

            dbConnection.Open();
            dbCommand.ExecuteNonQuery();
            dbConnection.Close();
        }

        /// <summary>
        /// This function gets user details from the database based on a user email.
        /// </summary>
        /// <param name="userEmail">User email as input.</param>
        /// <returns>User object.</returns>
        public User GetUser(string userEmail)
        {
            SqlConnection dbConnection = new SqlConnection(Connect);

            // Prepare sql query to get the user from our db:
            string sql = "SELECT * FROM " + Table + " WHERE Email = @email";

            SqlCommand dbCommand = new SqlCommand(sql, dbConnection);

            // Use SqlParameter to prevent SQL injection:
            dbCommand.Parameters.Add(new SqlParameter("@email", userEmail));

            dbConnection.Open();
            SqlDataReader reader = dbCommand.ExecuteReader();

            User item = new User();
            // If we have a match, assign the data to an instance of our user class:
            if (reader.Read())
            {
                item.ID = Convert.ToInt32(reader["ID"]);
                item.Firstname = reader["Firstname"].ToString();
                item.Lastname = reader["Lastname"].ToString();
                item.Email = reader["Email"].ToString();
                item.Password = reader["Password"].ToString();
            }

            dbConnection.Close();
            return item;
        }

    } // End class.
} // End namespace.
