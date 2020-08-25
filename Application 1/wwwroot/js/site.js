/*
This js method handles the dropdown site menu on a responsive design.
*/
function dropDown() {
    var x = document.getElementById("myTopnav");
    if (x.className === "topnav") {
        x.className += " responsive";
    } else {
        x.className = "topnav";
    }
}

/*
This js method can be called to display a confirmation box before an action is executed.
*/
function confirmAction(message, url) {
    if (confirm(message)) {
        window.location.href = url;
    }
}