-- Idempotent schema repair for authentication, MFA, beneficiaries, payments, dashboard counts, and audit logs.
-- Safe to run more than once.

IF OBJECT_ID('[tblCurrency]', 'U') IS NULL
BEGIN
    CREATE TABLE [tblCurrency] (
        [currency_Id] int NOT NULL IDENTITY(1,1),
        [currency_Name] nvarchar(100) NOT NULL,
        [currency_Code] nvarchar(10) NOT NULL,
        [exchange_Rate] decimal(18,4) NOT NULL,
        CONSTRAINT [PK_tblCurrency] PRIMARY KEY ([currency_Id])
    );
END

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = 'IX_tblCurrency_currency_Code'
      AND object_id = OBJECT_ID('tblCurrency')
)
BEGIN
    CREATE UNIQUE INDEX [IX_tblCurrency_currency_Code]
        ON [tblCurrency] ([currency_Code]);
END

MERGE [tblCurrency] AS target
USING (VALUES
    (N'South African Rand', N'ZAR', CAST(1.0000 AS decimal(18,4))),
    (N'US Dollar', N'USD', CAST(18.5000 AS decimal(18,4))),
    (N'Euro', N'EUR', CAST(20.0000 AS decimal(18,4))),
    (N'British Pound', N'GBP', CAST(23.5000 AS decimal(18,4))),
    (N'Japanese Yen', N'JPY', CAST(0.1200 AS decimal(18,4))),
    (N'Canadian Dollar', N'CAD', CAST(13.5000 AS decimal(18,4)))
) AS source ([currency_Name], [currency_Code], [exchange_Rate])
ON target.[currency_Code] = source.[currency_Code]
WHEN NOT MATCHED THEN
    INSERT ([currency_Name], [currency_Code], [exchange_Rate])
    VALUES (source.[currency_Name], source.[currency_Code], source.[exchange_Rate]);

IF COL_LENGTH('tblCustomer', 'currency_Id') IS NULL
BEGIN
    ALTER TABLE [tblCustomer] ADD [currency_Id] int NULL;
END

DECLARE @ZarCurrencyId int = (SELECT TOP 1 [currency_Id] FROM [tblCurrency] WHERE [currency_Code] = N'ZAR');

IF COL_LENGTH('tblCustomer', 'preferred_Currency') IS NOT NULL
BEGIN
    EXEC sp_executesql N'
        UPDATE c
        SET [currency_Id] = COALESCE(cur.[currency_Id], @FallbackCurrencyId)
        FROM [tblCustomer] c
        LEFT JOIN [tblCurrency] cur ON cur.[currency_Code] = c.[preferred_Currency]
        WHERE c.[currency_Id] IS NULL
           OR NOT EXISTS (
                SELECT 1
                FROM [tblCurrency] existingCurrency
                WHERE existingCurrency.[currency_Id] = c.[currency_Id]
           );
    ', N'@FallbackCurrencyId int', @FallbackCurrencyId = @ZarCurrencyId;
END
ELSE
BEGIN
    EXEC sp_executesql N'
        UPDATE [tblCustomer]
        SET [currency_Id] = @FallbackCurrencyId
        WHERE [currency_Id] IS NULL
           OR NOT EXISTS (
                SELECT 1
                FROM [tblCurrency] existingCurrency
                WHERE existingCurrency.[currency_Id] = [tblCustomer].[currency_Id]
           );
    ', N'@FallbackCurrencyId int', @FallbackCurrencyId = @ZarCurrencyId;
END

IF EXISTS (
    SELECT 1
    FROM sys.columns
    WHERE object_id = OBJECT_ID('tblCustomer')
      AND name = 'currency_Id'
      AND is_nullable = 1
)
BEGIN
    EXEC('ALTER TABLE [tblCustomer] ALTER COLUMN [currency_Id] int NOT NULL;');
END

