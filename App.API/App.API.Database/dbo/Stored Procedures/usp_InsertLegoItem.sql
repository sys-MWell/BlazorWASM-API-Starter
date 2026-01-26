
CREATE PROCEDURE [dbo].[usp_InsertLegoItem]
    @ItemID           INT,
    @LegoSetID        VARCHAR(50) = NULL,
    @LegoDescription  NVARCHAR(MAX) = NULL,
    @LegoTheme        VARCHAR(100) = NULL,
    @PieceCount       INT = NULL,
    @ResponseMessage  NVARCHAR(255) OUTPUT,
    @ErrorCode        INT OUTPUT
AS
BEGIN
    BEGIN TRY
        SET @ResponseMessage = NULL;
        SET @ErrorCode = NULL;

        BEGIN TRANSACTION;

        DECLARE @ErrorLogMessage NVARCHAR(MAX) = NULL;

        -- Validate ItemID exists
        IF NOT EXISTS (SELECT 1 FROM Items WHERE ItemID = @ItemID)
        BEGIN
            SET @ResponseMessage = 'Invalid ItemID';
            SET @ErrorLogMessage = @ResponseMessage;
            IF @@TRANCOUNT > 0 ROLLBACK;
            GOTO LogError;
        END

        -- Ensure LegoItem does not already exist
        IF EXISTS (SELECT 1 FROM LegoItems WHERE ItemID = @ItemID)
        BEGIN
            SET @ResponseMessage = 'Lego item already exists for this ItemID';
            SET @ErrorLogMessage = @ResponseMessage;
            IF @@TRANCOUNT > 0 ROLLBACK;
            GOTO LogError;
        END

        -- Insert LegoItem
        INSERT INTO LegoItems (ItemID, LegoSetID, LegoDescription, LegoTheme, PieceCount)
        VALUES (@ItemID, @LegoSetID, @LegoDescription, @LegoTheme, @PieceCount);

        COMMIT;
        SET @ResponseMessage = 'Lego item inserted successfully';
        SET @ErrorCode = 0;
        RETURN 0;

        LogError:
        BEGIN
            INSERT INTO ErrorLog (ErrorMessage, ErrorTime) VALUES (@ErrorLogMessage, GETDATE());
            SET @ResponseMessage = @ErrorLogMessage;
            SET @ErrorCode = 1;
            RETURN;
        END
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK;
        INSERT INTO ErrorLog (ErrorMessage, ErrorTime) VALUES (ERROR_MESSAGE(), GETDATE());
        SET @ResponseMessage = 'An error occurred. Please contact the administrator.';
        SET @ErrorCode = 1;
        RETURN;
    END CATCH
END