USE [master]
GO
/****** Object:  Database [TOMFURNITURE]    Script Date: 21/07/2025 10:34:38 AM ******/
CREATE DATABASE [TOMFURNITURE]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'TOMFURNITURE', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\TOMFURNITURE.mdf' , SIZE = 73728KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'TOMFURNITURE_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\TOMFURNITURE_log.ldf' , SIZE = 204800KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [TOMFURNITURE] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [TOMFURNITURE].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [TOMFURNITURE] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [TOMFURNITURE] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [TOMFURNITURE] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [TOMFURNITURE] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [TOMFURNITURE] SET ARITHABORT OFF 
GO
ALTER DATABASE [TOMFURNITURE] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [TOMFURNITURE] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [TOMFURNITURE] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [TOMFURNITURE] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [TOMFURNITURE] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [TOMFURNITURE] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [TOMFURNITURE] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [TOMFURNITURE] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [TOMFURNITURE] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [TOMFURNITURE] SET  DISABLE_BROKER 
GO
ALTER DATABASE [TOMFURNITURE] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [TOMFURNITURE] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [TOMFURNITURE] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [TOMFURNITURE] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [TOMFURNITURE] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [TOMFURNITURE] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [TOMFURNITURE] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [TOMFURNITURE] SET RECOVERY FULL 
GO
ALTER DATABASE [TOMFURNITURE] SET  MULTI_USER 
GO
ALTER DATABASE [TOMFURNITURE] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [TOMFURNITURE] SET DB_CHAINING OFF 
GO
ALTER DATABASE [TOMFURNITURE] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [TOMFURNITURE] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [TOMFURNITURE] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [TOMFURNITURE] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'TOMFURNITURE', N'ON'
GO
ALTER DATABASE [TOMFURNITURE] SET QUERY_STORE = ON
GO
ALTER DATABASE [TOMFURNITURE] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [TOMFURNITURE]
GO
/****** Object:  Table [dbo].[Backup_ProductVariantImageURL]    Script Date: 21/07/2025 10:34:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Backup_ProductVariantImageURL](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ImageURL] [nvarchar](255) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Banners]    Script Date: 21/07/2025 10:34:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Banners](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](200) NULL,
	[ImageURL] [nvarchar](255) NOT NULL,
	[LinkURL] [nvarchar](255) NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NOT NULL,
	[DisplayOrder] [int] NULL,
	[Position] [nvarchar](50) NULL,
	[IsActive] [bit] NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedDate] [datetime] NULL,
	[CreatedBy] [nvarchar](255) NULL,
	[UpdatedBy] [nvarchar](255) NULL,
	[UserId] [int] NULL,
	[ImageURLMobile] [nvarchar](255) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Brands]    Script Date: 21/07/2025 10:34:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Brands](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BrandName] [nvarchar](100) NULL,
	[ImageURL] [nvarchar](255) NULL,
	[IsActive] [bit] NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedDate] [datetime] NULL,
	[CreatedBy] [nvarchar](255) NULL,
	[UpdatedBy] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Cart]    Script Date: 21/07/2025 10:34:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Cart](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Quantity] [int] NOT NULL,
	[IsActive] [bit] NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedDate] [datetime] NULL,
	[CreatedBy] [nvarchar](255) NULL,
	[UpdatedBy] [nvarchar](255) NULL,
	[UserID] [int] NULL,
	[ProVarID] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Categories]    Script Date: 21/07/2025 10:34:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Categories](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CategoryName] [nvarchar](50) NULL,
	[Descriptions] [nvarchar](255) NULL,
	[ImageURL] [nvarchar](255) NULL,
	[IsActive] [bit] NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedDate] [datetime] NULL,
	[CreatedBy] [nvarchar](255) NULL,
	[UpdatedBy] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Colors]    Script Date: 21/07/2025 10:34:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Colors](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ColorName] [nvarchar](50) NOT NULL,
	[ColorCode] [nvarchar](100) NULL,
	[IsActive] [bit] NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedDate] [datetime] NULL,
	[CreatedBy] [nvarchar](255) NULL,
	[UpdatedBy] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Comment]    Script Date: 21/07/2025 10:34:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Comment](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[LikeCount] [int] NOT NULL,
	[Content] [nvarchar](max) NULL,
	[IsActive] [bit] NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedDate] [datetime] NULL,
	[CreatedBy] [nvarchar](255) NULL,
	[UpdatedBy] [nvarchar](255) NULL,
	[ProReviewID] [int] NULL,
	[UserId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ConfirmOTP]    Script Date: 21/07/2025 10:34:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ConfirmOTP](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OTPCode] [varchar](6) NOT NULL,
	[ExpiredDate] [datetime] NULL,
	[CreatedDate] [datetime] NULL,
	[UserID] [int] NULL,
	[FailedAttempt] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Countries]    Script Date: 21/07/2025 10:34:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Countries](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CountryName] [nvarchar](100) NULL,
	[ImageURL] [nvarchar](255) NULL,
	[IsActive] [bit] NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedDate] [datetime] NULL,
	[CreatedBy] [nvarchar](255) NULL,
	[UpdatedBy] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Feedbacks]    Script Date: 21/07/2025 10:34:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Feedbacks](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Message] [nvarchar](255) NOT NULL,
	[ParentFeedbackID] [int] NULL,
	[IsActive] [bit] NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedDate] [datetime] NULL,
	[CreatedBy] [nvarchar](255) NULL,
	[UpdatedBy] [nvarchar](255) NULL,
	[UserID] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ImageProductReviews]    Script Date: 21/07/2025 10:34:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ImageProductReviews](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ImageURL] [nvarchar](255) NULL,
	[Attribute] [nvarchar](255) NULL,
	[IsActive] [bit] NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedDate] [datetime] NULL,
	[CreatedBy] [nvarchar](255) NULL,
	[UpdatedBy] [nvarchar](255) NULL,
	[ProductReviewID] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Materials]    Script Date: 21/07/2025 10:34:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Materials](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MaterialName] [nvarchar](100) NOT NULL,
	[IsActive] [bit] NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedDate] [datetime] NULL,
	[CreatedBy] [nvarchar](255) NULL,
	[UpdatedBy] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[News]    Script Date: 21/07/2025 10:34:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[News](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](50) NOT NULL,
	[Content] [nvarchar](max) NULL,
	[NewsAvatar] [nvarchar](255) NULL,
	[IsActive] [bit] NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedDate] [datetime] NULL,
	[CreatedBy] [nvarchar](255) NULL,
	[UpdatedBy] [nvarchar](255) NULL,
	[UserId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrderAddress]    Script Date: 21/07/2025 10:34:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderAddress](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Recipient] [nvarchar](100) NOT NULL,
	[PhoneNumber] [varchar](15) NOT NULL,
	[AddressDetailRecipient] [nvarchar](500) NOT NULL,
	[City] [nvarchar](100) NOT NULL,
	[District] [nvarchar](100) NOT NULL,
	[Ward] [nvarchar](100) NOT NULL,
	[IsDeafaultAddress] [bit] NOT NULL,
	[IsActive] [bit] NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedDate] [datetime] NULL,
	[CreatedBy] [nvarchar](255) NULL,
	[UpdatedBy] [nvarchar](255) NULL,
	[UserID] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrderDetails]    Script Date: 21/07/2025 10:34:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderDetails](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Quantity] [int] NOT NULL,
	[Price] [decimal](15, 2) NOT NULL,
	[IsActive] [bit] NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedDate] [datetime] NULL,
	[CreatedBy] [nvarchar](255) NULL,
	[UpdatedBy] [nvarchar](255) NULL,
	[ProVarID] [int] NULL,
	[OrderID] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Orders]    Script Date: 21/07/2025 10:34:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Orders](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OrderDate] [datetime] NOT NULL,
	[Total] [decimal](15, 2) NULL,
	[PriceDiscount] [decimal](15, 2) NOT NULL,
	[ShippingPrice] [decimal](15, 2) NOT NULL,
	[IsPaid] [bit] NOT NULL,
	[Note] [nvarchar](max) NULL,
	[IsActive] [bit] NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedDate] [datetime] NULL,
	[CreatedBy] [nvarchar](255) NULL,
	[UpdatedBy] [nvarchar](255) NULL,
	[UserID] [int] NULL,
	[OrderAddID] [int] NULL,
	[OrderStaID] [int] NULL,
	[PaymentMethodID] [int] NULL,
	[PromotionID] [int] NULL,
	[DeliveryDate] [datetime] NULL,
	[IsUserGuest] [bit] NOT NULL,
	[UserGuestID] [int] NULL,
	[PaymentStatus] [nvarchar](100) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrderStatus]    Script Date: 21/07/2025 10:34:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderStatus](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OrderStatusName] [nvarchar](100) NOT NULL,
	[IsActive] [bit] NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedDate] [datetime] NULL,
	[CreatedBy] [nvarchar](255) NULL,
	[UpdatedBy] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PaymentMethods]    Script Date: 21/07/2025 10:34:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PaymentMethods](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[NamePaymentMethod] [nvarchar](100) NULL,
	[IsActive] [bit] NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedDate] [datetime] NULL,
	[CreatedBy] [nvarchar](255) NULL,
	[UpdatedBy] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProductReviews]    Script Date: 21/07/2025 10:34:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductReviews](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[StarRating] [int] NOT NULL,
	[Content] [nvarchar](max) NULL,
	[RelatedVideo] [nvarchar](255) NULL,
	[IsActive] [bit] NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedDate] [datetime] NULL,
	[CreatedBy] [nvarchar](255) NULL,
	[UpdatedBy] [nvarchar](255) NULL,
	[UserId] [int] NULL,
	[ProID] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Products]    Script Date: 21/07/2025 10:34:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Products](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProductName] [nvarchar](200) NOT NULL,
	[SpecificationDescription] [nvarchar](max) NULL,
	[ViewCount] [int] NOT NULL,
	[IsActive] [bit] NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedDate] [datetime] NULL,
	[CreatedBy] [nvarchar](255) NULL,
	[UpdatedBy] [nvarchar](255) NULL,
	[BrandID] [int] NULL,
	[CategoryID] [int] NULL,
	[CountriesID] [int] NULL,
	[SupplierID] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProductVariant]    Script Date: 21/07/2025 10:34:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductVariant](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OriginalPrice] [decimal](18, 2) NOT NULL,
	[DiscountedPrice] [decimal](18, 2) NULL,
	[StockQty] [int] NOT NULL,
	[ProductID] [int] NOT NULL,
	[ColorID] [int] NULL,
	[SizeID] [int] NULL,
	[MaterialID] [int] NULL,
	[UnitID] [int] NULL,
	[IsActive] [bit] NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedDate] [datetime] NULL,
	[CreatedBy] [nvarchar](255) NULL,
	[UpdatedBy] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProductVariantImages]    Script Date: 21/07/2025 10:34:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductVariantImages](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ImageURL] [nvarchar](255) NULL,
	[Attribute] [nvarchar](255) NULL,
	[DisplayOrder] [int] NULL,
	[IsActive] [bit] NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedDate] [datetime] NULL,
	[CreatedBy] [nvarchar](255) NULL,
	[UpdatedBy] [nvarchar](255) NULL,
	[ProVarID] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Promotions]    Script Date: 21/07/2025 10:34:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Promotions](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PromotionCode] [nvarchar](20) NOT NULL,
	[DiscountValue] [decimal](10, 2) NOT NULL,
	[OrderMinimum] [decimal](10, 2) NOT NULL,
	[MaximumDiscountAmount] [decimal](10, 2) NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NOT NULL,
	[CouponUsage] [int] NOT NULL,
	[IsActive] [bit] NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedDate] [datetime] NULL,
	[CreatedBy] [nvarchar](255) NULL,
	[UpdatedBy] [nvarchar](255) NULL,
	[PromotionTypeID] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PromotionTypes]    Script Date: 21/07/2025 10:34:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PromotionTypes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PromotionTypeName] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](255) NULL,
	[PromotionUnit] [int] NOT NULL,
	[IsActive] [bit] NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedDate] [datetime] NULL,
	[CreatedBy] [nvarchar](255) NULL,
	[UpdatedBy] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Roles]    Script Date: 21/07/2025 10:34:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Roles](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RoleName] [varchar](20) NOT NULL,
	[IsActive] [bit] NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedDate] [datetime] NULL,
	[CreatedBy] [nvarchar](255) NULL,
	[UpdatedBy] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Sizes]    Script Date: 21/07/2025 10:34:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Sizes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SizeName] [nvarchar](50) NOT NULL,
	[IsActive] [bit] NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedDate] [datetime] NULL,
	[CreatedBy] [nvarchar](255) NULL,
	[UpdatedBy] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Sliders]    Script Date: 21/07/2025 10:34:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Sliders](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](200) NULL,
	[ImageURL] [nvarchar](255) NOT NULL,
	[LinkURL] [nvarchar](255) NOT NULL,
	[IsPoster] [bit] NULL,
	[Position] [nvarchar](50) NULL,
	[DisplayOrder] [int] NOT NULL,
	[IsActive] [bit] NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedDate] [datetime] NULL,
	[CreatedBy] [nvarchar](255) NULL,
	[UpdatedBy] [nvarchar](255) NULL,
	[ProductId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StoreInformation]    Script Date: 21/07/2025 10:34:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StoreInformation](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[StoreName] [nvarchar](100) NULL,
	[StoreAddress] [nvarchar](255) NULL,
	[Logo] [nvarchar](255) NULL,
	[PhoneNumber] [nvarchar](20) NULL,
	[Email] [nvarchar](100) NULL,
	[LinkWebsite] [nvarchar](255) NULL,
	[Latitude] [decimal](9, 6) NULL,
	[Longitude] [decimal](9, 6) NULL,
	[OwnerName] [nvarchar](100) NULL,
	[BusinessType] [nvarchar](50) NOT NULL,
	[OperatingHours] [nvarchar](100) NULL,
	[StoreDescription] [nvarchar](500) NULL,
	[EstablishmentDate] [date] NULL,
	[TaxId] [varchar](20) NULL,
	[BranchCode] [varchar](20) NULL,
	[LinkSocialFacebook] [nvarchar](255) NULL,
	[LinkSocialTwitter] [nvarchar](255) NULL,
	[LinkSocialInstagram] [nvarchar](255) NULL,
	[LinkSocialTiktok] [nvarchar](255) NULL,
	[LinkSocialYoutube] [nvarchar](255) NULL,
	[IsActive] [bit] NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedDate] [datetime] NULL,
	[CreatedBy] [nvarchar](255) NULL,
	[UpdatedBy] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Suppliers]    Script Date: 21/07/2025 10:34:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Suppliers](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SupplierName] [nvarchar](100) NULL,
	[ContactName] [nvarchar](100) NULL,
	[Email] [nvarchar](100) NOT NULL,
	[PhoneNumber] [nvarchar](20) NULL,
	[ImageURL] [nvarchar](255) NULL,
	[Notes] [nvarchar](500) NULL,
	[TaxID] [varchar](20) NOT NULL,
	[IsActive] [bit] NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedDate] [datetime] NULL,
	[CreatedBy] [nvarchar](255) NULL,
	[UpdatedBy] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Units]    Script Date: 21/07/2025 10:34:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Units](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UnitName] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](255) NULL,
	[IsActive] [bit] NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedDate] [datetime] NULL,
	[CreatedBy] [nvarchar](255) NULL,
	[UpdatedBy] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserGuest]    Script Date: 21/07/2025 10:34:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserGuest](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FullName] [nvarchar](100) NOT NULL,
	[PhoneNumber] [nvarchar](20) NOT NULL,
	[Email] [nvarchar](100) NULL,
	[DetailAddress] [nvarchar](255) NOT NULL,
	[City] [nvarchar](100) NULL,
	[District] [nvarchar](100) NULL,
	[Ward] [nvarchar](100) NULL,
	[CreatedDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 21/07/2025 10:34:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [nvarchar](100) NOT NULL,
	[Gender] [bit] NOT NULL,
	[Email] [nvarchar](100) NOT NULL,
	[PasswordHash] [nvarchar](255) NOT NULL,
	[PhoneNumber] [varchar](15) NULL,
	[UserAddress] [nvarchar](255) NULL,
	[IsActive] [bit] NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedDate] [datetime] NULL,
	[CreatedBy] [nvarchar](255) NULL,
	[UpdatedBy] [nvarchar](255) NULL,
	[RoleID] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Brands] ON 

INSERT [dbo].[Brands] ([Id], [BrandName], [ImageURL], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (1, N'Mason', N'', 1, CAST(N'2025-07-03T13:20:27.013' AS DateTime), NULL, N'Admin', N'')
INSERT [dbo].[Brands] ([Id], [BrandName], [ImageURL], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (2, N'Hyper', N'', 1, CAST(N'2025-07-03T13:20:27.013' AS DateTime), NULL, N'Admin', N'')
INSERT [dbo].[Brands] ([Id], [BrandName], [ImageURL], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (3, N'ádasdasdasdasdasd', N'https://res.cloudinary.com/duelcb8ki/image/upload/v1752899009/h2sxwlb7urxlztzxg47i.png', 1, CAST(N'2025-07-19T11:22:54.860' AS DateTime), CAST(N'2025-07-19T11:23:28.270' AS DateTime), NULL, NULL)
SET IDENTITY_INSERT [dbo].[Brands] OFF
GO
SET IDENTITY_INSERT [dbo].[Categories] ON 

INSERT [dbo].[Categories] ([Id], [CategoryName], [Descriptions], [ImageURL], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (2, N'Press Tables', N'', N'https://res.cloudinary.com/duelcb8ki/image/upload/v1750869004/d14pp6v1ydb4rqy0yaid.jpg', 1, CAST(N'2025-07-03T13:18:32.873' AS DateTime), NULL, N'Admin', N'')
INSERT [dbo].[Categories] ([Id], [CategoryName], [Descriptions], [ImageURL], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (4, N'Spoke Sofa', N'', N'https://res.cloudinary.com/duelcb8ki/image/upload/v1750869017/rj9uv7mncbqjwyafa1uo.jpg', 1, CAST(N'2025-07-03T13:18:32.873' AS DateTime), NULL, N'Admin', N'')
INSERT [dbo].[Categories] ([Id], [CategoryName], [Descriptions], [ImageURL], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (7, N'Longe Chairs', N'', N'https://res.cloudinary.com/duelcb8ki/image/upload/v1750869049/mdulmxe0rldebqnomnut.jpg', 1, CAST(N'2025-07-03T13:18:32.873' AS DateTime), NULL, N'Admin', N'')
INSERT [dbo].[Categories] ([Id], [CategoryName], [Descriptions], [ImageURL], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (8, N'Bend Chairs', N'', N'https://res.cloudinary.com/duelcb8ki/image/upload/v1750869068/fofxc5kurt9bjis5ttsw.jpg', 1, CAST(N'2025-07-03T13:18:32.873' AS DateTime), NULL, N'Admin', N'')
INSERT [dbo].[Categories] ([Id], [CategoryName], [Descriptions], [ImageURL], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (9, N'Accessories', N'', N'https://res.cloudinary.com/duelcb8ki/image/upload/v1750869088/jip4ai0xcsnuktaolihz.jpg', 1, CAST(N'2025-07-03T13:18:32.873' AS DateTime), NULL, N'Admin', N'')
INSERT [dbo].[Categories] ([Id], [CategoryName], [Descriptions], [ImageURL], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (10, N'Cross Tables', N'', N'https://res.cloudinary.com/duelcb8ki/image/upload/v1750869095/zjh5ihwrfgkq7tzldc71.jpg', 1, CAST(N'2025-07-03T13:18:32.873' AS DateTime), NULL, N'Admin', N'')
INSERT [dbo].[Categories] ([Id], [CategoryName], [Descriptions], [ImageURL], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (11, N'Bar Chairs', N'', N'https://res.cloudinary.com/duelcb8ki/image/upload/v1750869104/y77ojxsj1yxep5ixggul.jpg', 1, CAST(N'2025-07-03T13:18:32.873' AS DateTime), NULL, N'Admin', N'')
INSERT [dbo].[Categories] ([Id], [CategoryName], [Descriptions], [ImageURL], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (12, N'Wardrobe', N'Wardrobe', N'https://res.cloudinary.com/duelcb8ki/image/upload/v1751525798/fksmvvbrrqmutpn7quip.webp', 1, CAST(N'2025-07-03T06:56:37.207' AS DateTime), NULL, NULL, NULL)
INSERT [dbo].[Categories] ([Id], [CategoryName], [Descriptions], [ImageURL], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (13, N'Shelf', NULL, N'https://res.cloudinary.com/duelcb8ki/image/upload/v1751527740/dy9kf9qwat6vh6nv5fqa.png', 1, CAST(N'2025-07-03T07:21:43.263' AS DateTime), CAST(N'2025-07-03T07:28:59.510' AS DateTime), NULL, NULL)
SET IDENTITY_INSERT [dbo].[Categories] OFF
GO
SET IDENTITY_INSERT [dbo].[Colors] ON 

INSERT [dbo].[Colors] ([Id], [ColorName], [ColorCode], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (1, N'Light Beige', N'#ded5c7', 1, CAST(N'2025-07-03T13:18:55.723' AS DateTime), NULL, N'Admin', N'')
INSERT [dbo].[Colors] ([Id], [ColorName], [ColorCode], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (2, N'Red', N'#9f3e37', 1, CAST(N'2025-07-03T13:18:55.723' AS DateTime), NULL, N'Admin', N'')
INSERT [dbo].[Colors] ([Id], [ColorName], [ColorCode], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (3, N'Soft Green', N'#cedbba', 1, CAST(N'2025-07-03T13:18:55.723' AS DateTime), NULL, N'Admin', N'')
INSERT [dbo].[Colors] ([Id], [ColorName], [ColorCode], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (4, N'Navy Blue', N'#323854', 1, CAST(N'2025-07-03T13:18:55.723' AS DateTime), NULL, N'Admin', N'')
INSERT [dbo].[Colors] ([Id], [ColorName], [ColorCode], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (5, N'Brown', N'#9f8568', 1, CAST(N'2025-07-03T13:18:55.723' AS DateTime), NULL, N'Admin', N'')
INSERT [dbo].[Colors] ([Id], [ColorName], [ColorCode], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (6, N'Grey', N'#e3e3e3', 1, CAST(N'2025-07-03T13:18:55.723' AS DateTime), NULL, N'Admin', N'')
INSERT [dbo].[Colors] ([Id], [ColorName], [ColorCode], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (7, N'Charcoal', N'#736e69', 1, CAST(N'2025-07-03T13:18:55.723' AS DateTime), NULL, N'Admin', N'')
INSERT [dbo].[Colors] ([Id], [ColorName], [ColorCode], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (8, N'Chocolate', N'#573a19', 1, CAST(N'2025-07-03T13:18:55.723' AS DateTime), NULL, N'Admin', N'')
INSERT [dbo].[Colors] ([Id], [ColorName], [ColorCode], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (9, N'Yellow', N'#f2c862', 1, CAST(N'2025-07-03T13:18:55.723' AS DateTime), NULL, N'Admin', N'')
INSERT [dbo].[Colors] ([Id], [ColorName], [ColorCode], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (10, N'Blue', N'#b8d8df', 1, CAST(N'2025-07-03T13:18:55.723' AS DateTime), NULL, N'Admin', N'')
INSERT [dbo].[Colors] ([Id], [ColorName], [ColorCode], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (11, N'Black', N'#000000', 1, CAST(N'2025-07-03T13:18:55.723' AS DateTime), NULL, N'Admin', N'')
INSERT [dbo].[Colors] ([Id], [ColorName], [ColorCode], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (12, N'White', N'#FFFFFF', 1, CAST(N'2025-07-03T13:18:55.723' AS DateTime), NULL, N'Admin', N'')
INSERT [dbo].[Colors] ([Id], [ColorName], [ColorCode], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (13, N'Olive', N'#334a2c', 1, CAST(N'2025-07-03T13:18:55.723' AS DateTime), NULL, N'Admin', N'')
SET IDENTITY_INSERT [dbo].[Colors] OFF
GO
SET IDENTITY_INSERT [dbo].[Countries] ON 

INSERT [dbo].[Countries] ([Id], [CountryName], [ImageURL], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (1, N'Vietnamese', N'https://res.cloudinary.com/duelcb8ki/image/upload/v1751035414/xfnr9vl8i3aigsy3ffhn.webp', 1, CAST(N'2025-07-03T13:18:37.630' AS DateTime), NULL, N'Admin', N'')
INSERT [dbo].[Countries] ([Id], [CountryName], [ImageURL], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (2, N'sweden', N'https://res.cloudinary.com/duelcb8ki/image/upload/v1751035406/rz5aciz2guaihxe1zxwa.png', 1, CAST(N'2025-07-03T13:18:37.630' AS DateTime), NULL, N'Admin', N'')
INSERT [dbo].[Countries] ([Id], [CountryName], [ImageURL], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (3, N'United State', N'https://res.cloudinary.com/duelcb8ki/image/upload/v1751035361/vumaslgjwrpl0bq3inpr.png', 1, CAST(N'2025-07-03T13:18:37.630' AS DateTime), NULL, N'Admin', N'')
SET IDENTITY_INSERT [dbo].[Countries] OFF
GO
SET IDENTITY_INSERT [dbo].[Materials] ON 

INSERT [dbo].[Materials] ([Id], [MaterialName], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (1, N'Pine', 1, CAST(N'2025-07-03T13:19:02.077' AS DateTime), NULL, N'Admin', N'')
INSERT [dbo].[Materials] ([Id], [MaterialName], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (2, N'Oak', 1, CAST(N'2025-07-03T13:19:02.077' AS DateTime), NULL, N'Admin', N'')
INSERT [dbo].[Materials] ([Id], [MaterialName], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (3, N'Wool', 1, CAST(N'2025-07-03T13:19:02.077' AS DateTime), NULL, N'Admin', N'')
INSERT [dbo].[Materials] ([Id], [MaterialName], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (4, N'Plastic', 1, CAST(N'2025-07-03T13:19:02.077' AS DateTime), NULL, N'Admin', N'')
INSERT [dbo].[Materials] ([Id], [MaterialName], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (5, N'Felt', 1, CAST(N'2025-07-03T13:19:02.077' AS DateTime), NULL, N'Admin', N'')
INSERT [dbo].[Materials] ([Id], [MaterialName], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (6, N'Metal', 1, CAST(N'2025-07-03T13:19:02.077' AS DateTime), NULL, N'Admin', N'')
INSERT [dbo].[Materials] ([Id], [MaterialName], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (7, N'Fabric', 1, CAST(N'2025-07-03T13:42:27.657' AS DateTime), NULL, NULL, NULL)
INSERT [dbo].[Materials] ([Id], [MaterialName], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (8, N'stainless steel', 1, CAST(N'2025-07-03T14:39:25.843' AS DateTime), NULL, NULL, NULL)
SET IDENTITY_INSERT [dbo].[Materials] OFF
GO
SET IDENTITY_INSERT [dbo].[OrderAddress] ON 

INSERT [dbo].[OrderAddress] ([Id], [Recipient], [PhoneNumber], [AddressDetailRecipient], [City], [District], [Ward], [IsDeafaultAddress], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [UserID]) VALUES (9, N'Tom Baker', N'0876654132', N'askdbasjdkbajskdas', N'HCM', N'11', N'10', 1, 1, CAST(N'2025-07-17T08:36:15.330' AS DateTime), NULL, NULL, NULL, 2)
INSERT [dbo].[OrderAddress] ([Id], [Recipient], [PhoneNumber], [AddressDetailRecipient], [City], [District], [Ward], [IsDeafaultAddress], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [UserID]) VALUES (10, N'Tom Parker', N'0876554132', N'HCM City', N'HCM', N'12', N'10', 1, 1, CAST(N'2025-07-17T09:08:36.253' AS DateTime), CAST(N'2025-07-17T12:50:35.430' AS DateTime), NULL, NULL, 1)
INSERT [dbo].[OrderAddress] ([Id], [Recipient], [PhoneNumber], [AddressDetailRecipient], [City], [District], [Ward], [IsDeafaultAddress], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [UserID]) VALUES (11, N'Hola', N'0876554132', N'HHHHHRTTTTTT', N'HCM', N'12', N'12', 0, 1, CAST(N'2025-07-17T12:50:30.420' AS DateTime), NULL, NULL, NULL, 1)
SET IDENTITY_INSERT [dbo].[OrderAddress] OFF
GO
SET IDENTITY_INSERT [dbo].[OrderDetails] ON 

INSERT [dbo].[OrderDetails] ([Id], [Quantity], [Price], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [ProVarID], [OrderID]) VALUES (22, 12, CAST(1200000.00 AS Decimal(15, 2)), 1, CAST(N'2025-07-20T03:58:15.920' AS DateTime), NULL, NULL, NULL, 1, 36)
SET IDENTITY_INSERT [dbo].[OrderDetails] OFF
GO
SET IDENTITY_INSERT [dbo].[Orders] ON 

INSERT [dbo].[Orders] ([Id], [OrderDate], [Total], [PriceDiscount], [ShippingPrice], [IsPaid], [Note], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [UserID], [OrderAddID], [OrderStaID], [PaymentMethodID], [PromotionID], [DeliveryDate], [IsUserGuest], [UserGuestID], [PaymentStatus]) VALUES (36, CAST(N'2025-07-20T10:58:16.297' AS DateTime), CAST(14449980.00 AS Decimal(15, 2)), CAST(20.00 AS Decimal(15, 2)), CAST(50000.00 AS Decimal(15, 2)), 0, N'Thank you', 1, CAST(N'2025-07-20T03:58:15.917' AS DateTime), NULL, NULL, NULL, 2, 9, 1, 1, 1, NULL, 0, NULL, N'Processing')
SET IDENTITY_INSERT [dbo].[Orders] OFF
GO
SET IDENTITY_INSERT [dbo].[OrderStatus] ON 

INSERT [dbo].[OrderStatus] ([Id], [OrderStatusName], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (1, N'Pending Confirmation', 1, CAST(N'2025-07-03T13:18:42.770' AS DateTime), NULL, N'Admin', N'')
INSERT [dbo].[OrderStatus] ([Id], [OrderStatusName], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (2, N'Confirmed', 1, CAST(N'2025-07-03T13:18:42.770' AS DateTime), NULL, N'Admin', N'')
INSERT [dbo].[OrderStatus] ([Id], [OrderStatusName], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (3, N'Processing', 1, CAST(N'2025-07-03T13:18:42.770' AS DateTime), NULL, N'Admin', N'')
INSERT [dbo].[OrderStatus] ([Id], [OrderStatusName], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (4, N'In Transit', 1, CAST(N'2025-07-03T13:18:42.770' AS DateTime), NULL, N'Admin', N'')
INSERT [dbo].[OrderStatus] ([Id], [OrderStatusName], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (5, N'Delivered', 1, CAST(N'2025-07-03T13:18:42.770' AS DateTime), NULL, N'Admin', N'')
INSERT [dbo].[OrderStatus] ([Id], [OrderStatusName], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (6, N'Cancelled', 1, CAST(N'2025-07-03T13:18:42.770' AS DateTime), NULL, N'Admin', N'')
INSERT [dbo].[OrderStatus] ([Id], [OrderStatusName], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (7, N'Returned and Refunded', 1, CAST(N'2025-07-03T13:18:42.770' AS DateTime), NULL, N'Admin', N'')
SET IDENTITY_INSERT [dbo].[OrderStatus] OFF
GO
SET IDENTITY_INSERT [dbo].[PaymentMethods] ON 

INSERT [dbo].[PaymentMethods] ([Id], [NamePaymentMethod], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (1, N'COD', 1, CAST(N'2025-07-11T13:16:10.093' AS DateTime), NULL, NULL, NULL)
INSERT [dbo].[PaymentMethods] ([Id], [NamePaymentMethod], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (2, N'VNPAY', 1, CAST(N'2025-07-11T13:16:36.093' AS DateTime), NULL, NULL, NULL)
SET IDENTITY_INSERT [dbo].[PaymentMethods] OFF
GO
SET IDENTITY_INSERT [dbo].[Products] ON 

INSERT [dbo].[Products] ([Id], [ProductName], [SpecificationDescription], [ViewCount], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [BrandID], [CategoryID], [CountriesID], [SupplierID]) VALUES (1, N'Cross Table Dune', N'Cross Table Dune', 0, 1, CAST(N'2025-07-03T06:32:43.587' AS DateTime), CAST(N'2025-07-03T06:34:29.070' AS DateTime), NULL, NULL, 2, 2, 2, 1)
INSERT [dbo].[Products] ([Id], [ProductName], [SpecificationDescription], [ViewCount], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [BrandID], [CategoryID], [CountriesID], [SupplierID]) VALUES (2, N'Ray Sofa Basic', N'Ray Sofa Basic', 0, 1, CAST(N'2025-07-03T06:45:16.467' AS DateTime), CAST(N'2025-07-03T06:46:04.203' AS DateTime), NULL, NULL, 1, 4, 2, 1)
INSERT [dbo].[Products] ([Id], [ProductName], [SpecificationDescription], [ViewCount], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [BrandID], [CategoryID], [CountriesID], [SupplierID]) VALUES (3, N'Era Shelf', N'Era Shelf', 0, 1, CAST(N'2025-07-03T06:53:08.033' AS DateTime), NULL, NULL, NULL, 2, 9, 2, 1)
INSERT [dbo].[Products] ([Id], [ProductName], [SpecificationDescription], [ViewCount], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [BrandID], [CategoryID], [CountriesID], [SupplierID]) VALUES (4, N'Wardrobe Medium', N'Wardrobe Medium', 0, 1, CAST(N'2025-07-03T07:05:33.683' AS DateTime), NULL, NULL, NULL, 2, 12, 2, 1)
INSERT [dbo].[Products] ([Id], [ProductName], [SpecificationDescription], [ViewCount], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [BrandID], [CategoryID], [CountriesID], [SupplierID]) VALUES (5, N'Velvet Club Chair', N'Velvet Club Chair', 0, 1, CAST(N'2025-07-03T07:13:12.317' AS DateTime), NULL, NULL, NULL, 2, 7, 2, 1)
INSERT [dbo].[Products] ([Id], [ProductName], [SpecificationDescription], [ViewCount], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [BrandID], [CategoryID], [CountriesID], [SupplierID]) VALUES (6, N'Book Shelves', N'Book Shelves', 0, 1, CAST(N'2025-07-03T07:31:06.573' AS DateTime), NULL, NULL, NULL, 2, 13, 2, 1)
INSERT [dbo].[Products] ([Id], [ProductName], [SpecificationDescription], [ViewCount], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [BrandID], [CategoryID], [CountriesID], [SupplierID]) VALUES (7, N'Chair Black Frame', N'Chair Black Frame', 0, 1, CAST(N'2025-07-03T07:36:28.377' AS DateTime), CAST(N'2025-07-03T07:36:52.020' AS DateTime), NULL, NULL, 2, 11, 2, 1)
INSERT [dbo].[Products] ([Id], [ProductName], [SpecificationDescription], [ViewCount], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [BrandID], [CategoryID], [CountriesID], [SupplierID]) VALUES (8, N'Cross Bar Chair Steel', N'Cross Bar Chair Steel', 0, 1, CAST(N'2025-07-03T07:46:43.243' AS DateTime), NULL, NULL, NULL, 2, 11, 3, 1)
INSERT [dbo].[Products] ([Id], [ProductName], [SpecificationDescription], [ViewCount], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [BrandID], [CategoryID], [CountriesID], [SupplierID]) VALUES (9, N'Swivel Table', N'Swivel Table', 0, 1, CAST(N'2025-07-03T07:54:24.070' AS DateTime), NULL, NULL, NULL, 2, 2, 2, 1)
INSERT [dbo].[Products] ([Id], [ProductName], [SpecificationDescription], [ViewCount], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [BrandID], [CategoryID], [CountriesID], [SupplierID]) VALUES (10, N'Stitch Layer', N'Stitch Layer', 0, 1, CAST(N'2025-07-03T07:57:51.740' AS DateTime), CAST(N'2025-07-03T07:58:08.053' AS DateTime), NULL, NULL, 2, 9, 2, 1)
SET IDENTITY_INSERT [dbo].[Products] OFF
GO
SET IDENTITY_INSERT [dbo].[ProductVariant] ON 

INSERT [dbo].[ProductVariant] ([Id], [OriginalPrice], [DiscountedPrice], [StockQty], [ProductID], [ColorID], [SizeID], [MaterialID], [UnitID], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (1, CAST(850.00 AS Decimal(18, 2)), CAST(819.00 AS Decimal(18, 2)), 20, 1, 1, 3, 2, 1, 1, CAST(N'2025-07-03T06:32:43.590' AS DateTime), CAST(N'2025-07-15T07:42:52.013' AS DateTime), NULL, NULL)
INSERT [dbo].[ProductVariant] ([Id], [OriginalPrice], [DiscountedPrice], [StockQty], [ProductID], [ColorID], [SizeID], [MaterialID], [UnitID], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (2, CAST(850.00 AS Decimal(18, 2)), CAST(819.00 AS Decimal(18, 2)), 20, 1, 5, 1, 2, 1, 1, CAST(N'2025-07-03T06:32:43.590' AS DateTime), CAST(N'2025-07-05T05:24:43.717' AS DateTime), NULL, NULL)
INSERT [dbo].[ProductVariant] ([Id], [OriginalPrice], [DiscountedPrice], [StockQty], [ProductID], [ColorID], [SizeID], [MaterialID], [UnitID], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (3, CAST(850.00 AS Decimal(18, 2)), CAST(819.00 AS Decimal(18, 2)), 12, 1, 1, 1, 1, 1, 1, CAST(N'2025-07-03T06:32:43.590' AS DateTime), CAST(N'2025-07-03T06:34:29.070' AS DateTime), NULL, NULL)
INSERT [dbo].[ProductVariant] ([Id], [OriginalPrice], [DiscountedPrice], [StockQty], [ProductID], [ColorID], [SizeID], [MaterialID], [UnitID], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (4, CAST(850.00 AS Decimal(18, 2)), CAST(850.00 AS Decimal(18, 2)), 12, 1, 5, 1, 1, 1, 1, CAST(N'2025-07-03T06:32:43.590' AS DateTime), CAST(N'2025-07-03T06:34:29.070' AS DateTime), NULL, NULL)
INSERT [dbo].[ProductVariant] ([Id], [OriginalPrice], [DiscountedPrice], [StockQty], [ProductID], [ColorID], [SizeID], [MaterialID], [UnitID], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (5, CAST(3500.00 AS Decimal(18, 2)), CAST(3289.00 AS Decimal(18, 2)), 123, 2, 3, 5, 7, 2, 1, CAST(N'2025-07-03T06:45:16.467' AS DateTime), CAST(N'2025-07-03T06:46:04.203' AS DateTime), NULL, NULL)
INSERT [dbo].[ProductVariant] ([Id], [OriginalPrice], [DiscountedPrice], [StockQty], [ProductID], [ColorID], [SizeID], [MaterialID], [UnitID], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (6, CAST(3500.00 AS Decimal(18, 2)), CAST(3289.00 AS Decimal(18, 2)), 123, 2, 12, 5, 7, 2, 1, CAST(N'2025-07-03T06:45:16.467' AS DateTime), CAST(N'2025-07-03T06:46:04.203' AS DateTime), NULL, NULL)
INSERT [dbo].[ProductVariant] ([Id], [OriginalPrice], [DiscountedPrice], [StockQty], [ProductID], [ColorID], [SizeID], [MaterialID], [UnitID], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (7, CAST(3500.00 AS Decimal(18, 2)), CAST(3289.00 AS Decimal(18, 2)), 123, 2, 3, 6, 7, 2, 1, CAST(N'2025-07-03T06:45:16.467' AS DateTime), CAST(N'2025-07-03T06:46:04.203' AS DateTime), NULL, NULL)
INSERT [dbo].[ProductVariant] ([Id], [OriginalPrice], [DiscountedPrice], [StockQty], [ProductID], [ColorID], [SizeID], [MaterialID], [UnitID], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (8, CAST(3500.00 AS Decimal(18, 2)), CAST(3289.00 AS Decimal(18, 2)), 123, 2, 12, 6, 7, 2, 1, CAST(N'2025-07-03T06:45:16.467' AS DateTime), CAST(N'2025-07-03T06:46:04.203' AS DateTime), NULL, NULL)
INSERT [dbo].[ProductVariant] ([Id], [OriginalPrice], [DiscountedPrice], [StockQty], [ProductID], [ColorID], [SizeID], [MaterialID], [UnitID], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (9, CAST(200.00 AS Decimal(18, 2)), CAST(185.00 AS Decimal(18, 2)), 123, 3, 10, 3, 2, 1, 1, CAST(N'2025-07-03T06:53:08.033' AS DateTime), NULL, NULL, NULL)
INSERT [dbo].[ProductVariant] ([Id], [OriginalPrice], [DiscountedPrice], [StockQty], [ProductID], [ColorID], [SizeID], [MaterialID], [UnitID], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (10, CAST(200.00 AS Decimal(18, 2)), CAST(185.00 AS Decimal(18, 2)), 123, 3, 1, 3, 2, 1, 1, CAST(N'2025-07-03T06:53:08.033' AS DateTime), NULL, NULL, NULL)
INSERT [dbo].[ProductVariant] ([Id], [OriginalPrice], [DiscountedPrice], [StockQty], [ProductID], [ColorID], [SizeID], [MaterialID], [UnitID], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (11, CAST(200.00 AS Decimal(18, 2)), CAST(185.00 AS Decimal(18, 2)), 123, 3, 10, 3, 1, 1, 1, CAST(N'2025-07-03T06:53:08.033' AS DateTime), NULL, NULL, NULL)
INSERT [dbo].[ProductVariant] ([Id], [OriginalPrice], [DiscountedPrice], [StockQty], [ProductID], [ColorID], [SizeID], [MaterialID], [UnitID], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (12, CAST(200.00 AS Decimal(18, 2)), CAST(185.00 AS Decimal(18, 2)), 123, 3, 1, 3, 1, 1, 1, CAST(N'2025-07-03T06:53:08.033' AS DateTime), NULL, NULL, NULL)
INSERT [dbo].[ProductVariant] ([Id], [OriginalPrice], [DiscountedPrice], [StockQty], [ProductID], [ColorID], [SizeID], [MaterialID], [UnitID], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (13, CAST(590.00 AS Decimal(18, 2)), CAST(590.00 AS Decimal(18, 2)), 123, 4, 1, 3, 2, 2, 1, CAST(N'2025-07-03T07:05:33.683' AS DateTime), NULL, NULL, NULL)
INSERT [dbo].[ProductVariant] ([Id], [OriginalPrice], [DiscountedPrice], [StockQty], [ProductID], [ColorID], [SizeID], [MaterialID], [UnitID], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (14, CAST(590.00 AS Decimal(18, 2)), CAST(590.00 AS Decimal(18, 2)), 123, 4, 5, 3, 2, 2, 1, CAST(N'2025-07-03T07:05:33.683' AS DateTime), NULL, NULL, NULL)
INSERT [dbo].[ProductVariant] ([Id], [OriginalPrice], [DiscountedPrice], [StockQty], [ProductID], [ColorID], [SizeID], [MaterialID], [UnitID], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (15, CAST(590.00 AS Decimal(18, 2)), CAST(590.00 AS Decimal(18, 2)), 123, 4, 1, 3, 1, 2, 1, CAST(N'2025-07-03T07:05:33.683' AS DateTime), NULL, NULL, NULL)
INSERT [dbo].[ProductVariant] ([Id], [OriginalPrice], [DiscountedPrice], [StockQty], [ProductID], [ColorID], [SizeID], [MaterialID], [UnitID], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (16, CAST(590.00 AS Decimal(18, 2)), CAST(590.00 AS Decimal(18, 2)), 123, 4, 5, 3, 1, 2, 1, CAST(N'2025-07-03T07:05:33.683' AS DateTime), NULL, NULL, NULL)
INSERT [dbo].[ProductVariant] ([Id], [OriginalPrice], [DiscountedPrice], [StockQty], [ProductID], [ColorID], [SizeID], [MaterialID], [UnitID], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (17, CAST(250.00 AS Decimal(18, 2)), CAST(217.00 AS Decimal(18, 2)), 122, 5, 3, 3, 3, 1, 1, CAST(N'2025-07-03T07:13:12.317' AS DateTime), NULL, NULL, NULL)
INSERT [dbo].[ProductVariant] ([Id], [OriginalPrice], [DiscountedPrice], [StockQty], [ProductID], [ColorID], [SizeID], [MaterialID], [UnitID], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (18, CAST(250.00 AS Decimal(18, 2)), CAST(217.00 AS Decimal(18, 2)), 123, 5, 10, 3, 3, 1, 1, CAST(N'2025-07-03T07:13:12.317' AS DateTime), NULL, NULL, NULL)
INSERT [dbo].[ProductVariant] ([Id], [OriginalPrice], [DiscountedPrice], [StockQty], [ProductID], [ColorID], [SizeID], [MaterialID], [UnitID], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (19, CAST(150.00 AS Decimal(18, 2)), CAST(128.00 AS Decimal(18, 2)), 123, 6, 5, 3, 2, 1, 1, CAST(N'2025-07-03T07:31:06.573' AS DateTime), NULL, NULL, NULL)
INSERT [dbo].[ProductVariant] ([Id], [OriginalPrice], [DiscountedPrice], [StockQty], [ProductID], [ColorID], [SizeID], [MaterialID], [UnitID], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (20, CAST(550.00 AS Decimal(18, 2)), CAST(529.00 AS Decimal(18, 2)), 123, 7, 1, 3, 2, 1, 1, CAST(N'2025-07-03T07:36:28.377' AS DateTime), CAST(N'2025-07-03T07:36:52.020' AS DateTime), NULL, NULL)
INSERT [dbo].[ProductVariant] ([Id], [OriginalPrice], [DiscountedPrice], [StockQty], [ProductID], [ColorID], [SizeID], [MaterialID], [UnitID], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (21, CAST(550.00 AS Decimal(18, 2)), CAST(529.00 AS Decimal(18, 2)), 123, 7, 2, 3, 2, 1, 1, CAST(N'2025-07-03T07:36:28.377' AS DateTime), CAST(N'2025-07-03T07:36:52.020' AS DateTime), NULL, NULL)
INSERT [dbo].[ProductVariant] ([Id], [OriginalPrice], [DiscountedPrice], [StockQty], [ProductID], [ColorID], [SizeID], [MaterialID], [UnitID], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (22, CAST(520.00 AS Decimal(18, 2)), CAST(429.00 AS Decimal(18, 2)), 123, 8, 6, 3, 8, 1, 1, CAST(N'2025-07-03T07:46:43.243' AS DateTime), NULL, NULL, NULL)
INSERT [dbo].[ProductVariant] ([Id], [OriginalPrice], [DiscountedPrice], [StockQty], [ProductID], [ColorID], [SizeID], [MaterialID], [UnitID], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (23, CAST(520.00 AS Decimal(18, 2)), CAST(429.00 AS Decimal(18, 2)), 123, 8, 11, 3, 8, 1, 1, CAST(N'2025-07-03T07:46:43.243' AS DateTime), NULL, NULL, NULL)
INSERT [dbo].[ProductVariant] ([Id], [OriginalPrice], [DiscountedPrice], [StockQty], [ProductID], [ColorID], [SizeID], [MaterialID], [UnitID], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (24, CAST(520.00 AS Decimal(18, 2)), CAST(429.00 AS Decimal(18, 2)), 123, 8, 5, 3, 8, 1, 1, CAST(N'2025-07-03T07:46:43.243' AS DateTime), NULL, NULL, NULL)
INSERT [dbo].[ProductVariant] ([Id], [OriginalPrice], [DiscountedPrice], [StockQty], [ProductID], [ColorID], [SizeID], [MaterialID], [UnitID], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (25, CAST(350.00 AS Decimal(18, 2)), CAST(305.00 AS Decimal(18, 2)), 123, 9, 8, 3, 2, 1, 1, CAST(N'2025-07-03T07:54:24.070' AS DateTime), NULL, NULL, NULL)
INSERT [dbo].[ProductVariant] ([Id], [OriginalPrice], [DiscountedPrice], [StockQty], [ProductID], [ColorID], [SizeID], [MaterialID], [UnitID], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (26, CAST(300.00 AS Decimal(18, 2)), CAST(240.00 AS Decimal(18, 2)), 123, 10, 3, 2, 7, 1, 1, CAST(N'2025-07-03T07:57:51.740' AS DateTime), CAST(N'2025-07-03T07:58:08.053' AS DateTime), NULL, NULL)
SET IDENTITY_INSERT [dbo].[ProductVariant] OFF
GO
SET IDENTITY_INSERT [dbo].[ProductVariantImages] ON 

INSERT [dbo].[ProductVariantImages] ([Id], [ImageURL], [Attribute], [DisplayOrder], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [ProVarID]) VALUES (5, N'https://res.cloudinary.com/duelcb8ki/image/upload/v1751524472/oefxchnaguytdf5fnbib.webp', N'123123123', 1, 1, CAST(N'2025-07-09T07:17:10.690' AS DateTime), NULL, NULL, NULL, 1)
INSERT [dbo].[ProductVariantImages] ([Id], [ImageURL], [Attribute], [DisplayOrder], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [ProVarID]) VALUES (6, N'https://res.cloudinary.com/duelcb8ki/image/upload/v1751525591/ctbh4jqds3tncqdqxwsf.webp', N'123123123', 1, 1, CAST(N'2025-07-09T07:17:22.717' AS DateTime), NULL, NULL, NULL, 2)
INSERT [dbo].[ProductVariantImages] ([Id], [ImageURL], [Attribute], [DisplayOrder], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [ProVarID]) VALUES (7, N'https://res.cloudinary.com/duelcb8ki/image/upload/v1751524472/oefxchnaguytdf5fnbib.webp', N'123123123', 1, 1, CAST(N'2025-07-09T07:17:32.910' AS DateTime), NULL, NULL, NULL, 3)
INSERT [dbo].[ProductVariantImages] ([Id], [ImageURL], [Attribute], [DisplayOrder], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [ProVarID]) VALUES (8, N'https://res.cloudinary.com/duelcb8ki/image/upload/v1751525591/ctbh4jqds3tncqdqxwsf.webp', N'123123123', 1, 1, CAST(N'2025-07-09T07:17:44.800' AS DateTime), NULL, NULL, NULL, 4)
INSERT [dbo].[ProductVariantImages] ([Id], [ImageURL], [Attribute], [DisplayOrder], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [ProVarID]) VALUES (9, N'https://res.cloudinary.com/duelcb8ki/image/upload/v1751524472/oefxchnaguytdf5fnbib.webp', N'123123123', 1, 1, CAST(N'2025-07-09T07:17:53.047' AS DateTime), NULL, NULL, NULL, 5)
INSERT [dbo].[ProductVariantImages] ([Id], [ImageURL], [Attribute], [DisplayOrder], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [ProVarID]) VALUES (10, N'https://res.cloudinary.com/duelcb8ki/image/upload/v1751525591/ctbh4jqds3tncqdqxwsf.webp', N'123123123', 1, 1, CAST(N'2025-07-09T07:18:12.160' AS DateTime), NULL, NULL, NULL, 6)
INSERT [dbo].[ProductVariantImages] ([Id], [ImageURL], [Attribute], [DisplayOrder], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [ProVarID]) VALUES (11, N'https://res.cloudinary.com/duelcb8ki/image/upload/v1751524472/oefxchnaguytdf5fnbib.webp', N'123123123', 1, 1, CAST(N'2025-07-09T07:18:23.567' AS DateTime), NULL, NULL, NULL, 7)
INSERT [dbo].[ProductVariantImages] ([Id], [ImageURL], [Attribute], [DisplayOrder], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [ProVarID]) VALUES (12, N'https://res.cloudinary.com/duelcb8ki/image/upload/v1751525591/ctbh4jqds3tncqdqxwsf.webp', N'123123123', 1, 1, CAST(N'2025-07-09T07:18:29.310' AS DateTime), NULL, NULL, NULL, 8)
INSERT [dbo].[ProductVariantImages] ([Id], [ImageURL], [Attribute], [DisplayOrder], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [ProVarID]) VALUES (13, N'https://res.cloudinary.com/duelcb8ki/image/upload/v1751524472/oefxchnaguytdf5fnbib.webp', N'123123123', 1, 1, CAST(N'2025-07-09T07:18:34.520' AS DateTime), NULL, NULL, NULL, 9)
INSERT [dbo].[ProductVariantImages] ([Id], [ImageURL], [Attribute], [DisplayOrder], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [ProVarID]) VALUES (14, N'https://res.cloudinary.com/duelcb8ki/image/upload/v1751525591/ctbh4jqds3tncqdqxwsf.webp', N'123123123', 1, 1, CAST(N'2025-07-09T07:18:39.737' AS DateTime), NULL, NULL, NULL, 10)
INSERT [dbo].[ProductVariantImages] ([Id], [ImageURL], [Attribute], [DisplayOrder], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [ProVarID]) VALUES (15, N'https://res.cloudinary.com/duelcb8ki/image/upload/v1751524472/oefxchnaguytdf5fnbib.webp', N'123123123', 1, 1, CAST(N'2025-07-09T07:18:45.693' AS DateTime), NULL, NULL, NULL, 11)
INSERT [dbo].[ProductVariantImages] ([Id], [ImageURL], [Attribute], [DisplayOrder], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [ProVarID]) VALUES (16, N'https://res.cloudinary.com/duelcb8ki/image/upload/v1751525591/ctbh4jqds3tncqdqxwsf.webp', N'123123123', 1, 1, CAST(N'2025-07-09T07:18:50.097' AS DateTime), NULL, NULL, NULL, 12)
INSERT [dbo].[ProductVariantImages] ([Id], [ImageURL], [Attribute], [DisplayOrder], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [ProVarID]) VALUES (17, N'https://res.cloudinary.com/duelcb8ki/image/upload/v1751524472/oefxchnaguytdf5fnbib.webp', N'123123123', 1, 1, CAST(N'2025-07-09T07:18:54.390' AS DateTime), NULL, NULL, NULL, 13)
INSERT [dbo].[ProductVariantImages] ([Id], [ImageURL], [Attribute], [DisplayOrder], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [ProVarID]) VALUES (18, N'https://res.cloudinary.com/duelcb8ki/image/upload/v1751525591/ctbh4jqds3tncqdqxwsf.webp', N'123123123', 1, 1, CAST(N'2025-07-09T07:18:59.643' AS DateTime), NULL, NULL, NULL, 14)
INSERT [dbo].[ProductVariantImages] ([Id], [ImageURL], [Attribute], [DisplayOrder], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [ProVarID]) VALUES (19, N'https://res.cloudinary.com/duelcb8ki/image/upload/v1751524472/oefxchnaguytdf5fnbib.webp', N'123123123', 1, 1, CAST(N'2025-07-09T07:19:04.420' AS DateTime), NULL, NULL, NULL, 15)
INSERT [dbo].[ProductVariantImages] ([Id], [ImageURL], [Attribute], [DisplayOrder], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [ProVarID]) VALUES (20, N'https://res.cloudinary.com/duelcb8ki/image/upload/v1751525591/ctbh4jqds3tncqdqxwsf.webp', N'123123123', 1, 1, CAST(N'2025-07-09T07:19:10.200' AS DateTime), NULL, NULL, NULL, 16)
INSERT [dbo].[ProductVariantImages] ([Id], [ImageURL], [Attribute], [DisplayOrder], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [ProVarID]) VALUES (21, N'https://res.cloudinary.com/duelcb8ki/image/upload/v1751524472/oefxchnaguytdf5fnbib.webp', N'123123123', 1, 1, CAST(N'2025-07-09T07:19:17.603' AS DateTime), NULL, NULL, NULL, 17)
INSERT [dbo].[ProductVariantImages] ([Id], [ImageURL], [Attribute], [DisplayOrder], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [ProVarID]) VALUES (22, N'https://res.cloudinary.com/duelcb8ki/image/upload/v1751525591/ctbh4jqds3tncqdqxwsf.webp', N'123123123', 1, 1, CAST(N'2025-07-09T07:19:23.217' AS DateTime), NULL, NULL, NULL, 18)
INSERT [dbo].[ProductVariantImages] ([Id], [ImageURL], [Attribute], [DisplayOrder], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [ProVarID]) VALUES (23, N'https://res.cloudinary.com/duelcb8ki/image/upload/v1751524472/oefxchnaguytdf5fnbib.webp', N'123123123', 1, 1, CAST(N'2025-07-09T07:19:27.050' AS DateTime), NULL, NULL, NULL, 19)
INSERT [dbo].[ProductVariantImages] ([Id], [ImageURL], [Attribute], [DisplayOrder], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [ProVarID]) VALUES (24, N'https://res.cloudinary.com/duelcb8ki/image/upload/v1751525591/ctbh4jqds3tncqdqxwsf.webp', N'123123123', 1, 1, CAST(N'2025-07-09T07:19:32.920' AS DateTime), NULL, NULL, NULL, 20)
INSERT [dbo].[ProductVariantImages] ([Id], [ImageURL], [Attribute], [DisplayOrder], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [ProVarID]) VALUES (25, N'https://res.cloudinary.com/duelcb8ki/image/upload/v1751524472/oefxchnaguytdf5fnbib.webp', N'123123123', 1, 1, CAST(N'2025-07-09T07:19:39.360' AS DateTime), NULL, NULL, NULL, 21)
INSERT [dbo].[ProductVariantImages] ([Id], [ImageURL], [Attribute], [DisplayOrder], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [ProVarID]) VALUES (26, N'https://res.cloudinary.com/duelcb8ki/image/upload/v1751525591/ctbh4jqds3tncqdqxwsf.webp', N'123123123', 1, 1, CAST(N'2025-07-09T07:19:44.530' AS DateTime), NULL, NULL, NULL, 22)
INSERT [dbo].[ProductVariantImages] ([Id], [ImageURL], [Attribute], [DisplayOrder], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [ProVarID]) VALUES (27, N'https://res.cloudinary.com/duelcb8ki/image/upload/v1752045590/ni77bwkwusuynrlt0iaa.png', N'123123123', 1, 1, CAST(N'2025-07-09T07:19:49.273' AS DateTime), NULL, NULL, NULL, 23)
INSERT [dbo].[ProductVariantImages] ([Id], [ImageURL], [Attribute], [DisplayOrder], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [ProVarID]) VALUES (28, N'https://res.cloudinary.com/duelcb8ki/image/upload/v1752045595/wsmsfsafjtar8lyjjsx9.png', N'123123123', 1, 1, CAST(N'2025-07-09T07:19:54.870' AS DateTime), NULL, NULL, NULL, 24)
INSERT [dbo].[ProductVariantImages] ([Id], [ImageURL], [Attribute], [DisplayOrder], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [ProVarID]) VALUES (29, N'https://res.cloudinary.com/duelcb8ki/image/upload/v1752045600/qnfynh6qf92mc3jlfkcb.png', N'123123123', 1, 1, CAST(N'2025-07-09T07:19:59.733' AS DateTime), NULL, NULL, NULL, 25)
INSERT [dbo].[ProductVariantImages] ([Id], [ImageURL], [Attribute], [DisplayOrder], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [ProVarID]) VALUES (30, N'https://res.cloudinary.com/duelcb8ki/image/upload/v1752045605/hjbwocvk6o71bb6srocd.png', N'123123123', 1, 1, CAST(N'2025-07-09T07:20:04.587' AS DateTime), NULL, NULL, NULL, 26)
SET IDENTITY_INSERT [dbo].[ProductVariantImages] OFF
GO
SET IDENTITY_INSERT [dbo].[Promotions] ON 

INSERT [dbo].[Promotions] ([Id], [PromotionCode], [DiscountValue], [OrderMinimum], [MaximumDiscountAmount], [StartDate], [EndDate], [CouponUsage], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [PromotionTypeID]) VALUES (1, N'WELCOME10', CAST(10.00 AS Decimal(10, 2)), CAST(50.00 AS Decimal(10, 2)), CAST(20.00 AS Decimal(10, 2)), CAST(N'2025-07-01T00:00:00.000' AS DateTime), CAST(N'2025-12-31T00:00:00.000' AS DateTime), 91, 1, CAST(N'2025-07-11T20:25:43.443' AS DateTime), CAST(N'2025-07-11T20:28:13.597' AS DateTime), N'admin', N'admin', 1)
INSERT [dbo].[Promotions] ([Id], [PromotionCode], [DiscountValue], [OrderMinimum], [MaximumDiscountAmount], [StartDate], [EndDate], [CouponUsage], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [PromotionTypeID]) VALUES (2, N'FLAT5', CAST(5.00 AS Decimal(10, 2)), CAST(30.00 AS Decimal(10, 2)), CAST(5.00 AS Decimal(10, 2)), CAST(N'2025-07-01T00:00:00.000' AS DateTime), CAST(N'2025-08-31T00:00:00.000' AS DateTime), 48, 1, CAST(N'2025-07-11T20:25:43.443' AS DateTime), CAST(N'2025-07-11T20:28:13.597' AS DateTime), N'admin', N'admin', 2)
INSERT [dbo].[Promotions] ([Id], [PromotionCode], [DiscountValue], [OrderMinimum], [MaximumDiscountAmount], [StartDate], [EndDate], [CouponUsage], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [PromotionTypeID]) VALUES (3, N'FREESHIP', CAST(0.00 AS Decimal(10, 2)), CAST(30.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(N'2025-07-01T00:00:00.000' AS DateTime), CAST(N'2025-09-30T00:00:00.000' AS DateTime), 9998, 1, CAST(N'2025-07-11T20:25:43.443' AS DateTime), CAST(N'2025-07-11T20:28:13.597' AS DateTime), N'admin', N'admin', 3)
SET IDENTITY_INSERT [dbo].[Promotions] OFF
GO
SET IDENTITY_INSERT [dbo].[PromotionTypes] ON 

INSERT [dbo].[PromotionTypes] ([Id], [PromotionTypeName], [Description], [PromotionUnit], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (1, N'Percentage Discount', N'Discount by percentage of total bill', 1, 1, CAST(N'2025-07-11T20:25:43.440' AS DateTime), CAST(N'2025-07-11T20:28:13.593' AS DateTime), N'admin', N'admin')
INSERT [dbo].[PromotionTypes] ([Id], [PromotionTypeName], [Description], [PromotionUnit], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (2, N'Fixed Amount Discount', N'Discount by fixed cash amount', 0, 1, CAST(N'2025-07-11T20:25:43.440' AS DateTime), CAST(N'2025-07-11T20:28:13.593' AS DateTime), N'admin', N'admin')
INSERT [dbo].[PromotionTypes] ([Id], [PromotionTypeName], [Description], [PromotionUnit], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (3, N'Free Shipping', N'Free shipping for eligible orders', 2, 1, CAST(N'2025-07-11T20:25:43.440' AS DateTime), CAST(N'2025-07-11T20:28:13.597' AS DateTime), N'admin', N'admin')
SET IDENTITY_INSERT [dbo].[PromotionTypes] OFF
GO
SET IDENTITY_INSERT [dbo].[Roles] ON 

INSERT [dbo].[Roles] ([Id], [RoleName], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (1, N'Admin', 1, CAST(N'2025-07-03T13:18:25.460' AS DateTime), NULL, N'Admin', N'Admin')
INSERT [dbo].[Roles] ([Id], [RoleName], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (2, N'User', 1, CAST(N'2025-07-03T13:18:25.460' AS DateTime), NULL, N'Admin', N'Admin')
SET IDENTITY_INSERT [dbo].[Roles] OFF
GO
SET IDENTITY_INSERT [dbo].[Sizes] ON 

INSERT [dbo].[Sizes] ([Id], [SizeName], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (1, N'767H × 481D × 502W mm', 1, CAST(N'2025-07-03T13:19:07.353' AS DateTime), NULL, N'Admin', N'')
INSERT [dbo].[Sizes] ([Id], [SizeName], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (2, N'Small', 1, CAST(N'2025-07-03T13:19:07.353' AS DateTime), NULL, N'Admin', N'')
INSERT [dbo].[Sizes] ([Id], [SizeName], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (3, N'Medium', 1, CAST(N'2025-07-03T13:19:07.353' AS DateTime), NULL, N'Admin', N'')
INSERT [dbo].[Sizes] ([Id], [SizeName], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (4, N'Large', 1, CAST(N'2025-07-03T13:19:07.353' AS DateTime), NULL, N'Admin', N'')
INSERT [dbo].[Sizes] ([Id], [SizeName], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (5, N'2 Seats', 1, CAST(N'2025-07-03T13:36:49.547' AS DateTime), CAST(N'2025-07-03T13:37:01.007' AS DateTime), NULL, NULL)
INSERT [dbo].[Sizes] ([Id], [SizeName], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (6, N'3 Seats', 1, CAST(N'2025-07-03T13:36:54.650' AS DateTime), CAST(N'2025-07-03T13:37:04.110' AS DateTime), NULL, NULL)
SET IDENTITY_INSERT [dbo].[Sizes] OFF
GO
SET IDENTITY_INSERT [dbo].[Sliders] ON 

INSERT [dbo].[Sliders] ([Id], [Title], [Description], [ImageURL], [LinkURL], [IsPoster], [Position], [DisplayOrder], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [ProductId]) VALUES (1, N'Cross Table Dune 001', NULL, N'https://res.cloudinary.com/duelcb8ki/image/upload/v1751524472/oefxchnaguytdf5fnbib.webp', N'/products/1', 1, N'Home Page', 5, 1, CAST(N'2025-07-03T06:34:31.210' AS DateTime), NULL, NULL, NULL, 1)
INSERT [dbo].[Sliders] ([Id], [Title], [Description], [ImageURL], [LinkURL], [IsPoster], [Position], [DisplayOrder], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [ProductId]) VALUES (2, N'Cross Table Dune 002', NULL, N'https://res.cloudinary.com/duelcb8ki/image/upload/v1751524473/xrsmi6nbjmwedl9dmztr.webp', N'/products/1', 1, N'Home Page', 3, 1, CAST(N'2025-07-03T06:34:31.920' AS DateTime), NULL, NULL, NULL, 1)
INSERT [dbo].[Sliders] ([Id], [Title], [Description], [ImageURL], [LinkURL], [IsPoster], [Position], [DisplayOrder], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [ProductId]) VALUES (3, N'Cross Table Dune 003', NULL, N'https://res.cloudinary.com/duelcb8ki/image/upload/v1751524473/c9ntvg7ui3jhbrouoh5o.webp', N'/products/1', 1, N'Home Page', 4, 1, CAST(N'2025-07-03T06:34:32.593' AS DateTime), NULL, NULL, NULL, 1)
INSERT [dbo].[Sliders] ([Id], [Title], [Description], [ImageURL], [LinkURL], [IsPoster], [Position], [DisplayOrder], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [ProductId]) VALUES (4, N'Ray Sofa Basic 001', NULL, N'https://res.cloudinary.com/duelcb8ki/image/upload/v1751525166/caxfdzciv96rkay82fcs.webp', N'/products/2', 1, N'Home Page', 3, 1, CAST(N'2025-07-03T06:46:05.823' AS DateTime), NULL, NULL, NULL, 2)
INSERT [dbo].[Sliders] ([Id], [Title], [Description], [ImageURL], [LinkURL], [IsPoster], [Position], [DisplayOrder], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [ProductId]) VALUES (5, N'Ray Sofa Basic 002', NULL, N'https://res.cloudinary.com/duelcb8ki/image/upload/v1751525167/osq0qpbpdtwlebvonimr.webp', N'/products/2', 1, N'Home Page', 1, 1, CAST(N'2025-07-03T06:46:06.623' AS DateTime), NULL, NULL, NULL, 2)
INSERT [dbo].[Sliders] ([Id], [Title], [Description], [ImageURL], [LinkURL], [IsPoster], [Position], [DisplayOrder], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [ProductId]) VALUES (6, N'Ray Sofa Basic 003', NULL, N'https://res.cloudinary.com/duelcb8ki/image/upload/v1751525168/lqz9suzx6rjhn7fzypdc.webp', N'/products/2', 1, N'Home Page', 2, 1, CAST(N'2025-07-03T06:46:07.400' AS DateTime), NULL, NULL, NULL, 2)
INSERT [dbo].[Sliders] ([Id], [Title], [Description], [ImageURL], [LinkURL], [IsPoster], [Position], [DisplayOrder], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [ProductId]) VALUES (7, N'Era Shelf 001', NULL, N'https://res.cloudinary.com/duelcb8ki/image/upload/v1751525591/ctbh4jqds3tncqdqxwsf.webp', N'/products/new', 1, N'Home Page', 2, 1, CAST(N'2025-07-03T06:53:10.203' AS DateTime), NULL, NULL, NULL, 3)
INSERT [dbo].[Sliders] ([Id], [Title], [Description], [ImageURL], [LinkURL], [IsPoster], [Position], [DisplayOrder], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [ProductId]) VALUES (8, N'Era Shelf 002', NULL, N'https://res.cloudinary.com/duelcb8ki/image/upload/v1751525592/eihvsbqldskifrhmikyn.webp', N'/products/new', 1, N'Home Page', 1, 1, CAST(N'2025-07-03T06:53:10.857' AS DateTime), NULL, NULL, NULL, 3)
INSERT [dbo].[Sliders] ([Id], [Title], [Description], [ImageURL], [LinkURL], [IsPoster], [Position], [DisplayOrder], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [ProductId]) VALUES (9, N'Wardrobe Medium 001', NULL, N'https://res.cloudinary.com/duelcb8ki/image/upload/v1751526336/w5uuhabmh3iz9hgnmoxm.webp', N'/products/new', 1, N'Home Page', 4, 1, CAST(N'2025-07-03T07:05:35.250' AS DateTime), NULL, NULL, NULL, 4)
INSERT [dbo].[Sliders] ([Id], [Title], [Description], [ImageURL], [LinkURL], [IsPoster], [Position], [DisplayOrder], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [ProductId]) VALUES (10, N'Wardrobe Medium 002', NULL, N'https://res.cloudinary.com/duelcb8ki/image/upload/v1751526337/a6auqledeporvnbmpw9d.webp', N'/products/new', 1, N'Home Page', 1, 1, CAST(N'2025-07-03T07:05:36.233' AS DateTime), NULL, NULL, NULL, 4)
INSERT [dbo].[Sliders] ([Id], [Title], [Description], [ImageURL], [LinkURL], [IsPoster], [Position], [DisplayOrder], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [ProductId]) VALUES (11, N'Wardrobe Medium 003', NULL, N'https://res.cloudinary.com/duelcb8ki/image/upload/v1751526338/f6mejpwzznclexyinypo.webp', N'/products/new', 1, N'Home Page', 2, 1, CAST(N'2025-07-03T07:05:37.177' AS DateTime), NULL, NULL, NULL, 4)
INSERT [dbo].[Sliders] ([Id], [Title], [Description], [ImageURL], [LinkURL], [IsPoster], [Position], [DisplayOrder], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [ProductId]) VALUES (12, N'Wardrobe Medium 004', NULL, N'https://res.cloudinary.com/duelcb8ki/image/upload/v1751526339/wzbykegi5qk2hvugfzgw.webp', N'/products/new', 1, N'Home Page', 3, 1, CAST(N'2025-07-03T07:05:37.880' AS DateTime), NULL, NULL, NULL, 4)
INSERT [dbo].[Sliders] ([Id], [Title], [Description], [ImageURL], [LinkURL], [IsPoster], [Position], [DisplayOrder], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [ProductId]) VALUES (13, N'Velvet Club Chair 001', NULL, N'https://res.cloudinary.com/duelcb8ki/image/upload/v1751526795/bvvfrod37zatbidhcles.webp', N'/products/new', 1, N'Home Page', 2, 1, CAST(N'2025-07-03T07:13:14.423' AS DateTime), NULL, NULL, NULL, 5)
INSERT [dbo].[Sliders] ([Id], [Title], [Description], [ImageURL], [LinkURL], [IsPoster], [Position], [DisplayOrder], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [ProductId]) VALUES (14, N'Velvet Club Chair 002', NULL, N'https://res.cloudinary.com/duelcb8ki/image/upload/v1751526796/iajlwukozup0nzi9dgfc.webp', N'/products/new', 1, N'Home Page', 1, 1, CAST(N'2025-07-03T07:13:15.133' AS DateTime), NULL, NULL, NULL, 5)
INSERT [dbo].[Sliders] ([Id], [Title], [Description], [ImageURL], [LinkURL], [IsPoster], [Position], [DisplayOrder], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [ProductId]) VALUES (15, N'Book Shelves 001', NULL, N'https://res.cloudinary.com/duelcb8ki/image/upload/v1751527869/qv4waclcpgyuhcokbbg7.png', N'/products/new', 1, N'Home Page', 1, 1, CAST(N'2025-07-03T07:31:08.937' AS DateTime), NULL, NULL, NULL, 6)
INSERT [dbo].[Sliders] ([Id], [Title], [Description], [ImageURL], [LinkURL], [IsPoster], [Position], [DisplayOrder], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [ProductId]) VALUES (16, N'Chair Black Frame 001', NULL, N'https://res.cloudinary.com/duelcb8ki/image/upload/v1751528214/ihrbgvyfpotislwar2jm.webp', N'/products/7', 1, N'Home Page', 3, 1, CAST(N'2025-07-03T07:36:53.483' AS DateTime), NULL, NULL, NULL, 7)
INSERT [dbo].[Sliders] ([Id], [Title], [Description], [ImageURL], [LinkURL], [IsPoster], [Position], [DisplayOrder], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [ProductId]) VALUES (17, N'Chair Black Frame 002', NULL, N'https://res.cloudinary.com/duelcb8ki/image/upload/v1751528215/hwzgupsjl28pgpuvgjpu.webp', N'/products/7', 1, N'Home Page', 1, 1, CAST(N'2025-07-03T07:36:54.110' AS DateTime), NULL, NULL, NULL, 7)
INSERT [dbo].[Sliders] ([Id], [Title], [Description], [ImageURL], [LinkURL], [IsPoster], [Position], [DisplayOrder], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [ProductId]) VALUES (18, N'Chair Black Frame 003', NULL, N'https://res.cloudinary.com/duelcb8ki/image/upload/v1751528216/cvtqkuyhxhvhh83pk5qn.webp', N'/products/7', 1, N'Home Page', 2, 1, CAST(N'2025-07-03T07:36:54.730' AS DateTime), NULL, NULL, NULL, 7)
INSERT [dbo].[Sliders] ([Id], [Title], [Description], [ImageURL], [LinkURL], [IsPoster], [Position], [DisplayOrder], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [ProductId]) VALUES (19, N'Image 001', NULL, N'https://res.cloudinary.com/duelcb8ki/image/upload/v1751528806/dyzmizxxb3lccmgkgggf.webp', N'/products/new', 1, N'Home Page', 4, 1, CAST(N'2025-07-03T07:46:44.977' AS DateTime), NULL, NULL, NULL, 8)
INSERT [dbo].[Sliders] ([Id], [Title], [Description], [ImageURL], [LinkURL], [IsPoster], [Position], [DisplayOrder], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [ProductId]) VALUES (20, N'Image 002', NULL, N'https://res.cloudinary.com/duelcb8ki/image/upload/v1751528806/bgsz5savzuhvrvatswvj.webp', N'/products/new', 1, N'Home Page', 1, 1, CAST(N'2025-07-03T07:46:45.630' AS DateTime), NULL, NULL, NULL, 8)
INSERT [dbo].[Sliders] ([Id], [Title], [Description], [ImageURL], [LinkURL], [IsPoster], [Position], [DisplayOrder], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [ProductId]) VALUES (21, N'Image 003', NULL, N'https://res.cloudinary.com/duelcb8ki/image/upload/v1751528807/xgyaqrxkwutxnugvdgkt.webp', N'/products/new', 1, N'Home Page', 2, 1, CAST(N'2025-07-03T07:46:46.357' AS DateTime), NULL, NULL, NULL, 8)
INSERT [dbo].[Sliders] ([Id], [Title], [Description], [ImageURL], [LinkURL], [IsPoster], [Position], [DisplayOrder], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [ProductId]) VALUES (22, N'Image 004', NULL, N'https://res.cloudinary.com/duelcb8ki/image/upload/v1751528808/rwmdqz2wtzgsvfkvwvd8.webp', N'/products/new', 1, N'Home Page', 3, 1, CAST(N'2025-07-03T07:46:47.043' AS DateTime), NULL, NULL, NULL, 8)
INSERT [dbo].[Sliders] ([Id], [Title], [Description], [ImageURL], [LinkURL], [IsPoster], [Position], [DisplayOrder], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [ProductId]) VALUES (23, N'Swivel Table 001', NULL, N'https://res.cloudinary.com/duelcb8ki/image/upload/v1751529267/f8frcye0qfnv7edznwco.webp', N'/products/new', 1, N'Home Page', 2, 1, CAST(N'2025-07-03T07:54:25.890' AS DateTime), NULL, NULL, NULL, 9)
INSERT [dbo].[Sliders] ([Id], [Title], [Description], [ImageURL], [LinkURL], [IsPoster], [Position], [DisplayOrder], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [ProductId]) VALUES (24, N'Swivel Table 002', NULL, N'https://res.cloudinary.com/duelcb8ki/image/upload/v1751529267/hufm0kiye674xhnkuura.webp', N'/products/new', 1, N'Home Page', 1, 1, CAST(N'2025-07-03T07:54:26.633' AS DateTime), NULL, NULL, NULL, 9)
INSERT [dbo].[Sliders] ([Id], [Title], [Description], [ImageURL], [LinkURL], [IsPoster], [Position], [DisplayOrder], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [ProductId]) VALUES (27, N'Stitch Layer 001', NULL, N'https://res.cloudinary.com/duelcb8ki/image/upload/v1751529490/aujw9xgikgewitd2v29j.webp', N'/products/10', 1, N'Home Page', 2, 1, CAST(N'2025-07-03T07:58:08.747' AS DateTime), NULL, NULL, NULL, 10)
INSERT [dbo].[Sliders] ([Id], [Title], [Description], [ImageURL], [LinkURL], [IsPoster], [Position], [DisplayOrder], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [ProductId]) VALUES (28, N'Stitch Layer 002', NULL, N'https://res.cloudinary.com/duelcb8ki/image/upload/v1751529490/pidtc6ztax1vjtb39dwv.webp', N'/products/10', 1, N'Home Page', 1, 1, CAST(N'2025-07-03T07:58:09.500' AS DateTime), NULL, NULL, NULL, 10)
INSERT [dbo].[Sliders] ([Id], [Title], [Description], [ImageURL], [LinkURL], [IsPoster], [Position], [DisplayOrder], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [ProductId]) VALUES (39, N'hhhhh', N'hhhhh', N'https://res.cloudinary.com/duelcb8ki/image/upload/v1752855563/zgk75wgcvjax96zwf0yt.jpg', N'https://localhost:7030/swagger/index.html', 0, N'Home Page', 1, 1, CAST(N'2025-07-18T16:19:21.673' AS DateTime), NULL, NULL, NULL, 1)
INSERT [dbo].[Sliders] ([Id], [Title], [Description], [ImageURL], [LinkURL], [IsPoster], [Position], [DisplayOrder], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [ProductId]) VALUES (40, N'hhhhhhhhhhhhhhhhh', N'hhhhhhhhhhhhhhhh', N'https://res.cloudinary.com/duelcb8ki/image/upload/v1752857469/pqxmvkhwisacossvq5gs.png', N'string', 1, N'hhhh', 2, 0, CAST(N'2025-07-18T16:50:10.423' AS DateTime), CAST(N'2025-07-18T16:51:07.230' AS DateTime), NULL, NULL, 1)
SET IDENTITY_INSERT [dbo].[Sliders] OFF
GO
SET IDENTITY_INSERT [dbo].[StoreInformation] ON 

INSERT [dbo].[StoreInformation] ([Id], [StoreName], [StoreAddress], [Logo], [PhoneNumber], [Email], [LinkWebsite], [Latitude], [Longitude], [OwnerName], [BusinessType], [OperatingHours], [StoreDescription], [EstablishmentDate], [TaxId], [BranchCode], [LinkSocialFacebook], [LinkSocialTwitter], [LinkSocialInstagram], [LinkSocialTiktok], [LinkSocialYoutube], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (1, N'TOM''S FURNITURE', N'180 Cao Lỗ, Phường 4, Quận 8, Tp. Hồ Chí Minh', N'https://res.cloudinary.com/duelcb8ki/image/upload/v1750926676/vamslwzyjoelzx6ipzth.png', N'0987665413', N'rholy921@gmail.com', N'https://tomsfurniture.vercel.app/', CAST(23.252449 AS Decimal(9, 6)), CAST(87.843443 AS Decimal(9, 6)), N'Tom', N'Seller', N'8h - 19h30', N'string', CAST(N'2025-08-06' AS Date), NULL, N'0110365518', N'https://www.facebook.com/', N'https://x.com/', N'https://www.instagram.com/', N'https://www.tiktok.com/', N'https://www.youtube.com/', 1, CAST(N'2025-07-03T13:18:21.793' AS DateTime), CAST(N'2025-07-03T13:18:21.793' AS DateTime), NULL, NULL)
SET IDENTITY_INSERT [dbo].[StoreInformation] OFF
GO
SET IDENTITY_INSERT [dbo].[Suppliers] ON 

INSERT [dbo].[Suppliers] ([Id], [SupplierName], [ContactName], [Email], [PhoneNumber], [ImageURL], [Notes], [TaxID], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (1, N'FoxEcom', N'Unknow', N'unknow', NULL, N'', N'', N'', 1, CAST(N'2025-07-03T13:20:35.547' AS DateTime), NULL, N'Admin', N'')
INSERT [dbo].[Suppliers] ([Id], [SupplierName], [ContactName], [Email], [PhoneNumber], [ImageURL], [Notes], [TaxID], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (2, N'ttttt', N'ttttt', N'tttt@gmail.com', N'0876554132', N'https://res.cloudinary.com/duelcb8ki/image/upload/v1752908788/aw4ochy8y26cm4ne8xaf.png', N'asdasdasdf', N'01123123123', 1, CAST(N'2025-07-19T07:04:15.020' AS DateTime), CAST(N'2025-07-19T07:06:26.367' AS DateTime), NULL, NULL)
SET IDENTITY_INSERT [dbo].[Suppliers] OFF
GO
SET IDENTITY_INSERT [dbo].[Units] ON 

INSERT [dbo].[Units] ([Id], [UnitName], [Description], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (1, N'Piece', N'', 1, CAST(N'2025-07-03T13:20:43.367' AS DateTime), NULL, N'Admin', N'')
INSERT [dbo].[Units] ([Id], [UnitName], [Description], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (2, N'Set', N'', 1, CAST(N'2025-07-03T13:20:43.367' AS DateTime), NULL, N'Admin', N'')
INSERT [dbo].[Units] ([Id], [UnitName], [Description], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (3, N'Box', N'', 1, CAST(N'2025-07-03T13:20:43.367' AS DateTime), NULL, N'Admin', N'')
INSERT [dbo].[Units] ([Id], [UnitName], [Description], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (4, N'Pack', N'', 1, CAST(N'2025-07-03T13:20:43.367' AS DateTime), NULL, N'Admin', N'')
INSERT [dbo].[Units] ([Id], [UnitName], [Description], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (5, N'Carton', N'', 1, CAST(N'2025-07-03T13:20:43.367' AS DateTime), NULL, N'Admin', N'')
INSERT [dbo].[Units] ([Id], [UnitName], [Description], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy]) VALUES (6, N'Bag', N'', 1, CAST(N'2025-07-03T13:20:43.367' AS DateTime), NULL, N'Admin', N'')
SET IDENTITY_INSERT [dbo].[Units] OFF
GO
SET IDENTITY_INSERT [dbo].[UserGuest] ON 

INSERT [dbo].[UserGuest] ([Id], [FullName], [PhoneNumber], [Email], [DetailAddress], [City], [District], [Ward], [CreatedDate]) VALUES (4, N'string', N'0888765413', N'string@gmail.com', N'string', N'string', N'asdasd', N'asdasdasdasd', CAST(N'2025-07-07T05:55:55.703' AS DateTime))
SET IDENTITY_INSERT [dbo].[UserGuest] OFF
GO
SET IDENTITY_INSERT [dbo].[Users] ON 

INSERT [dbo].[Users] ([Id], [UserName], [Gender], [Email], [PasswordHash], [PhoneNumber], [UserAddress], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [RoleID]) VALUES (1, N'Tom Parker', 0, N'dh52100199@student.stu.edu.vn', N'$2a$11$kRkKhRBl89Yh657VMR0nvujqdbAzc5nOk1LxYWUNimZDQpaGPIEN.', N'0765442122', N'New York City', 1, CAST(N'2025-07-03T13:18:49.047' AS DateTime), CAST(N'2025-07-17T18:18:42.360' AS DateTime), N'System', N'Admin', 1)
INSERT [dbo].[Users] ([Id], [UserName], [Gender], [Email], [PasswordHash], [PhoneNumber], [UserAddress], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [RoleID]) VALUES (2, N'Lê Trung Thịnh', 0, N'nvthinh127@gmail.com', N'$2a$11$9kId4WbmsWo74IQ0EK3xTe/fmBijUp3U8CzZF1VRfFlbuwEyKSND.', N'0765442135', N'New York', 1, CAST(N'2025-07-09T06:25:15.553' AS DateTime), CAST(N'2025-07-17T15:40:02.233' AS DateTime), N'System', N'Admin', 2)
INSERT [dbo].[Users] ([Id], [UserName], [Gender], [Email], [PasswordHash], [PhoneNumber], [UserAddress], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [RoleID]) VALUES (3, N'Jason', 1, N'jsonwilliam341@gmail.com', N'$2a$11$MqsPBSoqsi.aQGtzi5r9H.xCE0xr8SarUUdZF7liLueDf8vWpFuVu', N'0765442135', N'New York', 1, CAST(N'2025-07-15T17:57:21.930' AS DateTime), CAST(N'2025-07-16T06:46:12.730' AS DateTime), N'System', N'System', 2)
INSERT [dbo].[Users] ([Id], [UserName], [Gender], [Email], [PasswordHash], [PhoneNumber], [UserAddress], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy], [RoleID]) VALUES (6, N'Robin', 1, N'rholy921@gmail.com', N'$2a$11$C5OUzn3xU09Tdo3DKBt1Ieej/lRiNnZwvsEBQOKQUOocEheJNVNty', N'0765442135', N'New York', 1, CAST(N'2025-07-16T06:46:50.107' AS DateTime), CAST(N'2025-07-16T06:47:13.910' AS DateTime), N'System', N'System', 2)
SET IDENTITY_INSERT [dbo].[Users] OFF
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Banners__2CB664DCC72D8B40]    Script Date: 21/07/2025 10:34:39 AM ******/
ALTER TABLE [dbo].[Banners] ADD UNIQUE NONCLUSTERED 
(
	[Title] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [UQ__Cart__B37A193B716286C5]    Script Date: 21/07/2025 10:34:39 AM ******/
ALTER TABLE [dbo].[Cart] ADD  CONSTRAINT [UQ__Cart__B37A193B716286C5] UNIQUE NONCLUSTERED 
(
	[ProVarID] ASC,
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Colors__C71A5A7BCC4AFE6A]    Script Date: 21/07/2025 10:34:39 AM ******/
ALTER TABLE [dbo].[Colors] ADD UNIQUE NONCLUSTERED 
(
	[ColorName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [UQ__ConfirmO__1788CCAD58F24DB7]    Script Date: 21/07/2025 10:34:39 AM ******/
ALTER TABLE [dbo].[ConfirmOTP] ADD UNIQUE NONCLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Material__9C87053C2DBE2E36]    Script Date: 21/07/2025 10:34:39 AM ******/
ALTER TABLE [dbo].[Materials] ADD UNIQUE NONCLUSTERED 
(
	[MaterialName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__News__2CB664DC80BB2873]    Script Date: 21/07/2025 10:34:39 AM ******/
ALTER TABLE [dbo].[News] ADD UNIQUE NONCLUSTERED 
(
	[Title] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__OrderSta__837D0BC16B58C580]    Script Date: 21/07/2025 10:34:39 AM ******/
ALTER TABLE [dbo].[OrderStatus] ADD UNIQUE NONCLUSTERED 
(
	[OrderStatusName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Promotio__A617E4B624C68463]    Script Date: 21/07/2025 10:34:39 AM ******/
ALTER TABLE [dbo].[Promotions] ADD UNIQUE NONCLUSTERED 
(
	[PromotionCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Promotio__793B63B5DC8BB176]    Script Date: 21/07/2025 10:34:39 AM ******/
ALTER TABLE [dbo].[PromotionTypes] ADD UNIQUE NONCLUSTERED 
(
	[PromotionTypeName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Sizes__619EFC3E40EF8F9B]    Script Date: 21/07/2025 10:34:39 AM ******/
ALTER TABLE [dbo].[Sizes] ADD UNIQUE NONCLUSTERED 
(
	[SizeName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Sliders__2CB664DCD229C1C5]    Script Date: 21/07/2025 10:34:39 AM ******/
ALTER TABLE [dbo].[Sliders] ADD UNIQUE NONCLUSTERED 
(
	[Title] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Supplier__A9D105344265C7EB]    Script Date: 21/07/2025 10:34:39 AM ******/
ALTER TABLE [dbo].[Suppliers] ADD UNIQUE NONCLUSTERED 
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Units__B5EE6678A8849DBC]    Script Date: 21/07/2025 10:34:39 AM ******/
ALTER TABLE [dbo].[Units] ADD UNIQUE NONCLUSTERED 
(
	[UnitName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Users__A9D10534CF7550B8]    Script Date: 21/07/2025 10:34:39 AM ******/
ALTER TABLE [dbo].[Users] ADD UNIQUE NONCLUSTERED 
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Banners] ADD  DEFAULT ((0)) FOR [DisplayOrder]
GO
ALTER TABLE [dbo].[Comment] ADD  DEFAULT ((0)) FOR [LikeCount]
GO
ALTER TABLE [dbo].[ConfirmOTP] ADD  DEFAULT ((0)) FOR [FailedAttempt]
GO
ALTER TABLE [dbo].[OrderAddress] ADD  DEFAULT ((1)) FOR [IsDeafaultAddress]
GO
ALTER TABLE [dbo].[Orders] ADD  DEFAULT (getdate()) FOR [OrderDate]
GO
ALTER TABLE [dbo].[Orders] ADD  DEFAULT ((0)) FOR [PriceDiscount]
GO
ALTER TABLE [dbo].[Orders] ADD  DEFAULT ((0)) FOR [IsPaid]
GO
ALTER TABLE [dbo].[Orders] ADD  DEFAULT ((0)) FOR [IsUserGuest]
GO
ALTER TABLE [dbo].[Products] ADD  DEFAULT ((0)) FOR [ViewCount]
GO
ALTER TABLE [dbo].[ProductVariant] ADD  DEFAULT ((0)) FOR [StockQty]
GO
ALTER TABLE [dbo].[ProductVariantImages] ADD  DEFAULT ((0)) FOR [DisplayOrder]
GO
ALTER TABLE [dbo].[PromotionTypes] ADD  DEFAULT ((0)) FOR [PromotionUnit]
GO
ALTER TABLE [dbo].[Sliders] ADD  DEFAULT ((0)) FOR [IsPoster]
GO
ALTER TABLE [dbo].[Sliders] ADD  DEFAULT ((0)) FOR [DisplayOrder]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT ((1)) FOR [Gender]
GO
ALTER TABLE [dbo].[Banners]  WITH CHECK ADD FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[Cart]  WITH CHECK ADD FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[Cart]  WITH CHECK ADD  CONSTRAINT [FK_Cart_ProVarID] FOREIGN KEY([ProVarID])
REFERENCES [dbo].[ProductVariant] ([Id])
GO
ALTER TABLE [dbo].[Cart] CHECK CONSTRAINT [FK_Cart_ProVarID]
GO
ALTER TABLE [dbo].[Comment]  WITH CHECK ADD FOREIGN KEY([ProReviewID])
REFERENCES [dbo].[ProductReviews] ([Id])
GO
ALTER TABLE [dbo].[Comment]  WITH CHECK ADD FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[ConfirmOTP]  WITH CHECK ADD  CONSTRAINT [FK__ConfirmOT__UserI__3D2915A8] FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[ConfirmOTP] CHECK CONSTRAINT [FK__ConfirmOT__UserI__3D2915A8]
GO
ALTER TABLE [dbo].[Feedbacks]  WITH CHECK ADD FOREIGN KEY([ParentFeedbackID])
REFERENCES [dbo].[Feedbacks] ([Id])
GO
ALTER TABLE [dbo].[Feedbacks]  WITH CHECK ADD FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[ImageProductReviews]  WITH CHECK ADD FOREIGN KEY([ProductReviewID])
REFERENCES [dbo].[ProductReviews] ([Id])
GO
ALTER TABLE [dbo].[News]  WITH CHECK ADD FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[OrderAddress]  WITH CHECK ADD FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[OrderDetails]  WITH CHECK ADD FOREIGN KEY([OrderID])
REFERENCES [dbo].[Orders] ([Id])
GO
ALTER TABLE [dbo].[OrderDetails]  WITH CHECK ADD FOREIGN KEY([ProVarID])
REFERENCES [dbo].[ProductVariant] ([Id])
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD FOREIGN KEY([OrderAddID])
REFERENCES [dbo].[OrderAddress] ([Id])
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD FOREIGN KEY([OrderStaID])
REFERENCES [dbo].[OrderStatus] ([Id])
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD FOREIGN KEY([PaymentMethodID])
REFERENCES [dbo].[PaymentMethods] ([Id])
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD FOREIGN KEY([PromotionID])
REFERENCES [dbo].[Promotions] ([Id])
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [FK_Orders_UserGuest] FOREIGN KEY([UserGuestID])
REFERENCES [dbo].[UserGuest] ([Id])
GO
ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [FK_Orders_UserGuest]
GO
ALTER TABLE [dbo].[ProductReviews]  WITH CHECK ADD FOREIGN KEY([ProID])
REFERENCES [dbo].[Products] ([Id])
GO
ALTER TABLE [dbo].[ProductReviews]  WITH CHECK ADD FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[Products]  WITH CHECK ADD FOREIGN KEY([BrandID])
REFERENCES [dbo].[Brands] ([Id])
GO
ALTER TABLE [dbo].[Products]  WITH CHECK ADD FOREIGN KEY([CategoryID])
REFERENCES [dbo].[Categories] ([Id])
GO
ALTER TABLE [dbo].[Products]  WITH CHECK ADD FOREIGN KEY([CountriesID])
REFERENCES [dbo].[Countries] ([Id])
GO
ALTER TABLE [dbo].[Products]  WITH CHECK ADD FOREIGN KEY([SupplierID])
REFERENCES [dbo].[Suppliers] ([Id])
GO
ALTER TABLE [dbo].[ProductVariant]  WITH CHECK ADD  CONSTRAINT [FK__ProductVa__Color__5EBF139D] FOREIGN KEY([ColorID])
REFERENCES [dbo].[Colors] ([Id])
GO
ALTER TABLE [dbo].[ProductVariant] CHECK CONSTRAINT [FK__ProductVa__Color__5EBF139D]
GO
ALTER TABLE [dbo].[ProductVariant]  WITH CHECK ADD  CONSTRAINT [FK__ProductVa__Mater__60A75C0F] FOREIGN KEY([MaterialID])
REFERENCES [dbo].[Materials] ([Id])
GO
ALTER TABLE [dbo].[ProductVariant] CHECK CONSTRAINT [FK__ProductVa__Mater__60A75C0F]
GO
ALTER TABLE [dbo].[ProductVariant]  WITH CHECK ADD FOREIGN KEY([ProductID])
REFERENCES [dbo].[Products] ([Id])
GO
ALTER TABLE [dbo].[ProductVariant]  WITH CHECK ADD  CONSTRAINT [FK__ProductVa__SizeI__5FB337D6] FOREIGN KEY([SizeID])
REFERENCES [dbo].[Sizes] ([Id])
GO
ALTER TABLE [dbo].[ProductVariant] CHECK CONSTRAINT [FK__ProductVa__SizeI__5FB337D6]
GO
ALTER TABLE [dbo].[ProductVariant]  WITH CHECK ADD  CONSTRAINT [FK__ProductVa__UnitI__619B8048] FOREIGN KEY([UnitID])
REFERENCES [dbo].[Units] ([Id])
GO
ALTER TABLE [dbo].[ProductVariant] CHECK CONSTRAINT [FK__ProductVa__UnitI__619B8048]
GO
ALTER TABLE [dbo].[ProductVariantImages]  WITH CHECK ADD FOREIGN KEY([ProVarID])
REFERENCES [dbo].[ProductVariant] ([Id])
GO
ALTER TABLE [dbo].[Promotions]  WITH CHECK ADD FOREIGN KEY([PromotionTypeID])
REFERENCES [dbo].[PromotionTypes] ([Id])
GO
ALTER TABLE [dbo].[Sliders]  WITH CHECK ADD FOREIGN KEY([ProductId])
REFERENCES [dbo].[Products] ([Id])
GO
ALTER TABLE [dbo].[Users]  WITH CHECK ADD FOREIGN KEY([RoleID])
REFERENCES [dbo].[Roles] ([Id])
GO
ALTER TABLE [dbo].[ProductReviews]  WITH CHECK ADD CHECK  (([StarRating]>=(0) AND [StarRating]<=(5)))
GO
USE [master]
GO
ALTER DATABASE [TOMFURNITURE] SET  READ_WRITE 
GO
