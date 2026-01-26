CREATE TABLE [dbo].[Items] (
    [ItemID]      INT             IDENTITY (1, 1) NOT NULL,
    [ItemName]    VARCHAR (255)   NOT NULL,
    [BrandID]     INT             NULL,
    [SubBrandID]  INT             NULL,
    [CategoryID]  INT             NULL,
    [ConditionID] INT             NULL,
    [ReceiptID]   INT             NULL,
    [Price]       DECIMAL (10, 2) NULL,
    [Quantity]    INT             NULL,
    [Boxed]       BIT             CONSTRAINT [DF_Items_Boxed] DEFAULT ((0)) NULL,
    [Notes]       NVARCHAR (MAX)  NULL,
    [ImageID]     INT             NULL,
    [ItemTypeID]  INT             NULL,
    PRIMARY KEY CLUSTERED ([ItemID] ASC),
    CONSTRAINT [fk_items_brandid] FOREIGN KEY ([BrandID]) REFERENCES [dbo].[Brands] ([BrandID]),
    CONSTRAINT [fk_items_categoryid] FOREIGN KEY ([CategoryID]) REFERENCES [dbo].[Categories] ([CategoryID]),
    CONSTRAINT [fk_items_conditionid] FOREIGN KEY ([ConditionID]) REFERENCES [dbo].[Conditions] ([ConditionID]),
    CONSTRAINT [fk_items_imageid] FOREIGN KEY ([ImageID]) REFERENCES [dbo].[Images] ([ImageID]),
    CONSTRAINT [fk_items_itemtypeid] FOREIGN KEY ([ItemTypeID]) REFERENCES [dbo].[ItemTypes] ([ItemTypeID]),
    CONSTRAINT [fk_items_receiptid] FOREIGN KEY ([ReceiptID]) REFERENCES [dbo].[Receipts] ([ReceiptID]),
    CONSTRAINT [fk_items_subbrandid] FOREIGN KEY ([SubBrandID]) REFERENCES [dbo].[SubBrands] ([SubBrandID])
);


GO
CREATE NONCLUSTERED INDEX [IX_Items_CategoryID]
    ON [dbo].[Items]([CategoryID] ASC);

