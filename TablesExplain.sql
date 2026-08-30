--Chekin detaili
SELECT AutoID, TransactionDateTime, TransactionID, TransactionKey, OrderKey, OrderID, OrderDateTime, StationID, EmployeeID, RevenueCenterTypeID, MenuItemID, MenuItemKey, MenuItemText, MenuItemGroupText, MenumItemCategoryText, 
                  MenuItemUnitPrice, MenuItemCost, Quantity, ExtendedPrice, DiscountID, DiscountKey, DiscountLineAmount, DiscountCashAmount, DiscountTotalAmount, TransactionStatus, NotificationStatus, AdditionLinePrinted, TaxPercent, RoundID, 
                  Mod1ID, Mod1Cost, Mod2ID, Mod2Cost, Mod3ID, Mod3Cost, Mod4ID, Mod4Cost, Mod5ID, Mod5Cost, Mod6ID, Mod6Cost, Mod7ID, Mod7Cost, Mod8ID, Mod8Cost, Mod9ID, Mod9Cost, Mod10ID, Mod10Cost, Mod11ID, Mod11Cost, Mod12ID, 
                  Mod12Cost, Mod13ID, Mod13Cost, Mod14ID, Mod14Cost, Mod15ID, Mod15Cost, Mod16ID, Mod16Cost, Mod17ID, Mod17Cost, Mod18ID, Mod18Cost, Mod19ID, Mod19Cost, Mod20ID, Mod20Cost, SeatNumber, OnHoldUntilTime, Notes, 
                  SaleTaxAmount, UsedPrinterID1, UsedPrinterID2, UsedPrinterID3, UsedPrinterID4, UsedPrinterID5, UsedPrinterComplated1, UsedPrinterComplated2, UsedPrinterComplated3, UsedPrinterComplated4, UsedPrinterComplated5, 
                  LineDeleted, DeleteReason, CustomField1, CustomField2, CustomField3, CustomField4, CustomField5, EditKey, SyncKey, BranchID, AddUserID, AddDateTime, EditUserID, DayEnd, EditDateTime, AddUserName, EditUserName, 
                  EmployeeName, IsProduced, SplitDetail, ExternalOrderStatus, PromotionKey, DiscountAmountValue, DiscountBasisValue, TaxPercentReduction, UsedDiscountName, InvoicePrinted, SendOK, PromotionName, ProductKey, RetailData, 
                  OrderByWeight, DiscountUserName, PromotionCount, PromotionAmount, OwnerKey, AccountingCode, MainMenuItemText, MainMenuItemTransactionKey, PosVersion, ModuleVersion, IsMainCombo, LabelPrinted, CustomField6, 
                  CustomField7, MaxQuantity, IntegrationApprove, IntegrationReferenceNo, IntegrationCardNumber, PriceWeightPercent, CustomField8, CustomField9, CustomField10, RoundAmount, ECommerceCode
FROM     OrderTransactions
WHERE  (OrderID = 54296)

