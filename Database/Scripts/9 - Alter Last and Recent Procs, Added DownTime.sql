USE [ArduinoMonitor]

GO

ALTER PROCEDURE [dbo].[EventsLast]
(
	@Points int = 10
)
AS
	SELECT TOP (@Points) DATEDIFF(minute, Date, GETDATE()) as ElapsedMinutes, * FROM EventView ORDER BY Date desc

GO

ALTER PROCEDURE [dbo].[EventsRecent]
(
	@Minutes int = 10
)
AS
	SELECT DATEDIFF(minute, Date, GETDATE()) as ElapsedMinutes, * FROM EventView WHERE DATEDIFF(minute, Date, GETDATE()) <= @Minutes ORDER BY Date desc

GO

ALTER PROCEDURE [dbo].[SensorDataLast]
(
	@Points int = 10
)
AS
	SELECT TOP (@Points) DATEDIFF(minute, Date, GETDATE()) as ElapsedMinutes, * FROM SensorDataView ORDER BY Date desc

GO

ALTER PROCEDURE [dbo].[SensorDataRecent]
(
	@Minutes int = 10
)
AS
	SELECT DATEDIFF(minute, Date, GETDATE()) as ElapsedMinutes, * FROM SensorDataView WHERE DATEDIFF(minute, Date, GETDATE()) <= @Minutes ORDER BY Date desc

GO

CREATE PROCEDURE [dbo].[DownTime]
AS
	SELECT TOP 1 DATEDIFF(minute, Date, GETDATE()) as ElapsedMinutes FROM SensorDataView ORDER BY Date desc

GO

