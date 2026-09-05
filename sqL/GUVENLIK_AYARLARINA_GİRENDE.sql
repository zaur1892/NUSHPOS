-- 1. Sessiya sazlamaları
SET QUOTED_IDENTIFIER ON;
SET ARITHABORT OFF;
SET NUMERIC_ROUNDABORT OFF;
SET ANSI_WARNINGS ON;
SET ANSI_PADDING ON;
SET ANSI_NULLS ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET CURSOR_CLOSE_ON_COMMIT OFF;
SET IMPLICIT_TRANSACTIONS OFF;
SET LANGUAGE us_english;
SET DATEFORMAT mdy;
SET DATEFIRST 7;
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
GO

-- 2. Giriş jurnalı qeydi
EXEC sp_executesql 
    N'INSERT INTO [AccessLogs] (
        [BranchID], [LogDate], [StationID], [EmployeeID], [ActionName], 
        [WrongPassword], [AdditionalInfo], [IsSuccess], [OrderKey], 
        [TransactionKey], [AccessLogKey], [EditKey], [SyncKey]
    ) VALUES (
        @BranchID, GETDATE(), @StationID, @EmployeeID, @ActionName, 
        @WrongPassword, @AdditionalInfo, @IsSuccess, @OrderKey, 
        @TransactionKey, NEWID(), @EditKey, @SyncKey
    )',
    N'@BranchID int, @LogDate datetime, @StationID int, @EmployeeID int, 
      @ActionName nvarchar(26), @WrongPassword nvarchar(4000), @AdditionalInfo nvarchar(4000), 
      @IsSuccess bit, @OrderKey uniqueidentifier, @TransactionKey uniqueidentifier, 
      @AccessLogKey uniqueidentifier, @EditKey uniqueidentifier, @SyncKey uniqueidentifier',
    @BranchID = 422,
    @LogDate = '2026-09-05 09:13:20.370',
    @StationID = 1,
    @EmployeeID = 106,
    @ActionName = N'GÜVENLİK AYARLARINA ERİŞİM',
    @WrongPassword = NULL,
    @AdditionalInfo = NULL,
    @IsSuccess = 1,
    @OrderKey = '00000000-0000-0000-0000-000000000000',
    @TransactionKey = '00000000-0000-0000-0000-000000000000',
    @AccessLogKey = NULL,
    @EditKey = '13A93AA2-05EF-4CA3-8F76-A14B8FB841A7',
    @SyncKey = '4884B6A4-9F8A-4B89-9EF0-30CACA9BC511';
GO

-- 3. Səlahiyyətlər siyahısının oxunması
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SELECT 
    [AuthorityID], 
    [GroupName], 
    [AuthorityKey], 
    [AuthorityText], 
    [AuthorityDescription], 
    [DefaultLevel], 
    [DefaultLevel] AS OldLevel, 
    ISNULL([NeedAllways], 0) AS NeedAllways, 
    ISNULL([NeedAllways], 0) AS OldNeedAllways, 
    ISNULL([AllowManager], 0) AS AllowManager, 
    ISNULL([AllowManager], 0) AS OldAllowManager,
    ISNULL(AllowCashier, 0) AS AllowCashier,
    ISNULL(AllowCashier, 0) AS oldAllowCashier,
    [EditKey], 
    [SyncKey], 
    [BranchID] 
FROM [AuthorityList]  
WHERE ISNULL(DefaultLevel, 0) <= 10;
GO

-- 4. Qrup adlarının qruplaşdırılaraq oxunması
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SELECT 
    GroupName  
FROM [AuthorityList] 
GROUP BY GroupName;
GO
```[cite: 1]