--Chekin Headeri
SELECT AutoID, OrderID, ReceiptNo, MainOrderKey, MainOrderID, OrderTypeSourceID, OrderTypeSourceExternalNo, OrderKey, OrderDateTime, EmployeeID, StationID, RevenueCenterTypeID, OrderType, DineInTableID, CustomerID, 
                  DeliveryCharge, DeliveryComp, DeliveryZoneID, DriverEmployeeID, DriverDepartureTime, DriverArrivalTime, OnHoldUntilTime, SalesTaxRate, DiscountID, DiscountLineAmount, DiscountOrderAmount, DiscountCashAmount, 
                  DiscountTotalAmount, DiscountAmountValue, DiscountBasisValue, OrderStatus, BonusAmountUsed, BonusAmountEarned, BonusID, BonusCustomerID, AmountDue, PackagerAlreadyPrinted, GuestCheckPrinted, GuestCheckPrintCount, 
                  AdditionPrinted, AdditionPrintedLineCount, SurchargeID, SurchargeLineAmount, SurchargeOrderAmount, SurchargeCashAmount, SurchargeTotalAmount, ComplimentaryAmount, SubTotal, OrderCost, GratuityPercent, CashGratuity, 
                  SalesTaxAmount, DriveThruComplete, BarTabName, TableReady, GuestNumber, SpecificCustomerName, OrderPhone, InvoicePrinted, FiscalPrinted, OrderNotes, OrderExternalNotes, LineDeleted, DeleteReason, CustomField1, 
                  CustomField2, CustomField3, CustomField4, CustomField5, EditKey, SyncKey, BranchID, LockData, LockStationID, AddUserID, AddDateTime, EditUserID, EditDateTime, DayEnd, IsProduced, CustomerKey, DiscountKey, EmployeeKey, 
                  EmployeeName, DineInTableName, CustomerName, AddUserName, EditUserName, ExternalOrderStatus, FiscalKey, InvoiceDetail, SendOK, RetailData, FiscalStatus, DiscountUserName, ingenico, PaperNumber, PosVersion, 
                  ModuleVersion, InvoiceSendOK, IsInvoice, Inv_IsEarchive, Inv_IsEinvoice, Inv_SerialNo, Inv_CustomerName, Inv_TaxNo, Inv_TaxOffice, Inv_EMail, Inv_Phone, Inv_Address, Inv_Notes, Inv_ReferenceNo, Inv_IsDiplomatic, 
                  Inv_IsCustomAmount, Inv_CustomTaxName08, Inv_CustomTaxName18, Inv_CustomAmount08, Inv_CustomAmount18, CustomField6, CustomField7, CustomField8, CustomField9, CustomField10, IsKiosk, CustomerBalanceAmount, 
                  CustomerSpecialBonusPercent, CustomerBonusName, ReturnType, ReturnOrderNo, ReturnCustomerName, ReturnCustomerAddress, ReturnTaxNumber, ReturnTaxOffice, ReturnSerialNo, ReturnReason, ReturnReasonCode, 
                  UsdAmount, EurAmount, GbpAmount, OrderCounter, TsmStatus, FiscalGmpUniqueKey, Inv_Type, Inv_Urn, PromotionName, PromotionCashAmount, IsFrink, FrinkOrderNo, FrinkCustomerName, FrinkStatus, IsTekfenPersonal, 
                  TekfenActionKey, TekfenPersonalFullName, TekfenCardNumber, TekfenNotes, TekfenProcessType, TekfenCompanyName, WebOrderKey, RoundAmount, ManualProductionDate, IsCustomerAdvance, FiscalBoxResult, VoidSendOK, 
                  IsVigo, VigoStatus, VigoOrderId, VigoDeliveryId, VigoTimeline
FROM     OrderHeaders
WHERE  (OrderID = 54296)

--Kassa Girish Chixishi
SELECT RegisterSessionID, BranchID, EmployeeID, RegisterSessionKey, StationID, AccountingDateTime, SignInDateTime, RegisterStartAmount, SignOutDateTime, RegisterEndAmount, DiscrepancyAmount, DiscrepancyNotes, 
                  ManagerEmployeeID, TotalPaymentMethod1, TotalPaymentMethod2, TotalPaymentMethod3, TotalPaymentMethod4, TotalPaymentMethod5, TotalPaymentMethod6, TotalPaymentMethod7, TotalPaymentMethod8, TotalPaymentMethod9, 
                  TotalPaymentMethod10, TotalPaymentMethod11, TotalPaymentMethod12, TotalPaymentMethod13, TotalPaymentMethod14, TotalPaymentMethod15, TotalPaymentMethod16, TotalPaymentMethod17, TotalPaymentMethod18, 
                  TotalPaymentMethod19, TotalPaymentMethod20, ClosePaymentMethod1, ClosePaymentMethod2, ClosePaymentMethod3, ClosePaymentMethod4, ClosePaymentMethod5, ClosePaymentMethod6, ClosePaymentMethod7, 
                  ClosePaymentMethod8, ClosePaymentMethod9, ClosePaymentMethod10, ClosePaymentMethod11, ClosePaymentMethod12, ClosePaymentMethod13, ClosePaymentMethod14, ClosePaymentMethod15, ClosePaymentMethod16, 
                  ClosePaymentMethod17, ClosePaymentMethod18, ClosePaymentMethod19, ClosePaymentMethod20, ZReportID, EditKey, SyncKey, DiscrepancyNotes2, EmployeeKey, PaymentMethodName1, PaymentMethodName2, 
                  PaymentMethodName3, PaymentMethodName4, PaymentMethodName5, PaymentMethodName6, PaymentMethodName7, PaymentMethodName8, PaymentMethodName9, PaymentMethodName10, PaymentMethodName11, 
                  PaymentMethodName12, PaymentMethodName13, PaymentMethodName14, PaymentMethodName15, PaymentMethodName16, PaymentMethodName17, PaymentMethodName18, PaymentMethodName19, PaymentMethodName20, 
                  SendOK, TotalPaymentMethod21, TotalPaymentMethod22, TotalPaymentMethod23, TotalPaymentMethod24, TotalPaymentMethod25, TotalPaymentMethod26, TotalPaymentMethod27, TotalPaymentMethod28, TotalPaymentMethod29, 
                  TotalPaymentMethod30, TotalPaymentMethod31, TotalPaymentMethod32, TotalPaymentMethod33, TotalPaymentMethod34, TotalPaymentMethod35, TotalPaymentMethod36, TotalPaymentMethod37, TotalPaymentMethod38, 
                  TotalPaymentMethod39, TotalPaymentMethod40, ClosePaymentMethod21, ClosePaymentMethod22, ClosePaymentMethod23, ClosePaymentMethod24, ClosePaymentMethod25, ClosePaymentMethod26, ClosePaymentMethod27, 
                  ClosePaymentMethod28, ClosePaymentMethod29, ClosePaymentMethod30, ClosePaymentMethod31, ClosePaymentMethod32, ClosePaymentMethod33, ClosePaymentMethod34, ClosePaymentMethod35, ClosePaymentMethod36, 
                  ClosePaymentMethod37, ClosePaymentMethod38, ClosePaymentMethod39, ClosePaymentMethod40, PaymentMethodName21, PaymentMethodName22, PaymentMethodName23, PaymentMethodName24, PaymentMethodName25, 
                  PaymentMethodName26, PaymentMethodName27, PaymentMethodName28, PaymentMethodName29, PaymentMethodName30, PaymentMethodName31, PaymentMethodName32, PaymentMethodName33, PaymentMethodName34, 
                  PaymentMethodName35, PaymentMethodName36, PaymentMethodName37, PaymentMethodName38, PaymentMethodName39, PaymentMethodName40, FiscalBoxAccessToken
