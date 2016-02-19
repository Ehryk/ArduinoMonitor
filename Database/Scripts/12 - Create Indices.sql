
USE ArduinoMonitor
GO

CREATE INDEX idxDate ON SensorData([Date])
CREATE INDEX idxYear ON SensorData([Year])
CREATE INDEX idxDate ON EventLog([Date])
CREATE INDEX idxYear ON EventLog([Year])
