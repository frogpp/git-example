USE [master]
GO
/****** Object:  Database [MACSD_MVC_M]    Script Date: 2019/8/20 下午 03:21:57 ******/
CREATE DATABASE [MACSD_MVC_M]
 
ALTER DATABASE [MACSD_MVC_M] SET COMPATIBILITY_LEVEL = 140
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [MACSD_MVC_M].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [MACSD_MVC_M] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [MACSD_MVC_M] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [MACSD_MVC_M] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [MACSD_MVC_M] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [MACSD_MVC_M] SET ARITHABORT OFF 
GO
ALTER DATABASE [MACSD_MVC_M] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [MACSD_MVC_M] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [MACSD_MVC_M] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [MACSD_MVC_M] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [MACSD_MVC_M] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [MACSD_MVC_M] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [MACSD_MVC_M] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [MACSD_MVC_M] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [MACSD_MVC_M] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [MACSD_MVC_M] SET  ENABLE_BROKER 
GO
ALTER DATABASE [MACSD_MVC_M] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [MACSD_MVC_M] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [MACSD_MVC_M] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [MACSD_MVC_M] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [MACSD_MVC_M] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [MACSD_MVC_M] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [MACSD_MVC_M] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [MACSD_MVC_M] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [MACSD_MVC_M] SET  MULTI_USER 
GO
ALTER DATABASE [MACSD_MVC_M] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [MACSD_MVC_M] SET DB_CHAINING OFF 
GO
ALTER DATABASE [MACSD_MVC_M] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [MACSD_MVC_M] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [MACSD_MVC_M] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [MACSD_MVC_M] SET QUERY_STORE = OFF
GO
USE [MACSD_MVC_M]
GO
/****** Object:  UserDefinedFunction [dbo].[fn_ChkFlowExecute]    Script Date: 2019/8/20 下午 03:21:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		jedi_liao
-- Create date: 2017/10/19
-- Description:	check flow is working
-- =============================================
CREATE FUNCTION [dbo].[fn_ChkFlowExecute]
(
	@flow_code nvarchar(40)
)
RETURNS bit
AS
BEGIN
	RETURN iif(exists(select flow_odr from dbo.FLOW_PREARRANGE where flow_code=@flow_code and flow_status=1),1,0);
END
GO
/****** Object:  UserDefinedFunction [dbo].[fn_ChkSubFlowExecute]    Script Date: 2019/8/20 下午 03:21:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		jedi_liao
-- Create date: 2017/10/19
-- Description:	check sub flow is working
-- =============================================
CREATE FUNCTION [dbo].[fn_ChkSubFlowExecute]
(
	@sub_flow_code nvarchar(40)
)
RETURNS bit
AS
BEGIN
	RETURN iif(exists(select flow_odr from dbo.SUBFLOW_PREARRANGE where sub_flow_code=@sub_flow_code and flow_status=1),1,0);
END
GO
/****** Object:  UserDefinedFunction [dbo].[fn_GetAgentDisplay]    Script Date: 2019/8/20 下午 03:21:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		jedi_liao
-- Create date: 2017/05/17
-- Description:	get agent user name & user name
-- =============================================
CREATE FUNCTION [dbo].[fn_GetAgentDisplay] 
(
	@user_id nvarchar(50)
)
RETURNS nvarchar(50)
AS
BEGIN
	DECLARE @user_display nvarchar(50)=(select b.user_name+'代' from dbo.fn_Split(@user_id,'*') a inner join dbo.EMP_USER b on a.value=b.user_id for xml path('')); 
	RETURN left(@user_display,len(@user_display)-1);
END
GO
/****** Object:  UserDefinedFunction [dbo].[fn_GetFlowMapUser]    Script Date: 2019/8/20 下午 03:21:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		jedi_liao
-- Create date: 2017/05/22
-- Update date:  2019/06/17 參考TCG Tracko修正取得簽核角色
-- Description:	get flow role mapped user
-- =============================================
CREATE FUNCTION [dbo].[fn_GetFlowMapUser] 
(
	@org_id nvarchar(20),
	@role_id nvarchar(20)
)
RETURNS nvarchar(max)
AS
BEGIN
	DECLARE @parent_org_id nvarchar(20)=null;
	DECLARE @users nvarchar(max);
	select @parent_org_id=parent_id from EMP_ORG where org_id=@org_id;
	SET @users=stuff((select distinct ','+user_id from dbo.V_USER_FLOWROLE where isnull(role_id,'')=@role_id and isnull(org_id,'')=@org_id for xml path('')),1,1,'');
	WHILE ISNULL(@users,'')='' and @parent_org_id is not null
	BEGIN
		SET @users=stuff((select distinct ','+user_id from dbo.V_USER_FLOWROLE where isnull(role_id,'')=@role_id and isnull(org_id,'')=@parent_org_id for xml path('')),1,1,'');		
		select @parent_org_id=parent_id from EMP_ORG where org_id=@parent_org_id;
	END
	RETURN isnull(@users,'');
END

GO
/****** Object:  UserDefinedFunction [dbo].[fn_GetFlowMapUserName]    Script Date: 2019/8/20 下午 03:21:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		jedi_liao
-- Create date: 2018/06/20
-- Description:	get flow role mapped user name
-- =============================================
CREATE FUNCTION [dbo].[fn_GetFlowMapUserName] 
(
	@org_id nvarchar(20),
	@role_id nvarchar(20)
)
RETURNS nvarchar(max)
AS
BEGIN
	DECLARE @parent_org_id nvarchar(20)=null;
	DECLARE @users nvarchar(max);
	select @parent_org_id=parent_id from EMP_ORG where org_id=@org_id;
	SET @users=stuff((select distinct ','+b.user_name from dbo.V_USER_FLOWROLE a left outer join dbo.EMP_USER b on a.user_id=b.user_id where isnull(role_id,'')=@role_id and isnull(org_id,'')=@org_id for xml path('')),1,1,'');
	WHILE len(@users)=0 and @parent_org_id is not null
	BEGIN
		SET @users=stuff((select distinct ','+user_id from dbo.V_USER_FLOWROLE where isnull(role_id,'')=@role_id and isnull(org_id,'')=@org_id for xml path('')),1,1,'');		
	select @parent_org_id=parent_id from EMP_ORG where org_id=@org_id;
	END
	RETURN isnull(@users,'');
END
GO
/****** Object:  UserDefinedFunction [dbo].[fn_GetSetParam]    Script Date: 2019/8/20 下午 03:21:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		jedi_liao
-- Create date: 2017/05/17
-- Description:	get param value
-- =============================================
CREATE FUNCTION [dbo].[fn_GetSetParam]
(
	@set_item nvarchar(20),
	@set_type nvarchar(20)
)
RETURNS nvarchar(50)
AS
BEGIN
	RETURN (select top 1 set_value from dbo.SET_PARAM where set_item=@set_item and set_type=@set_type);
END
GO
/****** Object:  UserDefinedFunction [dbo].[fn_GetUsersOrgId]    Script Date: 2019/8/20 下午 03:21:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		jedi_liao	
-- Create date: 2017/10/18
-- Description:	get user's org
-- =============================================
CREATE FUNCTION [dbo].[fn_GetUsersOrgId]
(
	@user_id nvarchar(50)
)
RETURNS nvarchar(20)
AS
BEGIN
	RETURN (select top 1 org_id from dbo.V_USER_ORG where user_id in (select value from dbo.fn_Split(@user_id,'*')));
END
GO
/****** Object:  UserDefinedFunction [dbo].[fn_HasSubFlow]    Script Date: 2019/8/20 下午 03:21:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		jedi_liao
-- Create date: 2017/10/19
-- Description:	check sub flow is exists
-- =============================================
CREATE FUNCTION [dbo].[fn_HasSubFlow]
(
	@flow_code nvarchar(40),
	@signed_odr smallint
)
RETURNS bit
AS
BEGIN
	RETURN iif(exists(select distinct sub_flow_code from dbo.SUBFLOW_PREARRANGE a 
						inner join dbo.FLOW_SIGNEDLOG b on a.sub_flow_code=b.flow_code
						where a.flow_code=@flow_code and b.signed_memo=cast(@signed_odr as nvarchar(2))),1,0);
END
GO
/****** Object:  UserDefinedFunction [dbo].[fn_HasSubFlowExecute]    Script Date: 2019/8/20 下午 03:21:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		jedi_liao
-- Create date: 2017/10/19
-- Description:	check sub flow is working
-- =============================================
CREATE FUNCTION [dbo].[fn_HasSubFlowExecute]
(
	@flow_code nvarchar(40),
	@flow_odr smallint
)
RETURNS bit
AS
BEGIN
	RETURN iif(exists(select a.sub_flow_odr from dbo.SUBFLOW_PREARRANGE a 
						inner join dbo.FLOW_PREARRANGE b on a.flow_code=b.flow_code and a.flow_odr=b.flow_odr  
			where a.flow_code=@flow_code and a.flow_odr=@flow_odr and a.flow_status=1 and b.flow_status=1),1,0);
END
GO
/****** Object:  UserDefinedFunction [dbo].[fn_Split]    Script Date: 2019/8/20 下午 03:21:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		jedi_liao
-- Create date: 2017/06/19
-- Description:	split string to table
-- =============================================
CREATE FUNCTION [dbo].[fn_Split]
(	
	@words nvarchar(max),
	@tag nchar(1)
)
RETURNS @result TABLE
(
	[value] nvarchar(max) NULL
)
BEGIN 
    WHILE (CHARINDEX(@tag,@words)>0)	
	BEGIN
		INSERT INTO @result VALUES(SUBSTRING(@words,1,CHARINDEX(@tag,@words)-1));
		SET @words = SUBSTRING(@words,CHARINDEX(@tag,@words)+1,LEN(@words))
	END
	IF LEN(RTRIM(LTRIM(@words)))>0  
	   INSERT INTO @result VALUES(@words);
	RETURN
END

GO
/****** Object:  Table [dbo].[EMP_USER]    Script Date: 2019/8/20 下午 03:21:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EMP_USER](
	[user_id] [nvarchar](20) NOT NULL,
	[user_name] [nvarchar](50) NOT NULL,
	[user_tel] [varchar](50) NOT NULL,
	[user_email] [nvarchar](100) NOT NULL,
	[user_pwd] [nvarchar](50) NOT NULL,
	[user_token] [nvarchar](max) NOT NULL,
	[del_flg] [bit] NOT NULL,
	[crt_date] [datetime] NOT NULL,
	[crt_user] [nvarchar](50) NOT NULL,
	[mdf_date] [datetime] NOT NULL,
	[mdf_user] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_EMP_USER] PRIMARY KEY CLUSTERED 
(
	[user_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 95) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FLOW_PREARRANGE]    Script Date: 2019/8/20 下午 03:21:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FLOW_PREARRANGE](
	[flow_id] [nchar](20) NOT NULL,
	[flow_code] [nvarchar](50) NOT NULL,
	[flow_odr] [smallint] NOT NULL,
	[flow_status] [smallint] NOT NULL,
	[flow_org_id] [nvarchar](20) NOT NULL,
	[flow_role_id] [nvarchar](20) NOT NULL,
	[flow_user_id] [nvarchar](20) NOT NULL,
	[flow_decision] [bit] NOT NULL,
	[flow_option] [nvarchar](max) NOT NULL,
	[crt_date] [datetime] NOT NULL,
	[crt_user] [nvarchar](50) NOT NULL,
	[mdf_date] [datetime] NOT NULL,
	[mdf_user] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_FLOW_PREARRANGE] PRIMARY KEY CLUSTERED 
(
	[flow_code] ASC,
	[flow_odr] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SUBFLOW_PREARRANGE]    Script Date: 2019/8/20 下午 03:21:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SUBFLOW_PREARRANGE](
	[flow_id] [nchar](20) NOT NULL,
	[flow_code] [nvarchar](50) NOT NULL,
	[flow_odr] [smallint] NOT NULL,
	[sub_flow_code] [nvarchar](50) NOT NULL,
	[sub_flow_odr] [smallint] NOT NULL,
	[flow_status] [smallint] NOT NULL,
	[flow_org_id] [nvarchar](20) NOT NULL,
	[flow_role_id] [nvarchar](20) NOT NULL,
	[flow_user_id] [nvarchar](20) NOT NULL,
	[flow_decision] [bit] NOT NULL,
	[flow_option] [nvarchar](max) NOT NULL,
	[crt_date] [datetime] NOT NULL,
	[crt_user] [nvarchar](50) NOT NULL,
	[mdf_date] [datetime] NOT NULL,
	[mdf_user] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_SUBFLOW_PREARRANGE] PRIMARY KEY CLUSTERED 
(
	[sub_flow_code] ASC,
	[sub_flow_odr] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  UserDefinedFunction [dbo].[fn_GetFlowExpired]    Script Date: 2019/8/20 下午 03:21:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		jedi_liao
-- Create date: 2017/11/22
-- Description:	get flow expired
-- =============================================
CREATE FUNCTION [dbo].[fn_GetFlowExpired]
(	
	@flow_code nvarchar(50),
	@expiredDay smallint
)
RETURNS TABLE 
AS
RETURN 
(
	select flow_user_id as user_id,b.user_name,b.user_email
		from dbo.FLOW_PREARRANGE a
		left join dbo.EMP_USER b on b.user_id=a.flow_user_id
		where flow_code=@flow_code and flow_status=1 and datediff(day,a.mdf_date,getdate())>@expiredDay
	union 
	select flow_user_id as user_id,b.user_name,b.user_email
		from dbo.SUBFLOW_PREARRANGE a
		left join dbo.EMP_USER b on b.user_id=a.flow_user_id
		where flow_code=@flow_code and flow_status=1 and datediff(day,a.mdf_date,getdate())>@expiredDay
)
GO
/****** Object:  Table [dbo].[MAP_ORG_USER]    Script Date: 2019/8/20 下午 03:21:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MAP_ORG_USER](
	[org_id] [nvarchar](20) NOT NULL,
	[user_id] [nvarchar](20) NOT NULL,
	[crt_date] [datetime] NOT NULL,
	[crt_user] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_MAP_ORG_USER] PRIMARY KEY CLUSTERED 
(
	[org_id] ASC,
	[user_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MAP_USER_FLOWROLE]    Script Date: 2019/8/20 下午 03:21:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MAP_USER_FLOWROLE](
	[user_id] [nvarchar](20) NOT NULL,
	[role_id] [nvarchar](20) NOT NULL,
	[crt_date] [datetime] NOT NULL,
	[crt_user] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_FLOW_MAP_ROLE_USER] PRIMARY KEY CLUSTERED 
(
	[role_id] ASC,
	[user_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  UserDefinedFunction [dbo].[fn_GetFlowMapRole]    Script Date: 2019/8/20 下午 03:21:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		jedi_liao
-- Create date: 2017/10/20
-- Description:	get user mapped flow role
-- =============================================
CREATE FUNCTION [dbo].[fn_GetFlowMapRole]
(	
	@user_id nvarchar(50)
)
RETURNS TABLE 
AS
RETURN 
(
	select b.org_id+'_'+a.role_id as user_role 
		from dbo.MAP_USER_FLOWROLE a left join dbo.MAP_ORG_USER b on a.user_id=b.user_id 
		where a.user_id in (select value from dbo.fn_Split(@user_id,'*'))
	union
	select '_'+role_id as user_role from dbo.MAP_USER_FLOWROLE 
		where user_id in (select value from dbo.fn_Split(@user_id,'*'))
)
GO
/****** Object:  Table [dbo].[EMP_ORG]    Script Date: 2019/8/20 下午 03:21:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EMP_ORG](
	[org_id] [nvarchar](20) NOT NULL,
	[org_name] [nvarchar](50) NOT NULL,
	[parent_id] [nvarchar](20) NOT NULL,
	[del_flg] [bit] NOT NULL,
	[crt_date] [datetime] NOT NULL,
	[crt_user] [nvarchar](50) NOT NULL,
	[mdf_date] [datetime] NOT NULL,
	[mdf_user] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_EMP_ORG] PRIMARY KEY CLUSTERED 
(
	[org_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FLOW_ROLE]    Script Date: 2019/8/20 下午 03:21:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FLOW_ROLE](
	[role_id] [nvarchar](20) NOT NULL,
	[role_name] [nvarchar](50) NOT NULL,
	[del_flg] [bit] NOT NULL,
	[crt_date] [datetime] NOT NULL,
	[crt_user] [nvarchar](50) NOT NULL,
	[mdf_date] [datetime] NOT NULL,
	[mdf_user] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_FLOW_ROLE] PRIMARY KEY CLUSTERED 
(
	[role_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[V_FLOW_ACTIVELIST]    Script Date: 2019/8/20 下午 03:21:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- update date: 2019/06/17 參考TCG Tracko修正
-- =============================================

create VIEW [dbo].[V_FLOW_ACTIVELIST]
AS
SELECT          a.flow_code, dbo.fn_HasSubFlowExecute(a.flow_code, a.flow_odr) AS has_subflow, a.flow_status AS status, 
                            dbo.fn_GetSetParam(N' FlowStatus', a.flow_status) AS status_name, a.flow_org_id AS org_id, ISNULL(b.org_name, 
                            N'') AS org_name, a.flow_role_id AS role_id, ISNULL(c.role_name, N'') AS role_name, 
                            CASE WHEN ltrim(rtrim(a.flow_user_id)) = '' THEN dbo.fn_GetFlowMapUser(a.flow_org_id, a.flow_role_id) 
                            ELSE a.flow_user_id END AS user_id, CASE WHEN ltrim(rtrim(a.flow_user_id)) 
                            = '' THEN dbo.fn_GetFlowMapUserName(a.flow_org_id, a.flow_role_id) ELSE isnull(d .user_name, '') 
                            END AS user_name, ISNULL(d.user_email, N'') AS user_email, a.flow_decision, ISNULL(a.flow_option, N'') 
                            AS flow_option, a.mdf_date, a.mdf_user AS mdf_user_id, ISNULL(e.user_name, N'') AS mdf_user_name
FROM              dbo.FLOW_PREARRANGE AS a LEFT OUTER JOIN
                            dbo.EMP_ORG AS b ON a.flow_org_id = b.org_id LEFT OUTER JOIN
                            dbo.FLOW_ROLE AS c ON a.flow_role_id = c.role_id LEFT OUTER JOIN
                            dbo.EMP_USER AS d ON a.flow_user_id = d.user_id LEFT OUTER JOIN
                            dbo.EMP_USER AS e ON a.mdf_user = e.user_id
WHERE          (a.flow_status = 1)
GO
/****** Object:  Table [dbo].[MAIL_RECIPIENT]    Script Date: 2019/8/20 下午 03:21:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MAIL_RECIPIENT](
	[mail_id] [nvarchar](20) NOT NULL,
	[mail_type] [char](1) NOT NULL,
	[mail_role] [nvarchar](20) NOT NULL,
	[mail_address] [nvarchar](100) NOT NULL,
	[mail_title] [nvarchar](20) NOT NULL,
 CONSTRAINT [PK_MAIL_RECIPIENT] PRIMARY KEY CLUSTERED 
(
	[mail_id] ASC,
	[mail_type] ASC,
	[mail_role] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MAP_USER_MAILROLE]    Script Date: 2019/8/20 下午 03:21:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MAP_USER_MAILROLE](
	[user_id] [nvarchar](20) NOT NULL,
	[role_id] [nvarchar](20) NOT NULL,
	[crt_date] [datetime] NOT NULL,
	[crt_user] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_MAP_USER_MAILROLE] PRIMARY KEY CLUSTERED 
(
	[user_id] ASC,
	[role_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[V_MAIL_RECIPIENT]    Script Date: 2019/8/20 下午 03:21:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[V_MAIL_RECIPIENT]
AS
SELECT          dbo.MAIL_RECIPIENT.mail_id, dbo.MAIL_RECIPIENT.mail_type, dbo.MAIL_RECIPIENT.mail_address, 
                            dbo.MAIL_RECIPIENT.mail_title, ISNULL(dbo.EMP_USER.user_email, N'') AS mail_role_address, 
                            ISNULL(dbo.EMP_USER.user_name, N'') AS mail_role_title
FROM              dbo.MAIL_RECIPIENT LEFT OUTER JOIN
                            dbo.MAP_USER_MAILROLE ON 
                            dbo.MAIL_RECIPIENT.mail_role = dbo.MAP_USER_MAILROLE.role_id LEFT OUTER JOIN
                            dbo.EMP_USER ON dbo.MAP_USER_MAILROLE.user_id = dbo.EMP_USER.user_id
GO
/****** Object:  Table [dbo].[FLOW_SIGNEDLOG]    Script Date: 2019/8/20 下午 03:21:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FLOW_SIGNEDLOG](
	[flow_code] [nvarchar](50) NOT NULL,
	[flow_odr] [smallint] NOT NULL,
	[signed_odr] [smallint] NOT NULL,
	[signed_status] [smallint] NOT NULL,
	[signed_org_id] [nvarchar](20) NOT NULL,
	[signed_role_id] [nvarchar](20) NOT NULL,
	[signed_user_id] [nvarchar](50) NOT NULL,
	[signed_memo] [nvarchar](500) NOT NULL,
	[signed_date] [datetime] NOT NULL,
 CONSTRAINT [PK_FLOW_SIGNEDLOG] PRIMARY KEY CLUSTERED 
(
	[flow_code] ASC,
	[signed_odr] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[V_SUBFLOW_ACTIVELIST]    Script Date: 2019/8/20 下午 03:21:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[V_SUBFLOW_ACTIVELIST]
AS
SELECT          a.sub_flow_code, a.sub_flow_odr, a.flow_code, a.flow_odr, f.signed_memo AS signed_odr, a.flow_status AS status, 
                            dbo.fn_GetSetParam(N'FlowStatus', a.flow_status) AS status_name, a.flow_org_id AS org_id, ISNULL(b.org_name, N'') 
                            AS Expr1, a.flow_role_id AS role_id, ISNULL(c.role_name, N'') AS role_name, a.flow_user_id AS user_id, 
                            ISNULL(d.user_name, N'') AS user_name, ISNULL(d.user_email, N'') AS user_email, a.flow_decision, 
                            ISNULL(a.flow_option, N'') AS flow_option, a.mdf_date, a.mdf_user AS mdf_user_id, ISNULL(e.user_name, N'') 
                            AS mdf_user_name
FROM              dbo.SUBFLOW_PREARRANGE AS a LEFT OUTER JOIN
                            dbo.EMP_ORG AS b ON a.flow_org_id = b.org_id LEFT OUTER JOIN
                            dbo.FLOW_ROLE AS c ON a.flow_role_id = c.role_id LEFT OUTER JOIN
                            dbo.EMP_USER AS d ON a.flow_user_id = d.user_id LEFT OUTER JOIN
                            dbo.EMP_USER AS e ON a.mdf_user = e.user_id LEFT OUTER JOIN
                            dbo.FLOW_SIGNEDLOG AS f ON a.sub_flow_code = f.flow_code AND f.signed_odr = 0
WHERE          (a.flow_status = 1)
GO
/****** Object:  View [dbo].[V_SUBFLOW_LIST]    Script Date: 2019/8/20 下午 03:21:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[V_SUBFLOW_LIST]
AS
SELECT          a.sub_flow_code, a.sub_flow_odr, a.flow_code, a.flow_odr, f.signed_memo AS signed_odr, a.flow_status AS status, 
                            dbo.fn_GetSetParam(N'FlowStatus', a.flow_status) AS status_name, a.flow_org_id AS org_id, ISNULL(b.org_name, N'') 
                            AS Expr1, a.flow_role_id AS role_id, ISNULL(c.role_name, N'') AS role_name, a.flow_user_id AS user_id, 
                            ISNULL(d.user_name, N'') AS user_name, ISNULL(d.user_email, N'') AS user_email, a.flow_decision, 
                            ISNULL(a.flow_option, N'') AS flow_option, a.mdf_date, a.mdf_user AS mdf_user_id, ISNULL(e.user_name, N'') 
                            AS mdf_user_name
FROM              dbo.SUBFLOW_PREARRANGE AS a LEFT OUTER JOIN
                            dbo.EMP_ORG AS b ON a.flow_org_id = b.org_id LEFT OUTER JOIN
                            dbo.FLOW_ROLE AS c ON a.flow_role_id = c.role_id LEFT OUTER JOIN
                            dbo.EMP_USER AS d ON a.flow_user_id = d.user_id LEFT OUTER JOIN
                            dbo.EMP_USER AS e ON a.mdf_user = e.user_id LEFT OUTER JOIN
                            dbo.FLOW_SIGNEDLOG AS f ON a.sub_flow_code = f.flow_code AND f.signed_odr = 0
GO
/****** Object:  View [dbo].[V_USER_FLOWROLE]    Script Date: 2019/8/20 下午 03:21:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[V_USER_FLOWROLE]
AS
SELECT          dbo.EMP_USER.user_id, ISNULL(dbo.MAP_USER_FLOWROLE.role_id, N'') AS role_id, 
                            ISNULL(dbo.MAP_ORG_USER.org_id, N'') AS org_id
FROM              dbo.EMP_USER LEFT OUTER JOIN
                            dbo.MAP_USER_FLOWROLE ON dbo.EMP_USER.user_id = dbo.MAP_USER_FLOWROLE.user_id LEFT OUTER JOIN
                            dbo.MAP_ORG_USER ON dbo.EMP_USER.user_id = dbo.MAP_ORG_USER.user_id
GO
/****** Object:  Table [dbo].[MAP_RIGHT_FUNCTION]    Script Date: 2019/8/20 下午 03:21:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MAP_RIGHT_FUNCTION](
	[right_id] [nvarchar](20) NOT NULL,
	[function_id] [nvarchar](20) NOT NULL,
	[crt_date] [datetime] NOT NULL,
	[crt_user] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_MAP_RIGHT_FUNCTION] PRIMARY KEY CLUSTERED 
(
	[right_id] ASC,
	[function_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MAP_ROLE_RIGHT]    Script Date: 2019/8/20 下午 03:21:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MAP_ROLE_RIGHT](
	[role_id] [nvarchar](20) NOT NULL,
	[right_id] [nvarchar](20) NOT NULL,
	[crt_date] [datetime] NOT NULL,
	[crt_user] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_MAP_ROLE_RIGHT] PRIMARY KEY CLUSTERED 
(
	[role_id] ASC,
	[right_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MAP_USER_ROLE]    Script Date: 2019/8/20 下午 03:21:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MAP_USER_ROLE](
	[user_id] [nvarchar](20) NOT NULL,
	[role_id] [nvarchar](20) NOT NULL,
	[crt_date] [datetime] NOT NULL,
	[crt_user] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_MAP_USER_ROLE] PRIMARY KEY CLUSTERED 
(
	[user_id] ASC,
	[role_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 95) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SET_FUNCTION]    Script Date: 2019/8/20 下午 03:21:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SET_FUNCTION](
	[function_id] [nvarchar](20) NOT NULL,
	[function_name] [nvarchar](50) NOT NULL,
	[function_url] [nvarchar](100) NOT NULL,
	[parent_id] [nvarchar](20) NOT NULL,
	[sort_id] [smallint] NOT NULL,
	[del_flg] [bit] NOT NULL,
	[crt_date] [datetime] NOT NULL,
	[crt_user] [nvarchar](50) NOT NULL,
	[mdf_date] [datetime] NOT NULL,
	[mdf_user] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_SET_FUNCTION] PRIMARY KEY CLUSTERED 
(
	[function_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[V_USER_FUNCTION]    Script Date: 2019/8/20 下午 03:21:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[V_USER_FUNCTION]
AS
SELECT          dbo.EMP_USER.user_id, dbo.SET_FUNCTION.function_id, dbo.SET_FUNCTION.function_name, 
                            dbo.SET_FUNCTION.function_url, dbo.SET_FUNCTION.parent_id, dbo.SET_FUNCTION.sort_id, 
                            dbo.SET_FUNCTION.del_flg
FROM              dbo.EMP_USER LEFT OUTER JOIN
                            dbo.MAP_USER_ROLE ON dbo.MAP_USER_ROLE.user_id = dbo.EMP_USER.user_id LEFT OUTER JOIN
                            dbo.MAP_ROLE_RIGHT ON dbo.MAP_ROLE_RIGHT.role_id = dbo.MAP_USER_ROLE.role_id LEFT OUTER JOIN
                            dbo.MAP_RIGHT_FUNCTION ON 
                            dbo.MAP_RIGHT_FUNCTION.right_id = dbo.MAP_ROLE_RIGHT.right_id LEFT OUTER JOIN
                            dbo.SET_FUNCTION ON dbo.SET_FUNCTION.function_id = dbo.MAP_RIGHT_FUNCTION.function_id
WHERE          (dbo.EMP_USER.del_flg = 0) AND (dbo.SET_FUNCTION.del_flg = 0)
GO
/****** Object:  View [dbo].[V_USER_ORG]    Script Date: 2019/8/20 下午 03:21:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[V_USER_ORG]
AS
SELECT          dbo.EMP_USER.user_id, ISNULL(dbo.EMP_ORG.org_id, N'') AS org_id, ISNULL(dbo.EMP_ORG.org_name, N'') 
                            AS org_name, ISNULL(dbo.EMP_ORG.parent_id, N'') AS parent_id, ISNULL(dbo.EMP_ORG.del_flg, 0) AS del_flg
FROM              dbo.EMP_USER LEFT OUTER JOIN
                            dbo.MAP_ORG_USER ON dbo.EMP_USER.user_id = dbo.MAP_ORG_USER.user_id LEFT OUTER JOIN
                            dbo.EMP_ORG ON dbo.MAP_ORG_USER.org_id = dbo.EMP_ORG.org_id
GO
/****** Object:  Table [dbo].[DIM_RIGHT]    Script Date: 2019/8/20 下午 03:21:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DIM_RIGHT](
	[right_id] [nvarchar](20) NOT NULL,
	[right_name] [nvarchar](50) NOT NULL,
	[del_flg] [bit] NOT NULL,
	[crt_date] [datetime] NOT NULL,
	[crt_user] [nvarchar](50) NOT NULL,
	[mdf_date] [datetime] NOT NULL,
	[mdf_user] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_DIM_RIGHT] PRIMARY KEY CLUSTERED 
(
	[right_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[V_USER_RIGHT]    Script Date: 2019/8/20 下午 03:21:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[V_USER_RIGHT]
AS
SELECT          dbo.EMP_USER.user_id, dbo.DIM_RIGHT.right_id, dbo.DIM_RIGHT.right_name, dbo.DIM_RIGHT.del_flg
FROM              dbo.EMP_USER LEFT OUTER JOIN
                            dbo.MAP_USER_ROLE ON dbo.MAP_USER_ROLE.user_id = dbo.EMP_USER.user_id LEFT OUTER JOIN
                            dbo.MAP_ROLE_RIGHT ON dbo.MAP_ROLE_RIGHT.role_id = dbo.MAP_USER_ROLE.role_id LEFT OUTER JOIN
                            dbo.DIM_RIGHT ON dbo.DIM_RIGHT.right_id = dbo.MAP_ROLE_RIGHT.right_id
WHERE          (dbo.EMP_USER.del_flg = 0) AND (dbo.DIM_RIGHT.del_flg = 0)
GO
/****** Object:  Table [dbo].[DIM_ROLE]    Script Date: 2019/8/20 下午 03:21:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DIM_ROLE](
	[role_id] [nvarchar](20) NOT NULL,
	[role_name] [nvarchar](50) NOT NULL,
	[del_flg] [bit] NOT NULL,
	[crt_date] [datetime] NOT NULL,
	[crt_user] [nvarchar](50) NOT NULL,
	[mdf_date] [datetime] NOT NULL,
	[mdf_user] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_DIM_ROLE] PRIMARY KEY CLUSTERED 
(
	[role_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[V_USER_ROLE]    Script Date: 2019/8/20 下午 03:21:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[V_USER_ROLE]
AS
SELECT          dbo.EMP_USER.user_id, dbo.DIM_ROLE.role_id, dbo.DIM_ROLE.role_name, dbo.DIM_ROLE.del_flg
FROM              dbo.EMP_USER LEFT OUTER JOIN
                            dbo.MAP_USER_ROLE ON dbo.MAP_USER_ROLE.user_id = dbo.EMP_USER.user_id LEFT OUTER JOIN
                            dbo.DIM_ROLE ON dbo.DIM_ROLE.role_id = dbo.MAP_USER_ROLE.role_id
WHERE          (dbo.EMP_USER.del_flg = 0) AND (dbo.DIM_ROLE.del_flg = 0)
GO
/****** Object:  Table [dbo].[ANNOUNCEMENT]    Script Date: 2019/8/20 下午 03:21:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ANNOUNCEMENT](
	[sid] [bigint] IDENTITY(1,1) NOT NULL,
	[title] [nvarchar](100) NOT NULL,
	[comment] [nvarchar](max) NOT NULL,
	[effective_date] [datetime] NOT NULL,
	[expire_date] [datetime] NOT NULL,
	[attach_name] [nvarchar](max) NULL,
	[del_flg] [bit] NOT NULL,
	[crt_date] [datetime] NOT NULL,
	[crt_user] [nvarchar](50) NOT NULL,
	[mdf_date] [datetime] NOT NULL,
	[mdf_user] [nvarchar](50) NOT NULL,
	[off_doc] [nvarchar](max) NULL,
 CONSTRAINT [PK_ANNOUNCEMENT] PRIMARY KEY CLUSTERED 
(
	[sid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EFIELD_SET]    Script Date: 2019/8/20 下午 03:21:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EFIELD_SET](
	[field_id] [int] NOT NULL,
	[field_name] [nvarchar](50) NOT NULL,
	[field_size] [smallint] NULL,
	[field_type] [smallint] NOT NULL,
	[field_exttype] [smallint] NOT NULL,
	[field_option] [nvarchar](max) NULL,
	[field_isfill] [bit] NOT NULL,
	[memo] [nvarchar](500) NULL,
	[del_flg] [bit] NOT NULL,
	[crt_date] [datetime] NOT NULL,
	[crt_user] [nvarchar](50) NOT NULL,
	[mdf_date] [datetime] NOT NULL,
	[mdf_user] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_EFIELD_SET] PRIMARY KEY CLUSTERED 
(
	[field_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EFORM_DATAFILL]    Script Date: 2019/8/20 下午 03:21:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EFORM_DATAFILL](
	[fill_id] [nvarchar](20) NOT NULL,
	[form_id] [nvarchar](20) NOT NULL,
	[fill_data] [nvarchar](max) NOT NULL,
	[flow_code] [nvarchar](50) NULL,
	[del_flg] [bit] NOT NULL,
	[crt_date] [datetime] NOT NULL,
	[crt_user] [nvarchar](50) NOT NULL,
	[mdf_date] [datetime] NOT NULL,
	[mdf_user] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_EFORM_FILL] PRIMARY KEY CLUSTERED 
(
	[fill_id] ASC,
	[form_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EFORM_FLOWLOG]    Script Date: 2019/8/20 下午 03:21:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EFORM_FLOWLOG](
	[flow_code] [nvarchar](20) NOT NULL,
	[fill_id] [nvarchar](20) NOT NULL,
	[crt_date] [datetime] NOT NULL,
 CONSTRAINT [PK_EFORM_FLOWLOG] PRIMARY KEY CLUSTERED 
(
	[fill_id] ASC,
	[flow_code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EFORM_SET]    Script Date: 2019/8/20 下午 03:21:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EFORM_SET](
	[form_id] [nvarchar](20) NOT NULL,
	[form_type] [nvarchar](20) NOT NULL,
	[form_name] [nvarchar](50) NOT NULL,
	[effective_date] [datetime] NOT NULL,
	[expire_date] [datetime] NOT NULL,
	[memo] [nvarchar](500) NULL,
	[crt_date] [datetime] NOT NULL,
	[crt_user] [nvarchar](50) NOT NULL,
	[mdf_date] [datetime] NOT NULL,
	[mdf_user] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_EFORM_SET_1] PRIMARY KEY CLUSTERED 
(
	[form_id] ASC,
	[form_type] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EMP_AGENT]    Script Date: 2019/8/20 下午 03:21:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EMP_AGENT](
	[sid] [int] IDENTITY(1,1) NOT NULL,
	[user_id] [nvarchar](20) NOT NULL,
	[agent_id] [nvarchar](20) NOT NULL,
	[agent_from] [datetime] NOT NULL,
	[agent_to] [datetime] NOT NULL,
	[del_flg] [bit] NOT NULL,
	[crt_date] [datetime] NOT NULL,
	[crt_user] [nvarchar](50) NOT NULL,
	[mdf_date] [datetime] NOT NULL,
	[mdf_user] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_EMP_AGENT] PRIMARY KEY CLUSTERED 
(
	[sid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EMP_USER_COPY]    Script Date: 2019/8/20 下午 03:21:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EMP_USER_COPY](
	[user_id] [nvarchar](20) NOT NULL,
	[user_name] [nvarchar](50) NOT NULL,
	[user_tel] [varchar](50) NOT NULL,
	[user_email] [nvarchar](100) NOT NULL,
	[user_pwd] [nvarchar](50) NOT NULL,
	[user_token] [nvarchar](max) NOT NULL,
	[del_flg] [bit] NOT NULL,
	[crt_date] [datetime] NOT NULL,
	[crt_user] [nvarchar](50) NOT NULL,
	[mdf_date] [datetime] NOT NULL,
	[mdf_user] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_EMP_USER_COPY] PRIMARY KEY CLUSTERED 
(
	[user_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 95) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FLOW_SET]    Script Date: 2019/8/20 下午 03:21:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FLOW_SET](
	[flow_id] [nvarchar](20) NOT NULL,
	[flow_name] [nvarchar](50) NOT NULL,
	[effective_date] [datetime] NOT NULL,
	[expire_date] [datetime] NOT NULL,
	[memo] [nvarchar](500) NOT NULL,
	[crt_date] [datetime] NOT NULL,
	[crt_user] [nvarchar](50) NOT NULL,
	[mdf_date] [datetime] NOT NULL,
	[mdf_user] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_FLOW_SET] PRIMARY KEY CLUSTERED 
(
	[flow_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FLOW_SET_DETAIL]    Script Date: 2019/8/20 下午 03:21:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FLOW_SET_DETAIL](
	[flow_id] [nvarchar](20) NOT NULL,
	[set_odr] [smallint] NOT NULL,
	[set_org_id] [nvarchar](20) NOT NULL,
	[set_role_id] [nvarchar](20) NOT NULL,
	[set_user_id] [nvarchar](20) NOT NULL,
	[set_flow_id] [nvarchar](20) NOT NULL,
	[set_decision] [bit] NOT NULL,
	[set_option] [nvarchar](max) NOT NULL,
	[crt_date] [datetime] NOT NULL,
	[crt_user] [nvarchar](50) NOT NULL,
	[mdf_date] [datetime] NOT NULL,
	[mdf_user] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_FLOW_DTLSET] PRIMARY KEY CLUSTERED 
(
	[flow_id] ASC,
	[set_odr] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MAIL_LOG]    Script Date: 2019/8/20 下午 03:21:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MAIL_LOG](
	[sid] [bigint] IDENTITY(1,1) NOT NULL,
	[mail_sender] [nvarchar](max) NOT NULL,
	[mail_receiver] [nvarchar](max) NOT NULL,
	[cc_receiver] [nvarchar](max) NOT NULL,
	[bcc_receiver] [nvarchar](max) NOT NULL,
	[mail_subject] [nvarchar](100) NOT NULL,
	[mail_content] [nvarchar](max) NOT NULL,
	[send_flg] [bit] NOT NULL,
	[crt_date] [datetime] NOT NULL,
 CONSTRAINT [PK_MAIL_LOG] PRIMARY KEY CLUSTERED 
(
	[sid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MAIL_ROLE]    Script Date: 2019/8/20 下午 03:21:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MAIL_ROLE](
	[role_id] [nvarchar](20) NOT NULL,
	[role_name] [nvarchar](50) NOT NULL,
	[del_flg] [bit] NOT NULL,
	[crt_date] [datetime] NOT NULL,
	[crt_user] [nvarchar](50) NOT NULL,
	[mdf_date] [datetime] NOT NULL,
	[mdf_user] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_MAIL_ROLE] PRIMARY KEY CLUSTERED 
(
	[role_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MAIL_TEMPLATE]    Script Date: 2019/8/20 下午 03:21:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MAIL_TEMPLATE](
	[mail_id] [nvarchar](20) NOT NULL,
	[mail_name] [nvarchar](50) NOT NULL,
	[mail_subject] [nvarchar](100) NOT NULL,
	[mail_content] [nvarchar](max) NOT NULL,
	[del_flg] [bit] NOT NULL,
	[crt_date] [datetime] NOT NULL,
	[crt_user] [nvarchar](50) NOT NULL,
	[mdf_date] [datetime] NOT NULL,
	[mdf_user] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_MAIL_TEMPLATE] PRIMARY KEY CLUSTERED 
(
	[mail_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MAP_EFORM_EFIELD]    Script Date: 2019/8/20 下午 03:21:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MAP_EFORM_EFIELD](
	[form_id] [nvarchar](20) NOT NULL,
	[field_id] [int] NOT NULL,
	[sort_id] [smallint] NOT NULL,
	[issplitter] [bit] NOT NULL,
	[crt_date] [datetime] NOT NULL,
	[crt_user] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_MAP_EFORM_EFIELD] PRIMARY KEY CLUSTERED 
(
	[form_id] ASC,
	[field_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MAP_EFORM_FLOW]    Script Date: 2019/8/20 下午 03:21:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MAP_EFORM_FLOW](
	[form_id] [nvarchar](20) NOT NULL,
	[flow_id] [nvarchar](20) NOT NULL,
	[alert_day] [smallint] NOT NULL,
	[crt_date] [datetime] NOT NULL,
	[crt_user] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_MAP_EFORM_FLOW] PRIMARY KEY CLUSTERED 
(
	[form_id] ASC,
	[flow_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MAP_EFORM_ORG]    Script Date: 2019/8/20 下午 03:21:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MAP_EFORM_ORG](
	[form_id] [nvarchar](20) NOT NULL,
	[org_id] [nvarchar](20) NOT NULL,
	[crt_date] [datetime] NOT NULL,
	[crt_user] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_MAP_EFORM_ORG] PRIMARY KEY CLUSTERED 
(
	[form_id] ASC,
	[org_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MAP_FLOW_ORG]    Script Date: 2019/8/20 下午 03:21:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MAP_FLOW_ORG](
	[flow_id] [nvarchar](20) NOT NULL,
	[org_id] [nvarchar](20) NOT NULL,
	[crt_date] [datetime] NOT NULL,
	[crt_user] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_MAP_FLOW_ORG] PRIMARY KEY CLUSTERED 
(
	[flow_id] ASC,
	[org_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SET_PARAM]    Script Date: 2019/8/20 下午 03:21:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SET_PARAM](
	[set_item] [nvarchar](20) NOT NULL,
	[set_type] [nvarchar](20) NOT NULL,
	[set_value] [nvarchar](50) NOT NULL,
	[memo] [nvarchar](500) NULL,
	[del_flg] [bit] NOT NULL,
	[crt_date] [datetime] NOT NULL,
	[crt_user] [nvarchar](50) NOT NULL,
	[mdf_date] [datetime] NOT NULL,
	[mdf_user] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_SET_PARAMS] PRIMARY KEY CLUSTERED 
(
	[set_item] ASC,
	[set_type] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SET_PARAMITEM]    Script Date: 2019/8/20 下午 03:21:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SET_PARAMITEM](
	[set_item] [nvarchar](20) NOT NULL,
	[set_item_name] [nvarchar](50) NOT NULL,
	[memo] [nvarchar](500) NOT NULL,
	[editable] [bit] NOT NULL,
	[del_flg] [bit] NOT NULL,
	[crt_date] [datetime] NOT NULL,
	[crt_user] [nvarchar](50) NOT NULL,
	[mdf_date] [datetime] NOT NULL,
	[mdf_user] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_SET_PARAMITEM] PRIMARY KEY CLUSTERED 
(
	[set_item] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SQL_TRACE]    Script Date: 2019/8/20 下午 03:21:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SQL_TRACE](
	[sid] [bigint] IDENTITY(1,1) NOT NULL,
	[user_id] [nvarchar](50) NOT NULL,
	[user_ip] [nvarchar](50) NOT NULL,
	[commandtext] [nvarchar](max) NOT NULL,
	[parameters] [nvarchar](max) NOT NULL,
	[request_url] [nvarchar](max) NOT NULL,
	[log_date] [datetime] NOT NULL,
 CONSTRAINT [PK_SQL_TRACE] PRIMARY KEY CLUSTERED 
(
	[sid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 95) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
INSERT [dbo].[DIM_RIGHT] ([right_id], [right_name], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'AgentRight', N'代理人設定權利', 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[DIM_RIGHT] ([right_id], [right_name], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'AnnounceRight', N'公告模組使用權利', 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[DIM_RIGHT] ([right_id], [right_name], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'DemoRight', N'展示使用權利', 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2019-04-03T14:56:08.440' AS DateTime), N'crystal_chiang')
INSERT [dbo].[DIM_RIGHT] ([right_id], [right_name], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'FlowRight', N'流程使用權利', 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[DIM_RIGHT] ([right_id], [right_name], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'FlowSysRight', N'流程管理權利', 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[DIM_RIGHT] ([right_id], [right_name], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'FormRight', N'表單使用權利', 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[DIM_RIGHT] ([right_id], [right_name], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'FormSysRight', N'表單管理權利', 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[DIM_RIGHT] ([right_id], [right_name], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'MailRight', N'郵寄使用權限', 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[DIM_RIGHT] ([right_id], [right_name], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'MailSysRight', N'郵寄管理權利', 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-09-04T22:58:22.490' AS DateTime), N'crystal_chiang')
INSERT [dbo].[DIM_RIGHT] ([right_id], [right_name], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'SysopExercise', N'系統練習權利', 0, CAST(N'2018-09-05T11:22:07.303' AS DateTime), N'crystal_chiang', CAST(N'2019-05-27T10:21:04.303' AS DateTime), N'zoe_yu')
INSERT [dbo].[DIM_RIGHT] ([right_id], [right_name], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'SysopRight', N'系統管理權利', 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2019-07-30T10:38:14.133' AS DateTime), N'kelly_kung')
INSERT [dbo].[DIM_ROLE] ([role_id], [role_name], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'AAA', N'DDD', 1, CAST(N'2019-08-07T15:36:10.610' AS DateTime), N'james_chan', CAST(N'2019-08-07T15:37:44.360' AS DateTime), N'james_chan')
INSERT [dbo].[DIM_ROLE] ([role_id], [role_name], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'FlowRole', N'流程使用者', 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-09-14T11:22:47.623' AS DateTime), N'awei_chu')
INSERT [dbo].[DIM_ROLE] ([role_id], [role_name], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'FlowSysRole', N'流程管理者', 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[DIM_ROLE] ([role_id], [role_name], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'MailRole', N'郵寄使用者', 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[DIM_ROLE] ([role_id], [role_name], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'MailSysRole', N'郵寄管理者', 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[DIM_ROLE] ([role_id], [role_name], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'SysopExerciseRole', N'系統練習角色', 0, CAST(N'2018-09-05T11:22:32.490' AS DateTime), N'crystal_chiang', CAST(N'2018-12-10T18:17:35.730' AS DateTime), N'nick_kao')
INSERT [dbo].[DIM_ROLE] ([role_id], [role_name], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'SysopRole', N'系統管理者', 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[DIM_ROLE] ([role_id], [role_name], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'UserRole', N'一般使用者', 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2019-08-01T11:17:03.407' AS DateTime), N'kelly_kung')
INSERT [dbo].[EMP_USER] ([user_id], [user_name], [user_tel], [user_email], [user_pwd], [user_token], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'SYSOP', N'系統管理員', N'', N'', N'1234', N'', 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2019-08-20T15:18:03.337' AS DateTime), N'SYSOP')
INSERT [dbo].[MAP_ORG_USER] ([org_id], [user_id], [crt_date], [crt_user]) VALUES (N'AOSD1', N'addison_tsai', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[MAP_ORG_USER] ([org_id], [user_id], [crt_date], [crt_user]) VALUES (N'AOSD1', N'christopher_chen', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[MAP_ORG_USER] ([org_id], [user_id], [crt_date], [crt_user]) VALUES (N'AOSD1', N'cl_su', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[MAP_ORG_USER] ([org_id], [user_id], [crt_date], [crt_user]) VALUES (N'DC1', N'kelly_chao', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[MAP_ORG_USER] ([org_id], [user_id], [crt_date], [crt_user]) VALUES (N'DC1', N'steven_tsai', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[MAP_ORG_USER] ([org_id], [user_id], [crt_date], [crt_user]) VALUES (N'GPSD', N'jason_jr_huang', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[MAP_ORG_USER] ([org_id], [user_id], [crt_date], [crt_user]) VALUES (N'GPSD', N'julins_lin', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[MAP_ORG_USER] ([org_id], [user_id], [crt_date], [crt_user]) VALUES (N'GPSD', N'rick_chuang', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[MAP_ORG_USER] ([org_id], [user_id], [crt_date], [crt_user]) VALUES (N'GPSD', N'will_lee', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[MAP_ORG_USER] ([org_id], [user_id], [crt_date], [crt_user]) VALUES (N'GPSD', N'yy_tang', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[MAP_ORG_USER] ([org_id], [user_id], [crt_date], [crt_user]) VALUES (N'GSS', N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[MAP_ORG_USER] ([org_id], [user_id], [crt_date], [crt_user]) VALUES (N'LSBU2', N'mandy_wu', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[MAP_ORG_USER] ([org_id], [user_id], [crt_date], [crt_user]) VALUES (N'LSBU3', N'wayne_tien', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[MAP_ORG_USER] ([org_id], [user_id], [crt_date], [crt_user]) VALUES (N'MACSD', N'kelly_kung', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[MAP_ORG_USER] ([org_id], [user_id], [crt_date], [crt_user]) VALUES (N'MACSD', N'nancy_hsu', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[MAP_ORG_USER] ([org_id], [user_id], [crt_date], [crt_user]) VALUES (N'MACSD', N'neris_shih', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[MAP_ORG_USER] ([org_id], [user_id], [crt_date], [crt_user]) VALUES (N'MACSD', N'paul_chuang', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[MAP_ORG_USER] ([org_id], [user_id], [crt_date], [crt_user]) VALUES (N'PMFDC', N'jedi_liao', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[MAP_ORG_USER] ([org_id], [user_id], [crt_date], [crt_user]) VALUES (N'PMFDC', N'jessie_hu', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[MAP_ORG_USER] ([org_id], [user_id], [crt_date], [crt_user]) VALUES (N'PMFDC', N'michelle_mh_hsieh', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[MAP_ORG_USER] ([org_id], [user_id], [crt_date], [crt_user]) VALUES (N'PMFDC', N'waz_lin', CAST(N'2018-06-05T18:06:10.513' AS DateTime), N'SYSOP')
INSERT [dbo].[MAP_ORG_USER] ([org_id], [user_id], [crt_date], [crt_user]) VALUES (N'SDOBG', N'chris_yu', CAST(N'2018-09-21T15:59:15.313' AS DateTime), N'crystal_chiang')
INSERT [dbo].[MAP_ORG_USER] ([org_id], [user_id], [crt_date], [crt_user]) VALUES (N'SDSD2', N'danna_lin', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[MAP_ORG_USER] ([org_id], [user_id], [crt_date], [crt_user]) VALUES (N'SDSD2', N'makoto_chang', CAST(N'2018-07-05T16:55:00.440' AS DateTime), N'SYSOP')
INSERT [dbo].[MAP_ORG_USER] ([org_id], [user_id], [crt_date], [crt_user]) VALUES (N'SDSD2', N'mifee_huang', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[MAP_ORG_USER] ([org_id], [user_id], [crt_date], [crt_user]) VALUES (N'SDSD2', N'randylu', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[MAP_ORG_USER] ([org_id], [user_id], [crt_date], [crt_user]) VALUES (N'SEPSD', N'bobby_yao', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[MAP_ORG_USER] ([org_id], [user_id], [crt_date], [crt_user]) VALUES (N'測試組織', N'crystal_chiang', CAST(N'2018-11-29T11:41:56.800' AS DateTime), N'walker_lu')
INSERT [dbo].[MAP_ORG_USER] ([org_id], [user_id], [crt_date], [crt_user]) VALUES (N'測試組織', N'jessifer_nien', CAST(N'2018-11-29T11:41:56.813' AS DateTime), N'walker_lu')
INSERT [dbo].[MAP_ORG_USER] ([org_id], [user_id], [crt_date], [crt_user]) VALUES (N'測試組織', N'walker_lu', CAST(N'2018-11-29T11:41:56.847' AS DateTime), N'walker_lu')
INSERT [dbo].[MAP_RIGHT_FUNCTION] ([right_id], [function_id], [crt_date], [crt_user]) VALUES (N'AgentRight', N'Sysop_5', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[MAP_RIGHT_FUNCTION] ([right_id], [function_id], [crt_date], [crt_user]) VALUES (N'AnnounceRight', N'Announce_1', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[MAP_RIGHT_FUNCTION] ([right_id], [function_id], [crt_date], [crt_user]) VALUES (N'DemoRight', N'Sample_1', CAST(N'2019-04-03T14:56:08.457' AS DateTime), N'crystal_chiang')
INSERT [dbo].[MAP_RIGHT_FUNCTION] ([right_id], [function_id], [crt_date], [crt_user]) VALUES (N'DemoRight', N'Sample_10', CAST(N'2019-04-03T14:56:08.473' AS DateTime), N'crystal_chiang')
INSERT [dbo].[MAP_RIGHT_FUNCTION] ([right_id], [function_id], [crt_date], [crt_user]) VALUES (N'DemoRight', N'Sample_11', CAST(N'2019-04-03T14:56:08.487' AS DateTime), N'crystal_chiang')
INSERT [dbo].[MAP_RIGHT_FUNCTION] ([right_id], [function_id], [crt_date], [crt_user]) VALUES (N'DemoRight', N'Sample_12', CAST(N'2019-04-03T14:56:08.503' AS DateTime), N'crystal_chiang')
INSERT [dbo].[MAP_RIGHT_FUNCTION] ([right_id], [function_id], [crt_date], [crt_user]) VALUES (N'DemoRight', N'Sample_13', CAST(N'2019-04-03T14:56:08.800' AS DateTime), N'crystal_chiang')
INSERT [dbo].[MAP_RIGHT_FUNCTION] ([right_id], [function_id], [crt_date], [crt_user]) VALUES (N'DemoRight', N'Sample_2', CAST(N'2019-04-03T14:56:08.503' AS DateTime), N'crystal_chiang')
INSERT [dbo].[MAP_RIGHT_FUNCTION] ([right_id], [function_id], [crt_date], [crt_user]) VALUES (N'DemoRight', N'Sample_3', CAST(N'2019-04-03T14:56:08.520' AS DateTime), N'crystal_chiang')
INSERT [dbo].[MAP_RIGHT_FUNCTION] ([right_id], [function_id], [crt_date], [crt_user]) VALUES (N'DemoRight', N'Sample_4', CAST(N'2019-04-03T14:56:08.533' AS DateTime), N'crystal_chiang')
INSERT [dbo].[MAP_RIGHT_FUNCTION] ([right_id], [function_id], [crt_date], [crt_user]) VALUES (N'DemoRight', N'Sample_5', CAST(N'2019-04-03T14:56:08.550' AS DateTime), N'crystal_chiang')
INSERT [dbo].[MAP_RIGHT_FUNCTION] ([right_id], [function_id], [crt_date], [crt_user]) VALUES (N'DemoRight', N'Sample_6', CAST(N'2019-04-03T14:56:08.550' AS DateTime), N'crystal_chiang')
INSERT [dbo].[MAP_RIGHT_FUNCTION] ([right_id], [function_id], [crt_date], [crt_user]) VALUES (N'DemoRight', N'Sample_7', CAST(N'2019-04-03T14:56:08.597' AS DateTime), N'crystal_chiang')
INSERT [dbo].[MAP_RIGHT_FUNCTION] ([right_id], [function_id], [crt_date], [crt_user]) VALUES (N'DemoRight', N'Sample_8', CAST(N'2019-04-03T14:56:08.597' AS DateTime), N'crystal_chiang')
INSERT [dbo].[MAP_RIGHT_FUNCTION] ([right_id], [function_id], [crt_date], [crt_user]) VALUES (N'DemoRight', N'Sample_9', CAST(N'2019-04-03T14:56:08.613' AS DateTime), N'crystal_chiang')
INSERT [dbo].[MAP_RIGHT_FUNCTION] ([right_id], [function_id], [crt_date], [crt_user]) VALUES (N'DemoRight', N'Spec_1', CAST(N'2019-04-03T14:56:08.677' AS DateTime), N'crystal_chiang')
INSERT [dbo].[MAP_RIGHT_FUNCTION] ([right_id], [function_id], [crt_date], [crt_user]) VALUES (N'DemoRight', N'Spec_2', CAST(N'2019-04-03T14:56:08.723' AS DateTime), N'crystal_chiang')
INSERT [dbo].[MAP_RIGHT_FUNCTION] ([right_id], [function_id], [crt_date], [crt_user]) VALUES (N'DemoRight', N'Spec_3', CAST(N'2019-04-03T14:56:08.737' AS DateTime), N'crystal_chiang')
INSERT [dbo].[MAP_RIGHT_FUNCTION] ([right_id], [function_id], [crt_date], [crt_user]) VALUES (N'DemoRight', N'Spec_4', CAST(N'2019-04-03T14:56:08.753' AS DateTime), N'crystal_chiang')
INSERT [dbo].[MAP_RIGHT_FUNCTION] ([right_id], [function_id], [crt_date], [crt_user]) VALUES (N'DemoRight', N'Spec_5', CAST(N'2019-04-03T14:56:08.770' AS DateTime), N'crystal_chiang')
INSERT [dbo].[MAP_RIGHT_FUNCTION] ([right_id], [function_id], [crt_date], [crt_user]) VALUES (N'DemoRight', N'Spec_6', CAST(N'2019-04-03T14:56:08.783' AS DateTime), N'crystal_chiang')
INSERT [dbo].[MAP_RIGHT_FUNCTION] ([right_id], [function_id], [crt_date], [crt_user]) VALUES (N'FlowRight', N'FlowModule_1', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[MAP_RIGHT_FUNCTION] ([right_id], [function_id], [crt_date], [crt_user]) VALUES (N'FlowRight', N'Sample_7', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[MAP_RIGHT_FUNCTION] ([right_id], [function_id], [crt_date], [crt_user]) VALUES (N'FlowSysRight', N'FlowModule_2', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[MAP_RIGHT_FUNCTION] ([right_id], [function_id], [crt_date], [crt_user]) VALUES (N'FlowSysRight', N'FlowModule_3', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[MAP_RIGHT_FUNCTION] ([right_id], [function_id], [crt_date], [crt_user]) VALUES (N'FormRight', N'eFormModule_2', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[MAP_RIGHT_FUNCTION] ([right_id], [function_id], [crt_date], [crt_user]) VALUES (N'FormRight', N'eFormModule_3', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[MAP_RIGHT_FUNCTION] ([right_id], [function_id], [crt_date], [crt_user]) VALUES (N'FormSysRight', N'eFormModule_1', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[MAP_RIGHT_FUNCTION] ([right_id], [function_id], [crt_date], [crt_user]) VALUES (N'MailRight', N'MailModule_1', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[MAP_RIGHT_FUNCTION] ([right_id], [function_id], [crt_date], [crt_user]) VALUES (N'MailSysRight', N'MailModule_2', CAST(N'2018-09-04T22:58:22.490' AS DateTime), N'crystal_chiang')
INSERT [dbo].[MAP_RIGHT_FUNCTION] ([right_id], [function_id], [crt_date], [crt_user]) VALUES (N'MailSysRight', N'MailModule_3', CAST(N'2018-09-04T22:58:22.490' AS DateTime), N'crystal_chiang')
INSERT [dbo].[MAP_RIGHT_FUNCTION] ([right_id], [function_id], [crt_date], [crt_user]) VALUES (N'SysopExercise', N'CTBCReport', CAST(N'2019-05-27T10:21:04.320' AS DateTime), N'zoe_yu')
INSERT [dbo].[MAP_RIGHT_FUNCTION] ([right_id], [function_id], [crt_date], [crt_user]) VALUES (N'SysopExercise', N'Query', CAST(N'2019-05-27T10:21:04.320' AS DateTime), N'zoe_yu')
INSERT [dbo].[MAP_RIGHT_FUNCTION] ([right_id], [function_id], [crt_date], [crt_user]) VALUES (N'SysopExercise', N'Query_Zoe', CAST(N'2019-05-27T10:21:04.320' AS DateTime), N'zoe_yu')
INSERT [dbo].[MAP_RIGHT_FUNCTION] ([right_id], [function_id], [crt_date], [crt_user]) VALUES (N'SysopRight', N'f1', CAST(N'2019-07-30T10:38:14.150' AS DateTime), N'kelly_kung')
INSERT [dbo].[MAP_RIGHT_FUNCTION] ([right_id], [function_id], [crt_date], [crt_user]) VALUES (N'SysopRight', N'Sysop_1', CAST(N'2019-07-30T10:38:14.150' AS DateTime), N'kelly_kung')
INSERT [dbo].[MAP_RIGHT_FUNCTION] ([right_id], [function_id], [crt_date], [crt_user]) VALUES (N'SysopRight', N'Sysop_2', CAST(N'2019-07-30T10:38:14.150' AS DateTime), N'kelly_kung')
INSERT [dbo].[MAP_RIGHT_FUNCTION] ([right_id], [function_id], [crt_date], [crt_user]) VALUES (N'SysopRight', N'Sysop_3', CAST(N'2019-07-30T10:38:14.150' AS DateTime), N'kelly_kung')
INSERT [dbo].[MAP_RIGHT_FUNCTION] ([right_id], [function_id], [crt_date], [crt_user]) VALUES (N'SysopRight', N'Sysop_4', CAST(N'2019-07-30T10:38:14.150' AS DateTime), N'kelly_kung')
INSERT [dbo].[MAP_RIGHT_FUNCTION] ([right_id], [function_id], [crt_date], [crt_user]) VALUES (N'SysopRight', N'Sysop_6', CAST(N'2019-07-30T10:38:14.150' AS DateTime), N'kelly_kung')
INSERT [dbo].[MAP_RIGHT_FUNCTION] ([right_id], [function_id], [crt_date], [crt_user]) VALUES (N'SysopRight', N'Sysop_7', CAST(N'2019-07-30T10:38:14.150' AS DateTime), N'kelly_kung')
INSERT [dbo].[MAP_RIGHT_FUNCTION] ([right_id], [function_id], [crt_date], [crt_user]) VALUES (N'SysopRight', N'Sysop_8', CAST(N'2019-07-30T10:38:14.150' AS DateTime), N'kelly_kung')
INSERT [dbo].[MAP_ROLE_RIGHT] ([role_id], [right_id], [crt_date], [crt_user]) VALUES (N'FlowRole', N'FlowRight', CAST(N'2018-09-14T11:22:47.653' AS DateTime), N'awei_chu')
INSERT [dbo].[MAP_ROLE_RIGHT] ([role_id], [right_id], [crt_date], [crt_user]) VALUES (N'FlowRole', N'FormRight', CAST(N'2018-09-14T11:22:47.670' AS DateTime), N'awei_chu')
INSERT [dbo].[MAP_ROLE_RIGHT] ([role_id], [right_id], [crt_date], [crt_user]) VALUES (N'FlowSysRole', N'FlowSysRight', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[MAP_ROLE_RIGHT] ([role_id], [right_id], [crt_date], [crt_user]) VALUES (N'FlowSysRole', N'FormSysRight', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[MAP_ROLE_RIGHT] ([role_id], [right_id], [crt_date], [crt_user]) VALUES (N'MailRole', N'MailRight', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[MAP_ROLE_RIGHT] ([role_id], [right_id], [crt_date], [crt_user]) VALUES (N'MailSysRole', N'MailSysRight', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[MAP_ROLE_RIGHT] ([role_id], [right_id], [crt_date], [crt_user]) VALUES (N'SysopExerciseRole', N'DemoRight', CAST(N'2018-12-10T18:17:35.743' AS DateTime), N'nick_kao')
INSERT [dbo].[MAP_ROLE_RIGHT] ([role_id], [right_id], [crt_date], [crt_user]) VALUES (N'SysopExerciseRole', N'Nick_kao', CAST(N'2018-12-10T18:17:35.743' AS DateTime), N'nick_kao')
INSERT [dbo].[MAP_ROLE_RIGHT] ([role_id], [right_id], [crt_date], [crt_user]) VALUES (N'SysopExerciseRole', N'SysopExercise', CAST(N'2018-12-10T18:17:35.743' AS DateTime), N'nick_kao')
INSERT [dbo].[MAP_ROLE_RIGHT] ([role_id], [right_id], [crt_date], [crt_user]) VALUES (N'SysopRole', N'AnnounceRight', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[MAP_ROLE_RIGHT] ([role_id], [right_id], [crt_date], [crt_user]) VALUES (N'SysopRole', N'SysopRight', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[MAP_ROLE_RIGHT] ([role_id], [right_id], [crt_date], [crt_user]) VALUES (N'UserRole', N'DemoRight', CAST(N'2019-08-01T11:17:03.407' AS DateTime), N'kelly_kung')
INSERT [dbo].[MAP_ROLE_RIGHT] ([role_id], [right_id], [crt_date], [crt_user]) VALUES (N'UserRole', N'SysopRight', CAST(N'2019-08-01T11:17:03.423' AS DateTime), N'kelly_kung')
INSERT [dbo].[MAP_USER_ROLE] ([user_id], [role_id], [crt_date], [crt_user]) VALUES (N'SYSOP', N'FlowRole', CAST(N'2019-08-20T15:18:03.343' AS DateTime), N'SYSOP')
INSERT [dbo].[MAP_USER_ROLE] ([user_id], [role_id], [crt_date], [crt_user]) VALUES (N'SYSOP', N'FlowSysRole', CAST(N'2019-08-20T15:18:03.343' AS DateTime), N'SYSOP')
INSERT [dbo].[MAP_USER_ROLE] ([user_id], [role_id], [crt_date], [crt_user]) VALUES (N'SYSOP', N'MailRole', CAST(N'2019-08-20T15:18:03.343' AS DateTime), N'SYSOP')
INSERT [dbo].[MAP_USER_ROLE] ([user_id], [role_id], [crt_date], [crt_user]) VALUES (N'SYSOP', N'MailSysRole', CAST(N'2019-08-20T15:18:03.343' AS DateTime), N'SYSOP')
INSERT [dbo].[MAP_USER_ROLE] ([user_id], [role_id], [crt_date], [crt_user]) VALUES (N'SYSOP', N'SysopExerciseRole', CAST(N'2019-08-20T15:18:03.343' AS DateTime), N'SYSOP')
INSERT [dbo].[MAP_USER_ROLE] ([user_id], [role_id], [crt_date], [crt_user]) VALUES (N'SYSOP', N'SysopRole', CAST(N'2019-08-20T15:18:03.343' AS DateTime), N'SYSOP')
INSERT [dbo].[MAP_USER_ROLE] ([user_id], [role_id], [crt_date], [crt_user]) VALUES (N'SYSOP', N'UserRole', CAST(N'2019-08-20T15:18:03.343' AS DateTime), N'SYSOP')
INSERT [dbo].[SET_FUNCTION] ([function_id], [function_name], [function_url], [parent_id], [sort_id], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'Announce', N'公告模組', N'', N'', 2, 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2019-08-16T16:03:22.837' AS DateTime), N'rick_chuang')
INSERT [dbo].[SET_FUNCTION] ([function_id], [function_name], [function_url], [parent_id], [sort_id], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'Announce_1', N'公告填寫', N'~/Announcement/Query', N'Announce', 1, 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[SET_FUNCTION] ([function_id], [function_name], [function_url], [parent_id], [sort_id], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'CTBCReport', N'中信功能測試', N'~/CTBCReport/Query', N'SysopExercise', 0, 1, CAST(N'2019-03-04T11:39:44.910' AS DateTime), N'crystal_chiang', CAST(N'2019-08-20T15:18:50.067' AS DateTime), N'SYSOP')
INSERT [dbo].[SET_FUNCTION] ([function_id], [function_name], [function_url], [parent_id], [sort_id], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'eFormModule', N'表單模組', N'', N'', 4, 1, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2019-08-20T15:19:49.300' AS DateTime), N'SYSOP')
INSERT [dbo].[SET_FUNCTION] ([function_id], [function_name], [function_url], [parent_id], [sort_id], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'eFormModule_1', N'表單設定', N'~/eFormSet/Query', N'eFormModule', 1, 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[SET_FUNCTION] ([function_id], [function_name], [function_url], [parent_id], [sort_id], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'eFormModule_2', N'表單填寫', N'~/eForm/Query', N'eFormModule', 2, 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[SET_FUNCTION] ([function_id], [function_name], [function_url], [parent_id], [sort_id], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'eFormModule_3', N'表單簽核', N'~/eForm/QueryFlow', N'eFormModule', 3, 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[SET_FUNCTION] ([function_id], [function_name], [function_url], [parent_id], [sort_id], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'f1', N'tetet', N'', N'test', 0, 0, CAST(N'2019-07-30T10:37:31.133' AS DateTime), N'kelly_kung', CAST(N'2019-07-30T10:37:31.133' AS DateTime), N'kelly_kung')
INSERT [dbo].[SET_FUNCTION] ([function_id], [function_name], [function_url], [parent_id], [sort_id], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'FlowModule', N'流程模組', N'', N'', 5, 1, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2019-08-20T15:19:52.563' AS DateTime), N'SYSOP')
INSERT [dbo].[SET_FUNCTION] ([function_id], [function_name], [function_url], [parent_id], [sort_id], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'FlowModule_1', N'流程日誌', N'~/ActiveFlow/Query', N'FlowModule', 1, 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[SET_FUNCTION] ([function_id], [function_name], [function_url], [parent_id], [sort_id], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'FlowModule_2', N'流程角色', N'~/FlowRole/Query', N'FlowModule', 2, 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[SET_FUNCTION] ([function_id], [function_name], [function_url], [parent_id], [sort_id], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'FlowModule_3', N'流程設定', N'~/FlowSet/Query', N'FlowModule', 3, 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[SET_FUNCTION] ([function_id], [function_name], [function_url], [parent_id], [sort_id], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'MailModule', N'郵寄模組', N'', N'', 3, 1, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2019-08-20T15:19:57.197' AS DateTime), N'SYSOP')
INSERT [dbo].[SET_FUNCTION] ([function_id], [function_name], [function_url], [parent_id], [sort_id], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'MailModule_1', N'郵寄日誌', N'~/MailLog/Query', N'MailModule', 1, 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-07-13T15:50:27.613' AS DateTime), N'waz_lin')
INSERT [dbo].[SET_FUNCTION] ([function_id], [function_name], [function_url], [parent_id], [sort_id], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'MailModule_2', N'郵寄角色', N'~/MailRole/Query', N'MailModule', 2, 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-07-13T15:50:27.613' AS DateTime), N'waz_lin')
INSERT [dbo].[SET_FUNCTION] ([function_id], [function_name], [function_url], [parent_id], [sort_id], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'MailModule_3', N'郵件範本', N'~/MailSet/Query', N'MailModule', 3, 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-07-13T15:50:27.613' AS DateTime), N'waz_lin')
INSERT [dbo].[SET_FUNCTION] ([function_id], [function_name], [function_url], [parent_id], [sort_id], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'Query', N'Query_Steve', N'~/EmpUserForSteve/Query', N'SysopExercise', 0, 1, CAST(N'2018-12-10T18:11:05.727' AS DateTime), N'nick_kao', CAST(N'2019-08-20T15:18:52.530' AS DateTime), N'SYSOP')
INSERT [dbo].[SET_FUNCTION] ([function_id], [function_name], [function_url], [parent_id], [sort_id], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'Query_Andy', N'Query_Andy', N'~/EmpUserForAndy/Query', N'SysopExercise', 1, 1, CAST(N'2019-02-11T14:03:41.140' AS DateTime), N'andy_ja_lee', CAST(N'2019-03-04T11:40:16.113' AS DateTime), N'crystal_chiang')
INSERT [dbo].[SET_FUNCTION] ([function_id], [function_name], [function_url], [parent_id], [sort_id], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'Query_Arni', N'Query_Arni', N'~/Query/Query', N'SysopExercise', 2, 1, CAST(N'2018-11-29T10:55:06.183' AS DateTime), N'arni_lin', CAST(N'2019-03-04T11:40:18.647' AS DateTime), N'crystal_chiang')
INSERT [dbo].[SET_FUNCTION] ([function_id], [function_name], [function_url], [parent_id], [sort_id], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'Query_Bruce', N'Query_Bruce', N'~/EmpUserForBruce/Query', N'SysopExercise', 3, 1, CAST(N'2018-12-12T11:57:17.950' AS DateTime), N'bruce_lo', CAST(N'2019-03-04T11:40:20.710' AS DateTime), N'crystal_chiang')
INSERT [dbo].[SET_FUNCTION] ([function_id], [function_name], [function_url], [parent_id], [sort_id], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'Query_Mustang', N'Query_Mustang', N'~/EmpUserForMustang/Query', N'SysopExercise', 0, 1, CAST(N'2019-02-19T17:30:13.890' AS DateTime), N'mustang_lai', CAST(N'2019-03-04T11:40:13.210' AS DateTime), N'crystal_chiang')
INSERT [dbo].[SET_FUNCTION] ([function_id], [function_name], [function_url], [parent_id], [sort_id], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'Query_Nick', N'Query_Nick', N'~/EmpUserForNick/Query', N'SysopExercise', 4, 1, CAST(N'2018-12-11T10:01:14.317' AS DateTime), N'nick_kao', CAST(N'2019-03-04T11:40:22.757' AS DateTime), N'crystal_chiang')
INSERT [dbo].[SET_FUNCTION] ([function_id], [function_name], [function_url], [parent_id], [sort_id], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'Query_Patrick', N'Query_Patrick', N'~/EmpUserForPatrick/Query', N'SysopExercise', 0, 1, CAST(N'2018-12-10T17:33:05.990' AS DateTime), N'patrick_lu', CAST(N'2018-12-12T11:58:25.607' AS DateTime), N'bruce_lo')
INSERT [dbo].[SET_FUNCTION] ([function_id], [function_name], [function_url], [parent_id], [sort_id], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'Query_Zoe', N'Query_Zoe', N'~/EmpUserForZoe/Query', N'SysopExercise', 0, 1, CAST(N'2019-05-27T10:20:13.850' AS DateTime), N'zoe_yu', CAST(N'2019-08-20T15:18:55.913' AS DateTime), N'SYSOP')
INSERT [dbo].[SET_FUNCTION] ([function_id], [function_name], [function_url], [parent_id], [sort_id], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'QueryForBruce', N'查詢(Bruce)', N'~/EmpUserForBruce/Query', N'SysopExercise', 0, 1, CAST(N'2018-12-10T17:56:10.930' AS DateTime), N'bruce_lo', CAST(N'2018-12-12T11:58:21.293' AS DateTime), N'bruce_lo')
INSERT [dbo].[SET_FUNCTION] ([function_id], [function_name], [function_url], [parent_id], [sort_id], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'Sample', N'實作範例', N'', N'', 7, 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2019-08-16T16:03:22.837' AS DateTime), N'rick_chuang')
INSERT [dbo].[SET_FUNCTION] ([function_id], [function_name], [function_url], [parent_id], [sort_id], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'Sample_1', N'CSS', N'~/Demo/SampleCSS', N'Sample', 1, 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2019-04-03T14:55:42.677' AS DateTime), N'crystal_chiang')
INSERT [dbo].[SET_FUNCTION] ([function_id], [function_name], [function_url], [parent_id], [sort_id], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'Sample_10', N'條碼讀取', N'~/Demo/SampleBarcode', N'Sample', 10, 0, CAST(N'2019-03-13T15:59:32.517' AS DateTime), N'crystal_chiang', CAST(N'2019-04-03T14:55:42.783' AS DateTime), N'crystal_chiang')
INSERT [dbo].[SET_FUNCTION] ([function_id], [function_name], [function_url], [parent_id], [sort_id], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'Sample_11', N'公文編輯器', N'~/OfficialDoc/DemoOfficialDoc', N'Sample', 11, 0, CAST(N'2019-03-28T11:06:26.517' AS DateTime), N'crystal_chiang', CAST(N'2019-04-03T14:55:42.800' AS DateTime), N'crystal_chiang')
INSERT [dbo].[SET_FUNCTION] ([function_id], [function_name], [function_url], [parent_id], [sort_id], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'Sample_12', N'Word報表套版範例', N'~/ExportWordReport/ExportWordReport', N'Sample', 12, 0, CAST(N'2019-04-02T17:11:12.387' AS DateTime), N'crystal_chiang', CAST(N'2019-04-03T14:55:42.800' AS DateTime), N'crystal_chiang')
INSERT [dbo].[SET_FUNCTION] ([function_id], [function_name], [function_url], [parent_id], [sort_id], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'Sample_13', N'Excel報表套版範例', N'~/ExportExcelReport/ExportExcelReport', N'Sample', 13, 0, CAST(N'2019-04-03T14:54:22.470' AS DateTime), N'crystal_chiang', CAST(N'2019-04-03T14:55:42.817' AS DateTime), N'crystal_chiang')
INSERT [dbo].[SET_FUNCTION] ([function_id], [function_name], [function_url], [parent_id], [sort_id], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'Sample_2', N'Javascript', N'~/Demo/SampleJS', N'Sample', 2, 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2019-04-03T14:55:42.690' AS DateTime), N'crystal_chiang')
INSERT [dbo].[SET_FUNCTION] ([function_id], [function_name], [function_url], [parent_id], [sort_id], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'Sample_3', N'自訂控制項', N'~/Demo/SampleUsercontrol', N'Sample', 3, 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2019-04-03T14:55:42.707' AS DateTime), N'crystal_chiang')
INSERT [dbo].[SET_FUNCTION] ([function_id], [function_name], [function_url], [parent_id], [sort_id], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'Sample_4', N'Grid', N'~/Demo/SampleGrid', N'Sample', 4, 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2019-04-03T14:55:42.723' AS DateTime), N'crystal_chiang')
INSERT [dbo].[SET_FUNCTION] ([function_id], [function_name], [function_url], [parent_id], [sort_id], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'Sample_5', N'DAC', N'~/Demo/SampleDAC', N'Sample', 5, 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2019-04-03T14:55:42.723' AS DateTime), N'crystal_chiang')
INSERT [dbo].[SET_FUNCTION] ([function_id], [function_name], [function_url], [parent_id], [sort_id], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'Sample_6', N'App_Code', N'~/Demo/SampleAppCode', N'Sample', 6, 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2019-04-03T14:55:42.737' AS DateTime), N'crystal_chiang')
INSERT [dbo].[SET_FUNCTION] ([function_id], [function_name], [function_url], [parent_id], [sort_id], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'Sample_7', N'Flow Api', N'~/Demo/SampleFlowApi', N'Sample', 7, 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2019-04-03T14:55:42.753' AS DateTime), N'crystal_chiang')
INSERT [dbo].[SET_FUNCTION] ([function_id], [function_name], [function_url], [parent_id], [sort_id], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'Sample_8', N'SDO Api', N'~/Demo/SampleApi', N'Sample', 8, 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2019-04-03T14:55:42.753' AS DateTime), N'crystal_chiang')
INSERT [dbo].[SET_FUNCTION] ([function_id], [function_name], [function_url], [parent_id], [sort_id], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'Sample_9', N'eForm', N'~/Demo/SampleEform', N'Sample', 9, 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2019-04-03T14:55:42.770' AS DateTime), N'crystal_chiang')
INSERT [dbo].[SET_FUNCTION] ([function_id], [function_name], [function_url], [parent_id], [sort_id], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'Spec', N'架構規範', N'', N'', 6, 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2019-08-16T16:03:22.837' AS DateTime), N'rick_chuang')
INSERT [dbo].[SET_FUNCTION] ([function_id], [function_name], [function_url], [parent_id], [sort_id], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'Spec_1', N'Web.config', N'~/Spec/AboutWebconfig', N'Spec', 1, 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[SET_FUNCTION] ([function_id], [function_name], [function_url], [parent_id], [sort_id], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'Spec_2', N'Session', N'~/Spec/AboutSession', N'Spec', 2, 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[SET_FUNCTION] ([function_id], [function_name], [function_url], [parent_id], [sort_id], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'Spec_3', N'Log', N'~/Spec/AboutLog', N'Spec', 3, 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[SET_FUNCTION] ([function_id], [function_name], [function_url], [parent_id], [sort_id], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'Spec_4', N'版面', N'~/Spec/AboutLayout', N'Spec', 4, 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[SET_FUNCTION] ([function_id], [function_name], [function_url], [parent_id], [sort_id], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'Spec_5', N'訊息庫', N'~/Spec/AboutMessage', N'Spec', 5, 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[SET_FUNCTION] ([function_id], [function_name], [function_url], [parent_id], [sort_id], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'Spec_6', N'麵包屑', N'~/Spec/AboutSitemap', N'Spec', 6, 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[SET_FUNCTION] ([function_id], [function_name], [function_url], [parent_id], [sort_id], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'Sysop', N'系統管理', N'', N'', 8, 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2019-08-16T16:03:22.837' AS DateTime), N'rick_chuang')
INSERT [dbo].[SET_FUNCTION] ([function_id], [function_name], [function_url], [parent_id], [sort_id], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'Sysop_1', N'使用者', N'~/EmpUser/Query', N'Sysop', 1, 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[SET_FUNCTION] ([function_id], [function_name], [function_url], [parent_id], [sort_id], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'Sysop_2', N'角色', N'~/DimRole/Query', N'Sysop', 2, 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[SET_FUNCTION] ([function_id], [function_name], [function_url], [parent_id], [sort_id], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'Sysop_3', N'權利', N'~/DimRight/Query', N'Sysop', 3, 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[SET_FUNCTION] ([function_id], [function_name], [function_url], [parent_id], [sort_id], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'Sysop_4', N'功能', N'~/SetFunction/Query', N'Sysop', 4, 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[SET_FUNCTION] ([function_id], [function_name], [function_url], [parent_id], [sort_id], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'Sysop_5', N'代理人', N'~/EmpAgent/Query', N'Sysop', 5, 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[SET_FUNCTION] ([function_id], [function_name], [function_url], [parent_id], [sort_id], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'Sysop_6', N'組織', N'~/EmpOrg/Query', N'Sysop', 6, 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[SET_FUNCTION] ([function_id], [function_name], [function_url], [parent_id], [sort_id], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'Sysop_7', N'系統參數', N'~/SetParam/Query', N'Sysop', 7, 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[SET_FUNCTION] ([function_id], [function_name], [function_url], [parent_id], [sort_id], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'Sysop_8', N'SQL日誌', N'~/SqlLog/Query', N'Sysop', 8, 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[SET_FUNCTION] ([function_id], [function_name], [function_url], [parent_id], [sort_id], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'SysopExercise', N'系統練習', N'', N'', 1, 0, CAST(N'2018-09-05T11:21:04.367' AS DateTime), N'crystal_chiang', CAST(N'2019-08-16T16:03:22.837' AS DateTime), N'rick_chuang')
INSERT [dbo].[SET_FUNCTION] ([function_id], [function_name], [function_url], [parent_id], [sort_id], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'test', N'test', N'', N'', 1, 1, CAST(N'2019-07-30T10:36:53.243' AS DateTime), N'kelly_kung', CAST(N'2019-07-30T13:57:03.273' AS DateTime), N'kelly_kung')
INSERT [dbo].[SET_PARAM] ([set_item], [set_type], [set_value], [memo], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'eFieldExtType', N'11', N'多選', N'', 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[SET_PARAM] ([set_item], [set_type], [set_value], [memo], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'eFieldExtType', N'12', N'組織選單', N'{ "PARENT_NODEID": 指定根組織, "MULTISELECT": true || false }', 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[SET_PARAM] ([set_item], [set_type], [set_value], [memo], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'eFieldExtType', N'13', N'人員選單', N'', 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[SET_PARAM] ([set_item], [set_type], [set_value], [memo], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'eFieldExtType', N'14', N'Checkbox選單', N'[{ "Text": "選項", "Value": "數值 || _GROUP", "Disabled": "true || false"  },...]', 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[SET_PARAM] ([set_item], [set_type], [set_value], [memo], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'eFieldExtType', N'15', N'Radio選單', N'[{ "Text": "選項", "Value": "數值 || _GROUP", "Disabled": "true || false"  },...]', 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[SET_PARAM] ([set_item], [set_type], [set_value], [memo], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'eFieldExtType', N'21', N'數值', N'{ "format": n || c || p, "decimals": 小數點進位位置, "min": 最小值, "max": 最大值 }', 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[SET_PARAM] ([set_item], [set_type], [set_value], [memo], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'eFieldExtType', N'22', N'日期', N'{ "DISPLAYTIME": true || false, "CASCADEFROM": 連棟控制項ID }', 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[SET_PARAM] ([set_item], [set_type], [set_value], [memo], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'eFieldExtType', N'23', N'格式化文字', N'{ "mask": 格式化樣本 }', 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[SET_PARAM] ([set_item], [set_type], [set_value], [memo], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'eFieldExtType', N'31', N'簡易編輯文字框', N'', 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[SET_PARAM] ([set_item], [set_type], [set_value], [memo], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'eFieldExtType', N'32', N'完整編輯文字框', N'', 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[SET_PARAM] ([set_item], [set_type], [set_value], [memo], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'eFieldType', N'0', N'純文字顯示', N'', 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[SET_PARAM] ([set_item], [set_type], [set_value], [memo], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'eFieldType', N'1', N'下拉式選單', N'[{ "Text": "選項", "Value": "數值" }, ...]', 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[SET_PARAM] ([set_item], [set_type], [set_value], [memo], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'eFieldType', N'2', N'單行文字框', N'', 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[SET_PARAM] ([set_item], [set_type], [set_value], [memo], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'eFieldType', N'3', N'多行文字框', N'', 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[SET_PARAM] ([set_item], [set_type], [set_value], [memo], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'eFieldType', N'4', N'檔案上傳', N'{"MULTIPLE": true || false }', 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[SET_PARAM] ([set_item], [set_type], [set_value], [memo], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'eFormType', N'A', N'A類型表單', N'', 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[SET_PARAM] ([set_item], [set_type], [set_value], [memo], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'eFormType', N'B', N'B類型表單', N'', 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[SET_PARAM] ([set_item], [set_type], [set_value], [memo], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'eFormType', N'C', N'C類型表單', N'', 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[SET_PARAM] ([set_item], [set_type], [set_value], [memo], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'ESPApi', N'ApiKey', N'704ccdbaa4684bbe9ee5406d180c6e80', N'註冊並啟用的 API Key', 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-11-28T09:46:23.727' AS DateTime), N'walker_lu')
INSERT [dbo].[SET_PARAM] ([set_item], [set_type], [set_value], [memo], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'ESPApi', N'ApiUrl', N'http://172.16.50.12/ESP/api/', N'連結 KM Server Site 的 API 虛擬目錄 Url 路徑', 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[SET_PARAM] ([set_item], [set_type], [set_value], [memo], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'ESPApi', N'Tenant', N'default', N'多承租人的環境時應用', 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[SET_PARAM] ([set_item], [set_type], [set_value], [memo], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'ESPApi', N'UserID', N'admin', N'具有 KM 系統讀寫權限的帳號', 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[SET_PARAM] ([set_item], [set_type], [set_value], [memo], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'FlowStatus', N'0', N'待簽核', N'', 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-09-21T15:56:30.687' AS DateTime), N'crystal_chiang')
INSERT [dbo].[SET_PARAM] ([set_item], [set_type], [set_value], [memo], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'FlowStatus', N'1', N'簽核中', N'', 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[SET_PARAM] ([set_item], [set_type], [set_value], [memo], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'FlowStatus', N'2', N'簽核完成', N'', 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[SET_PARAM] ([set_item], [set_type], [set_value], [memo], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'FlowStatus', N'9', N'結束流程', N'', 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[SET_PARAM] ([set_item], [set_type], [set_value], [memo], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'MailConfig', N'MailServer', N'mail.gss.com.tw', N'郵件伺服器', 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[SET_PARAM] ([set_item], [set_type], [set_value], [memo], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'MailConfig', N'SenderMail', N'jedi_liao@mail.gss.com.tw', N'預設寄件者電子郵件', 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[SET_PARAM] ([set_item], [set_type], [set_value], [memo], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'MailConfig', N'SenderTitle', N'系統管理員', N'預設寄件者稱呼', 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[SET_PARAM] ([set_item], [set_type], [set_value], [memo], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'MailType', N'0', N'寄件者', N'', 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-10-02T13:35:25.940' AS DateTime), N'crystal_chiang')
INSERT [dbo].[SET_PARAM] ([set_item], [set_type], [set_value], [memo], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'MailType', N'1', N'收件者', N'', 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[SET_PARAM] ([set_item], [set_type], [set_value], [memo], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'MailType', N'2', N'副本', N'', 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[SET_PARAM] ([set_item], [set_type], [set_value], [memo], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'MailType', N'3', N'密件副本', N'', 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[SET_PARAM] ([set_item], [set_type], [set_value], [memo], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'SignedStatus', N'1', N'啟動流程', NULL, 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-11-28T09:46:02.570' AS DateTime), N'walker_lu')
INSERT [dbo].[SET_PARAM] ([set_item], [set_type], [set_value], [memo], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'SignedStatus', N'2', N'通過', N'', 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[SET_PARAM] ([set_item], [set_type], [set_value], [memo], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'SignedStatus', N'3', N'退回', N'', 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[SET_PARAM] ([set_item], [set_type], [set_value], [memo], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'SignedStatus', N'4', N'決行', N'', 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[SET_PARAM] ([set_item], [set_type], [set_value], [memo], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'SignedStatus', N'5', N'退回第一關', N'', 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[SET_PARAM] ([set_item], [set_type], [set_value], [memo], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'SignedStatus', N'6', N'分會', N'', 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[SET_PARAM] ([set_item], [set_type], [set_value], [memo], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'SignedStatus', N'9', N'結束流程', N'', 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[SET_PARAM] ([set_item], [set_type], [set_value], [memo], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'SystemConfig', N'ADServer', N'gss.com.tw', N'AD驗證用伺服器', 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[SET_PARAM] ([set_item], [set_type], [set_value], [memo], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'SystemConfig', N'AuthType', N'1', N'1:本地驗證 2:AD驗證 3:晶片卡驗證', 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[SET_PARAM] ([set_item], [set_type], [set_value], [memo], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'SystemConfig', N'ReportFolder', N'~\TemplateFile', N'報表套版範本檔案相對路徑', 0, CAST(N'2019-04-03T10:47:32.677' AS DateTime), N'crystal_chiang', CAST(N'2019-04-03T10:56:47.537' AS DateTime), N'crystal_chiang')
INSERT [dbo].[SET_PARAM] ([set_item], [set_type], [set_value], [memo], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'SystemConfig', N'SqlLogTrace', N'true', N'是否紀錄SQL操作流水帳', 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[SET_PARAM] ([set_item], [set_type], [set_value], [memo], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'SystemConfig', N'TabsDisplay', N'false', N'是否操作多重頁籤', 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-10-11T17:56:05.410' AS DateTime), N'crystal_chiang')
INSERT [dbo].[SET_PARAM] ([set_item], [set_type], [set_value], [memo], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'SystemConfig', N'UploadFolder', N'~\Uploads', N'實體檔案儲存相對路徑', 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[SET_PARAMITEM] ([set_item], [set_item_name], [memo], [editable], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'eFieldExtType', N'欄位延伸類別', N'電子表單欄位延伸定義類別', 0, 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[SET_PARAMITEM] ([set_item], [set_item_name], [memo], [editable], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'eFieldType', N'欄位類別', N'電子表單欄位基本定義類別', 0, 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[SET_PARAMITEM] ([set_item], [set_item_name], [memo], [editable], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'eFormType', N'表單類別', N'電子表單類別', 1, 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2019-05-20T14:21:08.813' AS DateTime), N'crystal_chiang')
INSERT [dbo].[SET_PARAMITEM] ([set_item], [set_item_name], [memo], [editable], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'ESPApi', N'ESP參數', N'Vitals ESP API 設定參數', 0, 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-11-28T09:36:24.850' AS DateTime), N'walker_lu')
INSERT [dbo].[SET_PARAMITEM] ([set_item], [set_item_name], [memo], [editable], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'FlowStatus', N'流程狀態', N'流程模組執行狀態', 1, 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[SET_PARAMITEM] ([set_item], [set_item_name], [memo], [editable], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'MailConfig', N'郵寄設定', N'郵件模組使用參數', 1, 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[SET_PARAMITEM] ([set_item], [set_item_name], [memo], [editable], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'MailType', N'郵寄類別', N'郵寄對象類別', 1, 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[SET_PARAMITEM] ([set_item], [set_item_name], [memo], [editable], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'SignedStatus', N'簽核狀態', N'流程模組簽核狀態', 1, 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
INSERT [dbo].[SET_PARAMITEM] ([set_item], [set_item_name], [memo], [editable], [del_flg], [crt_date], [crt_user], [mdf_date], [mdf_user]) VALUES (N'SystemConfig', N'系統設定', N'系統環境使用參數', 0, 0, CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP', CAST(N'2018-01-01T00:00:00.000' AS DateTime), N'SYSOP')
ALTER TABLE [dbo].[ANNOUNCEMENT] ADD  CONSTRAINT [DF_ANNOUNCEMENT_title]  DEFAULT ('') FOR [title]
GO
ALTER TABLE [dbo].[ANNOUNCEMENT] ADD  CONSTRAINT [DF_ANNOUNCEMENT_comment]  DEFAULT ('') FOR [comment]
GO
ALTER TABLE [dbo].[ANNOUNCEMENT] ADD  CONSTRAINT [DF_ANNOUNCEMENT_effective_date]  DEFAULT (getdate()) FOR [effective_date]
GO
ALTER TABLE [dbo].[ANNOUNCEMENT] ADD  CONSTRAINT [DF_ANNOUNCEMENT_expire_date]  DEFAULT (dateadd(year,(1),getdate())) FOR [expire_date]
GO
ALTER TABLE [dbo].[ANNOUNCEMENT] ADD  CONSTRAINT [DF_ANNOUNCEMENT_attach_name]  DEFAULT ('') FOR [attach_name]
GO
ALTER TABLE [dbo].[ANNOUNCEMENT] ADD  CONSTRAINT [DF_ANNOUNCEMENT_del_flg]  DEFAULT ((0)) FOR [del_flg]
GO
ALTER TABLE [dbo].[ANNOUNCEMENT] ADD  CONSTRAINT [DF_ANNOUNCEMENT_crt_date]  DEFAULT (getdate()) FOR [crt_date]
GO
ALTER TABLE [dbo].[ANNOUNCEMENT] ADD  CONSTRAINT [DF_ANNOUNCEMENT_crt_user]  DEFAULT ('SYSOP') FOR [crt_user]
GO
ALTER TABLE [dbo].[ANNOUNCEMENT] ADD  CONSTRAINT [DF_ANNOUNCEMENT_mdf_date]  DEFAULT (getdate()) FOR [mdf_date]
GO
ALTER TABLE [dbo].[ANNOUNCEMENT] ADD  CONSTRAINT [DF_ANNOUNCEMENT_mdf_user]  DEFAULT ('SYSOP') FOR [mdf_user]
GO
ALTER TABLE [dbo].[DIM_RIGHT] ADD  CONSTRAINT [DF_DIM_RIGHT_right_name]  DEFAULT ('') FOR [right_name]
GO
ALTER TABLE [dbo].[DIM_RIGHT] ADD  CONSTRAINT [DF_DIM_RIGHT_del_flg]  DEFAULT ((0)) FOR [del_flg]
GO
ALTER TABLE [dbo].[DIM_RIGHT] ADD  CONSTRAINT [DF_DIM_RIGHT_crt_date]  DEFAULT (getdate()) FOR [crt_date]
GO
ALTER TABLE [dbo].[DIM_RIGHT] ADD  CONSTRAINT [DF_DIM_RIGHT_crt_user]  DEFAULT ('SYSOP') FOR [crt_user]
GO
ALTER TABLE [dbo].[DIM_RIGHT] ADD  CONSTRAINT [DF_DIM_RIGHT_mdf_date]  DEFAULT (getdate()) FOR [mdf_date]
GO
ALTER TABLE [dbo].[DIM_RIGHT] ADD  CONSTRAINT [DF_DIM_RIGHT_mdf_user]  DEFAULT ('SYSOP') FOR [mdf_user]
GO
ALTER TABLE [dbo].[DIM_ROLE] ADD  CONSTRAINT [DF_DIM_ROLE_role_name]  DEFAULT ('') FOR [role_name]
GO
ALTER TABLE [dbo].[DIM_ROLE] ADD  CONSTRAINT [DF_DIM_ROLE_del_flg]  DEFAULT ((0)) FOR [del_flg]
GO
ALTER TABLE [dbo].[DIM_ROLE] ADD  CONSTRAINT [DF_DIM_ROLE_crt_date]  DEFAULT (getdate()) FOR [crt_date]
GO
ALTER TABLE [dbo].[DIM_ROLE] ADD  CONSTRAINT [DF_DIM_ROLE_crt_user]  DEFAULT ('SYSOP') FOR [crt_user]
GO
ALTER TABLE [dbo].[DIM_ROLE] ADD  CONSTRAINT [DF_DIM_ROLE_mdf_date]  DEFAULT (getdate()) FOR [mdf_date]
GO
ALTER TABLE [dbo].[DIM_ROLE] ADD  CONSTRAINT [DF_DIM_ROLE_mdf_user]  DEFAULT ('SYSOP') FOR [mdf_user]
GO
ALTER TABLE [dbo].[EFIELD_SET] ADD  CONSTRAINT [DF_EFIELD_SET_field_name]  DEFAULT ('') FOR [field_name]
GO
ALTER TABLE [dbo].[EFIELD_SET] ADD  CONSTRAINT [DF_EFIELD_SET_field_size]  DEFAULT ((0)) FOR [field_size]
GO
ALTER TABLE [dbo].[EFIELD_SET] ADD  CONSTRAINT [DF_EFIELD_SET_field_type]  DEFAULT ((0)) FOR [field_type]
GO
ALTER TABLE [dbo].[EFIELD_SET] ADD  CONSTRAINT [DF_Table_1_field_ext]  DEFAULT ((0)) FOR [field_exttype]
GO
ALTER TABLE [dbo].[EFIELD_SET] ADD  CONSTRAINT [DF_EFIELD_SET_field_option]  DEFAULT ('') FOR [field_option]
GO
ALTER TABLE [dbo].[EFIELD_SET] ADD  CONSTRAINT [DF_Table_1_files_isfill]  DEFAULT ((0)) FOR [field_isfill]
GO
ALTER TABLE [dbo].[EFIELD_SET] ADD  CONSTRAINT [DF_EFIELD_SET_memo]  DEFAULT ('') FOR [memo]
GO
ALTER TABLE [dbo].[EFIELD_SET] ADD  CONSTRAINT [DF_EFIELD_SET_del_flg]  DEFAULT ((0)) FOR [del_flg]
GO
ALTER TABLE [dbo].[EFIELD_SET] ADD  CONSTRAINT [DF_EFIELD_SET_crt_date]  DEFAULT (getdate()) FOR [crt_date]
GO
ALTER TABLE [dbo].[EFIELD_SET] ADD  CONSTRAINT [DF_EFIELD_SET_crt_user]  DEFAULT ('SYSOP') FOR [crt_user]
GO
ALTER TABLE [dbo].[EFIELD_SET] ADD  CONSTRAINT [DF_EFIELD_SET_mdf_date]  DEFAULT (getdate()) FOR [mdf_date]
GO
ALTER TABLE [dbo].[EFIELD_SET] ADD  CONSTRAINT [DF_EFIELD_SET_mdf_user]  DEFAULT ('SYSOP') FOR [mdf_user]
GO
ALTER TABLE [dbo].[EFORM_DATAFILL] ADD  CONSTRAINT [DF_EFORM_FILL_fill_content]  DEFAULT ('') FOR [fill_data]
GO
ALTER TABLE [dbo].[EFORM_DATAFILL] ADD  CONSTRAINT [DF_EFORM_FILL_flow_code]  DEFAULT ('') FOR [flow_code]
GO
ALTER TABLE [dbo].[EFORM_DATAFILL] ADD  CONSTRAINT [DF_EFORM_DATAFILL_del_flg]  DEFAULT ('N') FOR [del_flg]
GO
ALTER TABLE [dbo].[EFORM_DATAFILL] ADD  CONSTRAINT [DF_EFORM_FILL_crt_date]  DEFAULT (getdate()) FOR [crt_date]
GO
ALTER TABLE [dbo].[EFORM_DATAFILL] ADD  CONSTRAINT [DF_EFORM_FILL_crt_user]  DEFAULT ('SYSOP') FOR [crt_user]
GO
ALTER TABLE [dbo].[EFORM_DATAFILL] ADD  CONSTRAINT [DF_EFORM_FILL_mdf_date]  DEFAULT (getdate()) FOR [mdf_date]
GO
ALTER TABLE [dbo].[EFORM_DATAFILL] ADD  CONSTRAINT [DF_EFORM_FILL_mdf_user]  DEFAULT ('SYSOP') FOR [mdf_user]
GO
ALTER TABLE [dbo].[EFORM_FLOWLOG] ADD  CONSTRAINT [DF_EFORM_FLOWLOG_flow_id]  DEFAULT ('') FOR [flow_code]
GO
ALTER TABLE [dbo].[EFORM_FLOWLOG] ADD  CONSTRAINT [DF_EFORM_FLOWLOG_form_id]  DEFAULT ('') FOR [fill_id]
GO
ALTER TABLE [dbo].[EFORM_FLOWLOG] ADD  CONSTRAINT [DF_EFORM_FLOWLOG_crt_date]  DEFAULT (getdate()) FOR [crt_date]
GO
ALTER TABLE [dbo].[EFORM_SET] ADD  CONSTRAINT [DF_EFORM_SET_form_type]  DEFAULT ('') FOR [form_type]
GO
ALTER TABLE [dbo].[EFORM_SET] ADD  CONSTRAINT [DF_EFORM_SET_form_name]  DEFAULT ('') FOR [form_name]
GO
ALTER TABLE [dbo].[EFORM_SET] ADD  CONSTRAINT [DF_EFORM_SET_effective_date]  DEFAULT (getdate()) FOR [effective_date]
GO
ALTER TABLE [dbo].[EFORM_SET] ADD  CONSTRAINT [DF_EFORM_SET_expire_date]  DEFAULT (dateadd(year,(10),getdate())) FOR [expire_date]
GO
ALTER TABLE [dbo].[EFORM_SET] ADD  CONSTRAINT [DF_EFORM_SET_memo]  DEFAULT ('') FOR [memo]
GO
ALTER TABLE [dbo].[EFORM_SET] ADD  CONSTRAINT [DF_FLOW_FORMSET_crt_date]  DEFAULT (getdate()) FOR [crt_date]
GO
ALTER TABLE [dbo].[EFORM_SET] ADD  CONSTRAINT [DF_FLOW_FORMSET_crt_user]  DEFAULT ('SYSOP') FOR [crt_user]
GO
ALTER TABLE [dbo].[EFORM_SET] ADD  CONSTRAINT [DF_FLOW_FORMSET_mdf_date]  DEFAULT (getdate()) FOR [mdf_date]
GO
ALTER TABLE [dbo].[EFORM_SET] ADD  CONSTRAINT [DF_FLOW_FORMSET_mdf_user]  DEFAULT ('SYSOP') FOR [mdf_user]
GO
ALTER TABLE [dbo].[EMP_AGENT] ADD  CONSTRAINT [DF_EMP_AGENT_user_id]  DEFAULT ('') FOR [user_id]
GO
ALTER TABLE [dbo].[EMP_AGENT] ADD  CONSTRAINT [DF_EMP_AGENT_agent_id]  DEFAULT ('') FOR [agent_id]
GO
ALTER TABLE [dbo].[EMP_AGENT] ADD  CONSTRAINT [DF_EMP_AGENT_agent_from]  DEFAULT (getdate()) FOR [agent_from]
GO
ALTER TABLE [dbo].[EMP_AGENT] ADD  CONSTRAINT [DF_EMP_AGENT_agent_to]  DEFAULT (dateadd(day,(1),getdate())) FOR [agent_to]
GO
ALTER TABLE [dbo].[EMP_AGENT] ADD  CONSTRAINT [DF_EMP_AGENT_del_flg]  DEFAULT ((0)) FOR [del_flg]
GO
ALTER TABLE [dbo].[EMP_AGENT] ADD  CONSTRAINT [DF_EMP_AGENT_crt_date]  DEFAULT (getdate()) FOR [crt_date]
GO
ALTER TABLE [dbo].[EMP_AGENT] ADD  CONSTRAINT [DF_EMP_AGENT_crt_user]  DEFAULT ('SYSOP') FOR [crt_user]
GO
ALTER TABLE [dbo].[EMP_AGENT] ADD  CONSTRAINT [DF_EMP_AGENT_mdf_date]  DEFAULT (getdate()) FOR [mdf_date]
GO
ALTER TABLE [dbo].[EMP_AGENT] ADD  CONSTRAINT [DF_EMP_AGENT_mdf_user]  DEFAULT ('SYSOP') FOR [mdf_user]
GO
ALTER TABLE [dbo].[EMP_ORG] ADD  CONSTRAINT [DF_EMP_ORG_org_name]  DEFAULT ('') FOR [org_name]
GO
ALTER TABLE [dbo].[EMP_ORG] ADD  CONSTRAINT [DF_EMP_ORG_parent_id]  DEFAULT ('') FOR [parent_id]
GO
ALTER TABLE [dbo].[EMP_ORG] ADD  CONSTRAINT [DF_EMP_ORG_del_flg]  DEFAULT ((0)) FOR [del_flg]
GO
ALTER TABLE [dbo].[EMP_ORG] ADD  CONSTRAINT [DF_EMP_ORG_crt_date]  DEFAULT (getdate()) FOR [crt_date]
GO
ALTER TABLE [dbo].[EMP_ORG] ADD  CONSTRAINT [DF_EMP_ORG_crt_user]  DEFAULT ('SYSOP') FOR [crt_user]
GO
ALTER TABLE [dbo].[EMP_ORG] ADD  CONSTRAINT [DF_EMP_ORG_mdf_date]  DEFAULT (getdate()) FOR [mdf_date]
GO
ALTER TABLE [dbo].[EMP_ORG] ADD  CONSTRAINT [DF_EMP_ORG_mdf_user]  DEFAULT ('SYSOP') FOR [mdf_user]
GO
ALTER TABLE [dbo].[EMP_USER] ADD  CONSTRAINT [DF_EMP_USER_user_name]  DEFAULT ('') FOR [user_name]
GO
ALTER TABLE [dbo].[EMP_USER] ADD  CONSTRAINT [DF_EMP_USER_user_tel]  DEFAULT ('') FOR [user_tel]
GO
ALTER TABLE [dbo].[EMP_USER] ADD  CONSTRAINT [DF_EMP_USER_user_email]  DEFAULT ('') FOR [user_email]
GO
ALTER TABLE [dbo].[EMP_USER] ADD  CONSTRAINT [DF_EMP_USER_user_pwd]  DEFAULT ('') FOR [user_pwd]
GO
ALTER TABLE [dbo].[EMP_USER] ADD  CONSTRAINT [DF_EMP_USER_dn]  DEFAULT ('') FOR [user_token]
GO
ALTER TABLE [dbo].[EMP_USER] ADD  CONSTRAINT [DF_EMP_USER_del_flg]  DEFAULT ((0)) FOR [del_flg]
GO
ALTER TABLE [dbo].[EMP_USER] ADD  CONSTRAINT [DF_EMP_USER_crt_date]  DEFAULT (getdate()) FOR [crt_date]
GO
ALTER TABLE [dbo].[EMP_USER] ADD  CONSTRAINT [DF_EMP_USER_crt_user]  DEFAULT ('SYSOP') FOR [crt_user]
GO
ALTER TABLE [dbo].[EMP_USER] ADD  CONSTRAINT [DF_EMP_USER_mdf_date]  DEFAULT (getdate()) FOR [mdf_date]
GO
ALTER TABLE [dbo].[EMP_USER] ADD  CONSTRAINT [DF_EMP_USER_mdf_user]  DEFAULT ('SYSOP') FOR [mdf_user]
GO
ALTER TABLE [dbo].[EMP_USER_COPY] ADD  CONSTRAINT [DF_EMP_USER_COPY_user_name]  DEFAULT ('') FOR [user_name]
GO
ALTER TABLE [dbo].[EMP_USER_COPY] ADD  CONSTRAINT [DF_EMP_USER_COPY_user_tel]  DEFAULT ('') FOR [user_tel]
GO
ALTER TABLE [dbo].[EMP_USER_COPY] ADD  CONSTRAINT [DF_EMP_USER_COPY_user_email]  DEFAULT ('') FOR [user_email]
GO
ALTER TABLE [dbo].[EMP_USER_COPY] ADD  CONSTRAINT [DF_EMP_USER_COPY_user_pwd]  DEFAULT ('') FOR [user_pwd]
GO
ALTER TABLE [dbo].[EMP_USER_COPY] ADD  CONSTRAINT [DF_EMP_USER_COPY_dn]  DEFAULT ('') FOR [user_token]
GO
ALTER TABLE [dbo].[EMP_USER_COPY] ADD  CONSTRAINT [DF_EMP_USER_COPY_del_flg]  DEFAULT ((0)) FOR [del_flg]
GO
ALTER TABLE [dbo].[EMP_USER_COPY] ADD  CONSTRAINT [DF_EMP_USER_COPY_crt_date]  DEFAULT (getdate()) FOR [crt_date]
GO
ALTER TABLE [dbo].[EMP_USER_COPY] ADD  CONSTRAINT [DF_EMP_USER_COPY_crt_user]  DEFAULT ('SYSOP') FOR [crt_user]
GO
ALTER TABLE [dbo].[EMP_USER_COPY] ADD  CONSTRAINT [DF_EMP_USER_COPY_mdf_date]  DEFAULT (getdate()) FOR [mdf_date]
GO
ALTER TABLE [dbo].[EMP_USER_COPY] ADD  CONSTRAINT [DF_EMP_USER_COPY_mdf_user]  DEFAULT ('SYSOP') FOR [mdf_user]
GO
ALTER TABLE [dbo].[FLOW_PREARRANGE] ADD  CONSTRAINT [DF_FLOW_PREARRANGE_flow_id]  DEFAULT ('') FOR [flow_id]
GO
ALTER TABLE [dbo].[FLOW_PREARRANGE] ADD  CONSTRAINT [DF_FLOW_PREARRANGE_flow_status]  DEFAULT ((0)) FOR [flow_status]
GO
ALTER TABLE [dbo].[FLOW_PREARRANGE] ADD  CONSTRAINT [DF_FLOW_PREARRANGE_flow_org_id]  DEFAULT ('') FOR [flow_org_id]
GO
ALTER TABLE [dbo].[FLOW_PREARRANGE] ADD  CONSTRAINT [DF_FLOW_PREARRANGE_flow_role_id]  DEFAULT ('') FOR [flow_role_id]
GO
ALTER TABLE [dbo].[FLOW_PREARRANGE] ADD  CONSTRAINT [DF_FLOW_PREARRANGE_flow_user_id]  DEFAULT ('') FOR [flow_user_id]
GO
ALTER TABLE [dbo].[FLOW_PREARRANGE] ADD  CONSTRAINT [DF_Table_1_set_decision]  DEFAULT ((0)) FOR [flow_decision]
GO
ALTER TABLE [dbo].[FLOW_PREARRANGE] ADD  CONSTRAINT [DF_FLOW_PREARRANGE_flow_option]  DEFAULT ('') FOR [flow_option]
GO
ALTER TABLE [dbo].[FLOW_PREARRANGE] ADD  CONSTRAINT [DF_FLOW_PREARRANGE_crt_date]  DEFAULT (getdate()) FOR [crt_date]
GO
ALTER TABLE [dbo].[FLOW_PREARRANGE] ADD  CONSTRAINT [DF_FLOW_PREARRANGE_crt_user]  DEFAULT ('SYSOP') FOR [crt_user]
GO
ALTER TABLE [dbo].[FLOW_PREARRANGE] ADD  CONSTRAINT [DF_FLOW_PREARRANGE_mdf_date]  DEFAULT (getdate()) FOR [mdf_date]
GO
ALTER TABLE [dbo].[FLOW_PREARRANGE] ADD  CONSTRAINT [DF_FLOW_PREARRANGE_mdf_user]  DEFAULT ('SYSOP') FOR [mdf_user]
GO
ALTER TABLE [dbo].[FLOW_ROLE] ADD  CONSTRAINT [DF_FLOW_ROLE_role_name]  DEFAULT ('') FOR [role_name]
GO
ALTER TABLE [dbo].[FLOW_ROLE] ADD  CONSTRAINT [DF_FLOW_ROLE_del_flg]  DEFAULT ((0)) FOR [del_flg]
GO
ALTER TABLE [dbo].[FLOW_ROLE] ADD  CONSTRAINT [DF_FLOW_ROLE_crt_date]  DEFAULT (getdate()) FOR [crt_date]
GO
ALTER TABLE [dbo].[FLOW_ROLE] ADD  CONSTRAINT [DF_FLOW_ROLE_crt_user]  DEFAULT ('SYSOP') FOR [crt_user]
GO
ALTER TABLE [dbo].[FLOW_ROLE] ADD  CONSTRAINT [DF_FLOW_ROLE_mdf_date]  DEFAULT (getdate()) FOR [mdf_date]
GO
ALTER TABLE [dbo].[FLOW_ROLE] ADD  CONSTRAINT [DF_FLOW_ROLE_mdf_user]  DEFAULT ('SYSOP') FOR [mdf_user]
GO
ALTER TABLE [dbo].[FLOW_SET] ADD  CONSTRAINT [DF_FLOW_SET_flow_name]  DEFAULT ('') FOR [flow_name]
GO
ALTER TABLE [dbo].[FLOW_SET] ADD  CONSTRAINT [DF_FLOW_SET_effective_date]  DEFAULT (getdate()) FOR [effective_date]
GO
ALTER TABLE [dbo].[FLOW_SET] ADD  CONSTRAINT [DF_FLOW_SET_expire_date]  DEFAULT (dateadd(year,(10),getdate())) FOR [expire_date]
GO
ALTER TABLE [dbo].[FLOW_SET] ADD  CONSTRAINT [DF_FLOW_SET_memo]  DEFAULT ('') FOR [memo]
GO
ALTER TABLE [dbo].[FLOW_SET] ADD  CONSTRAINT [DF_FLOW_SET_crt_date]  DEFAULT (getdate()) FOR [crt_date]
GO
ALTER TABLE [dbo].[FLOW_SET] ADD  CONSTRAINT [DF_FLOW_SET_crt_user]  DEFAULT ('SYSOP') FOR [crt_user]
GO
ALTER TABLE [dbo].[FLOW_SET] ADD  CONSTRAINT [DF_FLOW_SET_mdf_date]  DEFAULT (getdate()) FOR [mdf_date]
GO
ALTER TABLE [dbo].[FLOW_SET] ADD  CONSTRAINT [DF_FLOW_SET_mdf_user]  DEFAULT ('SYSOP') FOR [mdf_user]
GO
ALTER TABLE [dbo].[FLOW_SET_DETAIL] ADD  CONSTRAINT [DF_FLOW_SET_DETAIL_set_org_id]  DEFAULT ('') FOR [set_org_id]
GO
ALTER TABLE [dbo].[FLOW_SET_DETAIL] ADD  CONSTRAINT [DF_FLOW_SET_DETAIL_set_role_id]  DEFAULT ('') FOR [set_role_id]
GO
ALTER TABLE [dbo].[FLOW_SET_DETAIL] ADD  CONSTRAINT [DF_FLOW_SET_DETAIL_set_user_id]  DEFAULT ('') FOR [set_user_id]
GO
ALTER TABLE [dbo].[FLOW_SET_DETAIL] ADD  CONSTRAINT [DF_FLOW_SET_DETAIL_set_flow_id]  DEFAULT ('') FOR [set_flow_id]
GO
ALTER TABLE [dbo].[FLOW_SET_DETAIL] ADD  CONSTRAINT [DF_FLOW_DTLSET_execute_pass]  DEFAULT ((0)) FOR [set_decision]
GO
ALTER TABLE [dbo].[FLOW_SET_DETAIL] ADD  CONSTRAINT [DF_FLOW_SET_DETAIL_set_option]  DEFAULT ('') FOR [set_option]
GO
ALTER TABLE [dbo].[FLOW_SET_DETAIL] ADD  CONSTRAINT [DF_FLOW_DTLSET_crt_date]  DEFAULT (getdate()) FOR [crt_date]
GO
ALTER TABLE [dbo].[FLOW_SET_DETAIL] ADD  CONSTRAINT [DF_FLOW_DTLSET_crt_user]  DEFAULT ('SYSOP') FOR [crt_user]
GO
ALTER TABLE [dbo].[FLOW_SET_DETAIL] ADD  CONSTRAINT [DF_FLOW_DTLSET_mdf_date]  DEFAULT (getdate()) FOR [mdf_date]
GO
ALTER TABLE [dbo].[FLOW_SET_DETAIL] ADD  CONSTRAINT [DF_FLOW_DTLSET_mdf_user]  DEFAULT ('SYSOP') FOR [mdf_user]
GO
ALTER TABLE [dbo].[FLOW_SIGNEDLOG] ADD  CONSTRAINT [DF_FLOW_SIGNEDLOG_flow_code]  DEFAULT ('') FOR [flow_code]
GO
ALTER TABLE [dbo].[FLOW_SIGNEDLOG] ADD  CONSTRAINT [DF_FLOW_SIGNEDLOG_flow_odr]  DEFAULT ((0)) FOR [flow_odr]
GO
ALTER TABLE [dbo].[FLOW_SIGNEDLOG] ADD  CONSTRAINT [DF_FLOW_SIGNEDLOG_signed_odr]  DEFAULT ((0)) FOR [signed_odr]
GO
ALTER TABLE [dbo].[FLOW_SIGNEDLOG] ADD  CONSTRAINT [DF_FLOW_SIGNEDLOG_signed_status]  DEFAULT ('') FOR [signed_status]
GO
ALTER TABLE [dbo].[FLOW_SIGNEDLOG] ADD  CONSTRAINT [DF_FLOW_SIGNEDLOG_signed_org_id]  DEFAULT ('') FOR [signed_org_id]
GO
ALTER TABLE [dbo].[FLOW_SIGNEDLOG] ADD  CONSTRAINT [DF_FLOW_SIGNEDLOG_signed_role_id]  DEFAULT ('') FOR [signed_role_id]
GO
ALTER TABLE [dbo].[FLOW_SIGNEDLOG] ADD  CONSTRAINT [DF_FLOW_SIGNEDLOG_signed_user_id]  DEFAULT ('') FOR [signed_user_id]
GO
ALTER TABLE [dbo].[FLOW_SIGNEDLOG] ADD  CONSTRAINT [DF_FLOW_SIGNEDLOG_memo]  DEFAULT ('') FOR [signed_memo]
GO
ALTER TABLE [dbo].[FLOW_SIGNEDLOG] ADD  CONSTRAINT [DF_Table_1_crt_date]  DEFAULT (getdate()) FOR [signed_date]
GO
ALTER TABLE [dbo].[MAIL_LOG] ADD  CONSTRAINT [DF_MAIL_LOG_mail_sender]  DEFAULT ('') FOR [mail_sender]
GO
ALTER TABLE [dbo].[MAIL_LOG] ADD  CONSTRAINT [DF_MAIL_LOG_mail_recevier]  DEFAULT ('') FOR [mail_receiver]
GO
ALTER TABLE [dbo].[MAIL_LOG] ADD  CONSTRAINT [DF_MAIL_LOG_cc_recevier]  DEFAULT ('') FOR [cc_receiver]
GO
ALTER TABLE [dbo].[MAIL_LOG] ADD  CONSTRAINT [DF_MAIL_LOG_cc_recevier1]  DEFAULT ('') FOR [bcc_receiver]
GO
ALTER TABLE [dbo].[MAIL_LOG] ADD  CONSTRAINT [DF_MAIL_LOG_mail_title]  DEFAULT ('') FOR [mail_subject]
GO
ALTER TABLE [dbo].[MAIL_LOG] ADD  CONSTRAINT [DF_MAIL_LOG_mail_content]  DEFAULT ('') FOR [mail_content]
GO
ALTER TABLE [dbo].[MAIL_LOG] ADD  CONSTRAINT [DF_MAIL_LOG_isFailed]  DEFAULT ((0)) FOR [send_flg]
GO
ALTER TABLE [dbo].[MAIL_LOG] ADD  CONSTRAINT [DF_MAIL_LOG_crt_date]  DEFAULT (getdate()) FOR [crt_date]
GO
ALTER TABLE [dbo].[MAIL_RECIPIENT] ADD  CONSTRAINT [DF_MAIL_RECIPIENT_mail_id]  DEFAULT ('') FOR [mail_id]
GO
ALTER TABLE [dbo].[MAIL_RECIPIENT] ADD  CONSTRAINT [DF_MAIL_RECIPIENT_mail_type]  DEFAULT ('') FOR [mail_type]
GO
ALTER TABLE [dbo].[MAIL_RECIPIENT] ADD  CONSTRAINT [DF_MAIL_RECIPIENT_mail_role]  DEFAULT ('') FOR [mail_role]
GO
ALTER TABLE [dbo].[MAIL_RECIPIENT] ADD  CONSTRAINT [DF_MAIL_RECIPIENT_mail]  DEFAULT ('') FOR [mail_address]
GO
ALTER TABLE [dbo].[MAIL_RECIPIENT] ADD  CONSTRAINT [DF_MAIL_RECIPIENT_mail_title]  DEFAULT ('') FOR [mail_title]
GO
ALTER TABLE [dbo].[MAIL_ROLE] ADD  CONSTRAINT [DF_MAIL_ROLE_role_id]  DEFAULT ('') FOR [role_id]
GO
ALTER TABLE [dbo].[MAIL_ROLE] ADD  CONSTRAINT [DF_MAIL_ROLE_rol_name]  DEFAULT ('') FOR [role_name]
GO
ALTER TABLE [dbo].[MAIL_ROLE] ADD  CONSTRAINT [DF_MAIL_ROLE_del_flg]  DEFAULT ((0)) FOR [del_flg]
GO
ALTER TABLE [dbo].[MAIL_ROLE] ADD  CONSTRAINT [DF_MAIL_ROLE_crt_date]  DEFAULT (getdate()) FOR [crt_date]
GO
ALTER TABLE [dbo].[MAIL_ROLE] ADD  CONSTRAINT [DF_MAIL_ROLE_crt_user]  DEFAULT ('SYSOP') FOR [crt_user]
GO
ALTER TABLE [dbo].[MAIL_ROLE] ADD  CONSTRAINT [DF_Table_1_crt_date1_1]  DEFAULT (getdate()) FOR [mdf_date]
GO
ALTER TABLE [dbo].[MAIL_ROLE] ADD  CONSTRAINT [DF_Table_1_crt_user1_1]  DEFAULT ('SYSOP') FOR [mdf_user]
GO
ALTER TABLE [dbo].[MAIL_TEMPLATE] ADD  CONSTRAINT [DF_MAIL_TEMPLATE_mail_id]  DEFAULT ('') FOR [mail_id]
GO
ALTER TABLE [dbo].[MAIL_TEMPLATE] ADD  CONSTRAINT [DF_MAIL_TEMPLATE_mail_name]  DEFAULT ('') FOR [mail_name]
GO
ALTER TABLE [dbo].[MAIL_TEMPLATE] ADD  CONSTRAINT [DF_MAIL_TEMPLATE_mail_title]  DEFAULT ('') FOR [mail_subject]
GO
ALTER TABLE [dbo].[MAIL_TEMPLATE] ADD  CONSTRAINT [DF_MAIL_TEMPLATE_mail_content]  DEFAULT ('') FOR [mail_content]
GO
ALTER TABLE [dbo].[MAIL_TEMPLATE] ADD  CONSTRAINT [DF_MAIL_TEMPLATE_del_flg]  DEFAULT ((0)) FOR [del_flg]
GO
ALTER TABLE [dbo].[MAIL_TEMPLATE] ADD  CONSTRAINT [DF_MAIL_TEMPLATE_crt_date]  DEFAULT (getdate()) FOR [crt_date]
GO
ALTER TABLE [dbo].[MAIL_TEMPLATE] ADD  CONSTRAINT [DF_MAIL_TEMPLATE_crt_user]  DEFAULT ('SYSOP') FOR [crt_user]
GO
ALTER TABLE [dbo].[MAIL_TEMPLATE] ADD  CONSTRAINT [DF_Table_1_crt_date1]  DEFAULT (getdate()) FOR [mdf_date]
GO
ALTER TABLE [dbo].[MAIL_TEMPLATE] ADD  CONSTRAINT [DF_Table_1_crt_user1]  DEFAULT ('SYSOP') FOR [mdf_user]
GO
ALTER TABLE [dbo].[MAP_EFORM_EFIELD] ADD  CONSTRAINT [DF_MAP_EFORM_EFIELD_issplitter]  DEFAULT ((0)) FOR [issplitter]
GO
ALTER TABLE [dbo].[MAP_EFORM_EFIELD] ADD  CONSTRAINT [DF_MAP_EFORM_EFIELD_crt_date]  DEFAULT (getdate()) FOR [crt_date]
GO
ALTER TABLE [dbo].[MAP_EFORM_EFIELD] ADD  CONSTRAINT [DF_MAP_EFORM_EFIELD_crt_user]  DEFAULT ('SYSOP') FOR [crt_user]
GO
ALTER TABLE [dbo].[MAP_EFORM_FLOW] ADD  CONSTRAINT [DF_MAP_EFORM_FLOW_alert_day]  DEFAULT ((7)) FOR [alert_day]
GO
ALTER TABLE [dbo].[MAP_EFORM_FLOW] ADD  CONSTRAINT [DF_FLOW_MAP_FORM_FLOW_crt_date]  DEFAULT (getdate()) FOR [crt_date]
GO
ALTER TABLE [dbo].[MAP_EFORM_FLOW] ADD  CONSTRAINT [DF_FLOW_MAP_FORM_FLOW_crt_user]  DEFAULT ('SYSOP') FOR [crt_user]
GO
ALTER TABLE [dbo].[MAP_EFORM_ORG] ADD  CONSTRAINT [DF_MAP_EFORM_ORG_crt_date]  DEFAULT (getdate()) FOR [crt_date]
GO
ALTER TABLE [dbo].[MAP_EFORM_ORG] ADD  CONSTRAINT [DF_MAP_EFORM_ORG_crt_user]  DEFAULT ('SYSOP') FOR [crt_user]
GO
ALTER TABLE [dbo].[MAP_FLOW_ORG] ADD  CONSTRAINT [DF_MAP_FLOW_ORG_flow_id]  DEFAULT ('') FOR [flow_id]
GO
ALTER TABLE [dbo].[MAP_FLOW_ORG] ADD  CONSTRAINT [DF_MAP_FLOW_ORG_org_id]  DEFAULT ('') FOR [org_id]
GO
ALTER TABLE [dbo].[MAP_FLOW_ORG] ADD  CONSTRAINT [DF_MAP_FLOW_ORG_crt_date]  DEFAULT (getdate()) FOR [crt_date]
GO
ALTER TABLE [dbo].[MAP_FLOW_ORG] ADD  CONSTRAINT [DF_MAP_FLOW_ORG_crt_user]  DEFAULT ('SYSOP') FOR [crt_user]
GO
ALTER TABLE [dbo].[MAP_ORG_USER] ADD  CONSTRAINT [DF_MAP_ORG_USER_crt_date]  DEFAULT (getdate()) FOR [crt_date]
GO
ALTER TABLE [dbo].[MAP_ORG_USER] ADD  CONSTRAINT [DF_MAP_ORG_USER_crt_user]  DEFAULT ('SYSOP') FOR [crt_user]
GO
ALTER TABLE [dbo].[MAP_RIGHT_FUNCTION] ADD  CONSTRAINT [DF_MAP_RIGHT_FUNCTION_crt_date]  DEFAULT (getdate()) FOR [crt_date]
GO
ALTER TABLE [dbo].[MAP_RIGHT_FUNCTION] ADD  CONSTRAINT [DF_MAP_RIGHT_FUNCTION_crt_user]  DEFAULT ('SYSOP') FOR [crt_user]
GO
ALTER TABLE [dbo].[MAP_ROLE_RIGHT] ADD  CONSTRAINT [DF_MAP_ROLE_RIGHT_crt_date]  DEFAULT (getdate()) FOR [crt_date]
GO
ALTER TABLE [dbo].[MAP_ROLE_RIGHT] ADD  CONSTRAINT [DF_MAP_ROLE_RIGHT_crt_user]  DEFAULT ('SYSOP') FOR [crt_user]
GO
ALTER TABLE [dbo].[MAP_USER_FLOWROLE] ADD  CONSTRAINT [DF_FLOW_MAP_ROLE_USER_crt_date]  DEFAULT (getdate()) FOR [crt_date]
GO
ALTER TABLE [dbo].[MAP_USER_FLOWROLE] ADD  CONSTRAINT [DF_FLOW_MAP_ROLE_USER_crt_user]  DEFAULT ('SYSOP') FOR [crt_user]
GO
ALTER TABLE [dbo].[MAP_USER_MAILROLE] ADD  CONSTRAINT [DF_MAP_USER_MAILROLE_crt_date]  DEFAULT (getdate()) FOR [crt_date]
GO
ALTER TABLE [dbo].[MAP_USER_MAILROLE] ADD  CONSTRAINT [DF_MAP_USER_MAILROLE_crt_user]  DEFAULT ('SYSOP') FOR [crt_user]
GO
ALTER TABLE [dbo].[MAP_USER_ROLE] ADD  CONSTRAINT [DF_MAP_USER_ROLE_crt_date]  DEFAULT (getdate()) FOR [crt_date]
GO
ALTER TABLE [dbo].[MAP_USER_ROLE] ADD  CONSTRAINT [DF_MAP_USER_ROLE_crt_user]  DEFAULT ('SYSOP') FOR [crt_user]
GO
ALTER TABLE [dbo].[SET_FUNCTION] ADD  CONSTRAINT [DF_SET_FUNCTION_function_name]  DEFAULT ('') FOR [function_name]
GO
ALTER TABLE [dbo].[SET_FUNCTION] ADD  CONSTRAINT [DF_SET_FUNCTION_function_url]  DEFAULT ('') FOR [function_url]
GO
ALTER TABLE [dbo].[SET_FUNCTION] ADD  CONSTRAINT [DF_SET_FUNCTION_parent_id]  DEFAULT ('') FOR [parent_id]
GO
ALTER TABLE [dbo].[SET_FUNCTION] ADD  CONSTRAINT [DF_SET_FUNCTION_sort_id]  DEFAULT ((0)) FOR [sort_id]
GO
ALTER TABLE [dbo].[SET_FUNCTION] ADD  CONSTRAINT [DF_SET_FUNCTION_del_flg]  DEFAULT ((0)) FOR [del_flg]
GO
ALTER TABLE [dbo].[SET_FUNCTION] ADD  CONSTRAINT [DF_SET_FUNCTION_crt_date]  DEFAULT (getdate()) FOR [crt_date]
GO
ALTER TABLE [dbo].[SET_FUNCTION] ADD  CONSTRAINT [DF_SET_FUNCTION_crt_user]  DEFAULT ('SYSOP') FOR [crt_user]
GO
ALTER TABLE [dbo].[SET_FUNCTION] ADD  CONSTRAINT [DF_SET_FUNCTION_mdf_date]  DEFAULT (getdate()) FOR [mdf_date]
GO
ALTER TABLE [dbo].[SET_FUNCTION] ADD  CONSTRAINT [DF_SET_FUNCTION_mdf_user]  DEFAULT ('SYSOP') FOR [mdf_user]
GO
ALTER TABLE [dbo].[SET_PARAM] ADD  CONSTRAINT [DF_SET_PARAMS_set_value]  DEFAULT ('') FOR [set_value]
GO
ALTER TABLE [dbo].[SET_PARAM] ADD  CONSTRAINT [DF_SET_PARAMS_memo]  DEFAULT ('') FOR [memo]
GO
ALTER TABLE [dbo].[SET_PARAM] ADD  CONSTRAINT [DF_SET_PARAMS_del_flg]  DEFAULT ((0)) FOR [del_flg]
GO
ALTER TABLE [dbo].[SET_PARAM] ADD  CONSTRAINT [DF_SET_PARAMS_crt_date]  DEFAULT (getdate()) FOR [crt_date]
GO
ALTER TABLE [dbo].[SET_PARAM] ADD  CONSTRAINT [DF_SET_PARAMS_crt_user]  DEFAULT ('SYSOP') FOR [crt_user]
GO
ALTER TABLE [dbo].[SET_PARAM] ADD  CONSTRAINT [DF_SET_PARAMS_mdf_date]  DEFAULT (getdate()) FOR [mdf_date]
GO
ALTER TABLE [dbo].[SET_PARAM] ADD  CONSTRAINT [DF_SET_PARAMS_mdf_user]  DEFAULT ('SYSOP') FOR [mdf_user]
GO
ALTER TABLE [dbo].[SET_PARAMITEM] ADD  CONSTRAINT [DF_SET_PARAMITEM_set_item_name]  DEFAULT ('') FOR [set_item_name]
GO
ALTER TABLE [dbo].[SET_PARAMITEM] ADD  CONSTRAINT [DF_SET_PARAMITEM_memo]  DEFAULT ('') FOR [memo]
GO
ALTER TABLE [dbo].[SET_PARAMITEM] ADD  CONSTRAINT [DF_SET_PARAMITEM_editable]  DEFAULT ((1)) FOR [editable]
GO
ALTER TABLE [dbo].[SET_PARAMITEM] ADD  CONSTRAINT [DF_SET_PARAMITEM_del_flg]  DEFAULT ((0)) FOR [del_flg]
GO
ALTER TABLE [dbo].[SET_PARAMITEM] ADD  CONSTRAINT [DF_SET_PARAMITEM_crt_date]  DEFAULT (getdate()) FOR [crt_date]
GO
ALTER TABLE [dbo].[SET_PARAMITEM] ADD  CONSTRAINT [DF_SET_PARAMITEM_crt_user]  DEFAULT ('SYSOP') FOR [crt_user]
GO
ALTER TABLE [dbo].[SET_PARAMITEM] ADD  CONSTRAINT [DF_SET_PARAMITEM_mdf_date]  DEFAULT (getdate()) FOR [mdf_date]
GO
ALTER TABLE [dbo].[SET_PARAMITEM] ADD  CONSTRAINT [DF_SET_PARAMITEM_mdf_user]  DEFAULT ('SYSOP') FOR [mdf_user]
GO
ALTER TABLE [dbo].[SQL_TRACE] ADD  CONSTRAINT [DF_SQL_TRACE_user_id]  DEFAULT ('') FOR [user_id]
GO
ALTER TABLE [dbo].[SQL_TRACE] ADD  CONSTRAINT [DF_SQL_TRACE_user_ip]  DEFAULT ('') FOR [user_ip]
GO
ALTER TABLE [dbo].[SQL_TRACE] ADD  CONSTRAINT [DF_SQL_TRACE_commandtext]  DEFAULT ('') FOR [commandtext]
GO
ALTER TABLE [dbo].[SQL_TRACE] ADD  CONSTRAINT [DF_SQL_TRACE_parameters]  DEFAULT ('') FOR [parameters]
GO
ALTER TABLE [dbo].[SQL_TRACE] ADD  CONSTRAINT [DF_SQL_TRACE_request_url]  DEFAULT ('') FOR [request_url]
GO
ALTER TABLE [dbo].[SQL_TRACE] ADD  CONSTRAINT [DF_SQL_TRACE_log_date]  DEFAULT (getdate()) FOR [log_date]
GO
ALTER TABLE [dbo].[SUBFLOW_PREARRANGE] ADD  CONSTRAINT [DF_SUBFLOW_PREARRANGE_flow_id]  DEFAULT ('') FOR [flow_id]
GO
ALTER TABLE [dbo].[SUBFLOW_PREARRANGE] ADD  CONSTRAINT [DF_SUBFLOW_PREARRANGE_flow_code]  DEFAULT ('') FOR [flow_code]
GO
ALTER TABLE [dbo].[SUBFLOW_PREARRANGE] ADD  CONSTRAINT [DF_SUBFLOW_PREARRANGE_flow_odr]  DEFAULT ((0)) FOR [flow_odr]
GO
ALTER TABLE [dbo].[SUBFLOW_PREARRANGE] ADD  CONSTRAINT [DF_FLOW_SUBPREARRANGE_flow_status]  DEFAULT ((0)) FOR [flow_status]
GO
ALTER TABLE [dbo].[SUBFLOW_PREARRANGE] ADD  CONSTRAINT [DF_FLOW_SUBPREARRANGE_flow_org_id]  DEFAULT ('') FOR [flow_org_id]
GO
ALTER TABLE [dbo].[SUBFLOW_PREARRANGE] ADD  CONSTRAINT [DF_FLOW_SUBPREARRANGE_flow_role_id]  DEFAULT ('') FOR [flow_role_id]
GO
ALTER TABLE [dbo].[SUBFLOW_PREARRANGE] ADD  CONSTRAINT [DF_FLOW_SUBPREARRANGE_flow_user_id]  DEFAULT ('') FOR [flow_user_id]
GO
ALTER TABLE [dbo].[SUBFLOW_PREARRANGE] ADD  CONSTRAINT [DF_FLOW_SUBPREARRANGE_flow_decision]  DEFAULT ((0)) FOR [flow_decision]
GO
ALTER TABLE [dbo].[SUBFLOW_PREARRANGE] ADD  CONSTRAINT [DF_FLOW_SUBPREARRANGE_flow_option]  DEFAULT ('') FOR [flow_option]
GO
ALTER TABLE [dbo].[SUBFLOW_PREARRANGE] ADD  CONSTRAINT [DF_FLOW_SUBPREARRANGE_crt_date]  DEFAULT (getdate()) FOR [crt_date]
GO
ALTER TABLE [dbo].[SUBFLOW_PREARRANGE] ADD  CONSTRAINT [DF_FLOW_SUBPREARRANGE_crt_user]  DEFAULT ('SYSOP') FOR [crt_user]
GO
ALTER TABLE [dbo].[SUBFLOW_PREARRANGE] ADD  CONSTRAINT [DF_FLOW_SUBPREARRANGE_mdf_date]  DEFAULT (getdate()) FOR [mdf_date]
GO
ALTER TABLE [dbo].[SUBFLOW_PREARRANGE] ADD  CONSTRAINT [DF_FLOW_SUBPREARRANGE_mdf_user]  DEFAULT ('SYSOP') FOR [mdf_user]
GO
/****** Object:  StoredProcedure [dbo].[usp_FLOW_GetDetail]    Script Date: 2019/8/20 下午 03:21:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		jedi_liao
-- Create date: 2017/05/17
-- Description:	get flow all the log and level
-- =============================================
CREATE PROCEDURE [dbo].[usp_FLOW_GetDetail]
	@flow_code nvarchar(40)
AS
BEGIN
	SET NOCOUNT ON;
	select has_signed,has_subflow,flow_code,row_number() over(order by sort_order asc) as sort_order,base_odr,status,status_name,org_id,org_name,role_id,role_name,user_id,user_name,user_email,memo,mdf_date,flow_decision,flow_option
	from ( 
		select cast(1 as bit) as has_signed,dbo.fn_HasSubFlow(flow_code,signed_odr) as has_subflow,flow_code,convert(decimal,signed_odr)*0.1 as sort_order,signed_odr as base_odr,
		signed_status as status,dbo.fn_GetSetParam('SignedStatus',signed_status) as status_name,signed_org_id as org_id,isnull(b.org_name,'') as org_name,
		signed_role_id as role_id,isnull(c.role_name,'') as role_name,signed_user_id as user_id,dbo.fn_GetAgentDisplay(signed_user_id) as user_name,isnull(d.user_email,'') as user_email,
		a.signed_memo as memo,a.signed_date as mdf_date,0 as flow_decision,'' as flow_option
		from dbo.FLOW_SIGNEDLOG a
		left join dbo.EMP_ORG b on b.org_id=a.signed_org_id
		left join dbo.FLOW_ROLE c on c.role_id=a.signed_role_id
		left join dbo.EMP_USER d on d.user_id=a.signed_user_id
		where flow_code=@flow_code
		union
		select cast(0 as bit) as has_signed,dbo.fn_HasSubFlow(flow_code,flow_odr) as has_subflow,flow_code,convert(decimal,flow_odr) as sort_order,flow_odr as base_odr,
		flow_status as status,dbo.fn_GetSetParam('FlowStatus',flow_status) as status_name,flow_org_id as org_id,isnull(b.org_name,'') as org_name,
		flow_role_id as role_id,isnull(c.role_name, '') as role_name,flow_user_id as user_id,d.user_name,d.user_email,
		'' as memo,a.mdf_date,a.flow_decision,a.flow_option 
		from dbo.FLOW_PREARRANGE a
		left join dbo.EMP_ORG b on b.org_id=a.flow_org_id
		left join dbo.FLOW_ROLE c on c.role_id=a.flow_role_id
		left join dbo.EMP_USER d on d.user_id=a.flow_user_id
		where flow_code=@flow_code and flow_status<=1
	) flowlog order by sort_order;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_FLOW_SetAccept]    Script Date: 2019/8/20 下午 03:21:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		jedi_liao
-- Create date: 2017/05/18
-- Upeare Date 2019/04/01 參考TCG Tracko修正流程
-- Description:	set flow pass to next task
-- =============================================

--exec dbo.usp_FLOW_SetAccept 'PJ1705186421605','ant_chen*jessie_hu','',0

CREATE PROCEDURE [dbo].[usp_FLOW_SetAccept]
	@flow_code nvarchar(40),
	@sender_user_id nvarchar(50),
	@signed_memo nvarchar(max)='',
	@signed_decision bit=0 --1:代決行全關卡
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @result table(RTN_FLAG bit,RTN_RESULT nvarchar(max));	
	DECLARE @flow_odr smallint;
	DECLARE @flow_org_id nvarchar(20);
	DECLARE @flow_role_id nvarchar(20);	
	select @flow_odr=flow_odr,@flow_role_id=flow_role_id from dbo.FLOW_PREARRANGE where flow_code=@flow_code and flow_status=1;
	IF dbo.fn_ChkFlowExecute(@flow_code)=0
		insert @result select 0,'流程已結束或不存在';
	ELSE IF dbo.fn_HasSubFlowExecute(@flow_code,@flow_odr)=1
		insert @result select 0,'尚有分會流程進行中';
	ELSE
	BEGIN
		BEGIN TRAN;
		BEGIN TRY			
			insert into dbo.FLOW_SIGNEDLOG 
				select @flow_code,@flow_odr,(select max(signed_odr)+1 from dbo.FLOW_SIGNEDLOG where flow_code=@flow_code),2,dbo.fn_GetUsersOrgId(@sender_user_id),@flow_role_id,@sender_user_id,@signed_memo,getdate();
			DECLARE @flow_signed_odr smallint=@flow_odr;
			DECLARE @flow_user_id nvarchar(20);			
			WHILE (1=1)	
			BEGIN
				SET @flow_odr+=1;
				select top 1 @flow_org_id=flow_org_id,@flow_role_id=flow_role_id,@flow_user_id=flow_user_id from dbo.FLOW_PREARRANGE where flow_code=@flow_code and flow_odr=@flow_odr;
				IF @@ROWCOUNT=0
					BREAK;
				ELSE 
				BEGIN
					IF @signed_decision=1 OR @flow_user_id in (select value from dbo.fn_Split(@sender_user_id,'*'))
					BEGIN
						insert into dbo.FLOW_SIGNEDLOG 
							select @flow_code,@flow_odr,(select max(signed_odr)+1 from dbo.FLOW_SIGNEDLOG where flow_code=@flow_code),iif(@signed_decision=1,4,2),dbo.fn_GetUsersOrgId(@sender_user_id),@flow_role_id,@sender_user_id,@signed_memo,getdate();
						SET @flow_signed_odr=@flow_odr;
					END
					ELSE
						BREAK;
				END
			END				
			print @flow_signed_odr;
			update dbo.FLOW_PREARRANGE 
				set flow_status=iif(flow_odr<=@flow_signed_odr,2,1),mdf_date=getdate(),mdf_user=@sender_user_id 
				where flow_code=@flow_code and flow_odr<=@flow_signed_odr+1 and flow_status<2;
			insert @result select 1,@flow_code;
			COMMIT TRAN;
		END TRY
		BEGIN CATCH
		   ROLLBACK TRAN;
		   delete from @result;
		   insert @result select 0,convert(nvarchar(20),ERROR_LINE())+';'+ERROR_MESSAGE();
		END CATCH 
	END
	select * from @result;
END

GO
/****** Object:  StoredProcedure [dbo].[usp_FLOW_SetActive]    Script Date: 2019/8/20 下午 03:21:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		jedi_liao
-- Create date: 2017/05/16
-- Upeare Date 2019/04/01 參考TCG Tracko修正流程
-- Description:	set flow active
-- =============================================

--exec dbo.usp_FLOW_SetActive 'flowB', 'jedi_liao'

CREATE PROCEDURE [dbo].[usp_FLOW_SetActive] 
	@flow_id nvarchar(20),
	@sender_user_id nvarchar(50),
	@flow_code nvarchar(40)='',
	@flow_odr smallint=1	
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @result table(RTN_FLAG bit,RTN_RESULT nvarchar(max));	
	SET @flow_code=iif(ltrim(rtrim(@flow_code))='', 'FL'+upper(left(dbo.fn_GetUsersOrgId(@sender_user_id),1)+left(@sender_user_id,1))+substring(convert(varchar,getdate(),112),3,6)+right(convert(decimal,convert(float,getdate())*power(10,9)),7),@flow_code);	
	IF not exists(select flow_id from dbo.FLOW_SET where flow_id=@flow_id and getdate() between effective_date and expire_date)
		insert @result select 0,'無法啟動流程，流程不存在或不在服務期間';
	ELSE
	BEGIN	
		IF @@TRANCOUNT=0
			BEGIN TRAN;
		BEGIN TRY
			DECLARE @tmp_flow_odr smallint=@flow_odr;
			DECLARE @set_odr smallint;
			DECLARE @set_flow_id nvarchar(20);	
			IF @tmp_flow_odr=1
			BEGIN
				insert into dbo.FLOW_PREARRANGE
					select @flow_id,@flow_code,@tmp_flow_odr,0,dbo.fn_GetUsersOrgId(@sender_user_id),'',@sender_user_id,0,'',getdate(),@sender_user_id,getdate(),@sender_user_id			
				SET @tmp_flow_odr+=1;
			END
			DECLARE flow_cursor CURSOR LOCAL FOR select set_odr,isnull(set_flow_id,'') from dbo.FLOW_SET_DETAIL where flow_id=@flow_id order by set_odr;	 
			OPEN flow_cursor;
			FETCH NEXT FROM flow_cursor INTO @set_odr,@set_flow_id;
			WHILE @@FETCH_STATUS=0
			BEGIN
				IF ltrim(rtrim(@set_flow_id))=''
				BEGIN
					insert into dbo.FLOW_PREARRANGE
						select @flow_id,@flow_code,@tmp_flow_odr,0,
						iif(ltrim(rtrim(set_org_id))='',dbo.fn_GetUsersOrgId(iif(ltrim(rtrim(set_user_id))='',@sender_user_id,set_user_id)),set_org_id),
						set_role_id,set_user_id,set_decision,set_option,getdate(),@sender_user_id,getdate(),@sender_user_id
						from dbo.FLOW_SET_DETAIL where flow_id=@flow_id and set_odr=@set_odr;			
					SET @tmp_flow_odr+=1;
				END
				ELSE
				BEGIN
					delete from @result;
					insert @result exec dbo.usp_FLOW_SetActive @set_flow_id,@sender_user_id,@flow_code,@tmp_flow_odr;				
					select top 1 @tmp_flow_odr=convert(smallint,RTN_RESULT) from @result;
				END
				FETCH NEXT FROM flow_cursor INTO @set_odr,@set_flow_id;
			END
			CLOSE flow_cursor;
			DEALLOCATE flow_cursor;  
			IF @flow_odr = 1 
			BEGIN
				update a set a.flow_org_id=b.org_id from dbo.FLOW_PREARRANGE a inner join dbo.V_USER_ORG b on a.flow_user_id=b.user_id where flow_code=@flow_code and ltrim(rtrim(a.flow_org_id))='';
				insert into FLOW_SIGNEDLOG select @flow_code,0,0,1,dbo.fn_GetUsersOrgId(@sender_user_id),'',@sender_user_id,'',getdate();
				update dbo.FLOW_PREARRANGE set flow_status=1,mdf_date=getdate(),mdf_user=@sender_user_id where flow_code=@flow_code and flow_odr=1;
				COMMIT TRAN;
			END	
		END TRY
		BEGIN CATCH
			IF @@TRANCOUNT>0 AND @flow_odr=1
				ROLLBACK TRAN;
			delete from @result;
			insert @result select 0,convert(nvarchar(20),ERROR_LINE())+';'+ERROR_MESSAGE();
		END CATCH  
		IF NOT EXISTS(select RTN_FLAG from @result where RTN_FLAG=0)
		BEGIN
			delete from @result;
			insert @result select 1,iif(@flow_odr=1,@flow_code,convert(nvarchar(40),@tmp_flow_odr));
		END
	END
	select * from @result;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_FLOW_SetDeactive]    Script Date: 2019/8/20 下午 03:21:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		jedi_liao
-- Create date: 2017/05/17
-- Description:	cancel the flow
-- =============================================

--exec dbo.usp_FLOW_SetDeactive 'PJ1705188614043','jedi_liao'

CREATE PROCEDURE [dbo].[usp_FLOW_SetDeactive]
	@flow_code nvarchar(40),
	@sender_user_id nvarchar(50),
	@signed_memo nvarchar(max)=''
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @result table(RTN_FLAG bit,RTN_RESULT nvarchar(max));	
	IF dbo.fn_ChkFlowExecute(@flow_code)=0
		insert @result select 0,'流程已結束或不存在'; 
	ELSE
	BEGIN
		BEGIN TRAN;
		BEGIN TRY
			insert into dbo.FLOW_SIGNEDLOG 
				select flow_code,flow_odr,(select max(signed_odr)+1 from dbo.FLOW_SIGNEDLOG where flow_code=@flow_code),9,dbo.fn_GetUsersOrgId(@sender_user_id),'',@sender_user_id,@signed_memo,getdate()
				from FLOW_PREARRANGE where flow_code=@flow_code and flow_status=1;;
			update dbo.FLOW_PREARRANGE set flow_status=9,mdf_date=getdate(),mdf_user=@sender_user_id where flow_code=@flow_code and flow_status<=1;
			insert into dbo.FLOW_SIGNEDLOG 
				select sub_flow_code,flow_odr,(select max(signed_odr)+1 from dbo.FLOW_SIGNEDLOG where flow_code=a.sub_flow_code),9,dbo.fn_GetUsersOrgId(@sender_user_id),'',@sender_user_id,@signed_memo,getdate()
				from dbo.SUBFLOW_PREARRANGE a where flow_code=@flow_code and flow_status=1;
			update dbo.SUBFLOW_PREARRANGE set flow_status=9,mdf_date=getdate(),mdf_user=@sender_user_id where flow_code=@flow_code and flow_status<=1;
			insert @result select 1,@flow_code;
			COMMIT TRAN;
		END TRY
		BEGIN CATCH
			ROLLBACK TRAN;
			insert @result select 0,convert(nvarchar(20),ERROR_LINE())+';'+ERROR_MESSAGE();
		END CATCH   
	END
	select * from @result; 
END
GO
/****** Object:  StoredProcedure [dbo].[usp_FLOW_SetReject]    Script Date: 2019/8/20 下午 03:21:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		jedi_liao
-- Create date: 2017/06/03
-- Description:	set flow reject to last task or started task
-- =============================================

--exec dbo.usp_FLOW_SetReject 'PJ1705186421605', 'ant_chen*jessie_hu', '', 0

CREATE PROCEDURE [dbo].[usp_FLOW_SetReject]
	@flow_code nvarchar(40),
	@sender_user_id nvarchar(50),
	@signed_memo nvarchar(max)='',
	@refuseAll bit=0 --1:無條件回到第一關
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @result table(RTN_FLAG bit,RTN_RESULT nvarchar(max));	
	DECLARE @flow_odr smallint;
	DECLARE @flow_role_id nvarchar(20);
	select @flow_odr=flow_odr,@flow_role_id=flow_role_id from dbo.FLOW_PREARRANGE where flow_code=@flow_code and flow_status=1;		
	IF dbo.fn_ChkFlowExecute(@flow_code)=0
		insert @result select 0,'流程已結束或不存在'; 
	ELSE IF dbo.fn_HasSubFlowExecute(@flow_code,@flow_odr)=1
		insert @result select 0,'尚有分會流程進行中'; 
	ELSE
	BEGIN	
		DECLARE @flow_user_id nvarchar(20);			
		BEGIN TRAN;
		BEGIN TRY
			insert into dbo.FLOW_SIGNEDLOG 
				select @flow_code,@flow_odr,(select max(signed_odr)+1 from dbo.FLOW_SIGNEDLOG where flow_code=@flow_code),iif(@refuseAll=1,5,3),dbo.fn_GetUsersOrgId(@sender_user_id),@flow_role_id,@sender_user_id,@signed_memo,getdate();
			SET @flow_odr=iif(@refuseAll=1,iif(@flow_odr=1,0,1),@flow_odr-1);	
			IF @flow_odr=0
				insert @result exec dbo.usp_FLOW_SetDeactive @flow_code,@sender_user_id;
			ELSE
			BEGIN
				update dbo.FLOW_PREARRANGE 
					set flow_status=iif(flow_odr>@flow_odr,0,1),mdf_date=getdate(),mdf_user=@sender_user_id 
					where flow_code=@flow_code and flow_odr>=@flow_odr and flow_status<9;
				insert @result select 1,@flow_code; 
			END
			COMMIT TRAN;
		END TRY
		BEGIN CATCH
		   ROLLBACK TRAN;
		   delete from @result;
		   insert @result select 0,convert(nvarchar(20),ERROR_LINE())+';'+ERROR_MESSAGE();
		END CATCH 
	END
	select top 1 * from @result;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_ResetDB]    Script Date: 2019/8/20 下午 03:21:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		reset db data
-- Create date: 2017/07/12
-- Description:	jedi_liao
-- =============================================
CREATE PROCEDURE [dbo].[usp_ResetDB]
AS
BEGIN
	SET NOCOUNT ON;
	truncate table dbo.SQL_TRACE;
	truncate table dbo.MAIL_LOG;
	truncate table dbo.SIGNEDLOG;
	truncate table dbo.FLOW_PREARRANGE;
	truncate table dbo.SUBFLOW_PREARRANGE;
	truncate table dbo.EMP_AGENT;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_ResetSqlLog]    Script Date: 2019/8/20 下午 03:21:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Jedi Liao
-- Create date: 2017/01/01
-- Description:	backup sql trace log for last month
-- =============================================
CREATE PROCEDURE [dbo].[usp_ResetSqlLog]
AS
BEGIN
	SET NOCOUNT ON;
	IF exists(select 1 from dbo.SQL_TRACE where datediff(month,log_date,getdate())>0) 
	BEGIN
		DECLARE @bakTable nvarchar(20)='SQL_TRACE_BAK'+left(convert(varchar,dateadd(month,-1,getdate()),112),6); 
		DECLARE @cmd nvarchar(max)='';
		IF exists(select 1 from INFORMATION_SCHEMA.TABLES where TABLE_TYPE='BASE TABLE' and TABLE_NAME=@bakTable)  
			SET @cmd='drop table [SQL_TRACE_BAK'+left(convert(varchar,dateadd(month,-1,getdate()),112),6)+'];';
		SET @cmd+='select * into '+@bakTable+' from dbo.SQL_TRACE where datediff(month,log_date,getdate())>0;';
		EXECUTE sp_executesql @cmd;
		truncate table dbo.SQL_TRACE;
	END
END
GO
/****** Object:  StoredProcedure [dbo].[usp_SUBFLOW_GetDetail]    Script Date: 2019/8/20 下午 03:21:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		jedi_liao
-- Create date: 2017/10/20
-- Description:	get subflow all the log and level
-- =============================================
CREATE PROCEDURE [dbo].[usp_SUBFLOW_GetDetail]
	@sub_flow_code nvarchar(40)
AS
BEGIN
	SET NOCOUNT ON;
	select has_signed,flow_code,row_number() over(order by sort_order asc) as sort_order,status,status_name,org_id,org_name,role_id,role_name,user_id,user_name,user_email,memo,mdf_date,flow_decision,flow_option
	from ( 
		select cast(1 as bit) as has_signed,flow_code,convert(decimal,signed_odr)*0.1 as sort_order,
		signed_status as status,dbo.fn_GetSetParam('SignedStatus',signed_status) as status_name,signed_org_id as org_id,isnull(b.org_name,'') as org_name,
		signed_role_id as role_id,isnull(c.role_name,'') as role_name,signed_user_id as user_id,dbo.fn_GetAgentDisplay(signed_user_id) as user_name,isnull(d.user_email,'') as user_email,
		iif(a.signed_odr>0,a.signed_memo,'') as memo,a.signed_date as mdf_date,0 as flow_decision,'' as flow_option 
		from dbo.FLOW_SIGNEDLOG a
		left join dbo.EMP_ORG b on b.org_id=a.signed_org_id
		left join dbo.FLOW_ROLE c on c.role_id=a.signed_role_id
		left join dbo.EMP_USER d on d.user_id=a.signed_user_id
		where flow_code=@sub_flow_code
		union
		select cast(0 as bit) as has_signed,sub_flow_code,convert(decimal,sub_flow_odr) as sort_order,
		flow_status as status,dbo.fn_GetSetParam('FlowStatus',flow_status) as status_name,flow_org_id as org_id,isnull(b.org_name,'') as org_name,
		flow_role_id as role_id,isnull(c.role_name, '') as role_name,flow_user_id as user_id,d.user_name,d.user_email,
		'' as memo,a.mdf_date,a.flow_decision,a.flow_option 
		from dbo.SUBFLOW_PREARRANGE a
		left join dbo.EMP_ORG b on b.org_id=a.flow_org_id
		left join dbo.FLOW_ROLE c on c.role_id=a.flow_role_id
		left join dbo.EMP_USER d on d.user_id=a.flow_user_id
		where sub_flow_code=@sub_flow_code and flow_status<=1
	) flowlog order by sort_order;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_SUBFLOW_SetAccept]    Script Date: 2019/8/20 下午 03:21:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		jedi_liao
-- Create date: 2017/10/18
-- Description:	set subflow pass to next task
-- =============================================
CREATE PROCEDURE [dbo].[usp_SUBFLOW_SetAccept]
	@sub_flow_code nvarchar(40),
	@sender_user_id nvarchar(50),
	@signed_memo nvarchar(max)=''
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @result table(RTN_FLAG bit,RTN_RESULT nvarchar(max));	
	IF dbo.fn_ChkSubFlowExecute(@sub_flow_code)=0
		insert @result select 0,'流程已結束或不存在';
	ELSE
	BEGIN
		BEGIN TRAN;
		BEGIN TRY
			DECLARE @sub_flow_odr smallint;
			DECLARE @flow_role_id nvarchar(20);	
			select @sub_flow_odr=sub_flow_odr,@flow_role_id=flow_role_id from dbo.SUBFLOW_PREARRANGE where sub_flow_code=@sub_flow_code and flow_status=1
			insert into dbo.FLOW_SIGNEDLOG 
				select @sub_flow_code,@sub_flow_odr,(select max(signed_odr)+1 from dbo.FLOW_SIGNEDLOG where flow_code=@sub_flow_code),2,dbo.fn_GetUsersOrgId(@sender_user_id),@flow_role_id,@sender_user_id,@signed_memo,getdate();
			DECLARE @flow_signed_odr smallint=@sub_flow_odr;
			DECLARE @flow_user_id nvarchar(20);
			WHILE (1=1)	
			BEGIN
				SET @sub_flow_odr+=1
				print @flow_signed_odr
				select top 1 @flow_role_id=flow_role_id,@flow_user_id=flow_user_id from dbo.SUBFLOW_PREARRANGE where sub_flow_code=@sub_flow_code and sub_flow_odr=@sub_flow_odr
				IF @@ROWCOUNT=0
					BREAK;
				ELSE 
				BEGIN
					IF @flow_user_id in (select value from dbo.fn_Split(@sender_user_id,'*'))
					BEGIN
						insert into dbo.FLOW_SIGNEDLOG select @sub_flow_code,@sub_flow_odr,(select max(signed_odr)+1 from dbo.FLOW_SIGNEDLOG where flow_code=@sub_flow_code),2,dbo.fn_GetUsersOrgId(@sender_user_id),@flow_role_id,@sender_user_id,@signed_memo,getdate();
						SET @flow_signed_odr=@sub_flow_odr;
					END
					ELSE
						BREAK;
				END
			END				
			update dbo.SUBFLOW_PREARRANGE 
				set flow_status=iif(sub_flow_odr<=@flow_signed_odr,2,1),mdf_date=getdate(),mdf_user=@sender_user_id 
				where sub_flow_code=@sub_flow_code and sub_flow_odr<=@flow_signed_odr+1 and flow_status<2;
			insert @result select 1,@sub_flow_code;
			COMMIT TRAN;
		END TRY
		BEGIN CATCH
		   ROLLBACK TRAN;
		   insert @result select 0,convert(nvarchar(20),ERROR_LINE())+';'+ERROR_MESSAGE();
		END CATCH 
	END
	select * from @result;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_SUBFLOW_SetActive]    Script Date: 2019/8/20 下午 03:21:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		jedi_liao
-- Create date: 2017/10/16
-- Description:	set sub flow active
-- =============================================

CREATE PROCEDURE [dbo].[usp_SUBFLOW_SetActive] 
	@flow_id nvarchar(20)='',
	@sender_user_id nvarchar(50),
	@flow_user_id nvarchar(50),
	@flow_code nvarchar(40),
	@sub_flow_code nvarchar(40)='',
	@flow_odr smallint=1	
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @result table(RTN_FLAG bit,RTN_RESULT nvarchar(max));	
	SET @sub_flow_code=iif(ltrim(rtrim(@sub_flow_code))='', 'SFL'+upper(left(dbo.fn_GetUsersOrgId(@flow_user_id),1)+left(@flow_user_id,1))+substring(convert(varchar,getdate(),112),3,6)+right(convert(decimal,convert(float,getdate())*power(10,9)),7),@sub_flow_code);	
	IF len(ltrim(rtrim(@flow_id)))>0 and not exists(select flow_id from dbo.FLOW_SET where flow_id=@flow_id and getdate() between effective_date and expire_date)
	BEGIN
		insert @result select 0,'無法啟動分會流程，流程不存在或不在服務期間';
	END
	BEGIN
		IF @@TRANCOUNT=0
			BEGIN TRAN;
		BEGIN TRY
			DECLARE @flow_main_odr smallint=(select top 1 flow_odr from dbo.FLOW_PREARRANGE where flow_code=@flow_code and flow_status=1);
			DECLARE @tmp_flow_odr smallint=@flow_odr;
			insert into dbo.SUBFLOW_PREARRANGE
				select @flow_id,@flow_code,@flow_main_odr,@sub_flow_code,@tmp_flow_odr,0,dbo.fn_GetUsersOrgId(@flow_user_id),'',@flow_user_id,'','',getdate(),@sender_user_id,getdate(),@sender_user_id;		
			SET @tmp_flow_odr+=1;
			IF len(ltrim(rtrim(@flow_id)))>0 
			BEGIN
				DECLARE @set_odr smallint;
				DECLARE @set_flow_id nvarchar(20);	
				DECLARE flow_cursor CURSOR LOCAL FOR select set_odr,isnull(set_flow_id,'') from dbo.FLOW_SET_DETAIL where flow_id=@flow_id order by set_odr;	 
				OPEN flow_cursor;
				FETCH NEXT FROM flow_cursor INTO @set_odr,@set_flow_id;
				WHILE @@FETCH_STATUS=0
				BEGIN
					IF ltrim(rtrim(@set_flow_id))=''
					BEGIN
						insert into dbo.SUBFLOW_PREARRANGE
							select @flow_id,@flow_code,@flow_main_odr,@sub_flow_code,@tmp_flow_odr,0,set_org_id,set_role_id, 
							iif(ltrim(rtrim(set_user_id))='',dbo.fn_GetFlowMapUser(iif(ltrim(rtrim(set_org_id))='',dbo.fn_GetUsersOrgId(@flow_user_id),set_org_id),set_role_id),set_user_id),
							isnull(set_decision,''),isnull(set_option,''),getdate(),@sender_user_id,getdate(),@sender_user_id
							from dbo.FLOW_SET_DETAIL where flow_id=@flow_id and set_odr=@set_odr;			
						SET @tmp_flow_odr+=1;
					END
					ELSE
					BEGIN
						delete from @result;
						insert @result exec dbo.usp_SUBFLOW_SetActive @set_flow_id,@sender_user_id,@flow_user_id,@flow_code,@sub_flow_code,@tmp_flow_odr;				
						select top 1 @tmp_flow_odr=convert(smallint,RTN_RESULT) from @result;
					END
					FETCH NEXT FROM flow_cursor INTO @set_odr,@set_flow_id;
				END
				CLOSE flow_cursor;
				DEALLOCATE flow_cursor;  
			END
			IF @flow_odr = 1 
			BEGIN
				update a set a.flow_org_id=b.org_id from dbo.SUBFLOW_PREARRANGE a inner join dbo.V_USER_ORG b on a.flow_user_id=b.user_id where sub_flow_code=@sub_flow_code and ltrim(rtrim(a.flow_org_id))='';
				insert into FLOW_SIGNEDLOG select @sub_flow_code,0,0,1,dbo.fn_GetUsersOrgId(@sender_user_id),'',@sender_user_id,@flow_main_odr,getdate();
				update dbo.SUBFLOW_PREARRANGE set flow_status=1,mdf_date=getdate(),mdf_user=@sender_user_id where sub_flow_code=@sub_flow_code and sub_flow_odr=1;
				COMMIT TRAN;
			END	
		END TRY
		BEGIN CATCH
			print @@TRANCOUNT
			IF @@TRANCOUNT>0 AND @flow_odr=1
				ROLLBACK TRAN;
			delete from @result;
			insert @result select 0,convert(nvarchar(20),ERROR_LINE())+';'+ERROR_MESSAGE();
		END CATCH 
		IF NOT EXISTS(select RTN_FLAG from @result where RTN_FLAG=0)
		BEGIN
			delete from @result;
			insert @result select 1,iif(@flow_odr=1,@sub_flow_code,convert(nvarchar(40),@tmp_flow_odr));
		END 
	END
	select * from @result;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_SUBFLOW_SetDeactive]    Script Date: 2019/8/20 下午 03:21:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		jedi_liao
-- Create date: 2017/10/17
-- Description:	cancel the sub flow
-- =============================================
CREATE PROCEDURE [dbo].[usp_SUBFLOW_SetDeactive]
	@sub_flow_code nvarchar(40),
	@sender_user_id nvarchar(50),
	@signed_memo nvarchar(max)=''
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @result table(success bit,[message] nvarchar(max));	
	IF dbo.fn_ChkSubFlowExecute(@sub_flow_code)=0
		insert @result select 0,'分會流程已結束或不存在'; 
	ELSE
	BEGIN
		BEGIN TRAN;
		BEGIN TRY
			insert into dbo.FLOW_SIGNEDLOG 
				select sub_flow_code,flow_odr,(select max(signed_odr)+1 from dbo.FLOW_SIGNEDLOG where flow_code=a.sub_flow_code),9,dbo.fn_GetUsersOrgId(@sender_user_id),'',@sender_user_id,@signed_memo,getdate()
				from dbo.SUBFLOW_PREARRANGE a where sub_flow_code=@sub_flow_code and flow_status=1;
			update dbo.SUBFLOW_PREARRANGE set flow_status=9,mdf_date=getdate(),mdf_user=@sender_user_id where sub_flow_code=@sub_flow_code and flow_status<=1;
			insert @result select 1,@sub_flow_code;
			COMMIT TRAN;
		END TRY
		BEGIN CATCH
			ROLLBACK TRAN;
			insert @result select 0,convert(nvarchar(20),ERROR_LINE())+';'+ERROR_MESSAGE();
		END CATCH   
	END
	select * from @result; 
END
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "MAIL_RECIPIENT"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 136
               Right = 203
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "MAP_USER_MAILROLE"
            Begin Extent = 
               Top = 6
               Left = 241
               Bottom = 136
               Right = 406
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "EMP_USER"
            Begin Extent = 
               Top = 6
               Left = 444
               Bottom = 136
               Right = 609
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_MAIL_RECIPIENT'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_MAIL_RECIPIENT'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "a"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 136
               Right = 210
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "b"
            Begin Extent = 
               Top = 6
               Left = 248
               Bottom = 136
               Right = 413
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "c"
            Begin Extent = 
               Top = 6
               Left = 451
               Bottom = 136
               Right = 616
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "d"
            Begin Extent = 
               Top = 6
               Left = 654
               Bottom = 136
               Right = 819
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "e"
            Begin Extent = 
               Top = 6
               Left = 857
               Bottom = 136
               Right = 1022
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "f"
            Begin Extent = 
               Top = 138
               Left = 38
               Bottom = 268
               Right = 209
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_SUBFLOW_ACTIVELIST'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_SUBFLOW_ACTIVELIST'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_SUBFLOW_ACTIVELIST'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "a"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 136
               Right = 210
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "b"
            Begin Extent = 
               Top = 6
               Left = 248
               Bottom = 136
               Right = 413
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "c"
            Begin Extent = 
               Top = 6
               Left = 451
               Bottom = 136
               Right = 616
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "d"
            Begin Extent = 
               Top = 6
               Left = 654
               Bottom = 136
               Right = 819
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "e"
            Begin Extent = 
               Top = 6
               Left = 857
               Bottom = 136
               Right = 1022
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "f"
            Begin Extent = 
               Top = 138
               Left = 38
               Bottom = 268
               Right = 209
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 20
         Width = 284
         Width = 1695
         Width = 1500
         Width = 1500
         Width = 1500' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_SUBFLOW_LIST'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_SUBFLOW_LIST'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_SUBFLOW_LIST'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "EMP_USER"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 136
               Right = 203
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "MAP_USER_FLOWROLE"
            Begin Extent = 
               Top = 6
               Left = 241
               Bottom = 136
               Right = 406
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "MAP_ORG_USER"
            Begin Extent = 
               Top = 6
               Left = 444
               Bottom = 136
               Right = 609
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_USER_FLOWROLE'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_USER_FLOWROLE'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "EMP_USER"
            Begin Extent = 
               Top = 7
               Left = 48
               Bottom = 165
               Right = 239
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "MAP_USER_ROLE"
            Begin Extent = 
               Top = 7
               Left = 287
               Bottom = 165
               Right = 478
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "MAP_ROLE_RIGHT"
            Begin Extent = 
               Top = 7
               Left = 526
               Bottom = 165
               Right = 717
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "MAP_RIGHT_FUNCTION"
            Begin Extent = 
               Top = 7
               Left = 765
               Bottom = 165
               Right = 956
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "SET_FUNCTION"
            Begin Extent = 
               Top = 7
               Left = 1004
               Bottom = 165
               Right = 1202
            End
            DisplayFlags = 280
            TopColumn = 5
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
   ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_USER_FUNCTION'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'      Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_USER_FUNCTION'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_USER_FUNCTION'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[41] 4[20] 2[15] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "EMP_USER"
            Begin Extent = 
               Top = 7
               Left = 48
               Bottom = 165
               Right = 239
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "MAP_ORG_USER"
            Begin Extent = 
               Top = 7
               Left = 287
               Bottom = 165
               Right = 478
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "EMP_ORG"
            Begin Extent = 
               Top = 7
               Left = 526
               Bottom = 165
               Right = 717
            End
            DisplayFlags = 280
            TopColumn = 3
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 10
         Width = 284
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_USER_ORG'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_USER_ORG'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "EMP_USER"
            Begin Extent = 
               Top = 7
               Left = 48
               Bottom = 165
               Right = 239
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "MAP_USER_ROLE"
            Begin Extent = 
               Top = 7
               Left = 287
               Bottom = 165
               Right = 478
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "MAP_ROLE_RIGHT"
            Begin Extent = 
               Top = 7
               Left = 526
               Bottom = 165
               Right = 717
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "DIM_RIGHT"
            Begin Extent = 
               Top = 7
               Left = 765
               Bottom = 165
               Right = 956
            End
            DisplayFlags = 280
            TopColumn = 2
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_USER_RIGHT'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_USER_RIGHT'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_USER_RIGHT'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "EMP_USER"
            Begin Extent = 
               Top = 7
               Left = 48
               Bottom = 165
               Right = 239
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "MAP_USER_ROLE"
            Begin Extent = 
               Top = 7
               Left = 287
               Bottom = 165
               Right = 478
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "DIM_ROLE"
            Begin Extent = 
               Top = 7
               Left = 526
               Bottom = 165
               Right = 717
            End
            DisplayFlags = 280
            TopColumn = 2
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_USER_ROLE'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_USER_ROLE'
GO
USE [master]
GO
ALTER DATABASE [MACSD_MVC_M] SET  READ_WRITE 
GO