FROM     RegisterSessions
WHERE  (EmployeeID = 106)
SELECT * FROM EmployeeFiles WHERE FirstName = 'RobotPOS'

--Masa Qruplari ve masalar.
SELECT AutoID, TableGroupID, TableGroupText, RevenueCenterTypeID, DeleteReason, CustomField1, CustomField2, CustomField3, CustomField4, CustomField5, EditKey, SyncKey, BranchID, AddUserID, AddDateTime, EditUserID, EditDateTime, TableGroupKey, TableRowCount, TableColumnCount, SendOK
FROM DineInTableGroups

SELECT AutoID, DineInTableID, DineInTableText, SectionNumber, TableGroupID, DisplayIndex, DineInTableActive, MaxGuests, Smoking, Window, Booth, Privacy, PictureName, AvarageSeatTime, RevenueCenterTypeID, SecurityLevel, DeleteReason, CustomField1, CustomField2, CustomField3, CustomField4, CustomField5, EditKey, SyncKey, BranchID, AddUserID, AddDateTime, EditUserID, EditDateTime, DineInTableKey, TableGroupKey, SendOK
FROM     DineInTables
WHERE  (DineInTableID = 1)

--Menular qrup ve kategoriyalari
SELECT AutoID, MenuItemID, MainMenuItemID, RevenueCenterTypeID, MenuItemText, MenuCategoryID, MenuGroupID, TaxGroupID, DisplayIndex, DefaultUnitPrice, MenuItemCost, MenuItemDescription, MenuItemNotification, MenuItemActive, 
                  MenuItemInStock, MenuItemTaxable, TaxPercent, MenuModifierID, MenuItemDiscountable, MenuItemPopUpHeaderID, MenuItemPopUpChoiceText, HasModifierPopUps, SecLangMenuItemText, SecLangPopUpChoiceText, PictureName, 
                  ShowCaption, IsComboMenu, IsTopMenu, ButtonColor, Barcode, Barcode2, ItemDelCharge, ItemDelComp, DineInPrice, BarTabPrice, TakeOutPrice, DriveThruPrice, DeliveryPrice, OrderByWeight, PrintPizzaLabel, KitchenSortNumber, 
                  ModBuilderTemplateID, MenuItemTypeID, AccountingCode, UsedPrinterID1, UsedPrinterID2, UsedPrinterID3, UsedPrinterID4, UsedPrinterID5, UseKds1, UseKds2, UseKds3, UseKds4, UseKds5, UseKds6, UseKds7, UseKds8, UseKds9, 
                  UseKds10, MainMenuItemKey, MenuCategoryKey, MenuGroupKey, MenuItemGlobalKey, MenuItemKey, MenuModifierForcedKey, MenuModifierForcedID, MenuModifierKey, TaxGroupKey, SaleStartDate, SaleEndDate, SecurityLevel, 
                  DeleteReason, CustomField1, CustomField2, CustomField3, CustomField4, CustomField5, EditKey, SyncKey, BranchID, AddUserID, AddDateTime, EditUserID, EditDateTime, CountDownDate, CountDownValue, CountDownActualResult, 
                  PrintOnLabel, SendOK, MaxSaleQuantity, Barcode3, Barcode4, Barcode5, Barcode6, Barcode7, Barcode8, Barcode9, Barcode10, CustomField6, CustomField7, CustomField8, CustomField9, CustomField10, CustomField11, CustomField12, 
                  CustomField13, CustomField14, CustomField15, CustomField16, ECommerceCode
