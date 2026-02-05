CREATE TABLE [dbo].[ErrorLog] (
    [ErrorLogID]   INT            IDENTITY (1, 1) NOT NULL,
    [ErrorMessage] NVARCHAR (MAX) NOT NULL,
    [ErrorTime]    DATETIME       NOT NULL,
    PRIMARY KEY CLUSTERED ([ErrorLogID] ASC)
);

