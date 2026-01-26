
CREATE PROCEDURE [dbo].[usp_InsertItem] 
    @ItemName        NVARCHAR(255), 
    @BrandID         INT = NULL, 
    @SubBrandID      INT = NULL, 
    @CategoryID      INT = NULL, 
    @ConditionID     INT = NULL, 
    @ReceiptID       INT = NULL, 
    @Price           DECIMAL(10, 2) = NULL, 
    @Quantity        INT = NULL, 
    @Boxed           BIT = NULL, 
    @Notes           NVARCHAR(MAX) = NULL, 
    @ImageID         INT = NULL, 
    @ItemTypeID      INT = NULL, 
    @NewItemID       INT OUTPUT, 
    @ResponseMessage NVARCHAR(255) OUTPUT, 
    @ErrorCode       INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @ErrorLogMessage NVARCHAR(MAX) = NULL;

    -- Reset outputs
    SET @NewItemID       = NULL;
    SET @ResponseMessage = NULL;
    SET @ErrorCode       = NULL;

    -------------------------------------------------
    -- Basic foreign key existence checks (early return on failure)
    -------------------------------------------------
    IF @BrandID IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.Brands WHERE BrandID = @BrandID)
    BEGIN
        SET @ErrorLogMessage = N'Invalid BrandID';
        INSERT INTO dbo.ErrorLog (ErrorMessage, ErrorTime) VALUES (@ErrorLogMessage, SYSDATETIME());
        SET @ResponseMessage = @ErrorLogMessage;
        SET @ErrorCode       = 1;
        RETURN;
    END

    IF @SubBrandID IS NOT NULL AND NOT EXISTS (
        SELECT 1
        FROM dbo.SubBrands
        WHERE SubBrandID = @SubBrandID
          AND (@BrandID IS NULL OR BrandID = @BrandID)
    )
    BEGIN
        SET @ErrorLogMessage = N'Invalid SubBrandID or does not match BrandID';
        INSERT INTO dbo.ErrorLog (ErrorMessage, ErrorTime) VALUES (@ErrorLogMessage, SYSDATETIME());
        SET @ResponseMessage = @ErrorLogMessage;
        SET @ErrorCode       = 1;
        RETURN;
    END

    IF @CategoryID IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.Categories WHERE CategoryID = @CategoryID)
    BEGIN
        SET @ErrorLogMessage = N'Invalid CategoryID';
        INSERT INTO dbo.ErrorLog (ErrorMessage, ErrorTime) VALUES (@ErrorLogMessage, SYSDATETIME());
        SET @ResponseMessage = @ErrorLogMessage;
        SET @ErrorCode       = 1;
        RETURN;
    END

    IF @ConditionID IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.Conditions WHERE ConditionID = @ConditionID)
    BEGIN
        SET @ErrorLogMessage = N'Invalid ConditionID';
        INSERT INTO dbo.ErrorLog (ErrorMessage, ErrorTime) VALUES (@ErrorLogMessage, SYSDATETIME());
        SET @ResponseMessage = @ErrorLogMessage;
        SET @ErrorCode       = 1;
        RETURN;
    END

    IF @ReceiptID IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.Receipts WHERE ReceiptID = @ReceiptID)
    BEGIN
        SET @ErrorLogMessage = N'Invalid ReceiptID';
        INSERT INTO dbo.ErrorLog (ErrorMessage, ErrorTime) VALUES (@ErrorLogMessage, SYSDATETIME());
        SET @ResponseMessage = @ErrorLogMessage;
        SET @ErrorCode       = 1;
        RETURN;
    END

    IF @ImageID IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.Images WHERE ImageID = @ImageID)
    BEGIN
        SET @ErrorLogMessage = N'Invalid ImageID';
        INSERT INTO dbo.ErrorLog (ErrorMessage, ErrorTime) VALUES (@ErrorLogMessage, SYSDATETIME());
        SET @ResponseMessage = @ErrorLogMessage;
        SET @ErrorCode       = 1;
        RETURN;
    END

    IF @ItemTypeID IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.ItemTypes WHERE ItemTypeID = @ItemTypeID)
    BEGIN
        SET @ErrorLogMessage = N'Invalid ItemTypeID';
        INSERT INTO dbo.ErrorLog (ErrorMessage, ErrorTime) VALUES (@ErrorLogMessage, SYSDATETIME());
        SET @ResponseMessage = @ErrorLogMessage;
        SET @ErrorCode       = 1;
        RETURN;
    END

    -------------------------------------------------
    -- Insert (identity auto-generates ItemID)
    -------------------------------------------------
    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @Inserted TABLE (ItemID INT NOT NULL);

        INSERT INTO dbo.Items
        (
            ItemName, BrandID, SubBrandID, CategoryID, ConditionID,
            ReceiptID, Price, Quantity, Boxed, Notes, ImageID, ItemTypeID
        )
        OUTPUT INSERTED.ItemID INTO @Inserted(ItemID)
        VALUES
        (
            @ItemName, @BrandID, @SubBrandID, @CategoryID, @ConditionID,
            @ReceiptID, @Price, @Quantity, @Boxed, @Notes, @ImageID, @ItemTypeID
        );

        COMMIT;

        SELECT @NewItemID = ItemID FROM @Inserted;

        SET @ResponseMessage = N'Item inserted successfully';
        SET @ErrorCode       = 0;
        RETURN 0;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK;

        -- Log the exception details
        INSERT INTO dbo.ErrorLog (ErrorMessage, ErrorTime)
        VALUES (ERROR_MESSAGE(), SYSDATETIME());

        SET @ResponseMessage = N'An error occurred. Please contact the administrator.';
        SET @ErrorCode       = 1;
        RETURN 1;
    END CATCH
END