
USE ArduinoMonitor

GO

/*
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EventLog]') AND type in (N'U'))
DROP TABLE [dbo].[EventLog]
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[EventView]'))
DROP VIEW [dbo].[EventView]
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EventGet]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[EventGet]
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EventsGet]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[EventsGet]
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EventInsert]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[EventInsert]
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EventUpdate]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[EventUpdate]
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EventDelete]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[EventDelete]
*/

GO

CREATE TABLE [dbo].[EventLog]
(
	ID int identity(1,1) primary key,

	[Year] int not null,
	Date datetime not null,
	Arduino int null,
	
	Source nvarchar(100),
	Type nvarchar(100),
	Message nvarchar(max),

	IsException bit not null default(0),
	Exception nvarchar(max),
	StackTrace nvarchar(max),

	Comment nvarchar(max),
	Deleted bit default 0
)

GO

ALTER TABLE [dbo].[EventLog] WITH CHECK ADD CONSTRAINT [FK__EventLog__Arduinos] FOREIGN KEY(Arduino)
REFERENCES [dbo].[Arduinos] ([ID])

GO

CREATE VIEW EventView
AS
	
	SELECT
		*
	FROM
		EventLog
	WHERE
		Deleted = 0

GO

CREATE PROCEDURE EventGet
(
	@ID		int
)
AS
BEGIN

	SELECT
		*
	FROM
		EventView
	WHERE
		ID = @ID;

END

GO

CREATE PROCEDURE EventsGet
(
	@ID			int = NULL,
	@Arduino	int = NULL,

	@Year		int = NULL,
	@RangeStart	datetime = NULL,
	@RangeEnd	datetime = NULL,
	
	@Type		nvarchar(100) = NULL,
	@Deleted	bit = 0
)
AS
BEGIN

	SELECT
		*
	FROM
		EventView
	WHERE
		(@ID is null OR ID = @ID) AND
		(@Arduino is null OR Arduino = @Arduino) AND

		(@Year is null OR [Year] = @Year) AND
		(@RangeStart is null OR Date >= @RangeStart) AND
		(@RangeEnd is null OR Date <= @RangeEnd) AND

		(@Type is null OR Type = @Type) AND
		(@Deleted is null OR Deleted = @Deleted)

END

GO

CREATE PROCEDURE EventInsert
(
	@Date datetime = null,
	@Arduino int = null,

	@Source nvarchar(100),
	@Type nvarchar(100),
	@Message nvarchar(max),

	@IsException bit = 0,
	@Exception nvarchar(max) = NULL,
	@StackTrace nvarchar(max) = NULL
)
AS
	IF @Date is null SET @Date = GETDATE()

	INSERT INTO EventLog
	(
		[Year],
		Date,
		Arduino,

		Source,
		Type,
		Message,

		IsException,
		Exception,
		StackTrace
	)
	OUTPUT
		INSERTED.ID
	VALUES
	(
		Year(@Date),
		@Date,
		@Arduino,
		
		@Source,
		@Type,
		@Message,

		@IsException,
		@Exception,
		@StackTrace
	)
GO

CREATE PROCEDURE [EventUpdate]
(
	@ID			INT,
	@Type		NVARCHAR(100),
	@Message	NVARCHAR(MAX),
	@Comment	NVARCHAR(MAX)
)
AS
BEGIN
	
	UPDATE 
		EventLog
	SET
		Type = @Type,
		Message = @Message,
		Comment = @Comment
	WHERE
		ID = @ID;

END

GO

CREATE PROCEDURE [EventDelete]
(
	@ID		int
)
AS

	UPDATE
		EventLog
	SET
		Deleted = 1
	WHERE
		ID = @ID

GO

GRANT EXECUTE ON EventGet    TO ArduinoMonitor
GRANT EXECUTE ON EventsGet   TO ArduinoMonitor
GRANT EXECUTE ON EventInsert TO ArduinoMonitor
GRANT EXECUTE ON EventUpdate TO ArduinoMonitor
GRANT EXECUTE ON EventDelete TO ArduinoMonitor

GO
