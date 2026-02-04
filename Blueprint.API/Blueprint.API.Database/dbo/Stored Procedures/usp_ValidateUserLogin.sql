-- Created by GitHub Copilot in SSMS - review carefully before executing
CREATE PROCEDURE [dbo].[usp_ValidateUserLogin]
    @Username NVARCHAR(100),
    @Password NVARCHAR(100),
    @ResponseMessage NVARCHAR(MAX) OUTPUT,
    @ErrorCode INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @ResponseMessage = NULL;
    SET @ErrorCode = 0;

    BEGIN TRY
        IF EXISTS (
            SELECT 1 
            FROM dbo.Users 
            WHERE Username = @Username 
              AND UserPassword = @Password
        )
        BEGIN
            SELECT
                UserID,
                Username,
                UserPassword,
                Role
            FROM dbo.Users
            WHERE Username = @Username
              AND UserPassword = @Password;

            SET @ResponseMessage = 'User authenticated successfully';
            SET @ErrorCode = 0;
        END
        ELSE
        BEGIN
            SET @ResponseMessage = 'Invalid username or password. Authentication failed.';
            SET @ErrorCode = 1;
        END
    END TRY
    BEGIN CATCH
        INSERT INTO dbo.ErrorLog (ErrorMessage, ErrorTime) 
        VALUES (ERROR_MESSAGE(), GETDATE());
        
        SET @ResponseMessage = 'An error occurred. Please contact the administrator';
        SET @ErrorCode = 2;
        
        RAISERROR('An error occurred. Please contact the administrator', 16, 1);
        RETURN;
    END CATCH

    RETURN 0;
END