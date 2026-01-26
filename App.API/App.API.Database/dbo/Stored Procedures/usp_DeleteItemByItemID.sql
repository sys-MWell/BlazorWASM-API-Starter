

CREATE PROCEDURE [dbo].[usp_DeleteItemByItemID]
	@ItemID INT,
	@ResponseMessage NVARCHAR(255) OUTPUT,
	@ErrorCode INT OUTPUT
AS
BEGIN
	BEGIN TRY
		BEGIN TRANSACTION
			
		-- Attempt to delete from ItemLocations
		IF EXISTS (SELECT 1 FROM ItemLocations WHERE ItemID = @ItemID)
		BEGIN
			DELETE FROM ItemLocations WHERE ItemID = @ItemID;
		END

		-- Attempt to delete from ItemTags
		IF EXISTS (SELECT 1 FROM ItemTags WHERE ItemID = @ItemID)
		BEGIN
			DELETE FROM ItemTags WHERE ItemID = @ItemID;
		END

		-- Attempt to delete from LegoDetails
		IF EXISTS (SELECT 1 FROM LegoDetails WHERE ItemID = @ItemID)
		BEGIN
			DELETE FROM LegoDetails WHERE ItemID = @ItemID;
		END

		-- Attempt to delete from Items
		IF EXISTS (SELECT 1 FROM Items WHERE ItemID = @ItemID)
		BEGIN
			DELETE FROM Items WHERE ItemID = @ItemID;
			SET @ResponseMessage = 'Item deleted successfully.';
		END
		ELSE
		BEGIN
			SET @ResponseMessage = 'Item with the specified ID does not exist.';
			ROLLBACK
			INSERT INTO ErrorLog (ErrorMessage, ErrorTime)
			VALUES ('Item with the specified ID does not exist. Failed to delete item. [dbo].[usp_DeleteItemByItemID]', GETDATE())
			SET @ErrorCode = 1;
			RETURN 1;
		END

		COMMIT
		SET @ErrorCode = 0;
		RETURN 0; -- Ensure success returns 0
	END TRY
	BEGIN CATCH
		IF @@TRANCOUNT > 0
			ROLLBACK

		-- Log the error
		INSERT INTO ErrorLog (ErrorMessage, ErrorTime) VALUES (ERROR_MESSAGE(), GETDATE())

		-- Return the error message
		SET @ResponseMessage = 'An error occurred. Please contact the administrator.';
		SET @ErrorCode = 1;
		RETURN 1; -- Ensure errors return 1
	END CATCH
END