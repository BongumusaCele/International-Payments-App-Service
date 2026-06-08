IF OBJECT_ID(N'[dbo].[tblEmployee]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[tblEmployee]
    (
        [employee_Id] [int] IDENTITY(1,1) NOT NULL,
        [username] [nvarchar](100) NOT NULL,
        [password_Hash] [nvarchar](max) NOT NULL,
        [full_Name] [nvarchar](150) NOT NULL,
        [is_Active] [bit] NOT NULL,
        CONSTRAINT [PK_tblEmployee] PRIMARY KEY CLUSTERED ([employee_Id] ASC)
    );
END
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE [name] = N'IX_tblEmployee_username'
      AND [object_id] = OBJECT_ID(N'[dbo].[tblEmployee]')
)
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX [IX_tblEmployee_username]
    ON [dbo].[tblEmployee] ([username] ASC);
END
GO

IF OBJECT_ID(N'[dbo].[tblEmployeeSession]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[tblEmployeeSession]
    (
        [employee_Session_Id] [int] IDENTITY(1,1) NOT NULL,
        [employee_Id] [int] NOT NULL,
        [login_Time] [datetime2](7) NOT NULL,
        [logout_Time] [datetime2](7) NULL,
        [is_Active] [bit] NOT NULL,
        [session_Token_Hash] [nvarchar](128) NOT NULL,
        [expires_On] [datetime2](7) NOT NULL,
        CONSTRAINT [PK_tblEmployeeSession] PRIMARY KEY CLUSTERED ([employee_Session_Id] ASC)
    );
END
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE [name] = N'IX_tblEmployeeSession_employee_Id'
      AND [object_id] = OBJECT_ID(N'[dbo].[tblEmployeeSession]')
)
BEGIN
    CREATE NONCLUSTERED INDEX [IX_tblEmployeeSession_employee_Id]
    ON [dbo].[tblEmployeeSession] ([employee_Id] ASC);
END
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE [name] = N'IX_tblEmployeeSession_session_Token_Hash'
      AND [object_id] = OBJECT_ID(N'[dbo].[tblEmployeeSession]')
)
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX [IX_tblEmployeeSession_session_Token_Hash]
    ON [dbo].[tblEmployeeSession] ([session_Token_Hash] ASC);
END
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.foreign_keys
    WHERE [name] = N'FK_tblEmployeeSession_tblEmployee_employee_Id'
)
BEGIN
    ALTER TABLE [dbo].[tblEmployeeSession] WITH CHECK
    ADD CONSTRAINT [FK_tblEmployeeSession_tblEmployee_employee_Id]
    FOREIGN KEY ([employee_Id])
    REFERENCES [dbo].[tblEmployee] ([employee_Id])
    ON DELETE NO ACTION;

    ALTER TABLE [dbo].[tblEmployeeSession]
    CHECK CONSTRAINT [FK_tblEmployeeSession_tblEmployee_employee_Id];
END
GO

IF COL_LENGTH(N'[dbo].[tblPayment]', N'verified_On') IS NULL
BEGIN
    ALTER TABLE [dbo].[tblPayment]
    ADD [verified_On] [datetime2](7) NULL;
END
GO

IF COL_LENGTH(N'[dbo].[tblPayment]', N'updated_On') IS NULL
BEGIN
    ALTER TABLE [dbo].[tblPayment]
    ADD [updated_On] [datetime2](7) NULL;
END
GO

IF COL_LENGTH(N'[dbo].[tblPayment]', N'verified_By_Employee_Id') IS NULL
BEGIN
    ALTER TABLE [dbo].[tblPayment]
    ADD [verified_By_Employee_Id] [int] NULL;
END
GO

IF COL_LENGTH(N'[dbo].[tblPayment]', N'from_Currency_Id') IS NULL
BEGIN
    ALTER TABLE [dbo].[tblPayment]
    ADD [from_Currency_Id] [int] NULL;
END
GO

IF COL_LENGTH(N'[dbo].[tblPayment]', N'to_Currency_Id') IS NULL
BEGIN
    ALTER TABLE [dbo].[tblPayment]
    ADD [to_Currency_Id] [int] NULL;
END
GO

IF COL_LENGTH(N'[dbo].[tblPayment]', N'exchange_Rate_Used') IS NULL
BEGIN
    ALTER TABLE [dbo].[tblPayment]
    ADD [exchange_Rate_Used] [decimal](18,4) NULL;
END
GO

IF COL_LENGTH(N'[dbo].[tblPayment]', N'converted_Amount') IS NULL
BEGIN
    ALTER TABLE [dbo].[tblPayment]
    ADD [converted_Amount] [decimal](18,2) NULL;
END
GO

IF COL_LENGTH(N'[dbo].[tblPayment]', N'payment_Reference') IS NULL
BEGIN
    ALTER TABLE [dbo].[tblPayment]
    ADD [payment_Reference] [nvarchar](max) NULL;
END
GO

IF COL_LENGTH(N'[dbo].[tblPayment]', N'payment_Reason') IS NULL
BEGIN
    ALTER TABLE [dbo].[tblPayment]
    ADD [payment_Reason] [nvarchar](max) NULL;
END
GO

IF COL_LENGTH(N'[dbo].[tblPayment]', N'payment_Provider') IS NULL
BEGIN
    ALTER TABLE [dbo].[tblPayment]
    ADD [payment_Provider] [nvarchar](50) NULL;
