CREATE TABLE [dbo].[User]
(
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
    [Address] BIGINT NOT NULL, 
    [Division] BIGINT NOT NULL, 
    [TimeZoneOffset] DATETIME2 NOT NULL, 
	[Login] [nvarchar](50) NOT NULL,
	[Email] [nvarchar](255) NOT NULL,
	[Password] [nvarchar](500) NULL,
	[CreationDate] DATETIME2 NULL,
	[ApprovalDate] DATETIME2 NULL,
    [ExpireDate] DATETIME2 NULL,
	[LastLoginDate] DATETIME2 NULL,
	[IsLocked] [bit] NOT NULL DEFAULT 0,
	[PasswordQuestion] [nvarchar](max) NULL,
	[PasswordAnswer] [nvarchar](max) NULL,
	[EmailConfirmed] [bit] NOT NULL DEFAULT 0,
	[SecurityStamp] [nvarchar](max) NULL,
	[PhoneNumber] [nvarchar](50) NULL,
	[PhoneNumberConfirmed] [bit] NOT NULL DEFAULT 0,
	[TwoFactorEnabled] [bit] NOT NULL DEFAULT 0,
	[LockoutEndDateUtc] DATETIME2 NULL,
	[LockoutEnabled] [bit] NOT NULL DEFAULT 0,
	[AccessFailedCount] [int] NOT NULL DEFAULT 0,
	
	CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [UX_User_Email] UNIQUE NONCLUSTERED ([Email] ASC),
	CONSTRAINT [UX_User_Login] UNIQUE NONCLUSTERED ([Login] ASC)
)
GO


CREATE TABLE [dbo].[Role] (
    [Id]   BIGINT IDENTITY (1, 1) NOT NULL,
    [Name] NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED ([Id] ASC)
)
GO

CREATE TABLE [dbo].[UserRole] (
    [UserId] BIGINT NOT NULL,
    [RoleId] BIGINT NOT NULL,
    CONSTRAINT [PK_UserRole] PRIMARY KEY CLUSTERED ([UserId] ASC, [RoleId] ASC),
    CONSTRAINT [FK_UserRole_Role] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Role] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_UserRole_User] FOREIGN KEY ([UserId]) REFERENCES [dbo].[User] ([Id]) ON DELETE CASCADE
)
GO

CREATE NONCLUSTERED INDEX [IX_RoleId]
    ON [dbo].[UserRole]([RoleId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_UserId]
    ON [dbo].[UserRole]([UserId] ASC);
GO

CREATE TABLE [dbo].[UserLogin] (
    [UserId]        BIGINT NOT NULL,
    [LoginProvider] NVARCHAR (128) NOT NULL,
    [ProviderKey]   NVARCHAR (128) NOT NULL,
    CONSTRAINT [PK_UserLogin] PRIMARY KEY CLUSTERED ([UserId] ASC, [LoginProvider] ASC, [ProviderKey] ASC),
    CONSTRAINT [FK_UserLogin_User] FOREIGN KEY ([UserId]) REFERENCES [dbo].[User] ([Id]) ON DELETE CASCADE
)
GO

CREATE NONCLUSTERED INDEX [IX_UserId]
    ON [dbo].[UserLogin]([UserId] ASC);
GO

CREATE TABLE [dbo].[UserClaim] (
    [Id]         BIGINT IDENTITY (1, 1) NOT NULL,
    [UserId]     BIGINT NOT NULL,
    [ClaimType]  NVARCHAR (MAX) NULL,
    [ClaimValue] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_UserClaim] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_UserClaim_User] FOREIGN KEY ([UserId]) REFERENCES [dbo].[User] ([Id]) ON DELETE CASCADE
)
GO

CREATE NONCLUSTERED INDEX [IX_User_Id]
    ON [dbo].[UserClaim]([UserId] ASC);

GO

CREATE TABLE [dbo].[Product]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY (1, 1), 
    [name] NVARCHAR(MAX) NOT NULL, 
    [categoryId] BIGINT NOT NULL, 
    [price] MONEY NOT NULL, 
    
)
GO


CREATE TABLE [dbo].[Category]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY (1, 1), 
    [name] NVARCHAR(MAX) NOT NULL
)
GO

