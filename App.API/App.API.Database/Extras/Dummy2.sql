-- 1. Images
INSERT INTO Images (ImageID, EntityType, ImageName, ImageLocation, ImageCaption) VALUES
(2000, 'Brand', 'brand1.png', '/images/brands/', 'Brand 1 Logo'),
(2001, 'ThemePark', 'themepark1.png', '/images/themeparks/', 'ThemePark 1 Logo'),
(2002, 'Park', 'park1.png', '/images/parks/', 'Park 1 Image'),
(2003, 'Store', 'store1.png', '/images/stores/', 'Store 1 Front'),
(2004, 'Receipt', 'receipt1.png', '/images/receipts/', 'Receipt Scan');

-- 2. Users
INSERT INTO Users (UserID, Username, UserPassword, Role) VALUES
(2000, 'admin', 'password123', 'Admin'),
(2001, 'user1', 'password123', 'User');

-- 3. ThemeParks
INSERT INTO ThemeParks (CompanyID, CompanyName, ImageID) VALUES
(2000, 'FunWorld', 2001);

-- 4. Locations
INSERT INTO Locations (LocationID, LocationName, LocationCity, LocationState, LocationCountry, LocationPostalCode) VALUES
(2000, 'Warehouse A', 'Springfield', 'IL', 'USA', '62704'),
(2001, 'Theme Park Central', 'Orlando', 'FL', 'USA', '32830');

-- 5. Parks
INSERT INTO Parks (ParkID, CompanyID, ParkName, LocationID, ImageID) VALUES
(2000, 2000, 'FunWorld Park', 2001, 2002);

-- 6. Stores
INSERT INTO Stores (StoreID, StoreName, ParkID, LocationID, ImageID) VALUES
(2000, 'Main Street Store', 2000, 2001, 2003);

-- 7. Receipts
INSERT INTO Receipts (ReceiptID, StoreID, PurchaseDate, TotalAmount, ImageID) VALUES
(2000, 2000, '2024-01-15', 49.99, 2004);

-- 8. Brands
INSERT INTO Brands (BrandID, BrandName, BrandType, BrandDescription, ImageID) VALUES
(2000, 'Lego', 'Toy', 'Building Blocks', 2000);

-- 9. SubBrands
INSERT INTO SubBrands (SubBrandID, BrandID) VALUES
(2000, 2000);

-- 10. Categories
INSERT INTO Categories (CategoryID, CategoryName, CategoryDescription, CategoryType) VALUES
(2000, 'Toys', 'Toys and Games', 'Physical');

-- 11. Conditions
INSERT INTO Conditions (ConditionID, ConditionType) VALUES
(2000, 'New'),
(2001, 'Used');

-- 12. Tags
INSERT INTO Tags (TagID, TagName) VALUES
(2000, 'Collectible'),
(2001, 'Rare');

-- 13. Items
INSERT INTO Items (ItemID, ItemName, BrandID, SubBrandID, CategoryID, ConditionID, ReceiptID, Price, Quantity, Boxed, Notes, ImageID) VALUES
(2000, 'Lego Star Wars Set', 2000, 2000, 2000, 2000, 2000, 49.99, 1, 1, 'Mint condition', 2000);

-- 14. ItemLocations
INSERT INTO ItemLocations (ItemLocationID, ItemID, LocationID, StoredDate, Notes) VALUES
(2000, 2000, 2000, '2024-02-01', 'Shelf 3B');

-- 15. ItemTags
INSERT INTO ItemTags (ItemID, TagID) VALUES
(2000, 2000),
(2000, 2001);

-- 16. LegoDetails
INSERT INTO LegoDetails (ItemID, LegoSetID, LegoDescription, LegoTheme, PieceCount) VALUES
(2000, '75257', 'Millennium Falcon', 'Star Wars', 1353);

-- 17. AuditLogs
INSERT INTO AuditLogs (AuditID, TableName, RecordID, ChangedByUserID, AuditType, ChangeSummary) VALUES
(2000, 'Items', 2000, 2000, 'INSERT', 'Initial creation of item');
