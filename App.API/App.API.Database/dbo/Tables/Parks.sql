CREATE TABLE [dbo].[Parks] (
    [ParkID]     INT           NOT NULL,
    [CompanyID]  INT           NOT NULL,
    [ParkName]   VARCHAR (100) NOT NULL,
    [LocationID] INT           NULL,
    [ImageID]    INT           NULL,
    PRIMARY KEY CLUSTERED ([ParkID] ASC),
    CONSTRAINT [fk_parks_companyid] FOREIGN KEY ([CompanyID]) REFERENCES [dbo].[Company] ([CompanyID]),
    CONSTRAINT [fk_parks_imageid] FOREIGN KEY ([ImageID]) REFERENCES [dbo].[Images] ([ImageID]),
    CONSTRAINT [fk_parks_locationid] FOREIGN KEY ([LocationID]) REFERENCES [dbo].[Locations] ([LocationID])
);

