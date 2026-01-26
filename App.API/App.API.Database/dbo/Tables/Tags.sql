CREATE TABLE [dbo].[Tags] (
    [TagID]   INT           NOT NULL,
    [TagName] VARCHAR (100) NOT NULL,
    PRIMARY KEY CLUSTERED ([TagID] ASC),
    UNIQUE NONCLUSTERED ([TagName] ASC)
);

