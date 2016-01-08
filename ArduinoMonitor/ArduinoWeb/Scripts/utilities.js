
if (!String.prototype.format) {
    String.prototype.format = function () {
        var args = arguments;
        return this.replace(/{(\d+)}/g, function (match, number) {
            return typeof args[number] != 'undefined' ? args[number] : match;
        });
    };
}

if (!Date.prototype.toUTC) {
    Date.prototype.toUTC = function () {
        return new Date(this.getTime() + this.getTimezoneOffset() * 60000);
    };
}

function isTrue(value) {
    if (typeof (value) == 'string') {
        value = value.toLowerCase();
    }
    switch (value) {
        case true:
        case "true":
        case 1:
        case "1":
        case "on":
        case "yes":
            return true;
        default:
            return false;
    }
}

monthsShort = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
monthsLong  = ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'];

function formatDate(date) {
    var ampm = date.getHours() >= 12 ? "PM" : "AM";
    var hours = date.getHours() % 12;
    if (hours == 0) hours = 12;
    return "{0}.{1}.{2} {3}:{4} {5}".format(date.getFullYear(), pad(date.getMonth(), 2), pad(date.getDate(), 2), hours, pad(date.getMinutes(), 2), ampm);
}

function formatDateLong(date) {
    var ampm = date.getHours() >= 12 ? "PM" : "AM";
    var hours = date.getHours() % 12;
    if (hours == 0) hours = 12;
    return "{0} {1} {2} {3}:{4} {5}".format(date.getFullYear(), monthsLong[date.getMonth()], pad(date.getDate(), 2), hours, pad(date.getMinutes(), 2), ampm);
}

function formatDateNoYear(date) {
    var ampm = date.getHours() >= 12 ? "PM" : "AM";
    var hours = date.getHours() % 12;
    if (hours == 0) hours = 12;
    return "{1} {2}, {3}:{4} {5}".format(date.getFullYear(), monthsLong[date.getMonth()], pad(date.getDate(), 2), hours, pad(date.getMinutes(), 2), ampm);
}

function getTimezoneAbbreviation(date) {
    date = date || new Date();
    var s = date.toString().split("(");
    if (s.length == 2) {
        var n = s[1].replace(")", "");
        return n.match(/[A-Z]/g).join("");
    }
    return "UTC";
}

function pad(n, width, z) {
    z = z || '0';
    n = n + '';
    return n.length >= width ? n : new Array(width - n.length + 1).join(z) + n;
}

function isNull(arg) {
    if (arg === null || arg === undefined || (typeof arg === 'number' && arg.toString() === 'NaN'))
        return true;
    
    return false;
}

function coalesce() {
    var i, undefined, arg;
    for (i = 0; i < arguments.length; i++) {
        arg = arguments[i];
        if (arg !== null && arg !== undefined && (typeof arg !== 'number' || arg.toString() !== 'NaN')) {
            return arg;
        }
    }
    return null;
}