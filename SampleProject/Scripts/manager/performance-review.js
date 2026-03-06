/* ================= RATING FIELDS ================= */
var ratingFields = ["Task Completion", "Productivity", "Consistency", "Quality of Work", "Communication", "Teamwork", "Punctuality", "Problem Solving", "Initiative", "Learning Ability"];
var ratingKeys = ["TaskCompletion", "Productivity", "Consistency", "QualityOfWork", "Communication", "Teamwork", "Punctuality", "ProblemSolving", "Initiative", "LearningAbility"];

/* ================= GRADE HELPER ================= */
function grade(avg) {
    if (avg >= 9) return { text: "Outstanding", color: "#3fb950" };
    if (avg >= 7.5) return { text: "Excellent", color: "#58a6ff" };
    if (avg >= 6) return { text: "Good", color: "#58a6ff" };
    if (avg >= 4) return { text: "Average", color: "#d29922" };
    return { text: "Needs Improvement", color: "#f85149" };
}

/* ================= BUILD SLIDERS ================= */
function buildSliders() {
    var html = "";
    $.each(ratingFields, function (i, f) {
        html += '<div class="slider-row"><label>' + f + '</label>' +
            '<input type="range" min="1" max="10" value="5" class="rating-slider" data-key="' + ratingKeys[i] + '" />' +
            '<span class="slider-val">5</span></div>';
    });
    $("#sliders").html(html);
    updateAvg();
}
function loadDepartmentPerformance() {

    $.get("/Manager/GetDepartmentPerformance", function (data) {

        if (!data || data.length === 0) {
            $("#departmentPerformance")
                .html('<div class="empty-msg">No data</div>');
            return;
        }

        var html = "";

        $.each(data, function (i, d) {

            html +=
                '<div class="review-card">' +
                '<div class="review-date">' + d.DepartmentName + '</div>' +
                '<div class="review-score">' +
                d.DepartmentAverageScore + ' / 10' +
                '</div>' +
                '<div class="review-feedback">' +
                'Employees: ' + d.TotalEmployees +
                '</div>' +
                '</div>';
        });

        $("#departmentPerformance").html(html);
    });
}
/* ================= UPDATE AVERAGE ================= */
function updateAvg() {
    var total = 0, count = 0;
    $(".rating-slider").each(function () {
        total += parseInt($(this).val());
        count++;
    });
    var avg = count ? (total / count) : 0;
    $("#avgDisplay").text(avg.toFixed(1));
    var g = grade(avg);
    $("#avgGrade").text(g.text).css("color", g.color);
}

/* ================= SLIDER INPUT ================= */
$(document).on("input", ".rating-slider", function () {
    $(this).next(".slider-val").text($(this).val());
    updateAvg();
});

/* ================= EMPLOYEE DROPDOWN — single merged handler ================= */
$(document).on("change", "#empDropdown", function () {
    var empId = $(this).val();
    var empName = $(this).find("option:selected").data("name") || "";

    if (!empId) {
        $("#empSummary").hide();
        $("#rightInner").addClass("disabled");
        $("#selectPrompt").show();
        $("#ratingArea").hide();
        return;
    }

    $("#selectedEmpName").text(empName);
    

    $.get("/Manager/GetTasksOfEmployee", { employeeId: empId }, function (tasks) {
        tasks = tasks || [];

        if (!tasks.length) {
            $("#sumTasks").text(0);
            $("#sumHours").text(0);
            $("#sumActivities").text(0);
            $("#empTaskList").html('<div class="empty-msg">No tasks found</div>');
            $("#empSummary").show();
            enableRight();
            loadReviewHistory(empId);
            return;
        }

        var pending = tasks.length, taskHours = {}, taskActs = {};

        $.each(tasks, function (i, t) {
            taskHours[t.TaskId] = 0;
            taskActs[t.TaskId] = 0;

            $.get("/Manager/GetActivitiesForManager", { taskId: t.TaskId }, function (acts) {
                var h = 0;
                $.each(acts, function (j, a) { h += parseFloat(a.ActivityHours) || 0; });
                taskHours[t.TaskId] = h;
                taskActs[t.TaskId] = acts.length;
                pending--;
                if (pending === 0) {
                    renderSummary(tasks, taskHours, taskActs);
                    loadReviewHistory(empId);
                }
            });
        });
    });
});

