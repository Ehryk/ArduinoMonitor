
USE ArduinoMonitor
GO

EXEC sp_change_users_login 'Auto_Fix', 'ArduinoMonitor'

EXEC sp_change_users_login 'Auto_Fix', 'ArduinoView'

EXEC sp_change_users_login 'Auto_Fix', 'ArduinoWeb'
