
$(document).ready(function () {

 
    function loadTasks() {

       
        var url = "/Task/GetTasks";

        
        if (selectedDate) {
            url = url + "?date=" + selectedDate;
        }

      
        $.get(url, function (tasks) {

            var totalHours = 0;

            
            if (!tasks || tasks.length === 0) {
                $("#taskList").html('<div class="empty-msg">No tasks on this date</div>');
                selectedTaskId = null;
                $("#showAddActivity").hide();
                $("#activityList").html('<div class="empty-msg">Select a task to see activities</div>');
                $("#totalHoursBox").text("Total Hours: 0.0 hrs");
                return;
            }

            
            var html = "";

            $.each(tasks, function (i, task) {

                
                totalHours += Number(task.TotalHours || 0);

                
                var d = parseDate(task.TaskDate);
                var dateLabel = d ? d.toLocaleDateString("en-GB") : "";

                
                html +=
                    '<div class="task-card" data-id="' + task.TaskId + '">' +
                
                    '<div class="task-header">' +
                    '<div class="task-info">' +
                    '<div class="project-name">' + esc(task.ClientName || "") + '</div>' +
                    '<h3>' + esc(task.ProjectName) + '</h3>' +
                    '<div class="client-name">' + esc(task.Title || "") + '</div>' +
                    '</div>' +
                    '<div class="badge">' + (task.TotalHours || 0) + ' hrs</div>' +
                    '</div>' +
                '<div class="task-date">' + dateLabel + '</div>' +
                '<button class="task-delete" data-id="' + task.TaskId + '">✕</button>' +
                    '</div>';
            });

            
            $("#taskList").html(html);

            
            $("#taskList .task-card").each(function () {
                $(this).hide().fadeIn(500);
            });

            
            $("#totalHoursBox").text("Total Hours: " + totalHours.toFixed(1) + " hrs");

            
            if (selectedTaskId) {
                var prevCard = $('.task-card[data-id="' + selectedTaskId + '"]');
                if (prevCard.length > 0) {
                    prevCard.addClass("active");
                    $("#showAddActivity").show();
                    loadActivities(selectedTaskId);
                    return;
                }
            }

            
            $("#taskList .task-card:first").trigger("click");
        });
    }

    
    window.loadTasks = loadTasks;


    
    $(document).on("click", ".task-card", function () {

        
        $(".task-card").removeClass("active");

        
        $(this).addClass("active");

        
        selectedTaskId = $(this).data("id");

       
        $("#showAddActivity").show();

        
        $("#addActivityForm").hide();

        
        loadActivities(selectedTaskId);
    });
    
    $(document).on("click", ".task-delete", function (e) {

        
        e.stopPropagation();

        var taskId = $(this).data("id");

        if (!confirm("Delete this task?")) return;

        $.ajax({
            url: "/Task/DeleteTask",
            type: "POST",
            data: { id: taskId },

            success: function (response) {

                if (response.success) {
                    selectedTaskId = null;
                    loadTasks();
                } else {
                    alert("Delete failed.");
                }
            }
        });
    });

    
    $(document).on("click", "#showAddTask", function () {

        
        clearTaskForm();

        
        $("#addTaskForm").slideDown(250);

        
        $("#showAddTask").hide();
    });


    
    $(document).on("click", "#taskDate", function (e) {

        
        e.stopPropagation();

        var cal = $("#taskCalendar");

       
        if (cal.is(":visible")) {
            cal.slideUp(150);
        } else {
            
            populateDateDropdown();
            cal.slideDown(200);
        }
    });


    
    $(document).on("click", ".task-cal-day:not(.task-cal-disabled)", function () {
        $("#taskCalendar").slideUp(150);
    });


   
    $(document).on("click", function (e) {


        var clickedInsideCalendar = $(e.target).closest("#taskCalendar, #taskDate").length;

        if (!clickedInsideCalendar) {
            $("#taskCalendar").hide();
        }
    });


    
    $(document).on("click", "#cancelTask", function () {

       
        $("#addTaskForm").slideUp(200);

        
        $("#taskCalendar").hide();

        
        $("#showAddTask").show();

       
        clearTaskForm();
    });


    
    $(document).on("click", "#saveTask", function () {

        
        var title = $("#taskTitle").val().trim();
        var date = $("#taskDate").data("value"); 

        
        if (!title || !date) {
            alert("Title and date are required.");
            return;
        }
        if (!$("#clientId").val() || !$("#projectId").val()) {
            alert("Client Or Project Is Required");
            return;
        }
        
        $.ajax({
            url: "/Task/AddTask",
            type: "POST",
            data: {
                taskName: title,
                taskDate: date,
                clientId: $("#clientId").val(),
                projectId: $("#projectId").val()
            },
            success: function (response) {

              
                if (response.success) {

                    
                    $("#addTaskForm").slideUp(200, function () {
                        clearTaskForm();
                    });

                    $("#taskCalendar").hide();
                    $("#showAddTask").show();

                  
                    selectedDate = date;
                    selectedTaskId = null;

                    
                    renderCalendar();
                    loadTasks();

                } else {
                    alert("Save failed.");
                }
            }
        });
    });


    
    function clearTaskForm() {
        $("#taskTitle").val("");
        $("#clientId").prop("selectedIndex", 0); 
        $("#projectId").html('<option value="">Select Project</option>'); 
        $("#taskDate").val("").removeData("value"); 
        $("#taskCalendar").empty().hide(); 
    }

    
    window.clearTaskForm = clearTaskForm;


}); 