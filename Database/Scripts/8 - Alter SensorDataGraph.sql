
USE [ArduinoMonitor]

GO

ALTER PROCEDURE [dbo].[SensorDataGraph]
(
	@Arduino	int = NULL,
	@Year		int = NULL,
	@RangeStart	datetime = NULL,
	@RangeEnd	datetime = NULL,
	@TempLow	float = NULL,
	@TempHigh	float = NULL,
	@Points		int = 100
)
AS
	WITH x AS
	(
		SELECT
			Date,
			TempFahrenheit,
			TempCelsius,
			Humidity,
			Light,
			ROW_NUMBER() OVER (ORDER BY Date) AS RowID
		FROM
			SensorData
		WHERE
			(@Arduino is null OR Arduino = @Arduino) AND
			(@Year is null OR Year = @Year) AND
			(@RangeStart is null OR Date >= @RangeStart) AND
			(@RangeEnd is null OR Date <= @RangeEnd) AND
			(@TempLow is null OR TempFahrenheit >= @TempLow) AND
			(@TempHigh is null OR TempFahrenheit <= @TempHigh)
	), y AS
	(
		SELECT
			Count(*) as Available,
			Count(*)/@Points as Modulus_RAW,
			dbo.GREATEST(COUNT(*)/@Points, 1) as Modulus
		FROM x
	), z AS
	(
	SELECT TOP (@Points) x.*, y.Available, y.Modulus FROM x CROSS JOIN y
	WHERE RowID % (SELECT Modulus FROM y) = 0
	--WHERE RowID % 199 = 0
	ORDER BY Date desc
	)
	SELECT * FROM z ORDER BY Date

GO

/*

DECLARE @Points int
SET @Points = 100;
WITH x AS
(
	SELECT
		Date,
		TempFahrenheit,
		TempCelsius,
		Humidity,
		Light,
		ROW_NUMBER() OVER (ORDER BY Date) AS RowID
	FROM
		SensorData
), y AS
(
	SELECT
		Count(*) as Available,
		Count(*)/@Points as Modulus_RAW,
		dbo.GREATEST(COUNT(*)/@Points, 1) as Modulus
	FROM x
), z AS
(
SELECT TOP (@Points) x.*, y.Available, y.Modulus FROM x CROSS JOIN y
WHERE RowID % (SELECT Modulus FROM y) = 0
--WHERE RowID % 199 = 0
ORDER BY Date desc
)
SELECT * FROM z ORDER BY Date

*/