CREATE TABLE [dbo].[Images] (
    [ImageID]       INT           NOT NULL,
    [EntityType]    VARCHAR (50)  NULL,
    [ImageName]     VARCHAR (255) NULL,
    [ImageLocation] VARCHAR (255) NULL,
    [ImageCaption]  TEXT          NULL,
    PRIMARY KEY CLUSTERED ([ImageID] ASC)
);