/* ================= RENDER SUMMARY ================= */
function renderSummary(tasks, taskHours, taskActs) {
    var totalHours = 0, totalActs = 0, html = "";

    $.each(tasks, function (i, t) {
        var h = taskHours[t.TaskId] || 0;
        var a = taskActs[t.TaskId] || 0;
        totalHours += h;
        totalActs += a;
        var d = parseDate(t.TaskDate);
        html +=
            '<div class="task-mini">' +
            '<span class="task-mini-title">' + esc(t.Title) + '</span>' +
            '<span class="task-mini-date">' + (d ? d.toLocaleDateString("en-GB") : "") + '</span>' +
            '<span class="task-mini-hrs">' + h.toFixed(1) + ' hrs</span>' +
            '</div>';
    });

    $("#sumTasks").text(tasks.length);
    $("#sumHours").text(totalHours.toFixed(1));
    $("#sumActivities").text(totalActs);
    $("#empTaskList").html(html);
    $("#empSummary").show();
    enableRight();
}

/* ================= ENABLE RIGHT PANEL ================= */
function enableRight() {
    $("#rightInner").removeClass("disabled");
    $("#selectPrompt").hide();
    $("#ratingArea").show();
    buildSliders();
}

/* ================= RESET ================= */
$(document).on("click", "#resetBtn", function () {
    buildSliders();
    $("#feedbackText").val("");
});

/* ================= SUBMIT REVIEW ================= */
$(document).on("click", "#submitReview", function () {
    var empId = $("#empDropdown").val();
    if (!empId) { alert("Please select an employee."); return; }

    var payload = {
        UserId: parseInt(empId),
        Feedback: $("#feedbackText").val().trim()
    };

    $(".rating-slider").each(function () {
        payload[$(this).data("key")] = parseInt($(this).val());
    });

    $.ajax({
        url: "/Manager/RateEmployee",
        type: "POST",
        data: payload,
        success: function (r) {
            if (r.success) {
                alert("Review submitted for " + $("#empDropdown").find("option:selected").data("name") + "!");
                $("#feedbackText").val("");
                $(".rating-slider").val(5);
                $(".slider-val").text(5);
                updateAvg();
                loadReviewHistory(empId);
            } else {
                alert("Failed: " + (r.message || "Unknown error"));
            }
        },
        error: function () {
            alert("Request failed.");
        }
    });
});

/* ================= LOAD REVIEW HISTORY ================= */
function loadReviewHistory(userId) {
    $("#reviewHistory").html('<div class="empty-msg">Loading reviews...</div>');

    $.get("/Manager/GetPerformanceHistory", { userId: userId }, function (reviews) {
        if (!reviews || reviews.length === 0) {
            $("#reviewHistory").html('<div class="empty-msg">No previous reviews</div>');
            return;
        }

        var html = "";
        $.each(reviews, function (i, r) {
            html +=
                '<div class="review-card">' +
                '<div class="review-date">Reviewed by Manager</div>' +
                '<div class="review-score">' + r.AverageScore + ' / 10</div>' +
                '<div class="review-feedback">' + (r.Feedback || "No feedback") + '</div>' +
                '</div>';
        });
        $("#reviewHistory").html(html);
    });
}

/* ================= HELPERS ================= */
function parseDate(s) {
    var m = s && s.match(/\d+/);
    return m ? new Date(parseInt(m[0])) : null;
}

function esc(s) {
    return String(s)
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;");
}
$(document).ready(function () {
    loadDepartmentPerformance();
});