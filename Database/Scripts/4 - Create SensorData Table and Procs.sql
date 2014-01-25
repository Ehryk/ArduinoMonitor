
USE ArduinoMonitor

GO

/*
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SensorData]') AND type in (N'U'))
DROP TABLE [dbo].[SensorData]
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[SensorDataView]'))
DROP VIEW [dbo].[SensorDataView]
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SensorDataGet]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SensorDataGet]
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SensorDataInsert]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SensorDataInsert]
*/

GO

CREATE TABLE [dbo].[SensorData]
(
	ID int identity(1,1) primary key,
	Arduino int not null,

	[Year] int not null,
	Date datetime not null,

	TempCelsius float,
	TempFahrenheit float,
	Humidity float,
	Light float
)

GO

ALTER TABLE [dbo].[SensorData] WITH CHECK ADD CONSTRAINT [FK__SensorData__Arduino] FOREIGN KEY(Arduino)
REFERENCES [dbo].[Arduinos] ([ID])

GO

CREATE VIEW SensorDataView
AS
	
	SELECT
		s.*,
		a.BuildDate,
		a.Location,
		a.Sensors,
		a.Comment
	FROM
		SensorData s LEFT OUTER JOIN
		Arduinos a on a.ID = s.Arduino

GO

CREATE PROCEDURE SensorDataGet
(
	@ID				int = NULL,
	@Arduino		int = NULL,

	@Year			int = NULL,
	@RangeStart		datetime = NULL,
	@RangeEnd		datetime = NULL,

	@RangeCMin		float = NULL,
	@RangeCMax		float = NULL,
	@RangeFMin		float = NULL,
	@RangeFMax		float = NULL,
	@RangeHMin		float = NULL,
	@RangeHMax		float = NULL,
	@RangeLMin		float = NULL,
	@RangeLMax		float = NULL,
	
	@Location		nvarchar(100) = NULL
)
AS
BEGIN

	SELECT
		*
	FROM
		SensorDataView
	WHERE
		(@ID is null OR ID = @ID) AND
		(@Arduino is null OR Arduino = @Arduino) AND

		(@Year is null OR [Year] = @Year) AND
		(@RangeStart is null OR Date >= @RangeStart) AND
		(@RangeEnd is null   OR Date <= @RangeEnd) AND
		
		(@RangeCMin is null  OR TempCelsius >= @RangeCMin) AND
		(@RangeCMax is null  OR TempCelsius <= @RangeCMax) AND
		(@RangeFMin is null  OR TempFahrenheit >= @RangeFMin) AND
		(@RangeFMax is null  OR TempFahrenheit <= @RangeFMax) AND
		(@RangeHMin is null  OR Humidity >= @RangeHMin) AND
		(@RangeHMax is null  OR Humidity <= @RangeHMax) AND
		(@RangeLMin is null  OR Light >= @RangeLMin) AND
		(@RangeLMax is null  OR Light <= @RangeLMax) AND

		(@Location is null OR Location like '%'+@Location+'%')

END

GO

CREATE PROCEDURE SensorDataInsert
(
	@Arduino		int,

	@Date			datetime = null,
	@TempCelsius	float = NULL,
	@TempFahrenheit	float = NULL,
	@Humidity		float = NULL,
	@Light			float = NULL
)
AS
	IF @Date is null SET @Date = GETDATE()

	INSERT INTO SensorData
	(
		Arduino,
		
		[Year],
		Date,

		TempCelsius,
		TempFahrenheit,
		Humidity,
		Light
	)
	OUTPUT
		INSERTED.ID
	VALUES
	(
		@Arduino,

		Year(@Date),
		@Date,
		
		@TempCelsius,
		@TempFahrenheit,
		@Humidity,
		@Light
	)

GO

GRANT EXECUTE ON SensorDataGet    TO ArduinoMonitor
GRANT EXECUTE ON SensorDataInsert TO ArduinoMonitor

GO
