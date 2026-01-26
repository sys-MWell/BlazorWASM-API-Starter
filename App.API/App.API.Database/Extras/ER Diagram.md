erDiagram
    Users {
        INT UserID PK
        VARCHAR Username
        VARCHAR UserPassword
        VARCHAR Role
    }

    Coampany {
        INT CompanyID PK
        VARCHAR CompanyName
        INT ImageID FK
    }

    Tags {
        INT TagID PK
        VARCHAR TagName
    }

    SubBrands {
        INT SubBrandID PK
        INT BrandID FK
    }

    Stores {
        INT StoreID PK
        VARCHAR StoreName
        INT ParkID FK
        INT LocationID FK
        INT ImageID FK
    }

    Receipts {
        INT ReceiptID PK
        INT StoreID FK
        DATE PurchaseDate
        DECIMAL TotalAmount
        INT ImageID FK
    }

    Parks {
        INT ParkID PK
        INT CompanyID FK
        VARCHAR ParkName
        INT LocationID FK
        INT ImageID FK
    }

    Locations {
        INT LocationID PK
        VARCHAR LocationName
        VARCHAR LocationAddressLine1
        VARCHAR LocationAddressLine2
        VARCHAR LocationAddressLine3
        VARCHAR LocationCity
        VARCHAR LocationState
        VARCHAR LocationCountry
        VARCHAR LocationPostalCode
        TEXT Notes
    }

    LegoDetails {
        INT ItemID PK
        VARCHAR LegoSetID
        TEXT LegoDescription
        VARCHAR LegoTheme
        INT PieceCount
    }

    ItemTypes {
        INT ItemTypeID PK
        VARCHAR TypeName
        VARCHAR TypeDescription
    }

    ItemTags {
        INT ItemID PK
        INT TagID PK
    }

    Items {
        INT ItemID PK
        VARCHAR ItemName
        INT BrandID FK
        INT SubBrandID FK
        INT CategoryID FK
        INT ConditionID FK
        INT ReceiptID FK
        DECIMAL Price
        INT Quantity
        BIT Boxed
        NVARCHAR Notes
        INT ImageID FK
        INT ItemTypeID FK
    }

    ItemLocations {
        INT ItemLocationID PK
        INT ItemID FK
        INT LocationID FK
        DATE StoredDate
        TEXT Notes
    }

    Images {
        INT ImageID PK
        VARCHAR EntityType
        VARCHAR ImageName
        VARCHAR ImageLocation
        TEXT ImageCaption
    }

    ErrorLog {
        INT ErrorLogID PK
        NVARCHAR ErrorMessage
        DATETIME ErrorTime
    }

    Conditions {
        INT ConditionID PK
        VARCHAR ConditionType
    }

    Categories {
        INT CategoryID PK
        VARCHAR CategoryName
        VARCHAR CategoryDescription
        VARCHAR CategoryType
    }

    Brands {
        INT BrandID PK
        VARCHAR BrandName
        VARCHAR BrandType
        VARCHAR BrandDescription
        INT ImageID FK
    }

    AuditLogs {
        INT AuditID PK
        VARCHAR TableName
        INT RecordID
        INT ChangedByUserID FK
        ROWVERSION ChangeDate
        VARCHAR AuditType
        TEXT ChangeSummary
    }

    BookDetails {
        INT ItemID PK
        VARCHAR Author
        VARCHAR ISBN
        VARCHAR Publisher
        INT PageCount
        TEXT Summary
    }

    %% Relationships
    Users ||--o{ AuditLogs : "ChangedByUserID"
    Coampany ||--o{ Parks : "CompanyID"
    Images ||--o{ Coampany : "ImageID"
    Images ||--o{ Stores : "ImageID"
    Images ||--o{ Receipts : "ImageID"
    Images ||--o{ Parks : "ImageID"
    Images ||--o{ Brands : "ImageID"
    Images ||--o{ Items : "ImageID"
    Items ||--o{ LegoDetails : "ItemID"
    Items ||--o{ BookDetails : "ItemID"
    Items ||--o{ ItemTags : "ItemID"
    Items ||--o{ ItemLocations : "ItemID"
    Items ||--o{ ItemTypes : "ItemTypeID"
    Items ||--o{ Brands : "BrandID"
    Items ||--o{ SubBrands : "SubBrandID"
    Items ||--o{ Categories : "CategoryID"
    Items ||--o{ Conditions : "ConditionID"
    Items ||--o{ Receipts : "ReceiptID"
    SubBrands ||--o{ Brands : "BrandID"
    Stores ||--o{ Parks : "ParkID"
    Stores ||--o{ Locations : "LocationID"
    Receipts ||--o{ Stores : "StoreID"
    Parks ||--o{ Locations : "LocationID"
    ItemTags ||--o{ Tags : "TagID"
    ItemLocations ||--o{ Locations : "LocationID"