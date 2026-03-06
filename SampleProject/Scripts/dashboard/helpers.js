function pad(n) { return n < 10 ? "0" + n : n; }

var today = new Date();
var todayStr = today.getFullYear() + "-" + pad(today.getMonth() + 1) + "-" + pad(today.getDate());
var selectedDate = todayStr;  
var currentDate = new Date();
var selectedTaskId = null;

function parseDate(s) {
    var m = s && s.match(/\d+/);
    if (!m) return null;
    var d = new Date(parseInt(m[0]));
    
    return new Date(d.getTime() + (d.getTimezoneOffset() * -60000));
}

function esc(s) {
    return String(s || "")
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;");
}