
USE ArduinoMonitor

GO

/*
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Arduinos]') AND type in (N'U'))
DROP TABLE [dbo].[Arduinos]
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[ArduinoView]'))
DROP VIEW [dbo].[ArduinoView]
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ArduinoGet]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[ArduinoGet]
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ArduinosGet]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[ArduinosGet]
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ArduinoInsert]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[ArduinoInsert]
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ArduinoUpdate]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[ArduinoUpdate]
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ArduinoDelete]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[ArduinoDelete]
*/

GO

CREATE TABLE [dbo].[Arduinos]
(
	ID int identity(1,1) primary key,

	BuildDate datetime null,
	Name nvarchar(100),

	Location nvarchar(max),
	Sensors nvarchar(max),
	Builder nvarchar(100),

	Comment nvarchar(max),
	Deleted bit default 0,

	CreateDate datetime,
	UpdateDate datetime,
	DeleteDate datetime
)

GO

CREATE VIEW ArduinoView
AS
	
	SELECT
		*
	FROM
		Arduinos
	WHERE
		Deleted = 0

GO

CREATE PROCEDURE ArduinoGet
(
	@ID		int
)
AS
BEGIN

	SELECT
		*
	FROM
		ArduinoView
	WHERE
		ID = @ID;

END

GO

CREATE PROCEDURE ArduinosGet
(
	@ID				int = NULL,
	@Name			nvarchar(100) = NULL,

	@RangeStart		datetime = NULL,
	@RangeEnd		datetime = NULL,
	
	@Location		nvarchar(100) = NULL,
	@Sensors		nvarchar(100) = NULL,
	@Deleted		bit = 0
)
AS
BEGIN

	SELECT
		*
	FROM
		ArduinoView
	WHERE
		(@ID is null OR ID = @ID) AND
		(@Name is null or Name like @Name) AND

		(@RangeStart is null OR BuildDate >= @RangeStart) AND
		(@RangeEnd is null OR BuildDate <= @RangeEnd) AND

		(@Location is null OR Location like '%'+@Location+'%') AND
		(@Sensors is null OR Sensors like '%'+@Sensors+'%') AND
		(@Deleted is null OR Deleted = @Deleted)

END

GO

CREATE PROCEDURE ArduinoInsert
(
	@BuildDate	datetime = null,
	@Name		nvarchar(100),

	@Location	nvarchar(max) = null,
	@Sensors	nvarchar(max) = null,
	@Builder	nvarchar(100) = null,

	@Comment	nvarchar(max) = NULL
)
AS
	INSERT INTO Arduinos
	(
		BuildDate,
		Name,

		Location,
		Sensors,
		Builder,

		Comment,

		CreateDate
	)
	OUTPUT
		INSERTED.ID
	VALUES
	(
		@BuildDate,
		@Name,
		
		@Location,
		@Sensors,
		@Builder,

		@Comment,

		GETDATE()
	)

GO

CREATE PROCEDURE [ArduinoUpdate]
(
	@ID			int,

	@BuildDate	datetime,
	@Name		nvarchar(100),

	@Location	nvarchar(max),
	@Sensors	nvarchar(max),
	@Builder	nvarchar(100),

	@Comment	nvarchar(max)
)
AS
BEGIN
	
	UPDATE 
		Arduinos
	SET
		BuildDate = @BuildDate,
		Name = @Name,

		Location = @Location,
		Sensors = @Sensors,
		Builder = @Builder,

		Comment = @Comment,

		UpdateDate = GETDATE()
	WHERE
		ID = @ID

END

GO

CREATE PROCEDURE [ArduinoDelete]
(
	@ID		int
)
AS

	UPDATE
		Arduinos
	SET
		Deleted = 1,
		DeleteDate = GETDATE()
	WHERE
		ID = @ID

GO

GRANT EXECUTE ON ArduinoGet    TO ArduinoMonitor
GRANT EXECUTE ON ArduinosGet   TO ArduinoMonitor
GRANT EXECUTE ON ArduinoInsert TO ArduinoMonitor
GRANT EXECUTE ON ArduinoUpdate TO ArduinoMonitor
GRANT EXECUTE ON ArduinoDelete TO ArduinoMonitor

GO
