CREATE TABLE [dbo].[Locations] (
    [LocationID]           INT           NOT NULL,
    [LocationName]         VARCHAR (255) NOT NULL,
    [LocationAddressLine1] VARCHAR (255) NULL,
    [LocationAddressLine2] VARCHAR (255) NULL,
    [LocationAddressLine3] VARCHAR (255) NULL,
    [LocationCity]         VARCHAR (100) NULL,
    [LocationState]        VARCHAR (100) NULL,
    [LocationCountry]      VARCHAR (100) NULL,
    [LocationPostalCode]   VARCHAR (10)  NULL,
    [Notes]                TEXT          NULL,
    PRIMARY KEY CLUSTERED ([LocationID] ASC)
);

