-- Created by GitHub Copilot in SSMS - review carefully before executing
CREATE PROCEDURE [dbo].[usp_GetPasswordHashByUsername]
    @Username NVARCHAR(100),
    @ResponseMessage NVARCHAR(MAX) OUTPUT,
    @ErrorCode INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @ResponseMessage = NULL;
    SET @ErrorCode = 0;

    BEGIN TRY
    IF EXISTS (SELECT 1 FROM dbo.Users WHERE Username = @Username)
    BEGIN
        SELECT UserPassword
        FROM dbo.Users
        WHERE Username = @Username;

        SET @ResponseMessage = 'Password hash retrieved successfully';
        SET @ErrorCode = 0;
    END
    ELSE
    BEGIN
        SET @ResponseMessage = 'User not found';
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