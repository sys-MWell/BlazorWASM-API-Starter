CREATE TABLE [dbo].[BookDetails] (
    [ItemID]    INT           NOT NULL,
    [Author]    VARCHAR (255) NULL,
    [ISBN]      VARCHAR (20)  NULL,
    [Publisher] VARCHAR (255) NULL,
    [PageCount] INT           NULL,
    [Summary]   TEXT          NULL,
    PRIMARY KEY CLUSTERED ([ItemID] ASC),
    CHECK ([PageCount]>=(0)),
    CONSTRAINT [fk_bookdetails_itemid] FOREIGN KEY ([ItemID]) REFERENCES [dbo].[Items] ([ItemID])
);

