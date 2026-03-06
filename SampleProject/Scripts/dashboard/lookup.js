function loadClients() {
    $.get("/Task/GetClients", function (data) {
        var ddl = $("#clientId");
        ddl.empty();
        ddl.append('<option value="">Select Client</option>');
        $.each(data, function (i, c) {
            ddl.append('<option value="' + c.ClientId + '">' + c.ClientName + '</option>');
        });
    });
}

$("#clientId").change(function () {
    var clientId = $(this).val();
    $.get("/Task/GetProjects", { clientId: clientId }, function (projects) {
        var ddl = $("#projectId");
        ddl.empty();
        ddl.append('<option>Select Project</option>');
        $.each(projects, function (i, p) {
            ddl.append('<option value="' + p.ProjectId + '">' + p.ProjectName + '</option>');
        });
    });
});

function loadActivityTypes() {
    $.get("/Task/GetActivityTypes", function (types) {
        var ddl = $("#actType");
        ddl.empty();
        ddl.append('<option value="">Select Activity Type</option>');
        $.each(types, function (i, t) {
            ddl.append('<option value="' + t.ActivityTypeId + '">' + t.ActivityTypeName + '</option>');
        });
    });
}

function loadTimeDropdowns() {
    var hour = $("#actHour");
    var min = $("#actMinute");
    hour.empty();
    min.empty();

    for (var i = 0; i <= 8; i++) {
        hour.append('<option value="' + i + '">' + i + ' hr</option>');
    }
    $.each([0, 15, 30, 45], function (i, m) {
        min.append('<option value="' + m + '">' + m + ' min</option>');
    });
}