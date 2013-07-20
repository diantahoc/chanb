/****** Object:  Table [dbo].[bans]    Script Date: 07/20/2013 08:13:55 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[bans]') AND type in (N'U'))
DROP TABLE [dbo].[bans]

/****** Object:  Table [dbo].[board]    Script Date: 07/20/2013 08:13:55 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[board]') AND type in (N'U'))
DROP TABLE [dbo].[board]

/****** Object:  Table [dbo].[files]    Script Date: 07/20/2013 08:13:55 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[files]') AND type in (N'U'))
DROP TABLE [dbo].[files]

/****** Object:  Table [dbo].[ioqueue]    Script Date: 07/20/2013 08:13:55 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ioqueue]') AND type in (N'U'))
DROP TABLE [dbo].[ioqueue]

/****** Object:  Table [dbo].[lockedT]    Script Date: 07/20/2013 08:13:55 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[lockedT]') AND type in (N'U'))
DROP TABLE [dbo].[lockedT]

/****** Object:  Table [dbo].[mods]    Script Date: 07/20/2013 08:13:55 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[mods]') AND type in (N'U'))
DROP TABLE [dbo].[mods]

/****** Object:  Table [dbo].[reports]    Script Date: 07/20/2013 08:13:55 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[reports]') AND type in (N'U'))
DROP TABLE [dbo].[reports]

/****** Object:  Default [DF_bans_effective]    Script Date: 07/20/2013 08:13:55 ******/
IF  EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_bans_effective]') AND parent_object_id = OBJECT_ID(N'[dbo].[bans]'))
Begin
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_bans_effective]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[bans] DROP CONSTRAINT [DF_bans_effective]
END


End

/****** Object:  Table [dbo].[reports]    Script Date: 07/20/2013 08:13:55 ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[reports]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[reports](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[postID] [int] NULL,
	[reporterIP] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[time] [datetime] NULL,
	[comment] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL
)
END

/****** Object:  Table [dbo].[mods]    Script Date: 07/20/2013 08:13:55 ******/
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

/****** Object:  Table [dbo].[lockedT]    Script Date: 07/20/2013 08:13:55 ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[lockedT]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[lockedT](
	[locked] [int] NOT NULL
)
END

/****** Object:  Table [dbo].[ioqueue]    Script Date: 07/20/2013 08:13:55 ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ioqueue]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ioqueue](
	[tid] [int] NOT NULL
)
END

/****** Object:  Table [dbo].[files]    Script Date: 07/20/2013 08:13:55 ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[files]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[files](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[postID] [int] NOT NULL,
	[chanbname] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[realname] [nvarchar](500) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[size] [bigint] NOT NULL,
	[dimension] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[md5] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[extension] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[mimetype] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[sfw] [bit] NULL
)
END

/****** Object:  Table [dbo].[board]    Script Date: 07/20/2013 08:13:55 ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[board]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[board](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[type] [int] NOT NULL,
	[parentT] [int] NULL,
	[time] [datetime] NULL,
	[postername] [nvarchar](1000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[comment] [nvarchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[email] [nvarchar](1000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[subject] [nvarchar](1000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[password] [nvarchar](1000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[IP] [varchar](16) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[bumplevel] [datetime] NULL,
	[ua] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[posterID] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[sticky] [bit] NOT NULL,
	[locked] [bit] NOT NULL,
	[mta] [bit] NOT NULL,
	[hasFile] [bit] NOT NULL
)
END

/****** Object:  Table [dbo].[bans]    Script Date: 07/20/2013 08:13:55 ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[bans]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[bans](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[perm] [bit] NOT NULL,
	[expiry] [datetime] NOT NULL,
	[comment] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[post] [int] NOT NULL,
	[IP] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[canview] [bit] NOT NULL,
	[modname] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[bannedon] [datetime] NOT NULL,
	[effective] [bit] NOT NULL
)
END

/****** Object:  Default [DF_bans_effective]    Script Date: 07/20/2013 08:13:55 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_bans_effective]') AND parent_object_id = OBJECT_ID(N'[dbo].[bans]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_bans_effective]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[bans] ADD  CONSTRAINT [DF_bans_effective]  DEFAULT ((1)) FOR [effective]
END


End

