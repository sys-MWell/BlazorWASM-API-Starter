
CREATE PROCEDURE [dbo].[usp_RegisterUser]
    @Username NVARCHAR(100), 
    @UserPassword NVARCHAR(255),
    @Role NVARCHAR(10), 
    @ResponseMessage NVARCHAR(MAX) OUTPUT,  
    @ErrorCode INT OUTPUT  
AS
BEGIN
    DECLARE @ErrorLogMessage NVARCHAR(MAX);

    BEGIN TRY
        IF EXISTS (SELECT 1 FROM [dbo].[Users] WHERE Username = @Username)
        BEGIN
            SET @ErrorLogMessage = 'User with the specified username already exists.';
            SET @ResponseMessage = @ErrorLogMessage;
            SET @ErrorCode = 1;
        END
        ELSE
        BEGIN
            BEGIN TRANSACTION

            INSERT INTO [dbo].[Users] (Username, UserPassword, Role)
            VALUES (@Username, @UserPassword, @Role)

            SET @ResponseMessage = 'User registered successfully.';  
            SET @ErrorCode = 0; 
			--Return user details
            SELECT
                [UserID],
                [Username],
                [Role]
            FROM [dbo].[Users]
            WHERE [Username] = @Username;

            COMMIT
        END
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK

        INSERT INTO ErrorLog (ErrorMessage, ErrorTime) VALUES (ERROR_MESSAGE(), GETDATE())
        SET @ResponseMessage = 'An error occurred. Please contact the administrator.';
        SET @ErrorCode = 2;
        RETURN
    END CATCH

    RETURN
END