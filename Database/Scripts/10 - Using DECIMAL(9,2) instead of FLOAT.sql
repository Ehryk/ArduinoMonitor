
USE ArduinoMonitor

ALTER TABLE SensorData ALTER COLUMN TempCelsius decimal(9,2)
ALTER TABLE SensorData ALTER COLUMN TempFahrenheit decimal(9,2)
ALTER TABLE SensorData ALTER COLUMN Humidity decimal(9,2)
ALTER TABLE SensorData ALTER COLUMN Light decimal(9,2)

GO

ALTER PROCEDURE [dbo].[SensorDataInsert]
(
	@Arduino		int,

	@Date			datetime = null,
	@TempCelsius	decimal(9,2) = NULL,
	@TempFahrenheit	decimal(9,2) = NULL,
	@Humidity		decimal(9,2) = NULL,
	@Light			decimal(9,2) = NULL
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

USE [ArduinoMonitor]
GO

ALTER PROCEDURE [dbo].[SensorDataGet]
(
	@ID				int = NULL,
	@Arduino		int = NULL,

	@Year			int = NULL,
	@RangeStart		datetime = NULL,
	@RangeEnd		datetime = NULL,

	@RangeCMin		decimal(9,2) = NULL,
	@RangeCMax		decimal(9,2) = NULL,
	@RangeFMin		decimal(9,2) = NULL,
	@RangeFMax		decimal(9,2) = NULL,
	@RangeHMin		decimal(9,2) = NULL,
	@RangeHMax		decimal(9,2) = NULL,
	@RangeLMin		decimal(9,2) = NULL,
	@RangeLMax		decimal(9,2) = NULL,
	
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

