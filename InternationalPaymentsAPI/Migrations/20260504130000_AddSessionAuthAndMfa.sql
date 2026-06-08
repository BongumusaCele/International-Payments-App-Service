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
