
CREATE PROCEDURE [dbo].[usp_UpdateLegoItem]
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

        -- Validate LegoItem exists
        IF NOT EXISTS (SELECT 1 FROM LegoItems WHERE ItemID = @ItemID)
        BEGIN
            SET @ResponseMessage = 'Lego item does not exist for the given ItemID';
            SET @ErrorLogMessage = @ResponseMessage;
            IF @@TRANCOUNT > 0 ROLLBACK;
            GOTO LogError;
        END

        -- Check for changes
        IF NOT EXISTS (
            SELECT 1
            FROM LegoItems
            WHERE ItemID = @ItemID
              AND (ISNULL(LegoSetID,'') != ISNULL(@LegoSetID,'') OR
                   ISNULL(LegoDescription,'') != ISNULL(@LegoDescription,'') OR
                   ISNULL(LegoTheme,'') != ISNULL(@LegoTheme,'') OR
                   ISNULL(PieceCount,-1) != ISNULL(@PieceCount,-1))
        )
        BEGIN
            SET @ResponseMessage = 'No changes detected';
            SET @ErrorLogMessage = @ResponseMessage;
            IF @@TRANCOUNT > 0 ROLLBACK;
            GOTO LogError;
        END

        -- Perform UPDATE
        UPDATE LegoItems
        SET
            LegoSetID       = COALESCE(@LegoSetID, LegoSetID),
            LegoDescription = COALESCE(@LegoDescription, LegoDescription),
            LegoTheme       = COALESCE(@LegoTheme, LegoTheme),
            PieceCount      = COALESCE(@PieceCount, PieceCount)
        WHERE ItemID = @ItemID;

        IF @@ROWCOUNT = 0
        BEGIN
            SET @ResponseMessage = 'No rows updated. Lego item may already have the specified values.';
            SET @ErrorLogMessage = @ResponseMessage;
            IF @@TRANCOUNT > 0 ROLLBACK;
            GOTO LogError;
        END

        COMMIT;
        SET @ResponseMessage = 'Lego item updated successfully';
        SET @ErrorCode = 0;
        RETURN 0;

        LogError:
        BEGIN
            IF @ErrorLogMessage IS NULL
                SET @ErrorLogMessage = 'An unknown error occurred in [dbo].[usp_UpdateLegoItem].';

            INSERT INTO ErrorLog (ErrorMessage, ErrorTime) 
            VALUES (@ErrorLogMessage, GETDATE());

            SET @ResponseMessage = @ErrorLogMessage;
            SET @ErrorCode = 1;
            RETURN;
        END
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK;

        INSERT INTO ErrorLog (ErrorMessage, ErrorTime) VALUES (ERROR_MESSAGE(), GETDATE());

        SET @ResponseMessage = 'An error occurred. Please contact the administrator.';
        SET @ErrorCode = 1;
        RETURN;
    END CATCH
END