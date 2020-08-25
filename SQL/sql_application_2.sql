/*
Change the database name and execute the SQL code below in SQL Server to
create the user DB table for application 2 with a default admin user.
The default password is set to "admin", but you can change this after your
first login.
*/

USE YourDatabaseName
GO

CREATE TABLE a_user (
	ID Int Primary Key Identity(1,1),
	Firstname Varchar(256),
	Lastname Varchar(256),
	Email Varchar(256) NOT NULL Unique,
	Password Varchar(500) NOT NULL
);

INSERT a_user 
VALUES('YourFirstName', -- Optional
       'YourLastName',  -- Optional
       'your@email.com', 
'AQAAAAEAACcQAAAAEBehHmgEHZmjXlTBGlKSW9KVuxMIHp1f4r8sC502SFQkGGxiYeef6HFntNMCMdZ76w==');