-- 1. İstifadəçi icazələrinin yoxlanışı
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SELECT [AuthorityID], [GroupName], [AuthorityKey], [AuthorityText], [AuthorityDescription], [DefaultLevel], [DefaultLevel] as OldLevel, isnull([NeedAllways],0) as NeedAllways, isnull([NeedAllways],0) as OldNeedAllways, isnull([AllowManager],0) as AllowManager, isnull([AllowManager],0) as OldAllowManager, isnull(AllowCashier,0) as AllowCashier, isnull(AllowCashier,0) as oldAllowCashier, [EditKey], [SyncKey], [BranchID] 
FROM [AuthorityList];

-- 2. İşçinin şifrə və ya kartla yoxlanışı
EXEC sp_executesql N'SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SELECT EmployeeFiles.AutoID, EmployeeFiles.EmployeeKey, EmployeeFiles.EmployeeID, EmployeeFiles.FirstName, EmployeeFiles.LastName,  
 EmployeeFiles.SocialSecurityNumber, EmployeeFiles.SmarCardCode, EmployeeFiles.MifareCardCode, EmployeeFiles.MailingAddress, EmployeeFiles.MailingZipCode, EmployeeFiles.DateHired, EmployeeFiles.DateReleased,  
 EmployeeFiles.EmployeeActive, EmployeeFiles.JobTitleID, EmployeeFiles.SecurityLevel, EmployeeFiles.AccessCode, EmployeeFiles.TipsReceived,  
 EmployeeFiles.PayBasis, EmployeeFiles.PayRate, EmployeeFiles.ScanCode, isnull(EmployeeFiles.DriverLicenseNumber,'''') as DriverLicenseNumber, EmployeeFiles.DriverLicenseExpires,  
 EmployeeFiles.CarInsurancePolicyCarrier, EmployeeFiles.CarInsurancePolicyNumber, EmployeeFiles.CarInsurancePolicyExpires,  
 EmployeeFiles.CarInsurancePolicyNotes, isnull(EmployeeFiles.PrefUserInterfaceLocale,''-'') as PrefUserInterfaceLocale, EmployeeFiles.EmployeeNotes, EmployeeFiles.OrderentryUseSecLang,  
 EmployeeFiles.EmployeeIsDriver, EmployeeFiles.DefaultOEMenuGroupID, EmployeeFiles.UseStaffBank, EmployeeFiles.ScheduleNotEnforced,  
 EmployeeFiles.UseHostess, EmployeeFiles.IsAServer, EmployeeFiles.IsOffline, EmployeeFiles.NoCashierOut, EmployeeFiles.EditTimestamp,  
 EmployeeFiles.PhoneNumber, EmployeeFiles.RevenueCenterTypeID, EmployeeFiles.DeleteReason, EmployeeFiles.CustomField1, EmployeeFiles.CustomField2,  
 EmployeeFiles.CustomField3, EmployeeFiles.CustomField4, EmployeeFiles.CustomField5, EmployeeFiles.EditKey, EmployeeFiles.SyncKey,  
 EmployeeFiles.BranchID, EmployeeFiles.AddUserID, EmployeeFiles.AddDateTime, EmployeeFiles.EditUserID, EmployeeFiles.EditDateTime,  
 EmployeeTitles.TitleName as JobTitleText, isnull(EmployeeFiles.MonthlyDinnerFee,0) as MonthlyDinnerFee, CAST(0 AS BIT) AS IsChecked 
 FROM EmployeeFiles  
 LEFT OUTER JOIN EmployeeTitles ON EmployeeFiles.JobTitleID = EmployeeTitles.TitleID  
 WHERE (EmployeeFiles.AccessCode=@password or EmployeeFiles.MifareCardCode=@password or EmployeeFiles.SmarCardCode=@password ) and isnull(EmployeeFiles.EmployeeActive,0)=1',
N'@password nvarchar(6)',
@password=N'106510';

-- 3. Oxunmamış mesajların yoxlanışı
SELECT ISNULL((SELECT TOP 1 CAST(MessageKey AS NVARCHAR(50)) FROM UserMessages WHERE Reciepments LIKE +'%,106,%' AND isnull(Readed,'') NOT LIKE +'%,106,%'),'-');

-- 4. AccessLog qeydinin yazılması (Şifrə/Yetki girişi)
EXEC sp_executesql N'INSERT INTO [AccessLogs] ([BranchID], [LogDate], [StationID], [EmployeeID], [ActionName], [WrongPassword], [AdditionalInfo], [IsSuccess], [OrderKey], [TransactionKey], [AccessLogKey], [EditKey], [SyncKey]) 
VALUES (@BranchID, getdate(), @StationID, @EmployeeID, @ActionName, @WrongPassword, @AdditionalInfo, @IsSuccess, @OrderKey, @TransactionKey, newid(), @EditKey, @SyncKey)',
N'@BranchID int,@LogDate datetime,@StationID int,@EmployeeID int,@ActionName nvarchar(13),@WrongPassword nvarchar(6),@AdditionalInfo nvarchar(21),@IsSuccess bit,@OrderKey uniqueidentifier,@TransactionKey uniqueidentifier,@AccessLogKey uniqueidentifier,@EditKey uniqueidentifier,@SyncKey uniqueidentifier',
@BranchID=422,
@LogDate='2026-08-17 13:10:33.627',
@StationID=1,
@EmployeeID=106,
@ActionName=N'ÜRÜN İNDİRİMİ',
@WrongPassword=N'106510',
@AdditionalInfo=N'Dönər Ət üçün indirim',
@IsSuccess=1,
@OrderKey='8A86D18A-F338-4E0A-B963-2B487A8B26BD',
@TransactionKey='8F9EFB8F-2190-4C2E-AC8A-D7AEF297AACB',
@AccessLogKey=NULL,
@EditKey='81DCD1AA-46A7-497C-8A98-DF11C80EBCCD',
@SyncKey='1A1D1AD5-8B07-4784-B883-494077CF9950';

-- 5. Məhsulun endirimə uyğunluğunun yoxlanışı
SELECT ISNULL((SELECT TOP 1 ISNULL(MenuItemDiscountable,0) AS DiscountAllow FROM [MenuItems] WHERE MenuItemKey='4d3b6a48-d341-4b1d-a039-63f445804fc1'), 1) AS AllowDiscount;

-- 6. Aktiv endirimlərin siyahısı
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SELECT DiscountID, DiscountKey, DiscountText, DiscountDescription, DiscountedAmountTaxable, isnull(DiscountExpireDate,'1/1/2099') as DiscountExpireDate, DiscountActive, DiscountAmount, DiscountBasis,  
 DiscountAllowedMinTicket, Barcode, DiscountMenuItemID, ButtonColor, PictureName, SecurityLevel, DeleteReason, CustomField1, CustomField2, CustomField3,  
 CustomField4, CustomField5, EditKey, SyncKey, BranchID, AddUserID, AddDateTime, EditUserID, EditDateTime, isnull(UseGroupFilter,0) AS UseGroupFilter,  
 isnull(GroupMinimumQuantity,0) AS GroupMinimumQuantity, isnull(UseMenuItemFilter,0) AS UseMenuItemFilter, isnull(MenuItemMinimumQuantity,0) AS MenuItemMinimumQuantity, 
 isnull(UseOrderTotalFilter,0) UseOrderTotalFilter, isnull(OrderTotalMinimum,0) AS OrderTotalMinimum, isnull(UseHourFilter,0) AS UseHourFilter, 
 isnull(HourStart,0) AS HourStart, isnull(HourEnd,0) AS HourEnd, isnull(OrderTotalMaximum,0) AS OrderTotalMaximum, isnull(DisablePromotion,0) AS DisablePromotion, isnull(UseDayFilter,0) AS UseDayFilter, isnull(DayList,'') AS DayList, isnull(SkipMenuDiscountStatus,0) AS SkipMenuDiscountStatus 
 FROM Discounts WHERE [DiscountActive]=1;

-- 7. Filial status parametri
SELECT ISNULL((SELECT TOP 1 ISNULL(ParamValue,'NO') FROM StoreSettings WHERE ParamKey='BranchStatus'),'NO') AS ParamValue;

-- 8. Endirim filtrlərinin çəkilməsi
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SELECT ISNULL(DiscountFilter.AutoID, 0) AS AutoID, DiscountFilter.DiscountKey, DiscountFilter.MenuGroupKey, DiscountFilter.MenuItemKey,  
 ISNULL(DiscountFilter.BranchID, 0) AS BranchID, ISNULL(DiscountFilter.IsSaleFilter, 0) AS IsSaleFilter, isnull(g.MenuSubCategoryText,'') AS MenuGroupName, isnull(m.MenuItemText,'') AS MenuItemText 
 FROM DiscountFilter  
 LEFT OUTER JOIN MenuItems AS m ON DiscountFilter.MenuItemKey = m.MenuItemKey  
 LEFT OUTER JOIN MenuSubCategories AS g ON DiscountFilter.MenuGroupKey = g.MenuSubCategoryKey;

-- 9. EditKey konkurentlik yoxlanışı (Optimistic Lock)
EXEC sp_executesql N'SELECT COUNT(r.EditKey) AS adet FROM (select isnull((SELECT top 1 EditKey FROM OrderHeaders AS h WHERE h.OrderKey=@OrderKey),NEWID()) AS EditKey) AS r WHERE r.EditKey=@EditKey;',
N'@OrderKey uniqueidentifier,@EditKey uniqueidentifier',
@OrderKey='8A86D18A-F338-4E0A-B963-2B487A8B26BD',
@EditKey='EDFECDD1-7DC6-4AD4-B70A-01160F7428E7';

-- 10. Sifarişin açıq olmasının yoxlanışı
EXEC sp_executesql N'SELECT COUNT(OrderKey) AS countValue FROM OrderHeaders with(nolock) WHERE OrderKey=@OrderKey AND OrderStatus<>1',
N'@OrderKey uniqueidentifier',
@OrderKey='8A86D18A-F338-4E0A-B963-2B487A8B26BD';

-- 11. Sifariş başlığının (OrderHeaders) yenilənməsi
EXEC sp_executesql N'UPDATE [OrderHeaders] SET [OrderID]=@OrderID, [ReceiptNo]=@ReceiptNo, [MainOrderKey]=@MainOrderKey, [MainOrderID]=@MainOrderID, [OrderTypeSourceID]=@OrderTypeSourceID, [OrderTypeSourceExternalNo]=@OrderTypeSourceExternalNo, [OrderDateTime]=@OrderDateTime, [EmployeeID]=@EmployeeID, [StationID]=@StationID, [RevenueCenterTypeID]=@RevenueCenterTypeID, [OrderType]=@OrderType, [DineInTableID]=@DineInTableID, [CustomerID]=@CustomerID, [DeliveryCharge]=@DeliveryCharge, [DeliveryComp]=@DeliveryComp, [DeliveryZoneID]=@DeliveryZoneID, [DriverEmployeeID]=@DriverEmployeeID, [DriverDepartureTime]=@DriverDepartureTime, [DriverArrivalTime]=@DriverArrivalTime, [OnHoldUntilTime]=@OnHoldUntilTime, [SalesTaxRate]=@SalesTaxRate, [DiscountID]=@DiscountID, [DiscountLineAmount]=@DiscountLineAmount, [DiscountOrderAmount]=@DiscountOrderAmount, [DiscountCashAmount]=@DiscountCashAmount, [DiscountTotalAmount]=@DiscountTotalAmount, [OrderStatus]=@OrderStatus, [BonusAmountUsed]=@BonusAmountUsed, [BonusAmountEarned]=@BonusAmountEarned, [BonusID]=@BonusID, [BonusCustomerID]=@BonusCustomerID, [AmountDue]=@AmountDue, [PackagerAlreadyPrinted]=@PackagerAlreadyPrinted, [GuestCheckPrinted]=@GuestCheckPrinted, [GuestCheckPrintCount]=@GuestCheckPrintCount, [AdditionPrinted]=@AdditionPrinted, [AdditionPrintedLineCount]=@AdditionPrintedLineCount, [SurchargeID]=@SurchargeID, [SurchargeLineAmount]=@SurchargeLineAmount, [SurchargeOrderAmount]=@SurchargeOrderAmount, [SurchargeCashAmount]=@SurchargeCashAmount, [SurchargeTotalAmount]=@SurchargeTotalAmount, [ComplimentaryAmount]=@ComplimentaryAmount, [SubTotal]=@SubTotal, [OrderCost]=@OrderCost, [GratuityPercent]=@GratuityPercent, [CashGratuity]=@CashGratuity, [SalesTaxAmount]=@SalesTaxAmount, [DriveThruComplete]=@DriveThruComplete, [BarTabName]=@BarTabName, [TableReady]=@TableReady, [GuestNumber]=@GuestNumber, [SpecificCustomerName]=@SpecificCustomerName, [OrderPhone]=@OrderPhone, [InvoicePrinted]=@InvoicePrinted, [FiscalPrinted]=@FiscalPrinted, [OrderNotes]=@OrderNotes, [OrderExternalNotes]=@OrderExternalNotes, [LineDeleted]=@LineDeleted, [DeleteReason]=@DeleteReason, [CustomField1]=@CustomField1, [CustomField2]=@CustomField2, [CustomField3]=@CustomField3, [CustomField4]=@CustomField4, [CustomField5]=@CustomField5, [EditKey]=NEWID(), [SyncKey]=NEWID(), [BranchID]=@BranchID, [LockData]=@LockData, [LockStationID]=@LockStationID, [EditUserID]=@EditUserID, [EditDateTime]=getdate(), [CustomerKey]=@CustomerKey, [DiscountKey]=@DiscountKey, [EmployeeKey]=@EmployeeKey, [EmployeeName]=@EmployeeName, [DineInTableName]=@DineInTableName, [CustomerName]=@CustomerName, [EditUserName]=@EditUserName, [DiscountAmountValue]=@DiscountAmountValue, [DiscountBasisValue]=@DiscountBasisValue, [AddUserName]=@AddUserName, [AddUserID]=@AddUserID, [InvoiceDetail]=@InvoiceDetail, [RetailData]=@RetailData, [DiscountUserName]=@DiscountUserName, [PosVersion]=@PosVersion WHERE [AutoID]=@AutoID and [EditKey]=@EditKey',
N'@AutoID int,@OrderID int,@ReceiptNo nvarchar(1),@MainOrderKey uniqueidentifier,@MainOrderID int,@OrderTypeSourceID int,@OrderTypeSourceExternalNo nvarchar(4000),@OrderKey uniqueidentifier,@OrderDateTime datetime,@EmployeeID int,@StationID int,@RevenueCenterTypeID int,@OrderType int,@DineInTableID int,@CustomerID int,@DeliveryCharge float,@DeliveryComp float,@DeliveryZoneID int,@DriverEmployeeID int,@DriverDepartureTime datetime,@DriverArrivalTime datetime,@OnHoldUntilTime datetime,@SalesTaxRate float,@DiscountID int,@DiscountLineAmount float,@DiscountOrderAmount float,@DiscountCashAmount float,@DiscountTotalAmount float,@OrderStatus int,@BonusAmountUsed float,@BonusAmountEarned float,@BonusID int,@BonusCustomerID int,@AmountDue float,@PackagerAlreadyPrinted bit,@GuestCheckPrinted bit,@GuestCheckPrintCount int,@AdditionPrinted bit,@AdditionPrintedLineCount int,@SurchargeID int,@SurchargeLineAmount float,@SurchargeOrderAmount float,@SurchargeCashAmount float,@SurchargeTotalAmount float,@ComplimentaryAmount float,@SubTotal float,@OrderCost float,@GratuityPercent int,@CashGratuity float,@SalesTaxAmount float,@DriveThruComplete datetime,@BarTabName nvarchar(4000),@TableReady bit,@GuestNumber int,@SpecificCustomerName nvarchar(4),@OrderPhone nvarchar(4000),@InvoicePrinted bit,@FiscalPrinted bit,@OrderNotes nvarchar(4000),@OrderExternalNotes nvarchar(4000),@LineDeleted bit,@DeleteReason nvarchar(4000),@CustomField1 nvarchar(4000),@CustomField2 nvarchar(4000),@CustomField3 nvarchar(4000),@CustomField4 nvarchar(4000),@CustomField5 nvarchar(4000),@EditKey uniqueidentifier,@SyncKey uniqueidentifier,@BranchID int,@LockData bit,@LockStationID int,@AddUserID int,@AddDateTime datetime,@EditUserID int,@EditDateTime datetime,@CustomerKey uniqueidentifier,@DiscountKey uniqueidentifier,@EmployeeKey uniqueidentifier,@EmployeeName nvarchar(8),@DineInTableName nvarchar(2),@CustomerName nvarchar(4000),@AddUserName nvarchar(8),@EditUserName nvarchar(8),@DiscountAmountValue float,@DiscountBasisValue int,@FiscalKey nvarchar(4000),@InvoiceDetail nvarchar(4000),@RetailData nvarchar(4000),@DiscountUserName nvarchar(4000),@PosVersion nvarchar(11)',
@AutoID=12,@OrderID=12,@ReceiptNo=N'1',@MainOrderKey=NULL,@MainOrderID=0,@OrderTypeSourceID=1,@OrderTypeSourceExternalNo=NULL,@OrderKey='8A86D18A-F338-4E0A-B963-2B487A8B26BD',@OrderDateTime='2026-08-16 21:50:08.260',@EmployeeID=106,@StationID=1,@RevenueCenterTypeID=1,@OrderType=1,@DineInTableID=22,@CustomerID=0,@DeliveryCharge=0,@DeliveryComp=0,@DeliveryZoneID=0,@DriverEmployeeID=0,@DriverDepartureTime=NULL,@DriverArrivalTime=NULL,@OnHoldUntilTime=NULL,@SalesTaxRate=0,@DiscountID=0,@DiscountLineAmount=0.45,@DiscountOrderAmount=0,@DiscountCashAmount=0,@DiscountTotalAmount=0.45,@OrderStatus=1,@BonusAmountUsed=0,@BonusAmountEarned=0,@BonusID=0,@BonusCustomerID=0,@AmountDue=4.55,@PackagerAlreadyPrinted=0,@GuestCheckPrinted=1,@GuestCheckPrintCount=2,@AdditionPrinted=0,@AdditionPrintedLineCount=0,@SurchargeID=0,@SurchargeLineAmount=10032718,@SurchargeOrderAmount=0,@SurchargeCashAmount=0,@SurchargeTotalAmount=0,@ComplimentaryAmount=0,@SubTotal=4.55,@OrderCost=0,@GratuityPercent=0,@CashGratuity=0,@SalesTaxAmount=0,@DriveThruComplete=NULL,@BarTabName=NULL,@TableReady=1,@GuestNumber=2,@SpecificCustomerName=N'test',@OrderPhone=NULL,@InvoicePrinted=0,@FiscalPrinted=NULL,@OrderNotes=NULL,@OrderExternalNotes=NULL,@LineDeleted=0,@DeleteReason=NULL,@CustomField1=NULL,@CustomField2=NULL,@CustomField3=NULL,@CustomField4=NULL,@CustomField5=NULL,@EditKey='EDFECDD1-7DC6-4AD4-B70A-01160F7428E7',@SyncKey='A5A878D5-D413-40EB-B390-2CBC04FD9844',@BranchID=422,@LockData=NULL,@LockStationID=NULL,@AddUserID=106,@AddDateTime='2026-08-16 21:50:08.260',@EditUserID=106,@EditDateTime=NULL,@CustomerKey=NULL,@DiscountKey=NULL,@EmployeeKey='4740D379-BF42-4288-BF67-4B46D795F77D',@EmployeeName=N'ROBOTPOS',@DineInTableName=N'64',@CustomerName=NULL,@AddUserName=N'ROBOTPOS',@EditUserName=N'ROBOTPOS',@DiscountAmountValue=0,@DiscountBasisValue=0,@FiscalKey=NULL,@InvoiceDetail=NULL,@RetailData=NULL,@DiscountUserName=NULL,@PosVersion=N'1.0.0.32718';

-- 12. Sətirlərin (OrderTransactions) yenilənməsi - Sətir 1 (Dönər Çörəkdə)
EXEC sp_executesql N'UPDATE [OrderTransactions] SET [TransactionDateTime]=@TransactionDateTime ,[TransactionID]=@TransactionID ,[TransactionKey]=@TransactionKey ,[OrderKey]=@OrderKey ,[OrderID]=@OrderID ,[OrderDateTime]=@OrderDateTime ,[StationID]=@StationID ,[EmployeeID]=@EmployeeID ,[RevenueCenterTypeID]=@RevenueCenterTypeID ,[MenuItemID]=@MenuItemID ,[MenuItemKey]=@MenuItemKey ,[MenuItemText]=@MenuItemText ,[MenuItemGroupText]=@MenuItemGroupText ,[MenumItemCategoryText]=@MenumItemCategoryText ,[MenuItemUnitPrice]=@MenuItemUnitPrice ,[MenuItemCost]=@MenuItemCost ,[Quantity]=@Quantity ,[ExtendedPrice]=@ExtendedPrice ,[DiscountID]=@DiscountID ,[DiscountKey]=@DiscountKey ,[DiscountLineAmount]=@DiscountLineAmount ,[DiscountCashAmount]=@DiscountCashAmount ,[DiscountTotalAmount]=@DiscountTotalAmount ,[TransactionStatus]=@TransactionStatus ,[NotificationStatus]=@NotificationStatus ,[AdditionLinePrinted]=@AdditionLinePrinted ,[TaxPercent]=@TaxPercent ,[RoundID]=@RoundID ,[Mod1ID]=@Mod1ID ,[Mod1Cost]=@Mod1Cost ,[Mod2ID]=@Mod2ID ,[Mod2Cost]=@Mod2Cost ,[Mod3ID]=@Mod3ID ,[Mod3Cost]=@Mod3Cost ,[Mod4ID]=@Mod4ID ,[Mod4Cost]=@Mod4Cost ,[Mod5ID]=@Mod5ID ,[Mod5Cost]=@Mod5Cost ,[Mod6ID]=@Mod6ID ,[Mod6Cost]=@Mod6Cost ,[Mod7ID]=@Mod7ID ,[Mod7Cost]=@Mod7Cost ,[Mod8ID]=@Mod8ID ,[Mod8Cost]=@Mod8Cost ,[Mod9ID]=@Mod9ID ,[Mod9Cost]=@Mod9Cost ,[Mod10ID]=@Mod10ID ,[Mod10Cost]=@Mod10Cost ,[Mod11ID]=@Mod11ID ,[Mod11Cost]=@Mod11Cost ,[Mod12ID]=@Mod12ID ,[Mod12Cost]=@Mod12Cost ,[Mod13ID]=@Mod13ID ,[Mod13Cost]=@Mod13Cost ,[Mod14ID]=@Mod14ID ,[Mod14Cost]=@Mod14Cost ,[Mod15ID]=@Mod15ID ,[Mod15Cost]=@Mod15Cost ,[Mod16ID]=@Mod16ID ,[Mod16Cost]=@Mod16Cost ,[Mod17ID]=@Mod17ID ,[Mod17Cost]=@Mod17Cost ,[Mod18ID]=@Mod18ID ,[Mod18Cost]=@Mod18Cost ,[Mod19ID]=@Mod19ID ,[Mod19Cost]=@Mod19Cost ,[Mod20ID]=@Mod20ID ,[Mod20Cost]=@Mod20Cost ,[SeatNumber]=@SeatNumber ,[OnHoldUntilTime]=@OnHoldUntilTime ,[Notes]=@Notes ,[SaleTaxAmount]=@SaleTaxAmount ,[TaxPercentReduction]=@TaxPercentReduction ,[LineDeleted]=@LineDeleted ,[DeleteReason]=@DeleteReason ,[EditKey]=NEWID(), [SyncKey]=NEWID(), [BranchID]=@BranchID, [EmployeeName]=@EmployeeName, [SplitDetail]=@SplitDetail, ExternalOrderStatus=isnull(@TransactionStatus,0)-1, [PromotionKey]=@PromotionKey, [UsedPrinterComplated1]=@UsedPrinterComplated1, [DiscountBasisValue]=@DiscountBasisValue, [DiscountAmountValue]=@DiscountAmountValue, [UsedDiscountName]=@UsedDiscountName, [RetailData]=@RetailData, [OrderByWeight]=@OrderByWeight, [DiscountUserName]=@DiscountUserName, [PromotionAmount]=@PromotionAmount, [PromotionCount]=@PromotionCount, [PosVersion]=@PosVersion WHERE [AutoID]=@AutoID and [EditKey]=@EditKey',
N'@AutoID int,@TransactionDateTime datetime,@TransactionID int,@TransactionKey uniqueidentifier,@OrderKey uniqueidentifier,@OrderID int,@OrderDateTime datetime,@StationID int,@EmployeeID int,@RevenueCenterTypeID int,@MenuItemID int,@MenuItemKey uniqueidentifier,@MenuItemText nvarchar(13),@MenuItemGroupText nvarchar(5),@MenumItemCategoryText nvarchar(6),@MenuItemUnitPrice float,@MenuItemCost float,@Quantity float,@ExtendedPrice float,@DiscountID int,@DiscountKey uniqueidentifier,@DiscountLineAmount float,@DiscountCashAmount float,@DiscountTotalAmount float,@TransactionStatus int,@NotificationStatus int,@AdditionLinePrinted bit,@TaxPercent float,@RoundID int,@Mod1ID int,@Mod1Cost float,@Mod2ID int,@Mod2Cost float,@Mod3ID int,@Mod3Cost float,@Mod4ID int,@Mod4Cost float,@Mod5ID int,@Mod5Cost float,@Mod6ID int,@Mod6Cost float,@Mod7ID int,@Mod7Cost float,@Mod8ID int,@Mod8Cost float,@Mod9ID int,@Mod9Cost float,@Mod10ID int,@Mod10Cost float,@Mod11ID int,@Mod11Cost float,@Mod12ID int,@Mod12Cost float,@Mod13ID int,@Mod13Cost float,@Mod14ID int,@Mod14Cost float,@Mod15ID int,@Mod15Cost float,@Mod16ID int,@Mod16Cost float,@Mod17ID int,@Mod17Cost float,@Mod18ID int,@Mod18Cost float,@Mod19ID int,@Mod19Cost float,@Mod20ID int,@Mod20Cost float,@SeatNumber int,@OnHoldUntilTime datetime,@Notes nvarchar(4000),@SaleTaxAmount float,@TaxPercentReduction float,@UsedPrinterID1 int,@UsedPrinterID2 int,@UsedPrinterID3 int,@UsedPrinterID4 int,@UsedPrinterID5 int,@UsedPrinterComplated1 bit,@UsedPrinterComplated2 bit,@UsedPrinterComplated3 bit,@UsedPrinterComplated4 bit,@UsedPrinterComplated5 bit,@LineDeleted bit,@DeleteReason nvarchar(4000),@CustomField1 nvarchar(4000),@CustomField2 nvarchar(4000),@CustomField3 nvarchar(4000),@CustomField4 nvarchar(4000),@CustomField5 nvarchar(4000),@EditKey uniqueidentifier,@SyncKey uniqueidentifier,@BranchID int,@AddUserID int,@AddDateTime datetime,@EditUserID int,@EditDateTime datetime,@AddUserName nvarchar(8),@EditUserName nvarchar(8),@EmployeeName nvarchar(4000),@SplitDetail nvarchar(4000),@PromotionKey uniqueidentifier,@DiscountAmountValue float,@DiscountBasisValue int,@UsedDiscountName nvarchar(4000),@PromotionName nvarchar(4000),@RetailData nvarchar(4000),@OrderByWeight bit,@DiscountUserName nvarchar(1),@PromotionAmount float,@PromotionCount float,@PosVersion nvarchar(11)',
@AutoID=72,@TransactionDateTime='2026-08-16 21:50:08.327',@TransactionID=72,@TransactionKey='7654D17D-CC5D-457B-9B04-4B4E63BB8679',@OrderKey='8A86D18A-F338-4E0A-B963-2B487A8B26BD',@OrderID=12,@OrderDateTime='2026-08-16 21:50:08.260',@StationID=1,@EmployeeID=106,@RevenueCenterTypeID=99,@MenuItemID=4,@MenuItemKey='FDE08677-5B6D-4C9E-9CB0-F1CB2462BFBA',@MenuItemText=N'Dönər Çörəkdə',@MenuItemGroupText=N'DÖNER',@MenumItemCategoryText=N'MUTFAK',@MenuItemUnitPrice=2,@MenuItemCost=0,@Quantity=1,@ExtendedPrice=2,@DiscountID=0,@DiscountKey=NULL,@DiscountLineAmount=0,@DiscountCashAmount=0,@DiscountTotalAmount=0,@TransactionStatus=1,@NotificationStatus=1,@AdditionLinePrinted=0,@TaxPercent=0,@RoundID=NULL,@Mod1ID=0,@Mod1Cost=0,@Mod2ID=0,@Mod2Cost=0,@Mod3ID=0,@Mod3Cost=0,@Mod4ID=0,@Mod4Cost=0,@Mod5ID=0,@Mod5Cost=0,@Mod6ID=0,@Mod6Cost=0,@Mod7ID=0,@Mod7Cost=0,@Mod8ID=0,@Mod8Cost=0,@Mod9ID=0,@Mod9Cost=0,@Mod10ID=0,@Mod10Cost=0,@Mod11ID=0,@Mod11Cost=0,@Mod12ID=0,@Mod12Cost=0,@Mod13ID=0,@Mod13Cost=0,@Mod14ID=0,@Mod14Cost=0,@Mod15ID=0,@Mod15Cost=0,@Mod16ID=0,@Mod16Cost=0,@Mod17ID=0,@Mod17Cost=0,@Mod18ID=0,@Mod18Cost=0,@Mod19ID=0,@Mod19Cost=0,@Mod20ID=0,@Mod20Cost=0,@SeatNumber=1,@OnHoldUntilTime='2026-08-16 21:50:07.943',@Notes=NULL,@SaleTaxAmount=0,@TaxPercentReduction=0,@UsedPrinterID1=0,@UsedPrinterID2=0,@UsedPrinterID3=0,@UsedPrinterID4=0,@UsedPrinterID5=0,@UsedPrinterComplated1=0,@UsedPrinterComplated2=0,@UsedPrinterComplated3=0,@UsedPrinterComplated4=0,@UsedPrinterComplated5=0,@LineDeleted=0,@DeleteReason=NULL,@CustomField1=NULL,@CustomField2=NULL,@CustomField3=NULL,@CustomField4=NULL,@CustomField5=NULL,@EditKey='C8C577DD-55F6-4C56-9DC4-4705E671B6E9',@SyncKey='E321B954-93CD-4744-BE02-B30222BD0C48',@BranchID=422,@AddUserID=106,@AddDateTime='2026-08-16 21:50:08.327',@EditUserID=NULL,@EditDateTime=NULL,@AddUserName=N'ROBOTPOS',@EditUserName=N'ROBOTPOS',@EmployeeName=NULL,@SplitDetail=NULL,@PromotionKey='00000000-0000-0000-0000-000000000000',@DiscountAmountValue=0,@DiscountBasisValue=0,@UsedDiscountName=NULL,@PromotionName=NULL,@RetailData=NULL,@OrderByWeight=0,@DiscountUserName=N'0',@PromotionAmount=NULL,@PromotionCount=NULL,@PosVersion=N'1.0.0.32718';

-- 13. Sətirlərin (OrderTransactions) yenilənməsi - Sətir 2 (Dönər Ət - 15% Endirim tətbiqi)
EXEC sp_executesql N'UPDATE [OrderTransactions] SET [TransactionDateTime]=@TransactionDateTime ,[TransactionID]=@TransactionID ,[TransactionKey]=@TransactionKey ,[OrderKey]=@OrderKey ,[OrderID]=@OrderID ,[OrderDateTime]=@OrderDateTime ,[StationID]=@StationID ,[EmployeeID]=@EmployeeID ,[RevenueCenterTypeID]=@RevenueCenterTypeID ,[MenuItemID]=@MenuItemID ,[MenuItemKey]=@MenuItemKey ,[MenuItemText]=@MenuItemText ,[MenuItemGroupText]=@MenuItemGroupText ,[MenumItemCategoryText]=@MenumItemCategoryText ,[MenuItemUnitPrice]=@MenuItemUnitPrice ,[MenuItemCost]=@MenuItemCost ,[Quantity]=@Quantity ,[ExtendedPrice]=@ExtendedPrice ,[DiscountID]=@DiscountID ,[DiscountKey]=@DiscountKey ,[DiscountLineAmount]=@DiscountLineAmount ,[DiscountCashAmount]=@DiscountCashAmount ,[DiscountTotalAmount]=@DiscountTotalAmount ,[TransactionStatus]=@TransactionStatus ,[NotificationStatus]=@NotificationStatus ,[AdditionLinePrinted]=@AdditionLinePrinted ,[TaxPercent]=@TaxPercent ,[RoundID]=@RoundID ,[Mod1ID]=@Mod1ID ,[Mod1Cost]=@Mod1Cost ,[Mod2ID]=@Mod2ID ,[Mod2Cost]=@Mod2Cost ,[Mod3ID]=@Mod3ID ,[Mod3Cost]=@Mod3Cost ,[Mod4ID]=@Mod4ID ,[Mod4Cost]=@Mod4Cost ,[Mod5ID]=@Mod5ID ,[Mod5Cost]=@Mod5Cost ,[Mod6ID]=@Mod6ID ,[Mod6Cost]=@Mod6Cost ,[Mod7ID]=@Mod7ID ,[Mod7Cost]=@Mod7Cost ,[Mod8ID]=@Mod8ID ,[Mod8Cost]=@Mod8Cost ,[Mod9ID]=@Mod9ID ,[Mod9Cost]=@Mod9Cost ,[Mod10ID]=@Mod10ID ,[Mod10Cost]=@Mod10Cost ,[Mod11ID]=@Mod11ID ,[Mod11Cost]=@Mod11Cost ,[Mod12ID]=@Mod12ID ,[Mod12Cost]=@Mod12Cost ,[Mod13ID]=@Mod13ID ,[Mod13Cost]=@Mod13Cost ,[Mod14ID]=@Mod14ID ,[Mod14Cost]=@Mod14Cost ,[Mod15ID]=@Mod15ID ,[Mod15Cost]=@Mod15Cost ,[Mod16ID]=@Mod16ID ,[Mod16Cost]=@Mod16Cost ,[Mod17ID]=@Mod17ID ,[Mod17Cost]=@Mod17Cost ,[Mod18ID]=@Mod18ID ,[Mod18Cost]=@Mod18Cost ,[Mod19ID]=@Mod19ID ,[Mod19Cost]=@Mod19Cost ,[Mod20ID]=@Mod20ID ,[Mod20Cost]=@Mod20Cost ,[SeatNumber]=@SeatNumber ,[OnHoldUntilTime]=@OnHoldUntilTime ,[Notes]=@Notes ,[SaleTaxAmount]=@SaleTaxAmount ,[TaxPercentReduction]=@TaxPercentReduction ,[LineDeleted]=@LineDeleted ,[DeleteReason]=@DeleteReason ,[EditKey]=NEWID(), [SyncKey]=NEWID(), [BranchID]=@BranchID, [EmployeeName]=@EmployeeName, [SplitDetail]=@SplitDetail, ExternalOrderStatus=isnull(@TransactionStatus,0)-1, [PromotionKey]=@PromotionKey, [UsedPrinterComplated1]=@UsedPrinterComplated1, [DiscountBasisValue]=@DiscountBasisValue, [DiscountAmountValue]=@DiscountAmountValue, [UsedDiscountName]=@UsedDiscountName, [RetailData]=@RetailData, [OrderByWeight]=@OrderByWeight, [DiscountUserName]=@DiscountUserName, [PromotionAmount]=@PromotionAmount, [PromotionCount]=@PromotionCount, [PosVersion]=@PosVersion WHERE [AutoID]=@AutoID and [EditKey]=@EditKey',
N'@AutoID int,@TransactionDateTime datetime,@TransactionID int,@TransactionKey uniqueidentifier,@OrderKey uniqueidentifier,@OrderID int,@OrderDateTime datetime,@StationID int,@EmployeeID int,@RevenueCenterTypeID int,@MenuItemID int,@MenuItemKey uniqueidentifier,@MenuItemText nvarchar(8),@MenuItemGroupText nvarchar(5),@MenumItemCategoryText nvarchar(6),@MenuItemUnitPrice float,@MenuItemCost float,@Quantity float,@ExtendedPrice float,@DiscountID int,@DiscountKey uniqueidentifier,@DiscountLineAmount float,@DiscountCashAmount float,@DiscountTotalAmount float,@TransactionStatus int,@NotificationStatus int,@AdditionLinePrinted bit,@TaxPercent float,@RoundID int,@Mod1ID int,@Mod1Cost float,@Mod2ID int,@Mod2Cost float,@Mod3ID int,@Mod3Cost float,@Mod4ID int,@Mod4Cost float,@Mod5ID int,@Mod5Cost float,@Mod6ID int,@Mod6Cost float,@Mod7ID int,@Mod7Cost float,@Mod8ID int,@Mod8Cost float,@Mod9ID int,@Mod9Cost float,@Mod10ID int,@Mod10Cost float,@Mod11ID int,@Mod11Cost float,@Mod12ID int,@Mod12Cost float,@Mod13ID int,@Mod13Cost float,@Mod14ID int,@Mod14Cost float,@Mod15ID int,@Mod15Cost float,@Mod16ID int,@Mod16Cost float,@Mod17ID int,@Mod17Cost float,@Mod18ID int,@Mod18Cost float,@Mod19ID int,@Mod19Cost float,@Mod20ID int,@Mod20Cost float,@SeatNumber int,@OnHoldUntilTime datetime,@Notes nvarchar(4000),@SaleTaxAmount float,@TaxPercentReduction float,@UsedPrinterID1 int,@UsedPrinterID2 int,@UsedPrinterID3 int,@UsedPrinterID4 int,@UsedPrinterID5 int,@UsedPrinterComplated1 bit,@UsedPrinterComplated2 bit,@UsedPrinterComplated3 bit,@UsedPrinterComplated4 bit,@UsedPrinterComplated5 bit,@LineDeleted bit,@DeleteReason nvarchar(4000),@CustomField1 nvarchar(4000),@CustomField2 nvarchar(8),@CustomField3 nvarchar(4000),@CustomField4 nvarchar(4000),@CustomField5 nvarchar(4000),@EditKey uniqueidentifier,@SyncKey uniqueidentifier,@BranchID int,@AddUserID int,@AddDateTime datetime,@EditUserID int,@EditDateTime datetime,@AddUserName nvarchar(8),@EditUserName nvarchar(8),@EmployeeName nvarchar(4000),@SplitDetail nvarchar(4000),@PromotionKey uniqueidentifier,@DiscountAmountValue float,@DiscountBasisValue int,@UsedDiscountName nvarchar(19),@PromotionName nvarchar(4000),@RetailData nvarchar(4000),@OrderByWeight bit,@DiscountUserName nvarchar(8),@PromotionAmount float,@PromotionCount float,@PosVersion nvarchar(11)',
@AutoID=73,@TransactionDateTime='2026-08-16 21:50:08.420',@TransactionID=73,@TransactionKey='8F9EFB8F-2190-4C2E-AC8A-D7AEF297AACB',@OrderKey='8A86D18A-F338-4E0A-B963-2B487A8B26BD',@OrderID=12,@OrderDateTime='2026-08-16 21:50:08.260',@StationID=1,@EmployeeID=106,@RevenueCenterTypeID=99,@MenuItemID=5,@MenuItemKey='4D3B6A48-D341-4B1D-A039-63F445804FC1',@MenuItemText=N'Dönər Ət',@MenuItemGroupText=N'DÖNER',@MenumItemCategoryText=N'MUTFAK',@MenuItemUnitPrice=3,@MenuItemCost=0,@Quantity=1,@ExtendedPrice=2.55,@DiscountID=16,@DiscountKey='C1F19A2F-9F77-401E-AE13-108884D52544',@DiscountLineAmount=0.45,@DiscountCashAmount=0,@DiscountTotalAmount=0.45,@TransactionStatus=1,@NotificationStatus=1,@AdditionLinePrinted=0,@TaxPercent=0,@RoundID=NULL,@Mod1ID=0,@Mod1Cost=0,@Mod2ID=0,@Mod2Cost=0,@Mod3ID=0,@Mod3Cost=0,@Mod4ID=0,@Mod4Cost=0,@Mod5ID=0,@Mod5Cost=0,@Mod6ID=0,@Mod6Cost=0,@Mod7ID=0,@Mod7Cost=0,@Mod8ID=0,@Mod8Cost=0,@Mod9ID=0,@Mod9Cost=0,@Mod10ID=0,@Mod10Cost=0,@Mod11ID=0,@Mod11Cost=0,@Mod12ID=0,@Mod12Cost=0,@Mod13ID=0,@Mod13Cost=0,@Mod14ID=0,@Mod14Cost=0,@Mod15ID=0,@Mod15Cost=0,@Mod16ID=0,@Mod16Cost=0,@Mod17ID=0,@Mod17Cost=0,@Mod18ID=0,@Mod18Cost=0,@Mod19ID=0,@Mod19Cost=0,@Mod20ID=0,@Mod20Cost=0,@SeatNumber=1,@OnHoldUntilTime='2026-08-16 21:50:08.007',@Notes=NULL,@SaleTaxAmount=0,@TaxPercentReduction=0,@UsedPrinterID1=0,@UsedPrinterID2=0,@UsedPrinterID3=0,@UsedPrinterID4=0,@UsedPrinterID5=0,@UsedPrinterComplated1=0,@UsedPrinterComplated2=0,@UsedPrinterComplated3=0,@UsedPrinterComplated4=0,@UsedPrinterComplated5=0,@LineDeleted=0,@DeleteReason=NULL,@CustomField1=NULL,@CustomField2=N'ROBOTPOS',@CustomField3=NULL,@CustomField4=NULL,@CustomField5=NULL,@EditKey='A0B12D4F-F9C3-45C8-8944-5215AEED1939',@SyncKey='7FFC9754-0857-49F4-BE84-65F5F1CCF938',@BranchID=422,@AddUserID=106,@AddDateTime='2026-08-16 21:50:08.420',@EditUserID=106,@EditDateTime=NULL,@AddUserName=N'ROBOTPOS',@EditUserName=N'ROBOTPOS',@EmployeeName=NULL,@SplitDetail=NULL,@PromotionKey='00000000-0000-0000-0000-000000000000',@DiscountAmountValue=15,@DiscountBasisValue=0,@UsedDiscountName=N'INDIRIM KUPONU 15 %',@PromotionName=NULL,@RetailData=NULL,@OrderByWeight=0,@DiscountUserName=N'ROBOTPOS',@PromotionAmount=NULL,@PromotionCount=NULL,@PosVersion=N'1.0.0.32718';

-- 14. İnteqrasiya və ID sinxronizasiyası
EXEC sp_executesql N'EXEC dbo.fncUpdateReceiptNo; 
UPDATE OrderTransactions SET TransactionID = AutoID WHERE (isnull(TransactionID,0)=0 or isnull(TransactionID,0)<>AutoID) and OrderKey=@OrderKey;
UPDATE OrderTransactions SET OrderID = OrderHeaders.OrderID, OrderDateTime = OrderHeaders.OrderDateTime FROM OrderTransactions INNER JOIN OrderHeaders ON OrderTransactions.OrderKey = OrderHeaders.OrderKey WHERE (isnull(OrderTransactions.OrderID,0)=0 or isnull(OrderTransactions.OrderID,0)<>OrderHeaders.AutoID) and OrderTransactions.OrderKey=@OrderKey;
UPDATE OrderPayments SET OrderPaymentID = AutoID WHERE (isnull(OrderPaymentID,0)=0 or isnull(OrderPaymentID,0)<>AutoID) and OrderKey=@OrderKey;
UPDATE OrderPayments SET OrderID = OrderHeaders.OrderID FROM OrderPayments INNER JOIN OrderHeaders ON OrderPayments.OrderKey = OrderHeaders.OrderKey WHERE (isnull(OrderPayments.OrderID,0)=0 or isnull(OrderPayments.OrderID,0)<>OrderPayments.OrderID) and OrderPayments.OrderKey=@OrderKey;',
N'@OrderKey uniqueidentifier',
@OrderKey='8A86D18A-F338-4E0A-B963-2B487A8B26BD';

-- 15. Parametrin yenilənməsi
DELETE FROM Params WHERE ParamName='OrderSaveTime';
INSERT INTO Params(ParamName,ParamValue) VALUES ('OrderSaveTime',GETDATE());

-- 16. Alt məhsul (modifier/combo) mətn və əlaqələrinin yenilənməsi
UPDATE t1 
SET 
	MainMenuItemText = t2.MenuItemText,
	MainMenuItemTransactionKey = t2.TransactionKey, 
	SendOk = 0
FROM OrderTransactions t1
LEFT JOIN dbo.OrderTransactions t2 ON t2.OrderKey = t1.OrderKey 
	AND t2.CustomField1 = SUBSTRING(t1.CustomField1, 0, CHARINDEX('-', t1.CustomField1) + 1) 
	AND t2.TransactionKey <> t1.TransactionKey 
WHERE 
	t1.CustomField1 IS NOT NULL 
	AND t1.MainMenuItemText IS NULL 
	AND t2.TransactionKey IS NOT NULL;

-- 17. Valyuta qarşılıqlarının (USD, EUR, GBP) yenilənməsi
EXEC sp_executesql N'
UPDATE h 
	SET 
	UsdAmount = CAST(ROUND(h.AmountDue/ISNULL(m1.ExchangeRate,1),2) AS DECIMAL(18,2)),
	EurAmount = CAST(ROUND(h.AmountDue/ISNULL(m2.ExchangeRate,1),2) AS DECIMAL(18,2)),
	GbpAmount = CAST(ROUND(h.AmountDue/ISNULL(m3.ExchangeRate,1),2) AS DECIMAL(18,2))
FROM OrderHeaders h
	LEFT JOIN PaymentMethods m1 ON m1.PaymentName IN (''DOLAR'', ''USD'')
	LEFT JOIN PaymentMethods m2 ON m2.PaymentName IN (''EURO'', ''EUR'')
	LEFT JOIN PaymentMethods m3 ON m3.PaymentName IN (''STERLIN'', ''GBP'')
WHERE 
	h.OrderKey=@orderKey;',
N'@OrderKey uniqueidentifier',
@OrderKey='8A86D18A-F338-4E0A-B963-2B487A8B26BD';

-- 18. Yenilənmiş Sifariş Başlığının təkrar oxunması
EXEC sp_executesql N'SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SELECT OrderHeaders.AutoID, isnull(OrderHeaders.AutoID,0) as OrderID, OrderHeaders.ReceiptNo, OrderHeaders.MainOrderKey, OrderHeaders.MainOrderID, OrderHeaders.OrderTypeSourceID, 
 isnull(OrderHeaders.OrderTypeSourceExternalNo,'''') as OrderTypeSourceExternalNo, OrderHeaders.OrderKey, OrderHeaders.OrderDateTime, OrderHeaders.EmployeeID, OrderHeaders.StationID, 
 OrderHeaders.RevenueCenterTypeID, OrderHeaders.OrderType, OrderHeaders.DineInTableID, OrderHeaders.CustomerID, OrderHeaders.DeliveryCharge, 
 OrderHeaders.DeliveryComp, OrderHeaders.DeliveryZoneID, OrderHeaders.DriverEmployeeID, OrderHeaders.DriverDepartureTime, OrderHeaders.DriverArrivalTime, 
 OrderHeaders.OnHoldUntilTime, OrderHeaders.SalesTaxRate, OrderHeaders.DiscountID, OrderHeaders.DiscountLineAmount, OrderHeaders.DiscountOrderAmount, 
 OrderHeaders.DiscountCashAmount, OrderHeaders.DiscountTotalAmount, OrderHeaders.OrderStatus, OrderHeaders.BonusAmountUsed, 
 OrderHeaders.BonusAmountEarned, OrderHeaders.BonusID, OrderHeaders.BonusCustomerID, (isnull(OrderHeaders.AmountDue,0.0)+isnull(OrderHeaders.CashGratuity,0.0)) AS GrandTotal, OrderHeaders.AmountDue, OrderHeaders.PackagerAlreadyPrinted, OrderHeaders.GuestCheckPrinted, 
 OrderHeaders.GuestCheckPrintCount, OrderHeaders.AdditionPrinted, isnull(OrderHeaders.AdditionPrintedLineCount,0) as AdditionPrintedLineCount, OrderHeaders.SurchargeID, OrderHeaders.SurchargeLineAmount, 
 OrderHeaders.SurchargeOrderAmount, OrderHeaders.SurchargeCashAmount, OrderHeaders.SurchargeTotalAmount, OrderHeaders.ComplimentaryAmount, 
 OrderHeaders.SubTotal, OrderHeaders.OrderCost, OrderHeaders.GratuityPercent, OrderHeaders.CashGratuity, OrderHeaders.SalesTaxAmount, isnull(OrderHeaders.IsProduced,0) as IsProduced, 
 OrderHeaders.DriveThruComplete, OrderHeaders.BarTabName, OrderHeaders.TableReady, OrderHeaders.GuestNumber, OrderHeaders.SpecificCustomerName, OrderHeaders.OrderPhone, 
 OrderHeaders.InvoicePrinted, OrderHeaders.FiscalPrinted, OrderHeaders.OrderNotes, OrderHeaders.OrderExternalNotes, OrderHeaders.LineDeleted, OrderHeaders.DeleteReason, 
 OrderHeaders.CustomField1, OrderHeaders.CustomField2, OrderHeaders.CustomField3, OrderHeaders.CustomField4, OrderHeaders.CustomField5, '''' as GratuityText, 
 OrderHeaders.CustomField6, OrderHeaders.CustomField7, OrderHeaders.CustomField8, OrderHeaders.CustomField9, OrderHeaders.CustomField10, 
 OrderHeaders.EditKey, OrderHeaders.SyncKey, OrderHeaders.BranchID, OrderHeaders.LockData, OrderHeaders.LockStationID, OrderHeaders.AddUserID, 
 OrderHeaders.AddDateTime, OrderHeaders.EditUserID, OrderHeaders.EditDateTime, isnull(OrderHeaders.EmployeeName,'''') AS EmployeeName, DineInTables.DineInTableText, 
 isnull(OrderHeaders.CustomerName,'''') as CustomerName, de.FirstName AS DriverEmployeeName, isnull(OrderHeaders.AddUserName,'''') AS AddEmployeeName, isnull(OrderHeaders.EditUserName,'''') AS EditEmployeeName, '''' as OrderInfo 
 ,(CASE dbo.OrderHeaders.OrderType WHEN 1 THEN ''MASA'' WHEN 2 THEN ''BAR SATIŞI'' WHEN 3 THEN ''AL GÖTÜR'' WHEN 4 THEN ''TEZGAH SATIŞI'' WHEN 5 THEN ''PAKET SATIŞI'' WHEN 66 THEN ''İADE'' ELSE ''-'' END) AS OrderTypeName 
 ,isnull(OrderHeaders.DiscountAmountValue,0.0) as DiscountAmountValue, isnull(OrderHeaders.DiscountBasisValue,0) as DiscountBasisValue, 
 OrderHeaders.EmployeeKey, OrderHeaders.CustomerKey, OrderHeaders.DiscountKey, '''' as AmountText, isnull(Discounts.DiscountText,'''') AS DiscountText, 
 (CASE OrderHeaders.OrderStatus WHEN 1 THEN ''AÇIK'' WHEN 2 THEN ''KAPALI'' WHEN 3 THEN ''İPTAL'' ELSE ''-'' END) AS OrderStatusName 
 ,isnull(br.BranchName,'''') AS BranchName, (ISNULL(OrderHeaders.AmountDue,0.0)+ISNULL(OrderHeaders.DiscountTotalAmount,0.0)) AS TotalPrice, isnull(OrderHeaders.ExternalOrderStatus,0) as ExternalOrderStatus, 
 isnull(Discounts.DiscountAmount,0) AS DiscountPercent, isnull(OrderHeaders.DineInTableName,'''') AS DineInTableName, isnull(DineInTableGroups.TableGroupText,'''') AS TableGroupText, isnull(OrderHeaders.AddUserName,'''') AS AddUserName, isnull(OrderHeaders.EditUserName,'''') AS EditUserName, isnull(OrderHeaders.FiscalKey,'''') AS FiscalKey, 
 isnull(OrderHeaders.InvoiceDetail,'''') AS InvoiceDetail, isnull(OrderHeaders.FiscalStatus,0) AS FiscalStatus, isnull(OrderHeaders.TsmStatus,0) AS TsmStatus, isnull(OrderHeaders.RetailData,'''') AS RetailData, isnull(OrderHeaders.DiscountUserName,'''') AS DiscountUserName, OrderHeaders.PaperNumber, OrderHeaders.ReturnType, OrderHeaders.ReturnOrderNo, OrderHeaders.ReturnCustomerName, OrderHeaders.ReturnCustomerAddress, OrderHeaders.ReturnTaxNumber, OrderHeaders.ReturnTaxOffice, OrderHeaders.ReturnSerialNo, OrderHeaders.ReturnReason, OrderHeaders.ReturnReasonCode, ISNULL(OrderHeaders.UsdAmount,0) AS UsdAmount, ISNULL(OrderHeaders.EurAmount,0) AS EurAmount, ISNULL(OrderHeaders.GbpAmount,0) AS GbpAmount, ISNULL(OrderHeaders.OrderCounter,0) AS OrderCounter 
 FROM OrderHeaders WITH (NOLOCK) 
 LEFT OUTER JOIN DineInTables ON OrderHeaders.DineInTableID = DineInTables.DineInTableID 
 LEFT OUTER JOIN DineInTableGroups ON DineInTables.TableGroupID = DineInTableGroups.AutoID 
 LEFT OUTER JOIN EmployeeFiles AS de ON OrderHeaders.DriverEmployeeID = de.AutoID 
 LEFT OUTER JOIN EmployeeFiles AS ee ON OrderHeaders.EditUserID = ee.AutoID 
 LEFT OUTER JOIN EmployeeFiles AS ea ON OrderHeaders.AddUserID = ea.AutoID 
 LEFT OUTER JOIN CustomerFiles ON OrderHeaders.CustomerKey = CustomerFiles.CustomerKey 
 LEFT OUTER JOIN EmployeeFiles AS e ON OrderHeaders.EmployeeID = e.AutoID 
 LEFT OUTER JOIN efr_Branchs AS br ON OrderHeaders.BranchID = br.BranchID 
 LEFT OUTER JOIN Discounts ON OrderHeaders.DiscountKey = Discounts.DiscountKey 
 WHERE OrderHeaders.[OrderKey]=@OrderKey',
N'@OrderKey uniqueidentifier',
@OrderKey='8A86D18A-F338-4E0A-B963-2B487A8B26BD';

-- 19. Sifariş sətirlərinin təkrar oxunması
EXEC sp_executesql N'SELECT DISTINCT OrderTransactions.AutoID, OrderTransactions.TransactionDateTime, OrderTransactions.TransactionID, OrderTransactions.TransactionKey, 
 OrderTransactions.OrderKey, OrderTransactions.OrderID, OrderTransactions.OrderDateTime, OrderTransactions.StationID, OrderTransactions.EmployeeID, 
 OrderTransactions.RevenueCenterTypeID, OrderTransactions.MenuItemID, OrderTransactions.MenuItemText, '''' as DisplayText, OrderTransactions.MenuItemGroupText, 
 OrderTransactions.MenumItemCategoryText, OrderTransactions.MenuItemUnitPrice, OrderTransactions.MenuItemCost, OrderTransactions.Quantity, 
 OrderTransactions.ExtendedPrice, OrderTransactions.DiscountID, OrderTransactions.DiscountLineAmount, OrderTransactions.DiscountCashAmount, 
 OrderTransactions.DiscountTotalAmount, OrderTransactions.TransactionStatus, OrderTransactions.NotificationStatus, OrderTransactions.AdditionLinePrinted, 
 isnull(OrderTransactions.TaxPercent,0) as TaxPercent, OrderTransactions.Mod1ID, OrderTransactions.Mod1Cost, OrderTransactions.Mod2ID, OrderTransactions.Mod2Cost, 
 OrderTransactions.Mod3ID, OrderTransactions.Mod3Cost, OrderTransactions.Mod4ID, OrderTransactions.Mod4Cost, OrderTransactions.Mod5ID, 
 OrderTransactions.Mod5Cost, OrderTransactions.Mod6ID, OrderTransactions.Mod6Cost, OrderTransactions.Mod7ID, OrderTransactions.Mod7Cost, 
 OrderTransactions.Mod8ID, OrderTransactions.Mod8Cost, OrderTransactions.Mod9ID, OrderTransactions.Mod9Cost, OrderTransactions.Mod10ID, 
 OrderTransactions.Mod10Cost, OrderTransactions.Mod11ID, OrderTransactions.Mod11Cost, OrderTransactions.Mod12ID, OrderTransactions.Mod12Cost, 
 OrderTransactions.Mod13ID, OrderTransactions.Mod13Cost, OrderTransactions.Mod14ID, OrderTransactions.Mod14Cost, OrderTransactions.Mod15ID, 
 OrderTransactions.Mod15Cost, OrderTransactions.Mod16ID, OrderTransactions.Mod16Cost, OrderTransactions.Mod17ID, OrderTransactions.Mod17Cost, 
 OrderTransactions.Mod18ID, OrderTransactions.Mod18Cost, OrderTransactions.Mod19ID, OrderTransactions.Mod19Cost, OrderTransactions.Mod20ID, 
 OrderTransactions.Mod20Cost, OrderTransactions.SeatNumber, OrderTransactions.OnHoldUntilTime, OrderTransactions.Notes, isnull(OrderTransactions.SaleTaxAmount,0) as SaleTaxAmount, isnull(OrderTransactions.TaxPercentReduction,0) as TaxPercentReduction, 
 OrderTransactions.UsedPrinterID1, OrderTransactions.UsedPrinterID2, OrderTransactions.UsedPrinterID3, OrderTransactions.UsedPrinterID4, 
 OrderTransactions.UsedPrinterID5, OrderTransactions.UsedPrinterComplated1, OrderTransactions.UsedPrinterComplated2, 
 OrderTransactions.UsedPrinterComplated3, OrderTransactions.UsedPrinterComplated4, OrderTransactions.UsedPrinterComplated5, OrderTransactions.LineDeleted, 
 OrderTransactions.DeleteReason, isnull(OrderTransactions.CustomField1,'''') as CustomField1, isnull(OrderTransactions.CustomField2,'''') as CustomField2, OrderTransactions.CustomField3, 
 OrderTransactions.CustomField4, OrderTransactions.CustomField5, OrderTransactions.EditKey, OrderTransactions.SyncKey, OrderTransactions.BranchID, 
 isnull(OrderTransactions.SplitDetail,'''') as SplitDetail, OrderTransactions.AddUserID, OrderTransactions.AddDateTime, OrderTransactions.EditUserID, OrderTransactions.EditDateTime, isnull(OrderTransactions.EmployeeName,'''') AS EmployeeName, 
 isnull(OrderTransactions.AddUserName,'''') AS AddEmployeeName, isnull(OrderTransactions.EditUserName,'''') AS EditUserName, isnull(OrderTransactions.EditUserName,'''') AS EditEmployeeName, isnull(Discounts.DiscountText,'''') as DiscountText, isnull(OrderTransactions.DiscountAmountValue,0) as DiscountAmountValue, isnull(OrderTransactions.DiscountBasisValue,0) as DiscountBasisValue, 
 mm1.MenuModifierText AS Mod1IDtext, mm2.MenuModifierText AS Mod2IDtext, mm3.MenuModifierText AS Mod3IDtext, mm4.MenuModifierText AS Mod4IDtext, mm5.MenuModifierText AS Mod5IDtext, 
 mm6.MenuModifierText AS Mod6IDtext, mm7.MenuModifierText AS Mod7IDtext, mm8.MenuModifierText AS Mod8IDtext, mm9.MenuModifierText AS Mod9IDtext, mm10.MenuModifierText AS Mod10IDtext, 
 mm11.MenuModifierText AS Mod11IDtext, mm12.MenuModifierText AS Mod12IDtext, mm13.MenuModifierText AS Mod13IDtext, mm14.MenuModifierText AS Mod14IDtext, mm15.MenuModifierText AS Mod15IDtext, 
 mm16.MenuModifierText AS Mod16IDtext, mm17.MenuModifierText AS Mod17IDtext, mm18.MenuModifierText AS Mod18IDtext, mm19.MenuModifierText AS Mod19IDtext, mm20.MenuModifierText AS Mod20IDtext, 
 lee.FirstName AS LastEmployeeName, isnull(Discounts.DiscountBasis,0) as DiscountBasis, isnull(Discounts.DiscountAmount,0) AS DiscountApplyAmount, 
 OrderTransactions.MenuItemKey, OrderTransactions.DiscountKey, OrderTransactions.PromotionKey, isnull(Promotions.PromotionName,'''') as PromotionName, (isnull(OrderTransactions.Quantity,0)*isnull(OrderTransactions.MenuItemUnitPrice,0)) as TotalPrice, ISNULL(mi.MenuItemDescription,mi.MenuItemText) AS MenuItemText2, isnull(mi.Barcode,'''') AS Barcode, isnull(mi.Barcode2,'''') AS Barcode2 
 ,isnull(OrderTransactions.AddUserName,'''') AS AddUserName, isnull(OrderTransactions.OrderByWeight,0) AS OrderByWeight, isnull(OrderTransactions.PromotionName,'''') AS PromotionName, isnull(OrderTransactions.EditUserName,'''') AS EditUserName, isnull(OrderTransactions.UsedDiscountName,'''') AS UsedDiscountName, isnull(OrderTransactions.DiscountUserName,0) as DiscountUserName, OrderTransactions.OwnerKey, OrderTransactions.PromotionCount, OrderTransactions.PromotionAmount, OrderTransactions.AccountingCode, OrderTransactions.MainMenuItemText, OrderTransactions.MainMenuItemTransactionKey, ISNULL(OrderTransactions.LabelPrinted, 0) AS LabelPrinted 
 FROM OrderTransactions WITH (NOLOCK) 
 LEFT OUTER JOIN EmployeeFiles AS lee ON isnull(OrderTransactions.EditUserID,OrderTransactions.AddUserID) = lee.AutoID 
 LEFT OUTER JOIN EmployeeFiles as lo ON OrderTransactions.EmployeeID = lo.AutoID 
 LEFT OUTER JOIN MenuItems AS mi ON mi.MenuItemKey=OrderTransactions.MenuItemKey 
 LEFT OUTER JOIN Discounts ON OrderTransactions.DiscountKey = Discounts.DiscountKey 
 LEFT OUTER JOIN MenuModifiers AS mm1 ON OrderTransactions.Mod1ID = mm1.MenuModifierID 
 LEFT OUTER JOIN MenuModifiers AS mm2 ON OrderTransactions.Mod2ID = mm2.MenuModifierID 
 LEFT OUTER JOIN MenuModifiers AS mm3 ON OrderTransactions.Mod3ID = mm3.MenuModifierID 
 LEFT OUTER JOIN MenuModifiers AS mm4 ON OrderTransactions.Mod4ID = mm4.MenuModifierID 
 LEFT OUTER JOIN MenuModifiers AS mm5 ON OrderTransactions.Mod5ID = mm5.MenuModifierID 
 LEFT OUTER JOIN MenuModifiers AS mm6 ON OrderTransactions.Mod6ID = mm6.MenuModifierID 
 LEFT OUTER JOIN MenuModifiers AS mm7 ON OrderTransactions.Mod7ID = mm7.MenuModifierID 
 LEFT OUTER JOIN MenuModifiers AS mm8 ON OrderTransactions.Mod8ID = mm8.MenuModifierID 
 LEFT OUTER JOIN MenuModifiers AS mm9 ON OrderTransactions.Mod9ID = mm9.MenuModifierID 
 LEFT OUTER JOIN MenuModifiers AS mm10 ON OrderTransactions.Mod10ID = mm10.MenuModifierID 
 LEFT OUTER JOIN MenuModifiers AS mm11 ON OrderTransactions.Mod11ID = mm11.MenuModifierID 
 LEFT OUTER JOIN MenuModifiers AS mm12 ON OrderTransactions.Mod12ID = mm12.MenuModifierID 
 LEFT OUTER JOIN MenuModifiers AS mm13 ON OrderTransactions.Mod13ID = mm13.MenuModifierID 
 LEFT OUTER JOIN MenuModifiers AS mm14 ON OrderTransactions.Mod14ID = mm14.MenuModifierID 
 LEFT OUTER JOIN MenuModifiers AS mm15 ON OrderTransactions.Mod15ID = mm15.MenuModifierID 
 LEFT OUTER JOIN MenuModifiers AS mm16 ON OrderTransactions.Mod16ID = mm16.MenuModifierID 
 LEFT OUTER JOIN MenuModifiers AS mm17 ON OrderTransactions.Mod17ID = mm17.MenuModifierID 
 LEFT OUTER JOIN MenuModifiers AS mm18 ON OrderTransactions.Mod18ID = mm18.MenuModifierID 
 LEFT OUTER JOIN MenuModifiers AS mm19 ON OrderTransactions.Mod19ID = mm19.MenuModifierID 
 LEFT OUTER JOIN MenuModifiers AS mm20 ON OrderTransactions.Mod20ID = mm20.MenuModifierID 
 LEFT OUTER JOIN Promotions ON OrderTransactions.PromotionKey = Promotions.PromotionKey 
 WHERE OrderTransactions.[OrderKey]=@OrderKey ORDER BY OrderTransactions.AutoID',
N'@OrderKey uniqueidentifier',
@OrderKey='8A86D18A-F338-4E0A-B963-2B487A8B26BD';

-- 20. Sifariş ödənişlərinin yoxlanışı
EXEC sp_executesql N'SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SELECT OrderPayments.AutoID, OrderPayments.OrderPaymentID, OrderPayments.PaymentKey, OrderPayments.OrderID, OrderPayments.OrderKey, OrderPayments.StationID,   
 OrderPayments.CustomerID, OrderPayments.CustomerEmployeeID, OrderPayments.CouponNumber, OrderPayments.RegisterSessionID, OrderPayments.RevenueCenterTypeID, OrderPayments.PaymentDateTime, OrderPayments.EmployeeID, OrderPayments.RegisterNo,   
 OrderPayments.PaymentMethodID, OrderPayments.AmountTendered, OrderPayments.AmountPaid, OrderPayments.AmountChange, OrderPayments.ExhangeRate,   
 OrderPayments.RoundingAmount, OrderPayments.IsAccountPayment, OrderPayments.IsAccountSale, OrderPayments.CurrencyID, OrderPayments.PaymentMethodCode, OrderPayments.PaymentNotes, isnull(OrderPayments.LineDeleted,0) as LineDeleted,   
 OrderPayments.DeleteReason, OrderPayments.CustomField1, OrderPayments.CustomField2, OrderPayments.CustomField3, OrderPayments.CustomField4,   
 OrderPayments.CustomField5, OrderPayments.EditKey, OrderPayments.SyncKey, OrderPayments.BranchID, OrderPayments.AddUserID,   
 OrderPayments.AddDateTime, OrderPayments.EditUserID, OrderPayments.EditDateTime, PaymentMethods.PaymentName AS PaymentMethodName 
 ,e.FirstName AS EmployeeName, ea.FirstName AS AddEmployeeName, ee.FirstName AS EditEmployeeName, c.CustomerName, 
 OrderPayments.CustomerKey, OrderPayments.CustomerEmployeeKey, OrderPayments.EmployeeKey, OrderPayments.PaymentMethodKey, isnull(OrderPayments.RetailData,'''') AS RetailData, OrderPayments.GlobalBankCode, OrderPayments.GlobalBankName, OrderPayments.AccountingCode, OrderPayments.BankAccountingCode, OrderPayments.IncomeAccountingCode, ISNULL(OrderPayments.PaymentStatus, 0) AS PaymentStatus, ISNULL(OrderPayments.IntegrationApprove, 0) AS IntegrationApprove, OrderPayments.IntegrationReferenceNo, ISNULL(OrderPayments.RefundDetail, '''') AS RefundDetail, ISNULL(OrderPayments.TsmUsed, 0) AS TsmUsed 
 FROM OrderPayments WITH (NOLOCK)   
 LEFT OUTER JOIN PaymentMethods ON OrderPayments.PaymentMethodID = PaymentMethods.PaymentMethodID   
 LEFT OUTER JOIN EmployeeFiles AS ee ON OrderPayments.EditUserID = ee.EmployeeID   
 LEFT OUTER JOIN EmployeeFiles AS ea ON OrderPayments.AddUserID = ea.EmployeeID   
 LEFT OUTER JOIN EmployeeFiles AS e ON OrderPayments.EmployeeID = e.EmployeeID  
 LEFT OUTER JOIN CustomerFiles AS c ON OrderPayments.CustomerID = c.CustomerID 
 WHERE OrderPayments.OrderKey=@OrderKey',
N'@OrderKey uniqueidentifier',
@OrderKey='8A86D18A-F338-4E0A-B963-2B487A8B26BD';

-- 21. Sifariş qeydlərinin (OrderNotes) yoxlanışı
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SELECT [AutoID], [BranchID], [OrderKey], [TypeName], [OrderData], [TransactionKey] 
FROM [OrderNotes] 
WHERE OrderKey='8a86d18a-f338-4e0a-b963-2b487a8b26bd';

-- 22. Mətbəxə çap edilməmiş sətirlərin sayılması
EXEC sp_executesql N'SELECT COUNT(t.AutoID) AS lineCount FROM OrderTransactions AS t WHERE isnull(t.UsedPrinterComplated1,0)=0 AND t.OrderKey=@OrderKey',
N'@OrderKey uniqueidentifier',
@OrderKey='8A86D18A-F338-4E0A-B963-2B487A8B26BD';

-- 23. Mətbəx çap logunun başlanması
EXEC sp_executesql N'INSERT INTO [AccessLogs] ([BranchID], [LogDate], [StationID], [EmployeeID], [ActionName], [WrongPassword], [AdditionalInfo], [IsSuccess], [OrderKey], [TransactionKey], [AccessLogKey], [EditKey], [SyncKey]) 
VALUES (@BranchID, getdate(), @StationID, @EmployeeID, @ActionName, @WrongPassword, @AdditionalInfo, @IsSuccess, @OrderKey, @TransactionKey, newid(), @EditKey, @SyncKey)',
N'@BranchID int,@LogDate datetime,@StationID int,@EmployeeID int,@ActionName nvarchar(6),@WrongPassword nvarchar(4000),@AdditionalInfo nvarchar(144),@IsSuccess bit,@OrderKey uniqueidentifier,@TransactionKey uniqueidentifier,@AccessLogKey uniqueidentifier,@EditKey uniqueidentifier,@SyncKey uniqueidentifier',
@BranchID=422,
@LogDate='2026-08-17 13:10:53.620',
@StationID=1,
@EmployeeID=106,
@ActionName=N'Mutfak',
@WrongPassword=NULL,
@AdditionalInfo=N'''8a86d18a-f338-4e0a-b963-2b487a8b26bd'', user:ROBOTPOS, source=OrderSaver_DoWork ,task=53f35367-db43-40c9-835d-bd2d4e3da18d printOrder başladı...',
@IsSuccess=1,
@OrderKey='00000000-0000-0000-0000-000000000000',
@TransactionKey='00000000-0000-0000-0000-000000000000',
@AccessLogKey='434ABE4E-380C-4096-95ED-3D0F22911B15',
@EditKey='0B399DE7-0DE1-49A2-A526-AF13610AAF76',
@SyncKey='DBD29D8F-5777-49BD-9A75-ABDA3BC52F15';

-- 24. Printer parametrləri və şablonların yüklənməsi
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SELECT [AutoID], [PrinterID], [PrinterName], [EditKey], [SyncKey] FROM [Printers];

SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SELECT [AutoID], [DesignKey], [DocumentTypeID], [DesignName], [DesignData], [IsDefault], [EditKey], [SyncKey] FROM [PrinterDesigns];

SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SELECT AutoID, StationID, CheckPrinterName, CheckAltPrinterName, CheckPrinterID, CheckDesignPath, DeliveryPrinterName, DeliveryAltPrinterName, DeliveryPrinterID,  
 DeliveryDesignPath, InvoicePrinterName, InvoiceAltPrinterName, InvoicePrinterID, InvoiceDesignPath, InvoiceRowCount, ReportPrinterName, ReportAltPrinterName,  
 ReportPrinterID, ReportDesignPath, AdditionPrinterName, AdditionPrinterID, AdditionDesignPath, AdditionRowCount, ReportA4PrinterName, ReportA4AltPrinterName,  
 ReportA4PrinterID, ReportA4DesignPath, Kitchen1PrinterName, Kitchen1AltPrinterName, Kitchen1PrinterID, Kitchen1DesignPath, Kitchen2PrinterName,  
 Kitchen2AltPrinterName, Kitchen2PrinterID, Kitchen2DesignPath, Kitchen3PrinterName, Kitchen3AltPrinterName, Kitchen3PrinterID, Kitchen3DesignPath,  
 Kitchen4PrinterName, Kitchen4AltPrinterName, Kitchen4PrinterID, Kitchen4DesignPath, Kitchen5PrinterName, Kitchen5AltPrinterName, Kitchen5PrinterID,  
 Kitchen5DesignPath, Kitchen6PrinterName, Kitchen6AltPrinterName, Kitchen6PrinterID, Kitchen6DesignPath, Kitchen7PrinterName, Kitchen7AltPrinterName,  
 Kitchen7PrinterID, Kitchen7DesignPath, Kitchen8PrinterName, Kitchen8AltPrinterName, Kitchen8PrinterID, Kitchen8DesignPath, Kitchen9PrinterName,  
 Kitchen9AltPrinterName, Kitchen9PrinterID, Kitchen9DesignPath, Kitchen10PrinterName, Kitchen10AltPrinterName, Kitchen10PrinterID, Kitchen10DesignPath,  
 Kitchen11PrinterName, Kitchen11AltPrinterName, Kitchen11PrinterID, Kitchen11DesignPath, Kitchen12PrinterName, Kitchen12AltPrinterName, Kitchen12PrinterID,  
 Kitchen12DesignPath, Kitchen13PrinterName, Kitchen13AltPrinterName, Kitchen13PrinterID, Kitchen13DesignPath, Kitchen14PrinterName, Kitchen14AltPrinterName,  
 Kitchen14PrinterID, Kitchen14DesignPath, Kitchen15PrinterName, Kitchen15AltPrinterName, Kitchen15PrinterID, Kitchen15DesignPath, Kitchen16PrinterName,  
 Kitchen16AltPrinterName, Kitchen16PrinterID, Kitchen16DesignPath, Kitchen17PrinterName, Kitchen17AltPrinterName, Kitchen17PrinterID, Kitchen17DesignPath,  
 Kitchen18PrinterName, Kitchen18AltPrinterName, Kitchen18PrinterID, Kitchen18DesignPath, Kitchen19PrinterName, Kitchen19AltPrinterName, Kitchen19PrinterID,  
 Kitchen19DesignPath, Kitchen20PrinterName, Kitchen20AltPrinterName, Kitchen20PrinterID, Kitchen20DesignPath, InvoiceTopFeed, AdditionTopFeed, 
 isnull(PrintDineInOrdersKitchen,1) as PrintDineInOrdersKitchen, isnull(PrintBarTableOrdersKitchen,1) as PrintBarTableOrdersKitchen, isnull(PrintTakeOutOrdersKitchen,1) as PrintTakeOutOrdersKitchen, isnull(PrintDriveThruOrdersKitchen,1) as PrintDriveThruOrdersKitchen, isnull(PrintDeliveryOrdersKitchen,1) as PrintDeliveryOrdersKitchen 
 ,[EditKey], [SyncKey], [LabelPrinterID], [LabelPrinterName], [LabelDesignPath], [ReturnPrinterID], [ReturnPrinterName], [ReturnDesignPath] 
 FROM StationPrinterSettings WHERE StationID=1;

-- 25. Mətbəx çap logunun tamamlanması
EXEC sp_executesql N'INSERT INTO [AccessLogs] ([BranchID], [LogDate], [StationID], [EmployeeID], [ActionName], [WrongPassword], [AdditionalInfo], [IsSuccess], [OrderKey], [TransactionKey], [AccessLogKey], [EditKey], [SyncKey]) 
VALUES (@BranchID, getdate(), @StationID, @EmployeeID, @ActionName, @WrongPassword, @AdditionalInfo, @IsSuccess, @OrderKey, @TransactionKey, newid(), @EditKey, @SyncKey)',
N'@BranchID int,@LogDate datetime,@StationID int,@EmployeeID int,@ActionName nvarchar(6),@WrongPassword nvarchar(4000),@AdditionalInfo nvarchar(142),@IsSuccess bit,@OrderKey uniqueidentifier,@TransactionKey uniqueidentifier,@AccessLogKey uniqueidentifier,@EditKey uniqueidentifier,@SyncKey uniqueidentifier',
@BranchID=422,
@LogDate='2026-08-17 13:10:53.773',
@StationID=1,
@EmployeeID=106,
@ActionName=N'Mutfak',
@WrongPassword=NULL,
@AdditionalInfo=N'''8a86d18a-f338-4e0a-b963-2b487a8b26bd'', user:ROBOTPOS, source=OrderSaver_DoWork ,task=53f35367-db43-40c9-835d-bd2d4e3da18d printOrder bitti...',
@IsSuccess=1,
@OrderKey='00000000-0000-0000-0000-000000000000',
@TransactionKey='00000000-0000-0000-0000-000000000000',
@AccessLogKey='2DF2683D-E136-4ABA-AE65-6726829E4E62',
@EditKey='83D59D19-84D3-404D-AD5B-C495542BB566',
@SyncKey='871D977F-41D1-4E8F-81FB-0E1D391D604F';

-- 26. Çap statusunun qeyd edilməsi
EXEC sp_executesql N'UPDATE OrderTransactions SET UsedPrinterComplated1=1, SyncKey=NEWID() WHERE (OrderKey=@OrderKey)',
N'@OrderKey uniqueidentifier',
@OrderKey='8A86D18A-F338-4E0A-B963-2B487A8B26BD';

-- 27. Masa qrupu və aktiv masaların yenilənməsi
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SELECT [AutoID], [TableGroupID], [TableGroupKey], [TableGroupText], [RevenueCenterTypeID], [DeleteReason], [CustomField1], [CustomField2], [CustomField3], [CustomField4], [CustomField5], [EditKey], [SyncKey], [BranchID], [AddUserID], [AddDateTime], [EditUserID], [EditDateTime], isnull(TableRowCount,8) as TableRowCount, isnull(TableColumnCount,9) as TableColumnCount 
FROM [DineInTableGroups] WHERE AutoID=2;

SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SELECT DineInTables.AutoID, DineInTables.DineInTableID, DineInTables.DineInTableKey, DineInTables.TableGroupKey, DineInTables.DineInTableText, DineInTables.SectionNumber, DineInTables.TableGroupID,  
 DineInTables.DisplayIndex, isnull(DineInTables.DineInTableActive,1) as DineInTableActive, DineInTables.MaxGuests, DineInTables.Smoking, DineInTables.Window, DineInTables.Booth,  
 DineInTables.Privacy, DineInTables.PictureName, DineInTables.AvarageSeatTime, DineInTables.RevenueCenterTypeID, isnull(DineInTables.SecurityLevel,0) as SecurityLevel,  
 DineInTables.DeleteReason, DineInTables.CustomField1, DineInTables.CustomField2, DineInTables.CustomField3, DineInTables.CustomField4,  
 DineInTables.CustomField5, DineInTables.EditKey, DineInTables.SyncKey, DineInTables.BranchID, DineInTables.AddUserID, DineInTables.AddDateTime,  
 DineInTables.EditUserID, DineInTables.EditDateTime, OrderHeaders.OrderKey AS ActiveOrderKey, (isnull(OrderHeaders.AmountDue,0.0)+isnull(OrderHeaders.CashGratuity,0.0)) as ActiveOrderAmountDue, OrderHeaders.OrderDateTime AS ActiveOrderDateTime, OrderHeaders.EditDateTime AS ActiveOrderEditTime, 
 isnull(EmployeeFiles.FirstName,'') AS ActiveOrderEmyloyeeName, isnull(OrderHeaders.GuestCheckPrinted,0) AS GuestCheckPrinted, isnull(OrderHeaders.TableReady,0) AS TableReady, 
 isnull(OrderHeaders.AdditionPrintedLineCount,0) AS AdditionPrintedLineCount, ISNULL(EmployeeFiles.EmployeeKey,'00000000-0000-0000-0000-000000000000') AS ActiveOrderEmyloyeeKey 
 FROM DineInTables with (nolock)  
 LEFT OUTER JOIN OrderHeaders ON OrderHeaders.DineInTableID = DineInTables.DineInTableID AND isnull(OrderHeaders.OrderStatus,1)=1 AND OrderHeaders.OrderType=1 
 LEFT OUTER JOIN EmployeeFiles ON EmployeeFiles.EmployeeID = OrderHeaders.EmployeeID 
 WHERE DineInTables.DineInTableActive=1 and DineInTables.TableGroupID=2;

-- 28. Son aktivlik vaxtları
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SELECT r.DineInTableID, MAX(r.lastDate) AS lastDate 
FROM ( 
 SELECT h.AutoID, h.OrderKey, h.DineInTableID, (ISNULL(ISNULL(d.EditDateTime,d.AddDateTime),h.AddDateTime)) AS lastDate   
 FROM OrderHeaders AS h WITH(NOLOCK) 
 INNER JOIN OrderTransactions AS d ON d.OrderKey = h.OrderKey  
 WHERE h.OrderDateTime > DATEADD(hour,-48,GETDATE()) AND isnull(h.LineDeleted,0)=0 AND h.OrderStatus<2 AND h.DineInTableID>0 
) AS r 
GROUP BY r.AutoID, r.OrderKey, r.DineInTableID;