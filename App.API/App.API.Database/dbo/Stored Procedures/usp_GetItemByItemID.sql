
CREATE PROCEDURE [dbo].[usp_GetItemByItemID]  
@ItemID INT,  
@ResponseMessage NVARCHAR(MAX) OUTPUT,  
@ErrorCode INT OUTPUT  
AS  
BEGIN  
    DECLARE @ErrorLogMessage NVARCHAR(MAX);  
    SET @ResponseMessage = NULL;  
    SET @ErrorCode = 0;  

    BEGIN TRY  
        -- Validate input parameter  
        IF NOT EXISTS (SELECT 1 FROM [dbo].[Items] WHERE ItemID = @ItemID)  
        BEGIN  
            SET @ErrorLogMessage = 'Item with the specified ID does not exist. Failed to fetch item. [dbo].[usp_GetItemByItemID]';  
            SET @ResponseMessage = @ErrorLogMessage;
            SET @ErrorCode = 1;
            
            -- Log the error
            INSERT INTO ErrorLog (ErrorMessage, ErrorTime)   
            VALUES (@ErrorLogMessage, GETDATE());  

            RETURN;
        END  

        BEGIN TRANSACTION

        -- Select item
        SELECT   
            i.ItemID,  
            i.ItemName,  
            i.BrandID,  
            i.SubBrandID,  
            i.CategoryID,  
            i.ConditionID,  
            i.ReceiptID,  
            i.Price,  
            i.Quantity,  
            i.Boxed,  
            i.Notes,  
            i.ImageID,  
            i.ItemTypeID  
        FROM [dbo].[Items] i  
        WHERE i.ItemID = @ItemID;  

        -- Set success response  
        SET @ResponseMessage = 'Item fetched successfully.';  
        SET @ErrorCode = 0;  

        COMMIT;  
    END TRY  
    BEGIN CATCH  
        IF @@TRANCOUNT > 0  
            ROLLBACK;  

        SET @ErrorLogMessage = ERROR_MESSAGE();  

        -- Log the error  
        INSERT INTO ErrorLog (ErrorMessage, ErrorTime)   
        VALUES (@ErrorLogMessage, GETDATE());  

        SET @ResponseMessage = @ErrorLogMessage;  
        SET @ErrorCode = 1;  
    END CATCH  
END