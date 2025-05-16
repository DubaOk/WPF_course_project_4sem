USE master
GO
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'CoachSearchDB')
CREATE DATABASE [CoachSearchDB]
GO


USE CoachSearchDB
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(50) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    Password NVARCHAR(100) NOT NULL
)
GO

USE CoachSearchDB
GO

ALTER TABLE Users
ADD UserType NVARCHAR(20) NOT NULL DEFAULT 'Client'
GO

SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Users'


USE CoachSearchDB;
GO

CREATE TABLE Tutors (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL, -- Связь с таблицей Users
    Name NVARCHAR(100) NOT NULL,
    PhotoPath NVARCHAR(255), -- Путь к файлу фото
    Subject NVARCHAR(50),
    Rating FLOAT,
    Reviews NVARCHAR(MAX), -- Храним отзывы в формате JSON
    PricePerHour DECIMAL(10, 2),
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);
GO

USE CoachSearchDB;
GO

ALTER TABLE Users
ADD Email NVARCHAR(100) NOT NULL;

ALTER TABLE Users
ADD CONSTRAINT UK_Users_Email UNIQUE (Email);
GO