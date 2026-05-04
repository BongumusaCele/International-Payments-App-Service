-- Migration: InitialMigration
-- This script creates all tables needed for the International Payments API

-- Drop tables if they exist (in reverse dependency order to handle foreign keys)
IF OBJECT_ID('[tblBeneficiary]', 'U') IS NOT NULL 
    DROP TABLE [tblBeneficiary];

IF OBJECT_ID('[tblCustomerSession]', 'U') IS NOT NULL 
    DROP TABLE [tblCustomerSession];

IF OBJECT_ID('[tblCustomer]', 'U') IS NOT NULL 
    DROP TABLE [tblCustomer];

IF OBJECT_ID('[__EFMigrationsHistory]', 'U') IS NOT NULL 
    DROP TABLE [__EFMigrationsHistory];

-- Create tblCustomer table
CREATE TABLE [tblCustomer] (
    [customer_Id] int NOT NULL IDENTITY(1,1),
    [first_Name] nvarchar(150) NOT NULL,
    [last_Name] nvarchar(150) NOT NULL,
    [id_Number] nvarchar(13) NOT NULL,
    [email_Address] nvarchar(150) NULL,
    [account_Number] int NOT NULL,
    [preferred_Currency] nvarchar(50) NULL,
    [username] nvarchar(100) NOT NULL,
    [password_hash] nvarchar(max) NOT NULL,
    [created_On] datetime2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_tblCustomer] PRIMARY KEY ([customer_Id])
);

-- Create tblBeneficiary table
CREATE TABLE [tblBeneficiary] (
    [beneficiary_Id] int NOT NULL IDENTITY(1,1),
    [customer_Id] int NOT NULL,
    [beneficiary_Name] nvarchar(100) NOT NULL,
    [bank_Name] nvarchar(100) NOT NULL,
    [account_Number] nvarchar(20) NOT NULL,
    [swift_Code] nvarchar(20) NOT NULL,
    [country] nvarchar(150) NULL,
    CONSTRAINT [PK_tblBeneficiary] PRIMARY KEY ([beneficiary_Id]),
    CONSTRAINT [FK_tblBeneficiary_tblCustomer_customer_Id] FOREIGN KEY ([customer_Id]) REFERENCES [tblCustomer]([customer_Id]) ON DELETE CASCADE
);

-- Create tblCustomerSession table
CREATE TABLE [tblCustomerSession] (
    [session_Id] int NOT NULL IDENTITY(1,1),
    [customer_Id] int NOT NULL,
    [login_Time] datetime2 NOT NULL,
    [logout_Time] datetime2 NULL,
    [is_Active] bit NOT NULL,
    CONSTRAINT [PK_tblCustomerSession] PRIMARY KEY ([session_Id]),
    CONSTRAINT [FK_tblCustomerSession_tblCustomer_customer_Id] FOREIGN KEY ([customer_Id]) REFERENCES [tblCustomer]([customer_Id]) ON DELETE CASCADE
);

-- Create indexes
CREATE INDEX [IX_tblBeneficiary_customer_Id] ON [tblBeneficiary] ([customer_Id]);
CREATE INDEX [IX_tblCustomerSession_customer_Id] ON [tblCustomerSession] ([customer_Id]);

-- Create __EFMigrationsHistory table if it doesn't exist
IF OBJECT_ID('[__EFMigrationsHistory]', 'U') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END

-- Insert migration history (delete if exists first)
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260504113500_InitialMigration')
    INSERT INTO [__EFMigrationsHistory] VALUES (N'20260504113500_InitialMigration', N'10.0.7');