IF NOT EXISTS (
    SELECT 1 FROM sys.foreign_keys
    WHERE name = 'FK_tblCustomer_tblCurrency_currency_Id'
      AND parent_object_id = OBJECT_ID('tblCustomer')
)
BEGIN
    EXEC('ALTER TABLE [tblCustomer]
        ADD CONSTRAINT [FK_tblCustomer_tblCurrency_currency_Id]
            FOREIGN KEY ([currency_Id]) REFERENCES [tblCurrency]([currency_Id]);');
END

IF COL_LENGTH('tblBeneficiary', 'currency_Id') IS NULL
BEGIN
    ALTER TABLE [tblBeneficiary] ADD [currency_Id] int NULL;
END

IF COL_LENGTH('tblBeneficiary', 'swift_Code') IS NULL
BEGIN
    ALTER TABLE [tblBeneficiary] ADD [swift_Code] nvarchar(20) NULL;
END

EXEC sp_executesql N'
    UPDATE b
    SET [currency_Id] = COALESCE(c.[currency_Id], @FallbackCurrencyId)
    FROM [tblBeneficiary] b
    LEFT JOIN [tblCustomer] c ON c.[customer_Id] = b.[customer_Id]
    WHERE b.[currency_Id] IS NULL
       OR NOT EXISTS (
            SELECT 1
            FROM [tblCurrency] existingCurrency
            WHERE existingCurrency.[currency_Id] = b.[currency_Id]
       );
', N'@FallbackCurrencyId int', @FallbackCurrencyId = @ZarCurrencyId;

IF EXISTS (
    SELECT 1
    FROM sys.columns
    WHERE object_id = OBJECT_ID('tblBeneficiary')
      AND name = 'currency_Id'
      AND is_nullable = 1
)
BEGIN
    EXEC('ALTER TABLE [tblBeneficiary] ALTER COLUMN [currency_Id] int NOT NULL;');
END

IF NOT EXISTS (
    SELECT 1 FROM sys.foreign_keys
    WHERE name = 'FK_tblBeneficiary_tblCurrency_currency_Id'
      AND parent_object_id = OBJECT_ID('tblBeneficiary')
)
BEGIN
    EXEC('ALTER TABLE [tblBeneficiary]
        ADD CONSTRAINT [FK_tblBeneficiary_tblCurrency_currency_Id]
            FOREIGN KEY ([currency_Id]) REFERENCES [tblCurrency]([currency_Id]);');
END

IF COL_LENGTH('tblCustomerSession', 'session_Token_Hash') IS NULL
BEGIN
    EXEC('ALTER TABLE [tblCustomerSession] ADD [session_Token_Hash] nvarchar(128) NULL;');
END

IF COL_LENGTH('tblCustomerSession', 'expires_On') IS NULL
BEGIN
    EXEC('ALTER TABLE [tblCustomerSession] ADD [expires_On] datetime2 NULL;');
END

EXEC('
UPDATE [tblCustomerSession]
SET
    [session_Token_Hash] = COALESCE([session_Token_Hash], CONCAT(''legacy-session-'', [session_Id])),
    [expires_On] = COALESCE([expires_On], DATEADD(day, -1, SYSUTCDATETIME())),
    [is_Active] = 0
WHERE [session_Token_Hash] IS NULL OR [expires_On] IS NULL;
');

IF EXISTS (
    SELECT 1
    FROM sys.columns
    WHERE object_id = OBJECT_ID('tblCustomerSession')
      AND name = 'session_Token_Hash'
      AND is_nullable = 1
)
BEGIN
    EXEC('ALTER TABLE [tblCustomerSession] ALTER COLUMN [session_Token_Hash] nvarchar(128) NOT NULL;');
END

IF EXISTS (
    SELECT 1
    FROM sys.columns
    WHERE object_id = OBJECT_ID('tblCustomerSession')
      AND name = 'expires_On'
      AND is_nullable = 1
)
BEGIN
    EXEC('ALTER TABLE [tblCustomerSession] ALTER COLUMN [expires_On] datetime2 NOT NULL;');
END

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_tblCustomerSession_session_Token_Hash'
      AND object_id = OBJECT_ID('tblCustomerSession')
)
BEGIN
    CREATE UNIQUE INDEX [IX_tblCustomerSession_session_Token_Hash]
        ON [tblCustomerSession] ([session_Token_Hash]);
END

IF OBJECT_ID('[tblMfaChallenge]', 'U') IS NULL
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
        CONSTRAINT [FK_tblMfaChallenge_tblCustomer_customer_Id] FOREIGN KEY ([customer_Id])
            REFERENCES [tblCustomer]([customer_Id]) ON DELETE CASCADE
    );

    CREATE INDEX [IX_tblMfaChallenge_customer_Id]
        ON [tblMfaChallenge] ([customer_Id]);
END

IF OBJECT_ID('[tblPayment]', 'U') IS NULL
BEGIN
    CREATE TABLE [tblPayment] (
        [payment_Id] int NOT NULL IDENTITY(1,1),
        [customer_Id] int NOT NULL,
        [beneficiary_Id] int NOT NULL,
        [from_Currency_Id] int NOT NULL,
        [to_Currency_Id] int NOT NULL,
        [amount] decimal(18,2) NOT NULL,
        [exchange_Rate_Used] decimal(18,4) NOT NULL,
        [converted_Amount] decimal(18,2) NOT NULL,
        [payment_Reference] nvarchar(100) NULL,
        [payment_Reason] nvarchar(255) NULL,
        [payment_Provider] nvarchar(50) NOT NULL CONSTRAINT [DF_tblPayment_payment_Provider] DEFAULT N'SWIFT',
        [swift_Code] nvarchar(20) NOT NULL,
        [status] nvarchar(30) NOT NULL CONSTRAINT [DF_tblPayment_status] DEFAULT N'Pending',
        [created_On] datetime2 NOT NULL CONSTRAINT [DF_tblPayment_created_On] DEFAULT SYSUTCDATETIME(),
        CONSTRAINT [PK_tblPayment] PRIMARY KEY ([payment_Id])
    );
END

IF COL_LENGTH('tblPayment', 'payment_Reference') IS NULL
    ALTER TABLE [tblPayment] ADD [payment_Reference] nvarchar(100) NULL;
IF COL_LENGTH('tblPayment', 'payment_Reason') IS NULL
    ALTER TABLE [tblPayment] ADD [payment_Reason] nvarchar(255) NULL;
IF COL_LENGTH('tblPayment', 'payment_Provider') IS NULL
    ALTER TABLE [tblPayment] ADD [payment_Provider] nvarchar(50) NOT NULL CONSTRAINT [DF_tblPayment_payment_Provider_added] DEFAULT N'SWIFT';
IF COL_LENGTH('tblPayment', 'swift_Code') IS NULL
    ALTER TABLE [tblPayment] ADD [swift_Code] nvarchar(20) NOT NULL CONSTRAINT [DF_tblPayment_swift_Code_added] DEFAULT N'UNKNOWN';
IF COL_LENGTH('tblPayment', 'status') IS NULL
    ALTER TABLE [tblPayment] ADD [status] nvarchar(30) NOT NULL CONSTRAINT [DF_tblPayment_status_added] DEFAULT N'Pending';
IF COL_LENGTH('tblPayment', 'created_On') IS NULL
    ALTER TABLE [tblPayment] ADD [created_On] datetime2 NOT NULL CONSTRAINT [DF_tblPayment_created_On_added] DEFAULT SYSUTCDATETIME();

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_tblPayment_customer_Id' AND object_id = OBJECT_ID('tblPayment'))
    CREATE INDEX [IX_tblPayment_customer_Id] ON [tblPayment] ([customer_Id]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_tblPayment_beneficiary_Id' AND object_id = OBJECT_ID('tblPayment'))
    CREATE INDEX [IX_tblPayment_beneficiary_Id] ON [tblPayment] ([beneficiary_Id]);

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_tblPayment_tblCustomer_customer_Id' AND parent_object_id = OBJECT_ID('tblPayment'))
    ALTER TABLE [tblPayment] ADD CONSTRAINT [FK_tblPayment_tblCustomer_customer_Id] FOREIGN KEY ([customer_Id]) REFERENCES [tblCustomer]([customer_Id]);
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_tblPayment_tblBeneficiary_beneficiary_Id' AND parent_object_id = OBJECT_ID('tblPayment'))
    ALTER TABLE [tblPayment] ADD CONSTRAINT [FK_tblPayment_tblBeneficiary_beneficiary_Id] FOREIGN KEY ([beneficiary_Id]) REFERENCES [tblBeneficiary]([beneficiary_Id]);
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_tblPayment_tblCurrency_from_Currency_Id' AND parent_object_id = OBJECT_ID('tblPayment'))
    ALTER TABLE [tblPayment] ADD CONSTRAINT [FK_tblPayment_tblCurrency_from_Currency_Id] FOREIGN KEY ([from_Currency_Id]) REFERENCES [tblCurrency]([currency_Id]);
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_tblPayment_tblCurrency_to_Currency_Id' AND parent_object_id = OBJECT_ID('tblPayment'))
    ALTER TABLE [tblPayment] ADD CONSTRAINT [FK_tblPayment_tblCurrency_to_Currency_Id] FOREIGN KEY ([to_Currency_Id]) REFERENCES [tblCurrency]([currency_Id]);

IF OBJECT_ID('[tblAuditLog]', 'U') IS NULL
BEGIN
    CREATE TABLE [tblAuditLog] (
        [audit_Id] int NOT NULL IDENTITY(1,1),
        [customer_Id] int NULL,
        [action_Type] nvarchar(100) NOT NULL,
        [table_Name] nvarchar(100) NULL,
        [record_Id] int NULL,
        [old_Value] nvarchar(max) NULL,
        [new_Value] nvarchar(max) NULL,
        [details] nvarchar(255) NULL,
        [created_On] datetime2 NOT NULL CONSTRAINT [DF_tblAuditLog_created_On] DEFAULT SYSUTCDATETIME(),
        CONSTRAINT [PK_tblAuditLog] PRIMARY KEY ([audit_Id])
    );
END

IF COL_LENGTH('tblAuditLog', 'customer_Id') IS NULL
    ALTER TABLE [tblAuditLog] ADD [customer_Id] int NULL;
IF COL_LENGTH('tblAuditLog', 'action_Type') IS NULL
    ALTER TABLE [tblAuditLog] ADD [action_Type] nvarchar(100) NOT NULL CONSTRAINT [DF_tblAuditLog_action_Type_added] DEFAULT N'UNKNOWN';
IF COL_LENGTH('tblAuditLog', 'table_Name') IS NULL
    ALTER TABLE [tblAuditLog] ADD [table_Name] nvarchar(100) NULL;
IF COL_LENGTH('tblAuditLog', 'record_Id') IS NULL
    ALTER TABLE [tblAuditLog] ADD [record_Id] int NULL;
IF COL_LENGTH('tblAuditLog', 'old_Value') IS NULL
    ALTER TABLE [tblAuditLog] ADD [old_Value] nvarchar(max) NULL;
IF COL_LENGTH('tblAuditLog', 'new_Value') IS NULL
    ALTER TABLE [tblAuditLog] ADD [new_Value] nvarchar(max) NULL;
IF COL_LENGTH('tblAuditLog', 'details') IS NULL
    ALTER TABLE [tblAuditLog] ADD [details] nvarchar(255) NULL;
IF COL_LENGTH('tblAuditLog', 'created_On') IS NULL
    ALTER TABLE [tblAuditLog] ADD [created_On] datetime2 NOT NULL CONSTRAINT [DF_tblAuditLog_created_On_added] DEFAULT SYSUTCDATETIME();

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_tblAuditLog_customer_Id' AND object_id = OBJECT_ID('tblAuditLog'))
    CREATE INDEX [IX_tblAuditLog_customer_Id] ON [tblAuditLog] ([customer_Id]);

IF NOT EXISTS (
    SELECT 1 FROM sys.foreign_keys
    WHERE name = 'FK_tblAuditLog_tblCustomer_customer_Id'
      AND parent_object_id = OBJECT_ID('tblAuditLog')
)
BEGIN
    ALTER TABLE [tblAuditLog]
    ADD CONSTRAINT [FK_tblAuditLog_tblCustomer_customer_Id]
        FOREIGN KEY ([customer_Id]) REFERENCES [tblCustomer]([customer_Id]);
END
