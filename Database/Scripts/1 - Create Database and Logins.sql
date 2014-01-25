
CREATE DATABASE ArduinoMonitor

GO

USE ArduinoMonitor

GO

CREATE LOGIN [ArduinoMonitor] WITH PASSWORD=N'password', 
	DEFAULT_DATABASE=[ArduinoMonitor], 
	DEFAULT_LANGUAGE=[us_english], 
	CHECK_EXPIRATION=OFF, 
	CHECK_POLICY=OFF

GO

CREATE USER [ArduinoMonitor]

GO

EXEC sp_change_users_login 'Auto_Fix', 'ArduinoMonitor'

GO
