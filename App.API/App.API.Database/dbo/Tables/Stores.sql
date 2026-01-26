CREATE TABLE [dbo].[Stores] (
    [StoreID]    INT           NOT NULL,
    [StoreName]  VARCHAR (100) NOT NULL,
    [ParkID]     INT           NULL,
    [LocationID] INT           NULL,
    [ImageID]    INT           NULL,
    PRIMARY KEY CLUSTERED ([StoreID] ASC),
    CONSTRAINT [fk_stores_imageid] FOREIGN KEY ([ImageID]) REFERENCES [dbo].[Images] ([ImageID]),
    CONSTRAINT [fk_stores_locationid] FOREIGN KEY ([LocationID]) REFERENCES [dbo].[Locations] ([LocationID]),
    CONSTRAINT [fk_stores_parkid] FOREIGN KEY ([ParkID]) REFERENCES [dbo].[Parks] ([ParkID])
);

