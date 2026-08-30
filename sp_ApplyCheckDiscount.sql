CREATE OR ALTER PROCEDURE [dbo].[sp_ApplyCheckDiscount]
    @OrderKey UNIQUEIDENTIFIER,
    @DiscountKey UNIQUEIDENTIFIER,
    @EmployeeID INT,
    @StationID INT,
    @OldEditKey UNIQUEIDENTIFIER,
    @Success BIT OUTPUT,
    @Message NVARCHAR(255) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        -- 1. Yoxlamalar (Sifarişin mövcudluğu və dəyişdirilib-dəyişdirilmədiyi)
        DECLARE @OrderStatus INT;
        DECLARE @CurrentEditKey UNIQUEIDENTIFIER;
        DECLARE @BranchID INT;
        DECLARE @SubTotal DECIMAL(18,2);

        SELECT @OrderStatus = OrderStatus, 
               @CurrentEditKey = EditKey,
               @BranchID = BranchID,
               @SubTotal = SubTotal
        FROM OrderHeaders
        WHERE OrderKey = @OrderKey;

        IF @OrderStatus <> 1
        BEGIN
            SET @Success = 0;
            SET @Message = N'Sifariş açıq deyil (və ya tapılmadı).';
            ROLLBACK TRANSACTION;
            RETURN;
        END

        IF @CurrentEditKey <> @OldEditKey
        BEGIN
            SET @Success = 0;
            SET @Message = N'Sifariş eyni anda başqa bir terminal və ya ofisiant tərəfindən dəyişdirilib. Zəhmət olmasa səhifəni yeniləyin.';
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- 2. Endirim məlumatlarının çəkilməsi
        DECLARE @DiscountID INT;
        DECLARE @DiscountAmountValue DECIMAL(18,2);
        DECLARE @DiscountBasisValue INT;
        DECLARE @DiscountText NVARCHAR(250);

        SELECT @DiscountID = DiscountID,
               @DiscountAmountValue = DiscountAmount,
               @DiscountBasisValue = DiscountBasis,
               @DiscountText = DiscountText
        FROM Discounts
        WHERE DiscountKey = @DiscountKey AND DiscountActive = 1;

        IF @DiscountID IS NULL
        BEGIN
            SET @Success = 0;
            SET @Message = N'Seçilmiş endirim tapılmadı və ya aktiv deyil.';
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- 3. Endirim məbləğinin hesablanması
        DECLARE @DiscountOrderAmount DECIMAL(18,2) = 0;
        
        -- DiscountBasisValue: 0 = Faiz(%), 1 = Məbləğ(Amount) ola bilər
        IF @DiscountBasisValue = 0 
        BEGIN
            SET @DiscountOrderAmount = (@SubTotal * @DiscountAmountValue) / 100.0;
        END
        ELSE 
        BEGIN
            SET @DiscountOrderAmount = @DiscountAmountValue;
        END

        DECLARE @AmountDue DECIMAL(18,2) = @SubTotal - @DiscountOrderAmount;
        
        DECLARE @NewEditKey UNIQUEIDENTIFIER = NEWID();
        DECLARE @NewSyncKey UNIQUEIDENTIFIER = NEWID();

        DECLARE @EmployeeName NVARCHAR(100);
        SELECT @EmployeeName = ISNULL(FirstName, '') + ' ' + ISNULL(LastName, '') FROM EmployeeFiles WHERE AutoID = @EmployeeID;

        -- 4. Access Log (Giriş) yazılması
        INSERT INTO [AccessLogs] (
            [BranchID], [LogDate], [StationID], [EmployeeID], [ActionName], 
            [IsSuccess], [OrderKey], [TransactionKey], [AccessLogKey], [EditKey], [SyncKey]
        ) VALUES (
            @BranchID, GETDATE(), @StationID, @EmployeeID, N'ÇEK İNDİRİMİ - ' + @DiscountText, 
            1, @OrderKey, '00000000-0000-0000-0000-000000000000', NEWID(), @NewEditKey, @NewSyncKey
        );

        -- 5. Sifariş Başlığının (OrderHeaders) Yenilənməsi
        UPDATE [OrderHeaders] 
        SET [DiscountID] = @DiscountID,
            [DiscountKey] = @DiscountKey,
            [DiscountOrderAmount] = @DiscountOrderAmount,
            [DiscountTotalAmount] = @DiscountOrderAmount,
            [DiscountAmountValue] = @DiscountAmountValue,
            [DiscountBasisValue] = @DiscountBasisValue,
            [DiscountUserName] = @EmployeeName,
            [AmountDue] = @AmountDue,
            [EditKey] = @NewEditKey,
            [SyncKey] = @NewSyncKey,
            [EditUserID] = @EmployeeID,
            [EditUserName] = @EmployeeName,
            [EditDateTime] = GETDATE()
        WHERE OrderKey = @OrderKey;

        -- 6. Sifariş Sətirlərinin (OrderTransactions) Yenilənməsi (Sync və Edit key üçün)
        UPDATE [OrderTransactions] 
        SET [EditKey] = NEWID(),
            [SyncKey] = NEWID(),
            [EditUserID] = @EmployeeID,
            [EditUserName] = @EmployeeName,
            [EditDateTime] = GETDATE()
        WHERE OrderKey = @OrderKey;

        -- 7. Valyutaların (USD, EUR, GBP) Yenilənməsi
        UPDATE h 
        SET UsdAmount = CAST(ROUND(h.AmountDue / ISNULL(m1.ExchangeRate, 1), 2) AS DECIMAL(18, 2)),
            EurAmount = CAST(ROUND(h.AmountDue / ISNULL(m2.ExchangeRate, 1), 2) AS DECIMAL(18, 2)),
            GbpAmount = CAST(ROUND(h.AmountDue / ISNULL(m3.ExchangeRate, 1), 2) AS DECIMAL(18, 2))
        FROM OrderHeaders h
        LEFT JOIN PaymentMethods m1 ON m1.PaymentName IN ('DOLAR', 'USD')
        LEFT JOIN PaymentMethods m2 ON m2.PaymentName IN ('EURO', 'EUR')
        LEFT JOIN PaymentMethods m3 ON m3.PaymentName IN ('STERLIN', 'GBP')
        WHERE h.OrderKey = @OrderKey;

        -- 8. Ümumi Yaddaşa Verilmə Vaxtı (OrderSaveTime) Parametrinin Yenilənməsi
        DELETE FROM Params WHERE ParamName = 'OrderSaveTime';
        INSERT INTO Params (ParamName, ParamValue) VALUES ('OrderSaveTime', GETDATE());

        -- 9. (Optional) Qəbz nömrələrinin və ID-lərin sinxronizasiyası funksiyası varsa çağırıla bilər:
        -- EXEC dbo.fncUpdateReceiptNo;

        COMMIT TRANSACTION;
        SET @Success = 1;
        SET @Message = N'Endirim uğurla tətbiq edildi.';
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        SET @Success = 0;
        SET @Message = ERROR_MESSAGE();
    END CATCH
END;