FROM     MenuItems
SELECT AutoID, MenuGroupID, MainMenuItemID, MenuItemID, DisplayIndex, BranchID, LayoutKey, MenuItemKey, EditKey, SyncKey, MainMenuItemKey, MenuGroupKey
FROM     MenuItemLayout

SELECT MenuCategoryID, MenuCategoryKey, MenuCategoryText, MenuCategoryActive, DefaultTaxPercent, RevenueCenterTypeID, DeleteReason, CustomField1, CustomField2, CustomField3, CustomField4, CustomField5, EditKey, SyncKey, 
                  BranchID, AddUserID, AddDateTime, EditUserID, EditDateTime
FROM     MenuCategories

SELECT MenuGroupID, MenuGroupText, DisplayIndex, MenuGroupActive, SecLangMenuGroupText, PictureName, ShowCaption, HideInDineIn, HideInBar, HideInTakeaway, HideInCounter, HideInDelivery, ButtonColor, RevenueCenterTypeID, 
                  DeleteReason, CustomField1, CustomField2, CustomField3, CustomField4, CustomField5, EditKey, SyncKey, BranchID, AddUserID, AddDateTime, EditUserID, EditDateTime, MenuGroupKey
FROM     MenuGroups

SELECT MenuSubCategoryID, MenuSubCategoryKey, MenuSubCategoryText, MenuSubCategoryActive, DefaultTaxPercent, RevenueCenterTypeID, DeleteReason, CustomField1, CustomField2, CustomField3, CustomField4, CustomField5, EditKey, 
                  SyncKey, BranchID, AddUserID, AddDateTime, EditUserID, EditDateTime
FROM     MenuSubCategories

--Endirim
SELECT DiscountID, DiscountKey, DiscountText, DiscountDescription, DiscountedAmountTaxable, DiscountExpireDate, DiscountActive, DiscountAmount, DiscountBasis, DiscountAllowedMinTicket, Barcode, DiscountMenuItemID, ButtonColor, 
                  PictureName, SecurityLevel, DeleteReason, CustomField1, CustomField2, CustomField3, CustomField4, CustomField5, EditKey, SyncKey, BranchID, AddUserID, AddDateTime, EditUserID, EditDateTime, UseGroupFilter, 
                  GroupMinimumQuantity, UseMenuItemFilter, MenuItemMinimumQuantity, UseOrderTotalFilter, OrderTotalMinimum, UseHourFilter, HourStart, HourEnd, UseDayFilter, DayList, DiscountStartDate, SkipMenuDiscountStatus, 
                  OrderTotalMaximum, DisablePromotion, SendOK, SecurityCode
FROM     Discounts

--Müştəri
SELECT AutoID, CustomerID, CustomerKey, CustomerGlobalKey, CustomerIsActive, CustomerName, CustomerFullName, CardNumber, CustomerNotes, OrderCount, LastCallDate, CallingCount, AllowHouseAccount, IsFrequentDiner, CreditLimit, 
                  CreditSatusID, DiscountPercent, SpecialBonusPercent, TotalDebt, TotalPayment, TotalRemainig, BonusStartupValue, TotalBonusUsed, TotalBonusEarned, TotalBonusRemaing, CityName, District, Neighborhood, Avenue, Street, Buildings, 
                  Block, Apartment, ApartmentNo, FlatNo, IsDefault, TaxOfficeName, TaxNumber, ZipCode, AddressNotes, AreaCode, CustomerSpecialNotes, BirthDay, MaritialStatus, Age, EmailAddress, Sexuality, FacebookAccount, TwitterAccount, 
                  WebSite, PhotoPath, ProximityCardID, EditKey, SyncKey, BranchID, LockData, LockStationID, AddUserID, AddDateTime, EditUserID, EditDateTime, OpenValue, WebUserName, WebPassword, IsEmployee, DetailData, AparmentFlatNo, 
                  SendOK
FROM     CustomerFiles
WHERE  (CustomerID = 3)

