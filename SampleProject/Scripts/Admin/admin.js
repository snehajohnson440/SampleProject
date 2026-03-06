function showTab(name, el) {

    $(".tab-panel").removeClass("active");
    $("#tab-" + name).addClass("active");

    $(".nav-item").removeClass("active");
    $(el).addClass("active");

    $(".add-form").hide();
    localStorage.setItem("activeAdminTab", name);
    animateCards();
}

function toggleForm(id) {
    $("#" + id).slideToggle(200);
}

function animateCards() {
    $(".tab-panel.active .card").fadeIn(200);
}

$(document).ready(function () {

    var savedTab = localStorage.getItem("activeAdminTab");

    if (savedTab) {
        var nav = $(".nav-item").filter(function () {
            return $(this).text().trim().toLowerCase().includes(savedTab);
        });

        showTab(savedTab, nav[0]);
    } else {
        animateCards();
    }
});