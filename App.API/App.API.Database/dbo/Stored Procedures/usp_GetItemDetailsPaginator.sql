
CREATE PROCEDURE [dbo].[usp_GetItemDetailsPaginator]
    @PageNumber INT = 1, -- Page number (default is 1)
    @PageSize INT = 10,  -- Number of items per page (default is 10)
    @ResponseMessage NVARCHAR(4000) OUTPUT,
    @ErrorCode INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION

        -- Validate input parameters
        IF @PageNumber < 1 OR @PageSize < 1
        BEGIN
            -- Log the error
            INSERT INTO ErrorLog (ErrorMessage, ErrorTime) 
            VALUES ('Invalid pagination parameters. @PageNumber and @PageSize must be greater than 0.', GETDATE());

            -- Set output parameters for error
            SET @ResponseMessage = 'Invalid pagination parameters. @PageNumber and @PageSize must be greater than 0.'
            SET @ErrorCode = 1

            ROLLBACK
            RETURN
        END

        -- Calculate the starting row for pagination
        DECLARE @Offset INT = (@PageNumber - 1) * @PageSize

        -- Select paginated results
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
        ORDER BY i.ItemID -- Ensure consistent ordering for pagination
        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

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