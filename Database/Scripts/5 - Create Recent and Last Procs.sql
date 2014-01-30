
USE ArduinoMonitor

GO

CREATE PROCEDURE SensorDataLast
AS
	SELECT TOP 10 * FROM SensorDataView ORDER BY Date desc

GO

CREATE PROCEDURE SensorDataRecent
AS
	SELECT DATEDIFF(minute, Date, GETDATE()) as ElapsedMinutes, * FROM SensorDataView WHERE DATEDIFF(minute, Date, GETDATE()) <= 10 ORDER BY Date desc

GO

CREATE PROCEDURE EventsLast
AS
	SELECT TOP 10 * FROM EventView ORDER BY Date desc

GO

CREATE PROCEDURE EventsRecent
AS
	SELECT DATEDIFF(minute, Date, GETDATE()) as ElapsedMinutes, * FROM EventView WHERE DATEDIFF(minute, Date, GETDATE()) <= 10 ORDER BY Date desc

GO

CREATE PROCEDURE LastUpdate
AS
	SELECT TOP 1 Date FROM SensorDataView ORDER BY Date desc

GO