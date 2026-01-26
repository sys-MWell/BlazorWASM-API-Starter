CREATE TABLE [dbo].[Brands] (
    [BrandID]          INT           NOT NULL,
    [BrandName]        VARCHAR (100) NOT NULL,
    [BrandType]        VARCHAR (100) NULL,
    [BrandDescription] VARCHAR (255) NULL,
    [ImageID]          INT           NULL,
    PRIMARY KEY CLUSTERED ([BrandID] ASC),
    CONSTRAINT [fk_brands_imageid] FOREIGN KEY ([ImageID]) REFERENCES [dbo].[Images] ([ImageID])
);

