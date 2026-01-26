
CREATE PROCEDURE [dbo].[usp_GetUserSummaryByUsername]
    @Username NVARCHAR(100),  
    @ResponseMessage NVARCHAR(MAX) OUTPUT,  
    @ErrorCode INT OUTPUT  
AS
BEGIN
    DECLARE @ErrorLogMessage NVARCHAR(MAX);  
    SET @ResponseMessage = NULL;  
    SET @ErrorCode = 0;  

    BEGIN TRY
        BEGIN TRANSACTION

        IF EXISTS (SELECT 1 FROM [dbo].[Users] WHERE Username = @Username)  
        BEGIN  
            SELECT
                [UserID],
                [Username],
                [Role]
            FROM [dbo].[Users]
            WHERE [Username] = @Username;

            SET @ResponseMessage = 'User summary fetched successfully';  
            SET @ErrorCode = 0; 
        END  
        ELSE
        BEGIN
            SET @ErrorLogMessage = 'User with the specified username does not exist. Failed to fetch user summary. [dbo].[usp_GetUserSummaryByUsername]';  
            SET @ResponseMessage = @ErrorLogMessage;
            SET @ErrorCode = 1;
        END

        COMMIT
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK

        INSERT INTO ErrorLog (ErrorMessage, ErrorTime) VALUES (ERROR_MESSAGE(), GETDATE())
        RAISERROR('An error occurred. Please contact the administrator', 16, 1)
        RETURN
    END CATCH

    RETURN 0
END