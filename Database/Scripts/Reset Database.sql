
--Delete Data
delete from SensorData
delete from EventLog
delete from Arduinos

--Reset Identity Values
DBCC CHECKIDENT (SensorData, RESEED, 0)
DBCC CHECKIDENT (EventLog,   RESEED, 0)
DBCC CHECKIDENT (Arduinos,   RESEED, 0)