--Bonus musterimeleri
SELECT AutoID, CustomerID, CustomerKey, CustomerGlobalKey, CustomerIsActive, CustomerName, CustomerFullName, CardNumber, PhoneNumber, CustomerNotes, OrderCount, LastCallDate, CallingCount, AllowHouseAccount, 
                  IsFrequentDiner, CreditLimit, CreditSatusID, DiscountPercent, SpecialBonusPercent, TotalDebt, TotalPayment, TotalRemainig, BonusStartupValue, TotalBonusUsed, TotalBonusEarned, TotalBonusRemaing, CityName, District, 
                  Neighborhood, Avenue, Street, Buildings, Block, Apartment, ApartmentNo, FlatNo, IsDefault, TaxOfficeName, TaxNumber, ZipCode, AddressNotes, AreaCode, CustomerSpecialNotes, BirthDay, MaritialStatus, Age, EmailAddress, Sexuality, 
                  FacebookAccount, TwitterAccount, WebSite, PhotoPath, ProximityCardID, EditKey, SyncKey, BranchID, LockData, LockStationID, AddUserID, AddDateTime, EditUserID, EditDateTime, WebUserName, WebPassword, OpenValue, DayCount, 
                  DailyAmount, CustomerCode, LastAddedCreditID, SendOK
FROM     BonusCustomerFiles WHERE CustomerID = 3

--Ödənişlər cədvəli
SELECT AutoID, OrderPaymentID, PaymentKey, OrderID, OrderKey, StationID, CustomerID, CustomerEmployeeID, CouponNumber, RegisterSessionID, RevenueCenterTypeID, PaymentDateTime, EmployeeID, RegisterNo, PaymentMethodID, 
                  AmountTendered, AmountPaid, AmountChange, ExhangeRate, RoundingAmount, CurrencyID, IsAccountPayment, IsAccountSale, PaymentMethodCode, PaymentNotes, LineDeleted, DeleteReason, CustomField1, CustomField2, 
                  CustomField3, CustomField4, CustomField5, EditKey, SyncKey, BranchID, AddUserID, AddDateTime, EditUserID, EditDateTime, DayEnd, CustomerEmployeeKey, CustomerKey, EmployeeKey, PaymentMethodKey, AddUserName, 
                  EditUserName, EmployeeName, PaymentMethodName, SendOK, RetailData, GlobalBankCode, ingenico, RegisterSessionKey, AccountingCode, BankAccountingCode, IncomeAccountingCode, GlobalBankName, PosVersion, 
                  ModuleVersion, PaymentStatus, IntegrationApprove, IntegrationReferenceNo, TsmUsed, OmerdConfirmationId, RefundDetail, IntegrationCardNumber, Pan
FROM     OrderPayments
WHERE  (OrderID = 54296)

--ODeme Tipleri
SELECT AutoID, PaymentMethodID, PaymentName, IsDefault, DenyInvoice, DenyFiscal, DenyMoneyChange, ExchangeRate, AccountingCode, SecurityLevel, IsLocked, PaymentMethodActive, EffectRegister, IsCoupon, IsAccountPayment, 
                  IsAccountSale, HideInRecievePayement, PictureName, DisplayIndex, ButtonColor, PaymentMethodKey, BranchID, EditKey, SyncKey, IsCampusCard, CampusCardServer, AskCustomerName, ForcedInvoice, FiscalPaymentName, 
                  FiscalPaymentID, PaymentTypeID, CustomField1, CustomField2, CustomField3, CustomField4, CustomField5, SendOK, GlobalBankCode
FROM     PaymentMethods

--Raporlar
SELECT AutoID, ReportID, ReportKey, ReportName, ReportCategoryID, ReportActive, SecurityLevel, ReportTypeID, IsPosReport, EditKey, SyncKey
FROM     Reports
--Rapor Kateqoriyalari
SELECT ReportCategoryID, CategoryName, MainCategoryID, EditKey, SyncKey
FROM     ReportCategories
--Rapor Dizaynlari
SELECT AutoID, ReportDesignID, ReportDesignKey, ReportID, ReportKey, DocumentTypeID, DesignName, DesignData, IsDefault, EditKey, SyncKey
FROM     ReportDesigns
---Rapor Sorgulari
SELECT AutoID, ReportQueryID, ReportQueryKey, ReportID, ReportKey, QueryName, QueryData, IsDefault, EditKey, SyncKey
FROM     ReportQueries

