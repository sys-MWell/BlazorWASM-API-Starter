CREATE TABLE [dbo].[SubBrands] (
    [SubBrandID] INT NOT NULL,
    [BrandID]    INT NOT NULL,
    PRIMARY KEY CLUSTERED ([SubBrandID] ASC),
    CONSTRAINT [fk_subbrands_brandid] FOREIGN KEY ([BrandID]) REFERENCES [dbo].[Brands] ([BrandID])
);

