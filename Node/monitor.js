var serial = require("serialport");
var tempC, tempF, himidity, light;
var lastRefresh;

var ports = serial.list(function (err, ports) {
  ports.forEach(function(port) {
    console.log(port.comName);
    console.log("PnP ID: " + port.pnpId);
    console.log("Manufacturer: " + port.manufacturer);
    console.log();
  });
});

setTimeout(begin, 200);

function begin() {
	var serialPort = new serial.SerialPort('COM4',{baudrate: 9600, parser: serial.parsers.readline("\n")}, true);

	serialPort.on ('open', function () {
	    console.log("Serial Port Opened");
	    //serialPort.write(0x05);
	    serialPort.on ('data', function(data) {
	    	var input = data.toString();
	        console.log(input);
	        processLine(input);
	    });
	});
}

function processLine(input) {
    if (input.indexOf('|') >= 0) {
        var data = input.split("|");
        tempC = data[0];
        tempF = data[1];
        humidity = data[2];
        light = data[3];
        lastRefresh = Date.now();
        //console.log(tempC + "C");
    }
}