--Isletme Ayarlari
SELECT SettingsID, OrderID, TabName, GroupName, ParamName, ParamKey, ParamValue, DefaultValue, ParamType, EditKey, SyncKey, BranchID, AddUserID, AddDateTime, EditUserID, EditDateTime, AllowEdit
FROM     StoreSettings
--Terminal Ayarlari
SELECT AutoID, StationID, StationName, AllowRegister, DefineValue, ActivateCashier, RememberCashier, BackgroundPicture, SideBarPicture, DefaultLoginEntrance, DefultTablePlan, SkinName, ShowDineInButton, ShowTakeOutButton, 
                  ShowDriveThruButton, ShowDeliveryButton, ShowRecallButton, ShowDriverStatusButton, ShowTimeCardButton, ShowOperationsButton, ShowBackOfficeButton, ShowQuickServiceDineIn, ShowQuickServiceTakeOut, 
                  ShowQuickServiceDriveThru, ShowQuickServiceDelivery, StayInOrderScreenTakeOut, StayInOrderScreenDriveThru, StayTablePlanInDineIn, TimeOutScreenLock, IsMobile, CustomerDisplayPortNo, CustomerDisplayLine1, 
                  CustomerDisplayLine2, WeightScale1PortNo, WeightScale1BaudRate, WeightScale2PortNo, WeightScale2BaudRate, CashRegisterModel, CashRegisterBaudRate, CashRegisterDataDelay, CashRegisterLineDelay, CashRegisterStations, 
                  CashRegisterPortNo, PaymentOverTime, DirectOpenOrderInEditMode, PrintVoidedLinesOnGuestCheck, StationKey, AskPasswordForReduce, MediaDisplayIsActive, MediaDisplayOnWaiting, MediaDisplayOnSale, 
                  MediaDisplayClosedMessage, MediaDisplayMoneyOver, EditKey, SyncKey, ShowCashTrayButton, DisableSaleOnOpenCashTray, BekoPosDesign, BekoPosOutput, BekoPosDatabase, EnableCallCenterClient, CallCenterClientAddress, 
                  KeyNote, ShowScaleOrderDineIn, ShowScaleOrderTakeOut, ShowScaleOrderDriveThru, ShowScaleOrderDelivery, UseRetailMode, HideOkButtonOnQuickService, UseStaticPlu, StaticPluNumber, EnableCentralCallCenterClient, 
                  CentralCallCenterClientAddress, UseSecondLangOnKitchenPrint, CallerIDPort, MediaDisplayFontSize, SendTareOnWeightMinus, ShowScaleOrderRetail, SendTareOnAfterAddButton, DefaultWarehouseID, BranchID, 
                  ShowComboOnMediaDisplay, ShowMenuModifierOnMediaDisplay, ShowPicturedModifierOnMediaDisplay, ShowSidePictureOnMediaDisplay, ScaleMode, ShowCashDiscountButtonOnMainScreen, SendOK, PosVersion, ViewTotalDiscount, 
                  HideOkButtonOnDineIn, HideOkButtonOnDelivery, HideOkButtonOnDriveThru, HideOkButtonOnTakeOut, MediaDisplaySaleOrderPercent, PaymentHideOnDineIn, PaymentHideOnTakeOut, PaymentHideOnDriveThru, 
                  PaymentHideOnDelivery, MediaDisplayImagePercent, ShowOtherPaymentType, GarsonStation, VersionInfo, UpdateDateTime, ShowEndOfDayButton, UseVigoServiceOperations, RunPromotionsForMobilApp
FROM     StationSettings

--Printerler
SELECT AutoID, PrinterID, PrinterName, EditKey, SyncKey
FROM     Printers

