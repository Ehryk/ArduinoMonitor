
USE ArduinoMonitor

GO

CREATE FUNCTION dbo.GREATEST(@val1 int, @val2 int)
RETURNS INT
AS
BEGIN
  IF @val1 > @val2
    RETURN @val1
  RETURN isnull(@val2,@val1)
END

GO

CREATE PROCEDURE SensorDataGraph
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
	SELECT TOP (@Points) * FROM x
	WHERE RowID % (SELECT dbo.GREATEST(Count(*)/@Points, 1)) = 0
	ORDER BY Date desc
	)
	SELECT * FROM y ORDER BY Date

GO

-- exec SensorDataGraph @Points = 200000, @RangeStart = '2014-01-30'
