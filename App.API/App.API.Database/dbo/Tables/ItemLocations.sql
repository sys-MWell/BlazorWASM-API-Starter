CREATE TABLE [dbo].[ItemLocations] (
    [ItemLocationID] INT  NOT NULL,
    [ItemID]         INT  NOT NULL,
    [LocationID]     INT  NOT NULL,
    [StoredDate]     DATE NULL,
    [Notes]          TEXT NULL,
    PRIMARY KEY CLUSTERED ([ItemLocationID] ASC),
    CONSTRAINT [fk_itemlocations_itemid] FOREIGN KEY ([ItemID]) REFERENCES [dbo].[Items] ([ItemID]),
    CONSTRAINT [fk_itemlocations_locationid] FOREIGN KEY ([LocationID]) REFERENCES [dbo].[Locations] ([LocationID])
);

