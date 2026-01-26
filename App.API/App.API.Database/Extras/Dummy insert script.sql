-- Users
INSERT INTO [dbo].[Users] ([UserID], [Username], [UserPassword], [Role])
VALUES
(1, 'admin', 'password123', 'Admin'),
(2, 'johndoe', 'password456', 'User'),
(3, 'janedoe', 'password789', 'User');

-- Images
INSERT INTO [dbo].[Images] ([ImageID], [EntityType], [ImageName], [ImageLocation], [ImageCaption])
VALUES
(1, 'Brand', 'brand1.jpg', '/images/brands/', 'Brand 1 Logo'),
(2, 'Item', 'item1.jpg', '/images/items/', 'Item 1 Image'),
(3, 'Park', 'park1.jpg', '/images/parks/', 'Park 1 Image');

-- Brands
INSERT INTO [dbo].[Brands] ([BrandID], [BrandName], [BrandType], [BrandDescription], [ImageID])
VALUES
(1, 'Brand A', 'Electronics', 'High-quality electronics', 1),
(2, 'Brand B', 'Toys', 'Popular toy brand', NULL);

-- Categories
INSERT INTO [dbo].[Categories] ([CategoryID], [CategoryName], [CategoryDescription], [CategoryType])
VALUES
(1, 'Electronics', 'Electronic items', 'Gadgets'),
(2, 'Toys', 'Toy items', 'Entertainment');

-- Conditions
INSERT INTO [dbo].[Conditions] ([ConditionID], [ConditionType])
VALUES
(1, 'New'),
(2, 'Used');

-- Items
INSERT INTO [dbo].[Items] ([ItemID], [ItemName], [BrandID], [SubBrandID], [CategoryID], [ConditionID], [ReceiptID], [Price], [Quantity], [Boxed], [Notes], [ImageID])
VALUES
(1, 'Smartphone', 1, NULL, 1, 1, NULL, 699.99, 10, 1, 'Latest model', 2),
(3, 'Wireless Headphones', 1, NULL, 1, 1, 2, 199.99, 1, 1, 'Noise-canceling headphones', NULL),
(2, 'Lego Set', 2, NULL, 2, 1, NULL, 49.99, 5, 0, 'Classic set', NULL);

-- SubBrands
INSERT INTO [dbo].[SubBrands] ([SubBrandID], [BrandID])
VALUES
(1, 1),
(2, 2);

-- Locations
INSERT INTO [dbo].[Locations] ([LocationID], [LocationName], [LocationAddressLine1], [LocationCity], [LocationState], [LocationCountry], [LocationPostalCode], [Notes])
VALUES
(1, 'Warehouse A', '123 Main St', 'New York', 'NY', 'USA', '10001', 'Main warehouse'),
(2, 'Store B', '456 Elm St', 'Los Angeles', 'CA', 'USA', '90001', 'Retail store');

-- ItemLocations
INSERT INTO [dbo].[ItemLocations] ([ItemLocationID], [ItemID], [LocationID], [StoredDate], [Notes])
VALUES
(1, 1, 1, '2023-01-01', 'Stored in warehouse'),
(2, 2, 2, '2023-02-01', 'Displayed in store');

-- Tags
INSERT INTO [dbo].[Tags] ([TagID], [TagName])
VALUES
(1, 'Electronics'),
(2, 'Toys');

-- ItemTags
INSERT INTO [dbo].[ItemTags] ([ItemID], [TagID])
VALUES
(1, 1),
(2, 2);

-- LegoDetails
INSERT INTO [dbo].[LegoDetails] ([ItemID], [LegoSetID], [LegoDescription], [LegoTheme], [PieceCount])
VALUES
(2, 'LEGO123', 'Classic Lego set', 'Classic', 500);

-- ThemeParks
INSERT INTO [dbo].[ThemeParks] ([CompanyID], [CompanyName], [ImageID])
VALUES
(1, 'Theme Park Co.', 3);

-- Parks
INSERT INTO [dbo].[Parks] ([ParkID], [CompanyID], [ParkName], [LocationID], [ImageID])
VALUES
(1, 1, 'Adventure Park', 2, 3);

-- Stores
INSERT INTO [dbo].[Stores] ([StoreID], [StoreName], [ParkID], [LocationID], [ImageID])
VALUES
(1, 'Store A', 1, 2, NULL);

-- Receipts
INSERT INTO [dbo].[Receipts] ([ReceiptID], [StoreID], [PurchaseDate], [TotalAmount], [ImageID])
VALUES
(1, 1, '2023-03-01', 749.99, NULL),
(2, 1, '2023-04-01', 199.99, NULL);

-- AuditLogs
INSERT INTO [dbo].[AuditLogs] ([AuditID], [TableName], [RecordID], [ChangedByUserID], [ChangeDate], [AuditType], [ChangeSummary])
VALUES
(1, 'Items', 1, 1, DEFAULT, 'INSERT', 'Added new item: Smartphone'),
(2, 'Items', 2, 2, DEFAULT, 'UPDATE', 'Updated quantity for Lego Set');