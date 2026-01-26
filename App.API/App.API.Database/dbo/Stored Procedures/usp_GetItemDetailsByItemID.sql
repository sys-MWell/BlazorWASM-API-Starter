
CREATE PROCEDURE [dbo].[usp_GetItemDetailsByItemID]
    @ItemID INT,
    @ResponseMessage NVARCHAR(4000) OUTPUT,
    @ErrorCode INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION

        -- Validate input parameter
		IF NOT EXISTS (SELECT 1 FROM [dbo].[Items] WHERE ItemID = @ItemID)
		BEGIN
			-- Log the error
			INSERT INTO ErrorLog (ErrorMessage, ErrorTime) 
			VALUES ('ItemID does not exist in the Items table.', GETDATE());

			-- Set output parameters for error
			SET @ResponseMessage = 'ItemID does not exist in the Items table.'
			SET @ErrorCode = 1

			ROLLBACK;
			RETURN;
		END

        -- Select detailed item information
        SELECT 
            i.ItemID,
            i.ItemName,
            i.Price,
            i.Quantity,
            i.Boxed,
            i.Notes,
            i.BrandID,
            i.SubBrandID,
            i.CategoryID,
            i.ConditionID,
            i.ReceiptID,
            i.ImageID,
            i.ItemTypeID,
            it.TypeName,
            it.TypeDescription,
            b.BrandName,
            b.BrandType,
            b.BrandDescription,
            sb.SubBrandID,
            c.CategoryName,
            c.CategoryDescription,
            c.CategoryType,
            cond.ConditionType,
            r.ReceiptID,
            r.PurchaseDate,
            r.TotalAmount,
            r.StoreID,
            r.ImageID AS ReceiptImageID,
            s.StoreName,
            s.ParkID,
            s.LocationID,
            p.ParkName,
            p.CompanyID,
            tp.CompanyName AS CompanyName,
            loc.LocationID,
            loc.LocationName,
            loc.LocationAddressLine1,
            loc.LocationAddressLine2,
            loc.LocationAddressLine3,
            loc.LocationCity,
            loc.LocationState,
            loc.LocationCountry,
            loc.LocationPostalCode,
            loc.Notes AS LocationNotes,
            img.ImageID AS ReceiptImageID,
            img.ImageName AS ReceiptImageName,
            img.ImageLocation AS ReceiptImageLocation,
            img.ImageCaption AS ReceiptImageCaption,
            storeImg.ImageID AS StoreImageID,
            storeImg.ImageName AS StoreImageName,
            storeImg.ImageLocation AS StoreImageLocation,
            storeImg.ImageCaption AS StoreImageCaption
        FROM [dbo].[Items] i
        LEFT JOIN [dbo].[ItemTypes] it ON i.ItemTypeID = it.ItemTypeID
        LEFT JOIN [dbo].[Brands] b ON i.BrandID = b.BrandID
        LEFT JOIN [dbo].[SubBrands] sb ON i.SubBrandID = sb.SubBrandID
        LEFT JOIN [dbo].[Categories] c ON i.CategoryID = c.CategoryID
        LEFT JOIN [dbo].[Conditions] cond ON i.ConditionID = cond.ConditionID
        LEFT JOIN [dbo].[Receipts] r ON i.ReceiptID = r.ReceiptID
        LEFT JOIN [dbo].[Stores] s ON r.StoreID = s.StoreID
        LEFT JOIN [dbo].[Parks] p ON s.ParkID = p.ParkID
        LEFT JOIN [dbo].[Company] tp ON p.CompanyID = tp.CompanyID
        LEFT JOIN [dbo].[Locations] loc ON s.LocationID = loc.LocationID
        LEFT JOIN [dbo].[Images] img ON r.ImageID = img.ImageID
        LEFT JOIN [dbo].[Images] storeImg ON s.ImageID = storeImg.ImageID
        WHERE i.ItemID = @ItemID;

        -- Set output parameters for success
        SET @ResponseMessage = 'Success'
        SET @ErrorCode = 0

        COMMIT
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK

        -- Log the error
        INSERT INTO ErrorLog (ErrorMessage, ErrorTime) VALUES (ERROR_MESSAGE(), GETDATE())

        -- Set output parameters for error
        SET @ResponseMessage = ERROR_MESSAGE()
        SET @ErrorCode = ERROR_NUMBER()

        -- Return the error message
        RAISERROR('An error occurred. Please contact the administrator.', 16, 1)
        RETURN
    END CATCH

    RETURN 0
END