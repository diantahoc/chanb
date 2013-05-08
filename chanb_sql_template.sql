/****** Object:  Table [dbo].[bans]    Script Date: 05/09/2013 02:31:17 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[bans]') AND type in (N'U'))
DROP TABLE [dbo].[bans]
GO
/****** Object:  Table [dbo].[board]    Script Date: 05/09/2013 02:31:17 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[board]') AND type in (N'U'))
DROP TABLE [dbo].[board]
GO
/****** Object:  Table [dbo].[mods]    Script Date: 05/09/2013 02:31:17 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[mods]') AND type in (N'U'))
DROP TABLE [dbo].[mods]
GO
/****** Object:  Table [dbo].[reports]    Script Date: 05/09/2013 02:31:17 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[reports]') AND type in (N'U'))
DROP TABLE [dbo].[reports]
GO
/****** Object:  Table [dbo].[reports]    Script Date: 05/09/2013 02:31:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[reports]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[reports](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[postID] [int] NULL,
	[reporterIP] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[time] [datetime] NULL
)
END
GO
/****** Object:  Table [dbo].[mods]    Script Date: 05/09/2013 02:31:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[mods]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[mods](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[username] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[password] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL
)
END
GO
/****** Object:  Table [dbo].[board]    Script Date: 05/09/2013 02:31:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[board]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[board](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[type] [int] NULL,
	[parentT] [int] NULL,
	[time] [datetime] NULL,
	[postername] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[comment] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[email] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[subject] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[password] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[imagename] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[IP] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[bumplevel] [datetime] NULL,
	[sticky] [int] NULL,
	[ua] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[posterID] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[locked] [int] NULL
)
END
GO
/****** Object:  Table [dbo].[bans]    Script Date: 05/09/2013 02:31:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[bans]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[bans](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[perm] [int] NULL,
	[expiry] [datetime] NULL,
	[comment] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[post] [int] NULL,
	[IP] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL
)
END
GO
