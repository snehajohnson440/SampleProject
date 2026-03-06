// ===============================
// MAIN SIDEBAR CALENDAR
// ===============================
function renderCalendar() {

    var calendar = document.getElementById("calendar");
    var year = currentDate.getFullYear();
    var month = currentDate.getMonth();
    var today = new Date();

    document.getElementById("monthYear").innerText =
        currentDate.toLocaleString("default", { month: "long", year: "numeric" });

    calendar.innerHTML = "";

   
    ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"].forEach(function (d) {
        var h = document.createElement("div");
        h.className = "day-header";
        h.innerText = d;
        calendar.appendChild(h);
    });

  
    var firstDayIndex = new Date(year, month, 1).getDay();
    for (var e = 0; e < firstDayIndex; e++) {
        var empty = document.createElement("div");
        empty.className = "day-empty";
        calendar.appendChild(empty);
    }

   
    var totalDays = new Date(year, month + 1, 0).getDate();

    for (var day = 1; day <= totalDays; day++) {
        (function (d) {
            var dayDiv = document.createElement("div");
            dayDiv.className = "day";
            dayDiv.innerText = d;

            var cellDate = new Date(year, month, d);
            var dateString = year + "-" + pad(month + 1) + "-" + pad(d);

            if (d === today.getDate() && month === today.getMonth() && year === today.getFullYear()) {
                dayDiv.classList.add("today");
            }

            if (cellDate > today) {
                dayDiv.classList.add("future-day");
            }

            if (selectedDate === dateString) {
                dayDiv.classList.add("selected");
            }

            dayDiv.onclick = function () {
                selectedDate = dateString; 
                renderCalendar();
                loadTasks();
            };

            calendar.appendChild(dayDiv);
        })(day);
    }
}

// MONTH NAVIGATION
document.getElementById("prevMonth").onclick = function () {
    currentDate.setMonth(currentDate.getMonth() - 1);
    renderCalendar();
};

document.getElementById("nextMonth").onclick = function () {
    currentDate.setMonth(currentDate.getMonth() + 1);
    renderCalendar();
};


// ===============================
// TASK DATE PICKER CALENDAR
// ===============================
function populateDateDropdown() {

    var calendar = document.getElementById("taskCalendar");
    var today = new Date();
    var viewDate = new Date();

    
    var existingValue = document.getElementById("taskDate").dataset.value;
    if (!existingValue) {
        var todayValue = today.getFullYear() + "-" + pad(today.getMonth() + 1) + "-" + pad(today.getDate());
        document.getElementById("taskDate").value = today.toLocaleDateString("en-GB");
        document.getElementById("taskDate").dataset.value = todayValue;
    }

    function drawCalendar() {

        var year = viewDate.getFullYear();
        var month = viewDate.getMonth();

        calendar.innerHTML = "";

        // HEADER
        var header = document.createElement("div");
        header.className = "task-cal-header";
        header.innerHTML =
            '<button id="tcPrev">&#9664;</button>' +
            '<span>' + viewDate.toLocaleString("default", { month: "long", year: "numeric" }) + '</span>' +
            '<button id="tcNext">&#9654;</button>';
        calendar.appendChild(header);

        // GRID
        var grid = document.createElement("div");
        grid.className = "task-cal-grid";

        // Day names
        ["Su", "Mo", "Tu", "We", "Th", "Fr", "Sa"].forEach(function (n) {
            var nameCell = document.createElement("div");
            nameCell.className = "task-cal-dayname";
            nameCell.innerText = n;
            grid.appendChild(nameCell);
        });

        // Empty offset
        var startIndex = new Date(year, month, 1).getDay();
        for (var x = 0; x < startIndex; x++) {
            grid.appendChild(document.createElement("div"));
        }

        // Date limits
        var minDate = new Date(today);
        minDate.setMonth(minDate.getMonth() - 3);
        minDate.setHours(0, 0, 0, 0);

        var totalDays = new Date(year, month + 1, 0).getDate();

        // DAY CELLS
        for (var d = 1; d <= totalDays; d++) {
            (function (day) {
                var cell = document.createElement("div");
                cell.className = "task-cal-day";
                cell.innerText = day;

                var cellDate = new Date(year, month, day);
                var dateStr = year + "-" + pad(month + 1) + "-" + pad(day);

                if (cellDate > today || cellDate < minDate) {
                    cell.classList.add("task-cal-disabled");
                } else {
                    cell.onclick = function () {
                       
                        document.getElementById("taskDate").value =
                            cellDate.toLocaleDateString("en-GB");
                        document.getElementById("taskDate").dataset.value = dateStr;

                        // highlight selected
                        var allCells = grid.querySelectorAll(".task-cal-day");
                        allCells.forEach(function (c) { c.classList.remove("task-cal-selected"); });
                        cell.classList.add("task-cal-selected");
                    };
                }

                if (cellDate.toDateString() === today.toDateString()) {
                    cell.classList.add("task-cal-today");
                }

                if (dateStr === document.getElementById("taskDate").dataset.value) {
                    cell.classList.add("task-cal-selected");
                }

                grid.appendChild(cell);
            })(d);
        }

        calendar.appendChild(grid);

      
        document.getElementById("tcPrev").onclick = function (e) {
            e.stopPropagation();
            viewDate.setMonth(viewDate.getMonth() - 1);
            drawCalendar();
        };
        document.getElementById("tcNext").onclick = function (e) {
            e.stopPropagation();
            viewDate.setMonth(viewDate.getMonth() + 1);
            drawCalendar();
        };
    }

    drawCalendar();
}