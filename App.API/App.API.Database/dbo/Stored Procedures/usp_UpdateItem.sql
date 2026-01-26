
CREATE PROCEDURE [dbo].[usp_UpdateItem]
    @ItemID           INT,
    @ItemName         NVARCHAR(255),
    @BrandID          INT = NULL,
    @SubBrandID       INT = NULL,
    @CategoryID       INT = NULL,
    @ConditionID      INT = NULL,
    @ReceiptID        INT = NULL,
    @Price            DECIMAL(10, 2) = NULL,
    @Quantity         INT = NULL,
    @Boxed            BIT = NULL,
    @Notes            NVARCHAR(MAX) = NULL,
    @ImageID          INT = NULL,
    @ItemTypeID       INT = NULL,
    @ResponseMessage  NVARCHAR(255) OUTPUT,
    @ErrorCode        INT OUTPUT
AS
BEGIN
    BEGIN TRY
        -- Initialize output parameters
        SET @ResponseMessage = NULL;
        SET @ErrorCode = NULL;

        -- Start transaction
        BEGIN TRANSACTION;

        -------------------------------------------
        -- Validate FK existence
        -------------------------------------------
        DECLARE @ErrorLogMessage NVARCHAR(MAX) = NULL;

        IF @BrandID IS NOT NULL AND NOT EXISTS (SELECT 1 FROM Brands WHERE BrandID = @BrandID)
        BEGIN
            SET @ResponseMessage = 'Invalid BrandID';
            SET @ErrorLogMessage = @ResponseMessage;
            IF @@TRANCOUNT > 0 ROLLBACK;
            GOTO LogError;
        END

        IF @SubBrandID IS NOT NULL AND NOT EXISTS (
            SELECT 1 FROM SubBrands WHERE SubBrandID = @SubBrandID AND (@BrandID IS NULL OR BrandID = @BrandID)
        )
        BEGIN
            SET @ResponseMessage = 'SubBrandID does not belong to the specified BrandID';
            SET @ErrorLogMessage = @ResponseMessage;
            IF @@TRANCOUNT > 0 ROLLBACK;
            GOTO LogError;
        END

        IF @CategoryID IS NOT NULL AND NOT EXISTS (SELECT 1 FROM Categories WHERE CategoryID = @CategoryID)
        BEGIN
            SET @ResponseMessage = 'Invalid CategoryID';
            SET @ErrorLogMessage = @ResponseMessage;
            IF @@TRANCOUNT > 0 ROLLBACK;
            GOTO LogError;
        END

        IF @ConditionID IS NOT NULL AND NOT EXISTS (SELECT 1 FROM Conditions WHERE ConditionID = @ConditionID)
        BEGIN
            SET @ResponseMessage = 'Invalid ConditionID';
            SET @ErrorLogMessage = @ResponseMessage;
            IF @@TRANCOUNT > 0 ROLLBACK;
            GOTO LogError;
        END

        IF @ReceiptID IS NOT NULL AND NOT EXISTS (SELECT 1 FROM Receipts WHERE ReceiptID = @ReceiptID)
        BEGIN
            SET @ResponseMessage = 'Invalid ReceiptID';
            SET @ErrorLogMessage = @ResponseMessage;
            IF @@TRANCOUNT > 0 ROLLBACK;
            GOTO LogError;
        END

        IF @ImageID IS NOT NULL AND NOT EXISTS (SELECT 1 FROM Images WHERE ImageID = @ImageID)
        BEGIN
            SET @ResponseMessage = 'Invalid ImageID';
            SET @ErrorLogMessage = @ResponseMessage;
            IF @@TRANCOUNT > 0 ROLLBACK;
            GOTO LogError;
        END

        IF @ItemTypeID IS NOT NULL AND NOT EXISTS (SELECT 1 FROM ItemTypes WHERE ItemTypeID = @ItemTypeID)
        BEGIN
            SET @ResponseMessage = 'Invalid ItemTypeID';
            SET @ErrorLogMessage = @ResponseMessage;
            IF @@TRANCOUNT > 0 ROLLBACK;
            GOTO LogError;
        END

        IF NOT EXISTS (SELECT 1 FROM Items WHERE ItemID = @ItemID)
        BEGIN
            SET @ResponseMessage = 'Invalid ItemID';
            SET @ErrorLogMessage = @ResponseMessage;
            IF @@TRANCOUNT > 0 ROLLBACK;
            GOTO LogError;
        END

        -------------------------------------------
        -- Check for changes
        -------------------------------------------
        IF NOT EXISTS (
            SELECT 1
            FROM Items
            WHERE ItemID = @ItemID
              AND (ItemName != @ItemName OR
                   BrandID != @BrandID OR
                   SubBrandID != @SubBrandID OR
                   CategoryID != @CategoryID OR
                   ConditionID != @ConditionID OR
                   ReceiptID != @ReceiptID OR
                   Price != @Price OR
                   Quantity != @Quantity OR
                   Boxed != @Boxed OR
                   Notes != @Notes OR
                   ImageID != @ImageID OR
                   ItemTypeID != @ItemTypeID)
        )
        BEGIN
            SET @ResponseMessage = 'No changes detected';
            SET @ErrorLogMessage = @ResponseMessage;
            IF @@TRANCOUNT > 0 ROLLBACK;
            GOTO LogError;
        END

        -------------------------------------------
        -- Perform UPDATE
        -------------------------------------------
		UPDATE Items
		SET
			ItemName    = COALESCE(@ItemName, ItemName),
			BrandID     = COALESCE(@BrandID, BrandID),
			SubBrandID  = COALESCE(@SubBrandID, SubBrandID),
			CategoryID  = COALESCE(@CategoryID, CategoryID),
			ConditionID = COALESCE(@ConditionID, ConditionID),
			ReceiptID   = COALESCE(@ReceiptID, ReceiptID),
			Price       = COALESCE(@Price, Price),
			Quantity    = COALESCE(@Quantity, Quantity),
			Boxed       = COALESCE(@Boxed, Boxed),
			Notes       = COALESCE(@Notes, Notes),
			ImageID     = COALESCE(@ImageID, ImageID),
			ItemTypeID  = COALESCE(@ItemTypeID, ItemTypeID)
		WHERE ItemID = @ItemID;

        IF @@ROWCOUNT = 0
        BEGIN
            SET @ResponseMessage = 'No rows updated. Item may already have the specified values.';
            SET @ErrorLogMessage = @ResponseMessage;
            IF @@TRANCOUNT > 0 ROLLBACK;
            GOTO LogError;
        END

        -- Commit transaction
        COMMIT;
        SET @ResponseMessage = 'Item updated successfully';
        SET @ErrorCode = 0;
        RETURN 0;

        -------------------------------------------
        -- Log Error Section
        -------------------------------------------
        LogError:
        BEGIN
            -- Ensure @ErrorLogMessage is not NULL
            IF @ErrorLogMessage IS NULL
                SET @ErrorLogMessage = 'An unknown error occurred in [dbo].[usp_UpdateItem].';

            -- Log the error
            INSERT INTO ErrorLog (ErrorMessage, ErrorTime) 
            VALUES (@ErrorLogMessage, GETDATE());

            -- Set output parameters
            SET @ResponseMessage = @ErrorLogMessage;
            SET @ErrorCode = 1;

            -- Terminate the procedure
            RETURN;
        END
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK;

        -- Handle the exception
        INSERT INTO ErrorLog (ErrorMessage, ErrorTime) VALUES (ERROR_MESSAGE(), GETDATE());

        -- Ensure output parameters are set
        SET @ResponseMessage = 'An error occurred. Please contact the administrator.';
        SET @ErrorCode = 1;
        RETURN;
    END CATCH
END