END
GO

IF COL_LENGTH(N'[dbo].[tblPayment]', N'submitted_To_Swift_On') IS NULL
BEGIN
    ALTER TABLE [dbo].[tblPayment]
    ADD [submitted_To_Swift_On] [datetime2](7) NULL;
END
GO

IF COL_LENGTH(N'[dbo].[tblPayment]', N'rejected_By_Employee_Id') IS NULL
BEGIN
    ALTER TABLE [dbo].[tblPayment]
    ADD [rejected_By_Employee_Id] [int] NULL;
END
GO

IF COL_LENGTH(N'[dbo].[tblPayment]', N'rejected_On') IS NULL
BEGIN
    ALTER TABLE [dbo].[tblPayment]
    ADD [rejected_On] [datetime2](7) NULL;
END
GO

IF COL_LENGTH(N'[dbo].[tblPayment]', N'rejection_Reason') IS NULL
BEGIN
    ALTER TABLE [dbo].[tblPayment]
    ADD [rejection_Reason] [nvarchar](max) NULL;
END
GO

DECLARE @defaultCurrencyId int;
DECLARE @targetCurrencyId int;

SELECT TOP (1) @defaultCurrencyId = [currency_Id]
FROM [dbo].[tblCurrency]
ORDER BY
    CASE WHEN [currency_Code] = N'ZAR' THEN 0 ELSE 1 END,
    [currency_Id];

SELECT TOP (1) @targetCurrencyId = [currency_Id]
FROM [dbo].[tblCurrency]
ORDER BY
    CASE WHEN [currency_Code] <> N'ZAR' THEN 0 ELSE 1 END,
    [currency_Id];

IF @targetCurrencyId IS NULL
BEGIN
    SET @targetCurrencyId = @defaultCurrencyId;
END

UPDATE [dbo].[tblPayment]
SET
    [from_Currency_Id] = COALESCE([from_Currency_Id], @defaultCurrencyId),
    [to_Currency_Id] = COALESCE([to_Currency_Id], @targetCurrencyId),
    [exchange_Rate_Used] = COALESCE([exchange_Rate_Used], 1),
    [converted_Amount] = COALESCE([converted_Amount], [amount]),
    [payment_Reference] = COALESCE([payment_Reference], CONCAT(N'PAY-', [payment_Id])),
    [updated_On] = COALESCE([updated_On], [created_On]);
GO

IF COL_LENGTH(N'[dbo].[tblPayment]', N'provider') IS NOT NULL
BEGIN
    EXEC sp_executesql N'
        UPDATE [dbo].[tblPayment]
        SET [payment_Provider] = COALESCE([payment_Provider], [provider], N''SWIFT'');';
END
ELSE
BEGIN
    UPDATE [dbo].[tblPayment]
    SET [payment_Provider] = COALESCE([payment_Provider], N'SWIFT');
END
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE [name] = N'IX_tblPayment_verified_By_Employee_Id'
      AND [object_id] = OBJECT_ID(N'[dbo].[tblPayment]')
)
BEGIN
    CREATE NONCLUSTERED INDEX [IX_tblPayment_verified_By_Employee_Id]
    ON [dbo].[tblPayment] ([verified_By_Employee_Id] ASC);
END
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.foreign_keys
    WHERE [name] = N'FK_tblPayment_tblEmployee_verified_By_Employee_Id'
)
BEGIN
    ALTER TABLE [dbo].[tblPayment] WITH CHECK
    ADD CONSTRAINT [FK_tblPayment_tblEmployee_verified_By_Employee_Id]
    FOREIGN KEY ([verified_By_Employee_Id])
    REFERENCES [dbo].[tblEmployee] ([employee_Id])
    ON DELETE NO ACTION;

    ALTER TABLE [dbo].[tblPayment]
    CHECK CONSTRAINT [FK_tblPayment_tblEmployee_verified_By_Employee_Id];
END
GO

MERGE [dbo].[tblEmployee] AS target
USING
(
    VALUES
        (N'employee1', N'PBKDF2$100000$bU2la46OPc8gFqXf1FFKLg==$TukXF+KQPLeCvmE9Qqwfu8C8P1L31ZxYDQy3K/maulo=', N'Andile Bolo', CAST(1 AS bit)),
        (N'employee2', N'PBKDF2$100000$bU2la46OPc8gFqXf1FFKLg==$TukXF+KQPLeCvmE9Qqwfu8C8P1L31ZxYDQy3K/maulo=', N'Themba Msomi', CAST(1 AS bit)),
        (N'employee3', N'PBKDF2$100000$bU2la46OPc8gFqXf1FFKLg==$TukXF+KQPLeCvmE9Qqwfu8C8P1L31ZxYDQy3K/maulo=', N'Thami Jantjie', CAST(1 AS bit))
) AS source ([username], [password_Hash], [full_Name], [is_Active])
ON target.[username] = source.[username]
WHEN MATCHED THEN
    UPDATE SET
        [password_Hash] = source.[password_Hash],
        [full_Name] = source.[full_Name],
        [is_Active] = source.[is_Active]
WHEN NOT MATCHED BY TARGET THEN
    INSERT ([username], [password_Hash], [full_Name], [is_Active])
    VALUES (source.[username], source.[password_Hash], source.[full_Name], source.[is_Active]);
GO
