
USE ArduinoMonitor

GO

CREATE LOGIN [ArduinoView] WITH PASSWORD=N'password', 
	DEFAULT_DATABASE=[ArduinoMonitor], 
	DEFAULT_LANGUAGE=[us_english], 
	CHECK_EXPIRATION=OFF, 
	CHECK_POLICY=OFF

GO

CREATE USER [ArduinoView]

GO

EXEC sp_change_users_login 'Auto_Fix', 'ArduinoView'

GO

EXEC sp_addrolemember 'db_datareader', 'ArduinoView'

GO

GRANT EXECUTE ON ArduinoGet to ArduinoView
GRANT EXECUTE ON ArduinosGet to ArduinoView
GRANT EXECUTE ON EventGet to ArduinoView
GRANT EXECUTE ON EventsGet to ArduinoView
GRANT EXECUTE ON EventsLast to ArduinoView
GRANT EXECUTE ON EventsRecent to ArduinoView
GRANT EXECUTE ON LastUpdate to ArduinoView
GRANT EXECUTE ON SensorDataGet to ArduinoView
GRANT EXECUTE ON SensorDataLast to ArduinoView
GRANT EXECUTE ON SensorDataRecent to ArduinoView
