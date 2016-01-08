
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
