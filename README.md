# Table of contents
<details>
   <summary>Click here to expand content list.</summary>
   
1. [General information](#1-general-information)
2. [License](#2-license)
3. [System description](#3-system-description)
4. [System requirements](#4-system-requirements)
5. [Supported features](#5-supported-features)
6. [Sequence diagram](#6-sequence-diagram)
7. [User interface](#7-user-interface)
8. [Setup guide](#8-setup-guide)
    * [8.1 Prerequisites](#81-prerequisites)
    * [8.2 Prepare the database and its tables](#82-prepare-the-database-and-its-tables)
    * [8.3 Configure the applications](#83-configure-the-applications)
    * [8.4 Install NuGet packages](#84-install-nuget-packages)
    * [8.5 Run and test the applications](#85-run-and-test-the-applications)
9. [REST API documentation](#9-rest-api-documentation)
10. [Contact details](#10-contact-details)
</details>

---

# 1 General information
“REST API JWT Auth 1.0” was created in Visual Studio Community by Annice Strömberg, 2020, with Annice.se as the primary download location. The script is a small integration system that enables data exchange between two independent ASP.NET Core web applications using REST APIs with JSON web token (JWT).

---

# 2 License
Released under the MIT license.

MIT: [http://rem.mit-license.org](http://rem.mit-license.org/), see [LICENSE](LICENSE).

---

# 3 System description
“REST API JWT Auth 1.0” is built in CSS3, HTML5, JavaScript, C# with ASP.NET Core 3.1, and Transact SQL using SQL Server as a database management system (DBMS).

Furthermore, the application 1 – which is the only app having a GUI – is built according the model-view-controller (MVC) pattern. In turn, the application 2 is built with a model, controller and database layer.

---

# 4 System requirements
The script can be run on servers that support C# 8.0 with ASP.NET Core 3.1 with the .NET Core 3.1 platform installed, along with an SQL Server supported database.

---

# 5 Supported features
The following functions and features are supported by this script:
  * Login system based on sessions.
  * User password encryption (HMAC-SHA256) based on ASP.NET Core Identity.
  * Protection against SQL injections.
  * Protection against cross-site forgery.
  * Write and edit permission of user details for the admin user.
  * Database storage of user details.
  * Responsive design.
  * Client and server side validation.

---
  
# 6 Sequence diagram
This section illustrates the context flow of the data exchange between application 1 and application 2 when a user logs in to be able to update user details.

<img src="https://diagrams.annice.se/c-sharp-rest-api-jwt-auth-1.0/seq-diagram.png" alt="" width="800">
  
---

# 7 User interface
Screenshot of the update page in desktop vs. responsive view.

<img src="https://diagrams.annice.se/c-sharp-rest-api-jwt-auth-1.0/gui-update-desktop.png" alt="" width="450"> <img src="https://diagrams.annice.se/c-sharp-rest-api-jwt-auth-1.0/gui-update-responsive.png" alt="" width="200">

---

# 8 Setup guide
As this script was created in Visual Studio Community with SQL Server, I will go through the necessary setup steps accordingly (all softwares used for this application setup are free).

## 8.1 Prerequisites
  * [Install SQL Server Express](https://www.microsoft.com/sv-se/sql-server/sql-server-downloads)
  * [Install SQL Server Management Studio (SSMS)](https://docs.microsoft.com/sv-se/sql/ssms/download-sql-server-management-studio-ssms?view=sql-server-ver15)
  * [Install .NET Core 3.1 (SDK)](https://dotnet.microsoft.com/download)
  * [Install Visual Studio Community](https://visualstudio.microsoft.com/vs/community/)
  
## 8.2 Prepare the database and its tables
1. [Create a database in SQL Server](https://docs.microsoft.com/en-us/sql/relational-databases/databases/create-a-database?view=sql-server-2017#SSMSProcedure).
2. [Create a user login in SQL Server](https://docs.microsoft.com/en-us/sql/relational-databases/databases/create-a-database?view=sql-server-2017#SSMSProcedure).
3. Launch SQL Server Management Studio (SSMS).
4. Navigate to the unzipped script folder path: REST API JWT Auth 1.0 > SQL.
In the SQL folder, open the file “sql_application_2.sql” in SSMS and change the commented values below to match your database name and user details (Note! The default password is set to “admin”, but can be changed after your first login):

```sql
USE YourDatabaseName -- Change to your DB name.
GO 

CREATE TABLE a_user 
  ( 
     id        INT PRIMARY KEY IDENTITY(1, 1), 
     firstname VARCHAR(256), 
     lastname  VARCHAR(256), 
     email     VARCHAR(256) NOT NULL UNIQUE, 
     password  VARCHAR(500) NOT NULL 
  );

INSERT a_user 
VALUES('YourFirstName', -- Optional
       'YourLastName',  -- Optional
       'your@email.com', -- Change to your email.
-- Leave the password below until after your first login. Default is set to "admin":
'AQAAAAEAACcQAAAAEBehHmgEHZmjXlTBGlKSW9KVuxMIHp1f4r8sC502SFQkGGxiYeef6HFntNMCMdZ76w==');
```

5. Execute the SQL script in SQL Server to create the App2 database table named “a_user” with a default user.

## 8.3 Configure the applications
6. Launch Visual Studio on your computer, e.g. via Windows start and then browse for Visual Studio.
7. In Visual Studio, select to open application 1 via the unzipped script folder path: *REST API JWT Auth 1.0 > Application 1 > MVCApp1.sln*
8. When you have opened the MVCApp1 solution in Visual Studio, you can choose to change the API client name in the “appsettings.json” file:

```json
{
  "APIClients": {
    "Application2": "App2", // Name the client we want to talk to from application 1.
"App2BaseURL": "https://localhost:44304/" // Set the base URL to the client we want to call.
  }
}

```

9. Save (CTRL+S) the “appsettings.json” file if it is changed.
10. Keep the MVCApp1 solution open in Visual Studio, and then select to launch another instance of Visual Studio (repeat step 6).
11. In the second instance of Visual Studio, select to open application 2 via the unzipped script folder path: 
*REST API JWT Auth 1.0 > Application 2 > App2.sln*

12.	Once the App2 solution is open in Visual Studio, select to open its “appsettings.json” file and change the commented values below to suit your own credentials:

```json
{
  "DBSettings": 
  {
    "ConnectionString": "Data Source=.\\SQLEXPRESS;initial catalog=YourDatabaseName;user id=YourDatabaseUser;password=YourDatabasePassword;MultipleActiveResultSets=True",
    "Table": "a_user"
  },

  "APIClients": 
  {
    "Application1": "MVCApp1", // Name the client we want to talk to from application 2.
"App1BaseURL": "https://localhost:44370/" // Set the base URL to the client we want to call.
  },

  "Jwt": 
  {
    "Key": "abc123^&%&^&%321", // Name the key with a minimum length of 16 characters.
    "Issuer": "Application2.com",
    "Audience": "Application1.com"
  }
}

```

13.	Save the “appsettings.json” file.

## 8.4 Install NuGet packages
14. Make sure you have the following NuGet packages installed for the solution, otherwise [install](https://docs.microsoft.com/en-us/nuget/consume-packages/install-use-packages-powershell) them:

**MVCApp1:**
  
    * Microsoft.Extensions.Http.Polly (3.1.3)
    * Microsoft.Extensions.Logging.Debug (3.1.3)
    * Microsoft.VisualStudio.Web.CodeGeneration.Design (3.0.0)
    * Newtonsoft.Json (12.0.3)   
 
**App2:**
  
    * Microsoft.AspNetCore.Authentication.JwtBearer (3.1.3)
    * Microsoft.Extensions.Http.Polly (3.1.3)
    * Microsoft.Extensions.Identity.Core (3.1.3)
    * Microsoft.Extensions.Logging.Debug (3.1.3)
    * Newtonsoft.Json (12.0.3)
    * System.Data.SqlClient (4.8.1)
    
## 8.5 Run and test the applications
15. Make sure you have two instances open of Visual Studio – one for the MVCApp1 solution, and the other one for the App2 solution.
16. Select to run both applications – each from its own instance.
17. On your first login, use the password “**admin**” along with the user email you specified when you executed the SQL code (see step 4).

---

# 9 REST API documentation
A quick way to test the APIs is through the Postman client which can be downloaded from the following link: [https://www.getpostman.com/downloads/](https://www.getpostman.com/downloads/)

With Postman you can test different APIs to check whether they work as expected without having to use your actual application GUI. Since this script is based on REST APIs, this can be done by sending JSON objects to different endpoints supported by this script.

Once you have setup the script and launched application 2 (from where user details are fetched) as well as launched Postman, you can quickly test different requests to the App2 API by entering a request URL (endpoint), attaching a JSON object/message, and then execute the message to the endpoint.

In Postman, this can be done by creating a request, and then putting the JSON message under the “Body” tab with the “raw” option selected as in the screenshot below. After this, you just click the send button in Postman to receive the response message from the API we just called.

Screenshot of an API request with a received JSON web token from application 2 based on a successful login post request:

<img src="https://diagrams.annice.se/c-sharp-rest-api-jwt-auth-1.0/postman.png" alt="" width="700">

Given that you have launched application 2, the following API URLs (changed to your server paths and port numbers) can be used in Postman to post requests to the App2 endpoints based on the following JSON object structures:

<img src="https://diagrams.annice.se/c-sharp-rest-api-jwt-auth-1.0/jsonobjects.png" alt="" width="700">

---

# 10 Contact details
For general feedback related to this script, such as any discovered bugs etc., you can contact me via the following email address: [info@annice.se](mailto:info@annice.se)
