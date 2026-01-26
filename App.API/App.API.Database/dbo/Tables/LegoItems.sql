CREATE TABLE [dbo].[LegoItems] (
    [ItemID]          INT            NOT NULL,
    [LegoSetID]       VARCHAR (50)   NULL,
    [LegoDescription] NVARCHAR (MAX) NULL,
    [LegoTheme]       VARCHAR (100)  NULL,
    [PieceCount]      INT            NULL,
    PRIMARY KEY CLUSTERED ([ItemID] ASC),
    CHECK ([PieceCount]>=(0)),
    CONSTRAINT [fk_legoitems_itemid] FOREIGN KEY ([ItemID]) REFERENCES [dbo].[Items] ([ItemID])
);

