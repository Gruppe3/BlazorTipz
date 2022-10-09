const targetDiv = document.getElementById("user");
const btn = document.getElementById("toggleu");
btn.onclick = function () {
    if (targetDiv.style.display !== "none") {
        targetDiv.style.display = "none";
    } else {
        targetDiv.style.display = "block";
    }
};

const targetDiv2 = document.getElementById("team");
const btn2 = document.getElementById("togglet");
btn2.onclick = function () {
    if (targetDiv2.style.display !== "none") {
        targetDiv2.style.display = "none";
    } else {
        targetDiv2.style.display = "block";
    }
};