CREATE TABLE [dbo].[Receipts] (
    [ReceiptID]    INT             NOT NULL,
    [StoreID]      INT             NOT NULL,
    [PurchaseDate] DATE            NULL,
    [TotalAmount]  DECIMAL (10, 2) NULL,
    [ImageID]      INT             NULL,
    PRIMARY KEY CLUSTERED ([ReceiptID] ASC),
    CONSTRAINT [fk_receipts_imageid] FOREIGN KEY ([ImageID]) REFERENCES [dbo].[Images] ([ImageID]),
    CONSTRAINT [fk_receipts_storeid] FOREIGN KEY ([StoreID]) REFERENCES [dbo].[Stores] ([StoreID])
);

