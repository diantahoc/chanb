/****** Object:  Table [dbo].[bans]    Script Date: 05/25/2013 02:32:30 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[bans]') AND type in (N'U'))
DROP TABLE [dbo].[bans]

/****** Object:  Table [dbo].[board]    Script Date: 05/25/2013 02:32:30 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[board]') AND type in (N'U'))
DROP TABLE [dbo].[board]

/****** Object:  Table [dbo].[mods]    Script Date: 05/25/2013 02:32:30 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[mods]') AND type in (N'U'))
DROP TABLE [dbo].[mods]

/****** Object:  Table [dbo].[reports]    Script Date: 05/25/2013 02:32:30 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[reports]') AND type in (N'U'))
DROP TABLE [dbo].[reports]

/****** Object:  Table [dbo].[reports]    Script Date: 05/25/2013 02:32:30 ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[reports]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[reports](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[postID] [int] NULL,
	[reporterIP] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[time] [datetime] NULL
)
END

/****** Object:  Table [dbo].[mods]    Script Date: 05/25/2013 02:32:30 ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[mods]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[mods](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[username] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[password] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[power] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL
)
END

/****** Object:  Table [dbo].[board]    Script Date: 05/25/2013 02:32:30 ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[board]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[board](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[type] [int] NULL,
	[parentT] [int] NULL,
	[time] [datetime] NULL,
	[postername] [nvarchar](1000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[comment] [nvarchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[email] [nvarchar](1000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[subject] [nvarchar](1000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[password] [nvarchar](1000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[imagename] [nvarchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[IP] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[bumplevel] [datetime] NULL,
	[sticky] [int] NULL,
	[ua] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[posterID] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[locked] [int] NULL,
	[mta] [int] NULL
)
END

/****** Object:  Table [dbo].[bans]    Script Date: 05/25/2013 02:32:30 ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

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

