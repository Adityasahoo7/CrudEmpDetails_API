-- Run this AFTER the ARM deployment, connected to the CrudDemoPratice database
-- (Azure Portal > SQL database > Query editor, or SSMS / Azure Data Studio).
-- Note: "USE [db]" is not supported in Azure SQL Database, so it is removed.

IF OBJECT_ID(N'[dbo].[Employeesds]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Employeesds](
        [Id]          [int] IDENTITY(1,1) NOT NULL,
        [Name]        [nvarchar](max) NOT NULL,
        [Salary]      [int] NOT NULL,
        [Phone]       [nvarchar](max) NOT NULL,
        [Email]       [nvarchar](max) NOT NULL,
        [Age]         [int] NOT NULL,
        [Department]  [nvarchar](max) NOT NULL,
        [JoiningDate] [date] NOT NULL,
        CONSTRAINT [PK_Employeesds] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END
GO

IF OBJECT_ID(N'[dbo].[Users]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Users](
        [Id]       [uniqueidentifier] NOT NULL,
        [Username] [nvarchar](50)  NOT NULL,
        [Password] [nvarchar](100) NOT NULL,
        [Role]     [nvarchar](max) NOT NULL,
        CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END
GO