CREATE TABLE [dbo].[Transaction]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY (1, 1), 
    [purchaserId] BIGINT NOT NULL, 
    [requestedForUserId] BIGINT NULL, 
    [productId] BIGINT NOT NULL, 
    [quantity] INT NOT NULL, 
    [unitPrice] MONEY NOT NULL, 
    [purchaseDate] DATETIME NOT NULL, 
    [cartId] BIGINT NOT NULL, 
    [isDeleted] BIT NOT NULL, 
    
)
GO

CREATE TABLE [dbo].[ReturnState]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY (1, 1), 
    [state] NVARCHAR(MAX) NOT NULL
)
GO

CREATE TABLE [dbo].[Division]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY (1, 1), 
    [name] NVARCHAR(MAX) NOT NULL
)
GO


CREATE TABLE [dbo].[Address]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY (1, 1), 
    [addressLine1] NVARCHAR(MAX) NOT NULL, 
    [addressLine2] NVARCHAR(MAX) NULL, 
    [city] NVARCHAR(MAX) NOT NULL, 
    [state]NVARCHAR(MAX) NOT NULL, 
    [postalCode] NVARCHAR(MAX) NOT NULL, 
    [country] NVARCHAR(MAX) NOT NULL
)
GO

CREATE TABLE [dbo].[NotRestricted]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY (1, 1), 
	[name] NVARCHAR(MAX) NOT NULL,
    [Role] BIGINT NOT NULL, 
    [Division] BIGINT NOT NULL
    
)
GO

CREATE TABLE [dbo].[ReturnLink]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY (1, 1),  
    [transactionId] BIGINT NOT NULL, 
    [returnStateId] BIGINT NOT NULL, 
    [dateStateChanged] DATETIME NOT NULL
)
GO

CREATE TABLE [dbo].[Notifications]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY (1, 1), 
	[AddressId] BIGINT NOT NULL,
	[Email] NVARCHAR(MAX) NOT NULL,
	[DivisionId] BIGINT NOT NULL,
    [notifyType] NVARCHAR(50) NOT NULL, 
    [notifyText] TEXT NOT NULL, 
    [timeStamp] DATETIME2 NOT NULL
)
GO

ALTER TABLE [dbo].[Notifications] ADD CONSTRAINT [FK_Notifications_AddressId] FOREIGN KEY ([AddressId]) REFERENCES [Address]([Id])
GO

ALTER TABLE [dbo].[Notifications] ADD CONSTRAINT [FK_Notifications_DivisionId] FOREIGN KEY ([DivisionId]) REFERENCES [Division]([Id])
GO


ALTER TABLE [dbo].[User] ADD CONSTRAINT [FK_Address] FOREIGN KEY ([Address]) REFERENCES [Address]([Id])
GO

ALTER TABLE [dbo].[User] ADD CONSTRAINT [FK_Division] FOREIGN KEY ([Division]) REFERENCES [Division]([Id])
GO

ALTER TABLE [dbo].[Product] ADD CONSTRAINT [FK_categoryId] FOREIGN KEY ([categoryId]) REFERENCES [Category]([Id])
GO

ALTER TABLE [dbo].[Transaction] ADD CONSTRAINT [FK_productId] FOREIGN KEY ([productId]) REFERENCES [Product]([Id])
GO

ALTER TABLE [dbo].[Transaction] ADD CONSTRAINT [FK_purchaserId] FOREIGN KEY ([purchaserId]) REFERENCES [User]([Id])
GO

ALTER TABLE [dbo].[Transaction] ADD CONSTRAINT [FK_requestedForUserId] FOREIGN KEY ([requestedForUserId]) REFERENCES [User]([Id])
GO

ALTER TABLE [dbo].[ReturnLink] ADD CONSTRAINT [FK_transactionId] FOREIGN KEY ([transactionId]) REFERENCES [Transaction]([Id])
GO

ALTER TABLE [dbo].[ReturnLink] ADD CONSTRAINT [FK_returnStateId] FOREIGN KEY ([returnStateId]) REFERENCES [ReturnState]([Id])
GO

ALTER TABLE [dbo].[NotRestricted] ADD CONSTRAINT [FKLink_Role] FOREIGN KEY ([Role]) REFERENCES [Role]([Id])
GO

ALTER TABLE [dbo].[NotRestricted] ADD CONSTRAINT [FKLink_Division] FOREIGN KEY ([Division]) REFERENCES [Division]([Id])
GO