--Printer Tenzimemeleri
SELECT AutoID, StationID, CheckPrinterName, CheckAltPrinterName, CheckPrinterID, CheckDesignPath, DeliveryPrinterName, DeliveryAltPrinterName, DeliveryPrinterID, DeliveryDesignPath, InvoicePrinterName, InvoiceAltPrinterName, 
                  InvoicePrinterID, InvoiceDesignPath, InvoiceRowCount, InvoiceTopFeed, ReportPrinterName, ReportAltPrinterName, ReportPrinterID, ReportDesignPath, AdditionPrinterName, AdditionPrinterID, AdditionDesignPath, AdditionRowCount, 
                  AdditionTopFeed, ReportA4PrinterName, ReportA4AltPrinterName, ReportA4PrinterID, ReportA4DesignPath, Kitchen1PrinterName, Kitchen1AltPrinterName, Kitchen1PrinterID, Kitchen1DesignPath, Kitchen2PrinterName, 
                  Kitchen2AltPrinterName, Kitchen2PrinterID, Kitchen2DesignPath, Kitchen3PrinterName, Kitchen3AltPrinterName, Kitchen3PrinterID, Kitchen3DesignPath, Kitchen4PrinterName, Kitchen4AltPrinterName, Kitchen4PrinterID, 
                  Kitchen4DesignPath, Kitchen5PrinterName, Kitchen5AltPrinterName, Kitchen5PrinterID, Kitchen5DesignPath, Kitchen6PrinterName, Kitchen6AltPrinterName, Kitchen6PrinterID, Kitchen6DesignPath, Kitchen7PrinterName, 
                  Kitchen7AltPrinterName, Kitchen7PrinterID, Kitchen7DesignPath, Kitchen8PrinterName, Kitchen8AltPrinterName, Kitchen8PrinterID, Kitchen8DesignPath, Kitchen9PrinterName, Kitchen9AltPrinterName, Kitchen9PrinterID, 
                  Kitchen9DesignPath, Kitchen10PrinterName, Kitchen10AltPrinterName, Kitchen10PrinterID, Kitchen10DesignPath, Kitchen11PrinterName, Kitchen11AltPrinterName, Kitchen11PrinterID, Kitchen11DesignPath, Kitchen12PrinterName, 
                  Kitchen12AltPrinterName, Kitchen12PrinterID, Kitchen12DesignPath, Kitchen13PrinterName, Kitchen13AltPrinterName, Kitchen13PrinterID, Kitchen13DesignPath, Kitchen14PrinterName, Kitchen14AltPrinterName, Kitchen14PrinterID, 
                  Kitchen14DesignPath, Kitchen15PrinterName, Kitchen15AltPrinterName, Kitchen15PrinterID, Kitchen15DesignPath, Kitchen16PrinterName, Kitchen16AltPrinterName, Kitchen16PrinterID, Kitchen16DesignPath, Kitchen17PrinterName, 
                  Kitchen17AltPrinterName, Kitchen17PrinterID, Kitchen17DesignPath, Kitchen18PrinterName, Kitchen18AltPrinterName, Kitchen18PrinterID, Kitchen18DesignPath, Kitchen19PrinterName, Kitchen19AltPrinterName, Kitchen19PrinterID, 
                  Kitchen19DesignPath, Kitchen20PrinterName, Kitchen20AltPrinterName, Kitchen20PrinterID, Kitchen20DesignPath, PrintDineInOrdersKitchen, PrintBarTableOrdersKitchen, PrintTakeOutOrdersKitchen, PrintDriveThruOrdersKitchen, 
                  PrintDeliveryOrdersKitchen, EditKey, SyncKey, LabelPrinterName, LabelPrinterID, LabelDesignPath, PieceProductionDesign, PieceProductionDesignPrint, SemiProductionDesign, SemiProductionDesignPrint, WarehouseTransferDesign, 
                  WarehouseTransferDesignPrint, PieceProductionCopy, SemiProductionCopy, WarehouseTransferCopy, ReturnPrinterName, ReturnPrinterID, ReturnDesignPath, PrintOrderAllProductsKitchen
FROM     StationPrinterSettings

--Printer Cek Dizaynlari
SELECT AutoID, DesignKey, DocumentTypeID, DesignName, DesignData, IsDefault, EditKey, SyncKey
FROM     PrinterDesigns

--Guvenlik Ayarlari
SELECT AuthorityID, GroupName, AuthorityKey, AuthorityText, AuthorityDescription, DefaultLevel, NeedAllways, AllowManager, AllowCashier, EditKey, SyncKey, BranchID
FROM     AuthorityList

--Personel Dosyalari
SELECT AutoID, EmployeeID, FirstName, LastName, SocialSecurityNumber, MailingAddress, MailingZipCode, SmarCardCode, MifareCardCode, DateHired, DateReleased, EmployeeActive, JobTitleID, SecurityLevel, AccessCode, TipsReceived, 
                  PayBasis, PayRate, ScanCode, DriverLicenseNumber, DriverLicenseExpires, CarInsurancePolicyCarrier, CarInsurancePolicyNumber, CarInsurancePolicyExpires, CarInsurancePolicyNotes, PrefUserInterfaceLocale, EmployeeNotes, 
                  OrderEntryUseSecLang, EmployeeIsDriver, DefaultOEMenuGroupID, UseStaffBank, ScheduleNotEnforced, UseHostess, IsAServer, IsOffline, NoCashierOut, EditTimestamp, PhoneNumber, RevenueCenterTypeID, DeleteReason, 
                  CustomField1, CustomField2, CustomField3, CustomField4, CustomField5, EditKey, SyncKey, BranchID, AddUserID, AddDateTime, EditUserID, EditDateTime, EmployeeKey, MonthlyDinnerFee, SendOK
FROM     EmployeeFiles

