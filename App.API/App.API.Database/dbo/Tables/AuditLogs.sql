CREATE TABLE [dbo].[AuditLogs] (
    [AuditID]         INT           IDENTITY (1, 1) NOT NULL,
    [TableName]       VARCHAR (100) NOT NULL,
    [RecordID]        INT           NOT NULL,
    [ChangedByUserID] INT           NULL,
    [ChangeDate]      ROWVERSION    NOT NULL,
    [AuditType]       VARCHAR (10)  NULL,
    [ChangeSummary]   TEXT          NULL,
    PRIMARY KEY CLUSTERED ([AuditID] ASC),
    CONSTRAINT [fk_auditlogs_user] FOREIGN KEY ([ChangedByUserID]) REFERENCES [dbo].[Users] ([UserID])
);

