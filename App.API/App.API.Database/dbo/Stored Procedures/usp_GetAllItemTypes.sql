
CREATE PROCEDURE [dbo].[usp_GetAllItemTypes]
    @ResponseMessage NVARCHAR(MAX) OUTPUT,
    @ErrorCode INT OUTPUT
AS
BEGIN
    SET @ResponseMessage = NULL;
    SET @ErrorCode = 0;

    BEGIN TRY
        BEGIN TRANSACTION

        SELECT
            [ItemTypeID],
            [TypeName],
            [TypeDescription]
        FROM [dbo].[ItemTypes]
        WHERE [TypeName] IS NOT NULL AND [TypeName] <> ''

        SET @ResponseMessage = 'Item types fetched successfully.';
        SET @ErrorCode = 0;

        COMMIT
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK

        DECLARE @ErrorLogMessage NVARCHAR(MAX) = ERROR_MESSAGE();

        -- Log the error
        INSERT INTO ErrorLog (ErrorMessage, ErrorTime) VALUES (@ErrorLogMessage, GETDATE());

        -- Set output parameters
        SET @ResponseMessage = @ErrorLogMessage;
        SET @ErrorCode = 1;

        RETURN
    END CATCH

    RETURN 0
END