--Personel Yemek Karti
SELECT c.AutoID, c.CustomerID, c.CustomerKey, c.CustomerGlobalKey, c.CustomerIsActive, c.CustomerName, c.CustomerFullName, c.CardNumber, c.CustomerNotes, 
 c.OrderCount, c.LastCallDate, c.CallingCount, c.AllowHouseAccount, c.IsFrequentDiner, c.CreditLimit, c.CreditSatusID, c.DiscountPercent, c.SpecialBonusPercent, 
 c.TotalDebt, c.TotalPayment, c.TotalRemainig, c.BonusStartupValue, c.TotalBonusUsed, c.TotalBonusEarned, c.TotalBonusRemaing, c.CityName, c.District, 
 c.Neighborhood, c.Avenue, c.Street, c.Buildings, c.Block, c.Apartment, c.ApartmentNo, c.FlatNo, c.IsDefault, c.TaxOfficeName, c.TaxNumber, c.ZipCode, 
 c.AddressNotes, c.AreaCode, c.CustomerSpecialNotes, c.BirthDay, c.MaritialStatus, c.Age, c.EmailAddress, c.Sexuality, c.FacebookAccount, c.TwitterAccount, 
 c.WebSite, c.PhotoPath, c.ProximityCardID, c.EditKey, c.SyncKey, c.BranchID, c.LockData, c.LockStationID, c.AddUserID, c.AddDateTime, c.EditUserID, c.EditDateTime, 
 c.OpenValue, c.WebUserName, c.WebPassword 
 FROM CustomerFiles AS c INNER JOIN 
 EmployeeFiles AS e ON c.CustomerKey = e.EmployeeKey where isnull(c.CustomerIsActive,0)=1 AND isnull(e.EmployeeActive,0)=1 

 --Mesaj Qruplari - Modifikasiyalar ve modifikasiya qruplari
SELECT MenuModifierID, MenuModifierText, AdditionalCost, MenuModifierActive, SecLangModifierText, PictureName, ShowCaption, PizzaCrust, PizzaTopping, BarMixer, IsKdsAddition, IsKdsSent, SecurityLevel, DeleteReason, CustomField1, 
                  CustomField2, CustomField3, CustomField4, CustomField5, EditKey, SyncKey, BranchID, AddUserID, AddDateTime, EditUserID, EditDateTime, MenuModifierKey, DepositWeight, OrderByWeightAddProduct
FROM     MenuModifiers
SELECT AutoID, BranchID, MenuModifierGroupID, MenuModifierID, DisplayIndex, IsSelected, MenuModifierLayoutKey, MenuModifierGroupKey, MenuModifierKey, EditKey, SyncKey
FROM     MenuModifierLayout
SELECT MenuModifierGroupID, MenuModifierGroupText, DisplayIndex, MenuModifierGroupActive, SecLangMenuModifierGroupText, PictureName, ButtonColor, RevenueCenterTypeID, DeleteReason, CustomField1, CustomField2, CustomField3, 
                  CustomField4, CustomField5, EditKey, SyncKey, BranchID, AddUserID, AddDateTime, EditUserID, EditDateTime, MenuModifierGroupKey
FROM     MenuModifierGroups

--Endirim Filteri
SELECT AutoID, DiscountKey, MenuGroupKey, MenuItemKey, BranchID, IsSaleFilter
FROM     DiscountFilter

--Promosyonlar
SELECT PromotionID, PromotionKey, PromotionName, PromotionNotes, IsActive, UseAuto, MenuGroupKey, MenuGroupCondition, MenuGroupMinSale, MenuItemKey, MenuItemsCondition, MenuItemsMinSale, StartDate, EndDate, StartHour, 
                  EndHour, SaleTypes, SaleTypeCondition, Stations, StationCondition, Days, DayCondition, Employees, EmployeeCondition, CustomerGroup, CustomerGroupCondition, PaymentTypes, PaymentTypeCondition, GiftMenuGroupKey, 
                  GiftMenuGroupContidion, GiftMenuGroupQuantity, GiftMenuItemKey, GiftMenuItemsCondition, GiftMenuItemQuantity, GiftMenuDiscountAmount, GiftMenuDiscountPercent, GiftOrderDiscountAmount, GiftOrderDiscountPercent, 
                  GiftStaticPrice, GiftBonusPercent, EditKey, SyncKey, BranchID, AddUserID, AddDateTime, EditUserID, EditDateTime, MenuItemKeyList, MinimumOrderAmount, PromotionPriority, AllowJointlyUse, GiftMenuItemKeyList, 
                  IgnoredPromotionKeys
FROM     Promotions

--Yemek Sebeti Eshleshme
SELECT AutoID, MenuItemKey, MenuItemText,'' AS YemekSepetiText,isnull(SecLangMenuItemText,'') AS YemekSepetiSKU,'' AS oldYemekSepetiText,0 as IsRepeated FROM MenuItems ORDER BY MenuItemText

--Siparis Kartlari
SELECT [AutoID] , [CardName] , [CardNumber] , [CardName] as oldCardName , [CardNumber] as oldCardNumber FROM [OrderCards]