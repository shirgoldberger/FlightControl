﻿
function uploadOnDragOver() {
    document.getElementById("dropArea").style.display = "inline";
}

function uploadOnDragLeave() {
    document.getElementById("dropArea").style.display = "none";
}
function uploadOnDrop() {
    document.getElementById("dropArea").style.display = "none";
    setTimeout(submit, 100);
}

function submit() {
    let inputFile = document.getElementById("myFlightsInput").files[0];
    let reader = new FileReader();
    reader.readAsText(inputFile);
    let jdata;
    reader.onload = function () {
        jdata = reader.result.replace('/r', '');
        if (fileIsJson(jdata)) {
            postData(jdata);
        }
        else {
            showAlert("This project supports only valid JSON files");
        }
    }
}
async function postData(jdata) {
        let request = new XMLHttpRequest();
        request.open("POST", "/api/FlightPlan", true);
        request.setRequestHeader("Content-Type", "application/json");
        request.send(jdata);
}

function fileIsJson(str) {
    try {
        JSON.parse(str);
    } catch (e) {
        return false;
    }
    return true;
}