
USE [ArduinoMonitor]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[DatesBetween]
(
	@StartDate datetime = NULL, 
	@EndDate datetime = NULL
)
RETURNS @Dates TABLE
(
	[Index] int PRIMARY KEY NOT NULL IDENTITY(1,1),
	[Date] date NOT NULL
)
AS
BEGIN

	DECLARE @CurrentDate date
		
	IF (@StartDate IS NULL) 
		SET @StartDate = GETDATE()
	IF (@EndDate IS NULL) 
		SET @EndDate = GETDATE()

	SET @CurrentDate = @StartDate
	INSERT INTO @Dates ([Date]) SELECT @CurrentDate

	WHILE(@CurrentDate <= @EndDate)
	BEGIN

		INSERT INTO @Dates ([Date]) SELECT @CurrentDate
		SELECT @CurrentDate = DATEADD(DAY, 1, @CurrentDate)

	END

	RETURN

END

GO

CREATE FUNCTION dbo.MinutesInDay(@Date date)
RETURNS @MinutesInDay TABLE 
(
    [Index] int PRIMARY KEY NOT NULL, 
    [Date] smalldatetime NOT NULL
)
AS
BEGIN

	DECLARE @StartDate AS smalldatetime, @EndDate AS smalldatetime
	SET @StartDate = @Date;
	SET @EndDate = DATEADD(day, 1, @Date);

	WITH DateIntervalsCTE AS
	(
		SELECT 1 i, @StartDate AS [Date]
		UNION ALL
		SELECT i + 1, DATEADD(minute, i, @StartDate)
		FROM DateIntervalsCTE
		WHERE DATEADD(minute, i, @StartDate) < @EndDate
	)
	INSERT INTO @MinutesInDay([Index], [Date])
	SELECT d.i, d.[Date]
	FROM 
		DateIntervalsCTE d
	ORDER BY
		d.i
	OPTION (MAXRECURSION 32767);

	RETURN;

END

GO

CREATE FUNCTION dbo.MinutesBetween
(
	@Start smalldatetime = NULL, 
	@End smalldatetime = NULL
)
RETURNS @Minutes TABLE 
(
    [Index] int PRIMARY KEY NOT NULL IDENTITY(1,1), 
    [Date] smalldatetime NOT NULL
)
AS
BEGIN

	DECLARE @Current smalldatetime
		
	IF (@Start IS NULL) 
		SET @Start = GETDATE()
	IF (@End IS NULL) 
		SET @End = GETDATE()

	SET @Current = @Start
	INSERT INTO @Minutes ([Date]) SELECT @Current

	WHILE(@Current <= @End)
	BEGIN

		INSERT INTO @Minutes ([Date]) SELECT @Current
		SELECT @Current = DATEADD(MINUTE, 1, @Current)

	END

	RETURN

END

GO

CREATE FUNCTION dbo.MinutesElapsed
(
	@Minutes bigint = 60, 
	@End smalldatetime = NULL
)
RETURNS @MinutesElapsed TABLE 
(
    [Index] int PRIMARY KEY NOT NULL IDENTITY(1, 1), 
    [Date] smalldatetime NOT NULL
)
AS
BEGIN

	DECLARE @Current smalldatetime
		
	IF (@End IS NULL) 
		SET @End = GETDATE()

	SET @Current = DATEADD(MINUTE, 1 - @Minutes, @End)
	INSERT INTO @MinutesElapsed ([Date]) SELECT @Current

	WHILE(@Current < @End)
	BEGIN

		INSERT INTO @MinutesElapsed ([Date]) SELECT @Current
		SELECT @Current = DATEADD(MINUTE, 1, @Current)

	END

	RETURN

END

GO

CREATE PROCEDURE [dbo].[SensorDataCurrent]
(
	@Minutes int = 10
)
AS
BEGIN

	SELECT
		DATEDIFF(minute, m.[Date], GETDATE()) as ElapsedMinutes,
		sd.ID,
		sd.Arduino,
		sd.[Year],
		m.[Date],
		sd.TempCelsius,
		sd.TempFahrenheit,
		sd.Humidity,
		sd.Light
	FROM
		dbo.MinutesElapsed(@Minutes, GETDATE()) m
		LEFT OUTER JOIN SensorData sd on sd.[Date] >= DATEADD(minute, -1 - @Minutes, GETDATE()) AND CONVERT(smalldatetime, sd.[Date]) = m.[Date]
	ORDER BY m.[Date] desc
	
END

GO

ALTER PROCEDURE [dbo].[SensorDataLast]
(
	@Points int = 10
)
AS
BEGIN

	SELECT TOP (@Points) 
		DATEDIFF(minute, [Date], GETDATE()) as ElapsedMinutes, 
		* 
	FROM SensorData
	ORDER BY Date desc

END

GO

ALTER PROCEDURE [dbo].[SensorDataRecent]
(
	@Minutes int = 10
)
AS
BEGIN

	SELECT 
		DATEDIFF(minute, [Date], GETDATE()) as ElapsedMinutes, 
		* 
	FROM SensorData
	WHERE [Date] >= DATEADD(minute, 0 - @Minutes, GETDATE()) 
	ORDER BY [Date] desc

END

GO

ALTER PROCEDURE [dbo].[EventsLast]
(
	@Events int = 10
)
AS
BEGIN

	SELECT TOP (@Events) 
		DATEDIFF(minute, [Date], GETDATE()) as ElapsedMinutes, 
		*
	FROM EventView
	ORDER BY Date desc

END

GO

ALTER PROCEDURE [dbo].[EventsRecent]
(
	@Minutes int = 10
)
AS
BEGIN

	SELECT 
		DATEDIFF(minute, [Date], GETDATE()) as ElapsedMinutes, 
		* 
	FROM EventView 
	WHERE [Date] >= DATEADD(minute, 0 - @Minutes, GETDATE()) 
	ORDER BY [Date] desc

END

GO

GRANT EXECUTE ON SensorDataGraph TO ArduinoWeb, ArduinoView
GRANT EXECUTE ON SensorDataRecent TO ArduinoWeb, ArduinoView
GRANT EXECUTE ON SensorDataLast TO ArduinoWeb, ArduinoView
GRANT EXECUTE ON SensorDataCurrent TO ArduinoWeb, ArduinoView
GRANT EXECUTE ON EventsLast TO ArduinoWeb, ArduinoView
GRANT EXECUTE ON EventsRecent TO ArduinoWeb, ArduinoView
GRANT EXECUTE ON ArduinosGet TO ArduinoWeb, ArduinoView
