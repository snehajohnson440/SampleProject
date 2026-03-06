// ======================================
// LOAD ACTIVITIES FOR A TASK
// ======================================
function loadActivities(taskId) {

    
    $("#activityList").html(
        '<div class="empty-msg">Loading...</div>'
    );

   
    $.get("/Task/GetActivities", { taskId: taskId }, function (activities) {

        
        if (!activities || activities.length === 0) {
            $("#activityList").html(
                '<div class="empty-msg">No activities for this task</div>'
            );
            return;
        }

        var html = "";

        
        $.each(activities, function (index, activity) {

            html +=
                '<div class="activity-card">' +
            
            '<div class="task-header">' +

            '<h3>' + esc(activity.Title) + '</h3>' +

            '<div class="activity-actions">' +

            '<div class="badge">' +
            activity.ActivityHours + ' hrs' +
            '</div>' +

            '<button class="activity-delete" data-id="' + activity.ActivityId + '">✕</button>' +

            '</div>' +

            '</div>'+

                '<p>' + esc(activity.Description || "") + '</p>' +

                '<div class="task-date">' +
                esc(activity.ActivityTypeName || "General") +
                '</div>' +

                '</div>';
        });

        
        $("#activityList").html(html);

       
        $("#activityList .activity-card").each(function () {
            $(this).hide().fadeIn(500);
        });
    });
}
// DELETE ACTIVITY
$(document).on("click", ".activity-delete", function (e) {

    e.stopPropagation();

    var activityId = $(this).data("id");

    if (!confirm("Delete this activity?")) return;

    $.ajax({
        url: "/Task/DeleteActivity",
        type: "POST",
        data: { id: activityId },

        success: function (response) {

            if (response.success) {
                loadActivities(selectedTaskId);
                loadTasks(); 
            } else {
                alert("Delete failed.");
            }
        }
    });
});

// ======================================
// SHOW ADD ACTIVITY FORM
// ======================================
$("#showAddActivity").click(function () {
    clearActivityForm();
    $("#addActivityForm").slideDown(250);
    $("#showAddActivity").hide();
});


// ======================================
// CANCEL ADD ACTIVITY
// ======================================
$("#cancelActivity").click(function () {

    $("#addActivityForm").slideUp(200);
    $("#showAddActivity").show();

    clearActivityForm();
});


// ======================================
// SAVE ACTIVITY
// ======================================
$("#saveActivity").click(function () {

    
    if (!selectedTaskId) {
        return;
    }

    var typeId = $("#actType").val();
    var title = $("#actTitle").val().trim();

    var hour = parseInt($("#actHour").val()) || 0;
    var minute = parseInt($("#actMinute").val()) || 0;

    var totalHours = hour + (minute / 60);

    
    if (!typeId || !title || totalHours <= 0 || totalHours > 8) {
        alert("Please fill all fields. Hours must be between 0 and 8.");
        return;
    }

   
    $.ajax({
        url: "/Task/AddActivity",
        type: "POST",
        contentType: "application/json",

        data: JSON.stringify({
            TaskId: selectedTaskId,
            ActivityTypeId: typeId,
            Title: title,
            Description: $("#actDesc").val().trim(),
            Hours: totalHours
        }),

        success: function (response) {

            if (response.success) {

                clearActivityForm();

                $("#addActivityForm").slideUp(200);
                $("#showAddActivity").show();

                // reload tasks + activities
                loadTasks();
            }
        }
    });
});


// ======================================
// CLEAR ACTIVITY FORM
// ======================================
function clearActivityForm() {

    $("#actTitle").val("");
    $("#actDesc").val("");

    $("#actType").prop("selectedIndex", 0);
    $("#actHour").val("0");
    $("#actMinute").val("0");
}