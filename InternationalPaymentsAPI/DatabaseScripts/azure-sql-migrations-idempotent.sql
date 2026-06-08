IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606115704_InitialCreate'
)
BEGIN
    CREATE TABLE [tblCustomer] (
        [customer_Id] int NOT NULL IDENTITY,
        [first_Name] nvarchar(150) NOT NULL,
        [last_Name] nvarchar(150) NOT NULL,
        [id_Number] nvarchar(13) NOT NULL,
        [email_Address] nvarchar(150) NULL,
        [account_Number] int NOT NULL,
        [preferred_Currency] nvarchar(50) NULL,
        [username] nvarchar(100) NOT NULL,
        [password_hash] nvarchar(max) NOT NULL,
        [created_On] datetime2 NOT NULL DEFAULT (GETDATE()),
        CONSTRAINT [PK_tblCustomer] PRIMARY KEY ([customer_Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606115704_InitialCreate'
)
BEGIN
    CREATE TABLE [tblEmployee] (
        [employee_Id] int NOT NULL IDENTITY,
        [username] nvarchar(100) NOT NULL,
        [password_Hash] nvarchar(max) NOT NULL,
        [full_Name] nvarchar(150) NOT NULL,
        [is_Active] bit NOT NULL,
        CONSTRAINT [PK_tblEmployee] PRIMARY KEY ([employee_Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606115704_InitialCreate'
)
BEGIN
    CREATE TABLE [tblBeneficiary] (
        [beneficiary_Id] int NOT NULL IDENTITY,
        [customer_Id] int NOT NULL,
        [beneficiary_Name] nvarchar(100) NOT NULL,
        [bank_Name] nvarchar(100) NOT NULL,
        [account_Number] nvarchar(20) NOT NULL,
        [swift_Code] nvarchar(20) NOT NULL,
        [country] nvarchar(150) NULL,
        CONSTRAINT [PK_tblBeneficiary] PRIMARY KEY ([beneficiary_Id]),
        CONSTRAINT [FK_tblBeneficiary_tblCustomer_customer_Id] FOREIGN KEY ([customer_Id]) REFERENCES [tblCustomer] ([customer_Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606115704_InitialCreate'
)
BEGIN
    CREATE TABLE [tblCustomerSession] (
        [session_Id] int NOT NULL IDENTITY,
        [customer_Id] int NOT NULL,
        [login_Time] datetime2 NOT NULL,
        [logout_Time] datetime2 NULL,
        [is_Active] bit NOT NULL,
        [session_Token_Hash] nvarchar(128) NOT NULL,
        [expires_On] datetime2 NOT NULL,
        CONSTRAINT [PK_tblCustomerSession] PRIMARY KEY ([session_Id]),
        CONSTRAINT [FK_tblCustomerSession_tblCustomer_customer_Id] FOREIGN KEY ([customer_Id]) REFERENCES [tblCustomer] ([customer_Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606115704_InitialCreate'
)
BEGIN
    CREATE TABLE [tblMfaChallenge] (
        [mfa_Challenge_Id] uniqueidentifier NOT NULL,
        [customer_Id] int NOT NULL,
        [otp_Code_Hash] nvarchar(128) NOT NULL,
        [created_On] datetime2 NOT NULL,
        [expires_On] datetime2 NOT NULL,
        [consumed_On] datetime2 NULL,
        [attempt_Count] int NOT NULL,
        CONSTRAINT [PK_tblMfaChallenge] PRIMARY KEY ([mfa_Challenge_Id]),
        CONSTRAINT [FK_tblMfaChallenge_tblCustomer_customer_Id] FOREIGN KEY ([customer_Id]) REFERENCES [tblCustomer] ([customer_Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606115704_InitialCreate'
)
BEGIN
    CREATE TABLE [tblPayment] (
        [payment_Id] int NOT NULL IDENTITY,
        [customer_Id] int NOT NULL,
        [beneficiary_Id] int NOT NULL,
        [amount] decimal(18,2) NOT NULL,
        [currency] nvarchar(3) NOT NULL,
        [provider] nvarchar(50) NOT NULL,
        [swift_Code] nvarchar(20) NOT NULL,
        [status] nvarchar(30) NOT NULL,
        [created_On] datetime2 NOT NULL,
        [verified_On] datetime2 NULL,
        [verified_By_Employee_Id] int NULL,
        [submitted_To_Swift_On] datetime2 NULL,
        CONSTRAINT [PK_tblPayment] PRIMARY KEY ([payment_Id]),
        CONSTRAINT [FK_tblPayment_tblBeneficiary_beneficiary_Id] FOREIGN KEY ([beneficiary_Id]) REFERENCES [tblBeneficiary] ([beneficiary_Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_tblPayment_tblCustomer_customer_Id] FOREIGN KEY ([customer_Id]) REFERENCES [tblCustomer] ([customer_Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_tblPayment_tblEmployee_verified_By_Employee_Id] FOREIGN KEY ([verified_By_Employee_Id]) REFERENCES [tblEmployee] ([employee_Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606115704_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_tblBeneficiary_customer_Id] ON [tblBeneficiary] ([customer_Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606115704_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_tblCustomerSession_customer_Id] ON [tblCustomerSession] ([customer_Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606115704_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_tblCustomerSession_session_Token_Hash] ON [tblCustomerSession] ([session_Token_Hash]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606115704_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_tblEmployee_username] ON [tblEmployee] ([username]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606115704_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_tblMfaChallenge_customer_Id] ON [tblMfaChallenge] ([customer_Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606115704_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_tblPayment_beneficiary_Id] ON [tblPayment] ([beneficiary_Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606115704_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_tblPayment_customer_Id] ON [tblPayment] ([customer_Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606115704_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_tblPayment_verified_By_Employee_Id] ON [tblPayment] ([verified_By_Employee_Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606115704_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260606115704_InitialCreate', N'9.0.9');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606124346_AddEmployeeSessions'
)
BEGIN
    CREATE TABLE [tblEmployeeSession] (
        [employee_Session_Id] int NOT NULL IDENTITY,
        [employee_Id] int NOT NULL,
        [login_Time] datetime2 NOT NULL,
        [logout_Time] datetime2 NULL,
        [is_Active] bit NOT NULL,
        [session_Token_Hash] nvarchar(128) NOT NULL,
        [expires_On] datetime2 NOT NULL,
        CONSTRAINT [PK_tblEmployeeSession] PRIMARY KEY ([employee_Session_Id]),
        CONSTRAINT [FK_tblEmployeeSession_tblEmployee_employee_Id] FOREIGN KEY ([employee_Id]) REFERENCES [tblEmployee] ([employee_Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606124346_AddEmployeeSessions'
)
BEGIN
    CREATE INDEX [IX_tblEmployeeSession_employee_Id] ON [tblEmployeeSession] ([employee_Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606124346_AddEmployeeSessions'
)
BEGIN
    CREATE UNIQUE INDEX [IX_tblEmployeeSession_session_Token_Hash] ON [tblEmployeeSession] ([session_Token_Hash]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606124346_AddEmployeeSessions'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260606124346_AddEmployeeSessions', N'9.0.9');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260607141440_AddPaymentRejectionFields'
)
BEGIN
    ALTER TABLE [tblPayment] ADD [rejected_By_Employee_Id] int NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260607141440_AddPaymentRejectionFields'
)
BEGIN
    ALTER TABLE [tblPayment] ADD [rejected_On] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260607141440_AddPaymentRejectionFields'
)
BEGIN
    ALTER TABLE [tblPayment] ADD [rejection_Reason] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260607141440_AddPaymentRejectionFields'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260607141440_AddPaymentRejectionFields', N'9.0.9');
END;

COMMIT;
GO

