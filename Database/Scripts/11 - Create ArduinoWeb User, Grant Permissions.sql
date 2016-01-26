
USE ArduinoMonitor

GO

CREATE LOGIN [ArduinoWeb] WITH PASSWORD=N'password', 
	DEFAULT_DATABASE=[ArduinoMonitor], 
	DEFAULT_LANGUAGE=[us_english], 
	CHECK_EXPIRATION=OFF, 
	CHECK_POLICY=OFF

GO

CREATE USER [ArduinoWeb] FOR LOGIN [ArduinoWeb]

GO

GRANT EXECUTE ON SensorDataLast to ArduinoWeb

ALTER LOGIN ArduinoWeb WITH DEFAULT_DATABASE = [ArduinoMonitor]
GO

EXEC sp_change_users_login 'Auto_Fix', 'ArduinoWeb'

exec sp_updatestats

dbcc freeproccache