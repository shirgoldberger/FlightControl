//idan:




let group = L.layerGroup();
let polyline;
let map = createMap();
let newFlight_Id = [];
let oldFlight_Id = [];
let planes = new Map();



let markedMarkerIcon = L.icon({
    iconUrl: '/pic/marked-plain.png',
    iconSize: [50, 47], // size of the icon
    iconAnchor: [50, 47], // point of the icon which will correspond to marker's location
    popupAnchor: [-3, -76] // point from which the popup should open relative to the iconAnchor
});


let markerIcon = L.icon({
    iconUrl: '/pic/plain.png',
    iconSize: [50, 47], // size of the icon
    iconAnchor: [50, 47], // point of the icon which will correspond to marker's location
    popupAnchor: [-3, -76] // point from which the popup should open relative to the iconAnchor
});


$(document).ready(function () {
    group.addTo(map);
    map.on('click', function () {
        group.clearLayers();
        deleteFlightDetails()

        //switch all markers icons' to non-marked icon
        map.eachLayer(function (layer) {
            if (layer instanceof L.Marker) {
                layer.setIcon(markerIcon);
            }
        });
    });
    g(map, polyline);
})


function g(map, polyline) {
    //key = flight_id, value = marker on map

    setInterval(function () {
        let d_ = new Date();
        let date = getUTC(d_);
        console.log(date);
        let url = "/api/Flights?relative_to=";
        url = url.concat(date);
        url = url.concat("&sync_all=true");
        $.ajax({
            type: "GET",
            url: url,
            dataType: 'json',
            success: function (jdata) {
                handleFlights(jdata, markerIcon, polyline);
            },
            error: errorCallback

        });
    }, 20000);
}

function getUTC(d) {
    let day = ('0' + d.getUTCDate()).substr(-2);
    let hours = ('0' + d.getUTCHours()).substr(-2);
    let year = ('0000' + d.getUTCFullYear()).substr(-4);
    let minute = ('0' + d.getUTCMinutes()).substr(-2);
    let month = ('0' + (d.getUTCMonth() + 1)).substr(-2);
    let second = ('0' + d.getUTCSeconds()).substr(-2);
    let date = year.concat("-").concat(month).concat("-").concat(day)
        .concat("T").concat(hours).concat(":").concat(minute).concat(":").concat(second).concat("Z");
    return date;
}


function handleFlights(jdata, markerIcon, polyline) {
    jdata.forEach(function (item, i) {
        //current location of the flight
        let lat = parseFloat(item.latitude);
        let long = parseFloat(item.longitude);
        //enter to new id's list
        newFlight_Id.push(item.flight_id);
        //the flight is already exist
        if (planes.has(item.flight_id)) {
            let flightMarker = planes.get(item.flight_id);
            //set latitude, longitude.
            flightMarker.setLatLng([lat, long]);
            //new flight
        } else {
            //create a new marker on map.
            let marker = L.marker([lat, long], { icon: markerIcon }).addTo(map)
                .openPopup().on('click', function (e) {
                    onClick(item, polyline, e);
                });
            marker.id = item.flight_id;
            planes.set(item.flight_id, marker);
            appendItem(item);
        }
    });
    //remove all flights that doesn't apeared at this section.
    //maybe they were deleted or ended.
    oldFlight_Id.forEach(removeIrrelevant);
    //update lists
    oldFlight_Id = [];
    oldFlight_Id = newFlight_Id.slice();;
    newFlight_Id = [];


}

function removeIrrelevant(id, index) {
    if (!newFlight_Id.includes(id)) {
        //remove flight from view
        let tr = document.getElementById(id);
        if (tr) {
            tr.parentNode.removeChild(tr);

            //delete the marker from the map.
            deleteMarker(id);

            //delete filght details if it was presed
            deleteFlightDetails(id);
        }

    }
}
function errorCallback() {
    alert("Error");
}

//create a new row at the initial flights
function appendItem(item) {
    let tableRef = document.getElementById("myFlightsTable").getElementsByTagName('tbody')[0];
    let row = tableRef.insertRow();
    //onClick event
    row.addEventListener("click", function (e) {
        rowListener(item, row);
    });
    row.setAttribute("id",item.flight_id);
    let cell1 = row.insertCell(0);
    let cell2 = row.insertCell(1);
    let cell3 = row.insertCell(2);
    let cell4 = row.insertCell(3);
    cell1.innerHTML = item.flight_id;
    cell2.innerHTML = item.company_name;
    cell3.innerHTML = item.date_time;
    let x = document.createElement("INPUT");
    x.setAttribute("type", "image");
    x.setAttribute("src", "pic/garbage.png");
    x.setAttribute("style", "width: 20px;height: 20px");
    x.addEventListener("click", function () {
        garbageFunc(item);
    });
    cell4.appendChild(x);
    //<td><input type="button" value="Delete Row" onclick="SomeDeleteRowFunction()"></td>

}


function rowListener(item, row) {
    updateDetails(item);
    markRow(row);
    //locate the the the marker with same ID, mark it and show the polyline
    for (const k of planes.keys()) {
        if (k.toString() === item.flight_id) {
            let marker = planes.get(k);
            marker.setIcon(markedMarkerIcon);
        }
        else {
            let marker = planes.get(k);
            marker.setIcon(markerIcon);
        }
    }
    //polyline
    //idannnn
    //getFlightPlanByItem(item);

}


function markRow(row) {
    var tds = document.querySelectorAll('#myFlightsTable tbody tr'), i;
    //delete mark from other rows in myFlightsTable
    for (i = 0; i < tds.length; ++i) {
        tds[i].classList.remove("bg-primary");
    }
    var tds = document.querySelectorAll('#externalFlightsTable tbody tr'), i;
    //delete mark from other rows in externalFlightsTable
    for (i = 0; i < tds.length; ++i) {
        tds[i].classList.remove("bg-primary");
    }
    // mark current row
    row.classList.add("bg-primary");
}


function garbageFunc(item) {
    // event.target will be the input element.
    let td = event.target.parentNode;
    let tr = td.parentNode; // the row to be removed
    tr.parentNode.removeChild(tr);
    //let trId = tr.id;
    //delete flight from database if its a local flight.
    if (!item.is_external) {
        let url = "/api/Flights/"
        url = url.concat(item.flight_id);
        $.ajax({
            type: "DELETE",
            url: url,
            dataType: 'json',
            error: function () {
                alert("problemm delete");
            }
        });
    }

    //delete the marker from the map.
    deleteMarker(item.flight_id);

    //delete filght details if it was presed
    deleteFlightDetails(item.flight_id);
}

//get a flight and delete it from the markers(planes) list if its there.
function deleteMarker(id) {
    if (planes.has(id))
        for (const k of planes.keys()) {
            if (k.toString() === id) {
                let marker = planes.get(k);
                planes.delete(k);
                map.removeLayer(marker);
            }
        }
}

//idannnn
//if the flight details was shown
function deleteFlightDetails(flight_id) {
    let flightId = document.getElementById("flightID").textContent;
    if (flight_id === flightId) {
        document.getElementById("flightID").textContent = "";
        document.getElementById("Company_name").textContent = "";
        document.getElementById("Latitude").textContent = "";
        document.getElementById("Longitude").textContent = "";
        document.getElementById("Passengers").textContent = "";
        document.getElementById("Date_time").textContent = "";
        document.getElementById("Is_external").textContent = "";
        group.clearLayers();

    }
}


function onClick(item, polyline, e) {
    updateDetails(item);

    var tds = document.querySelectorAll('#myFlightsTable tbody tr'), i;
    //delete mark from other rows in myFlightsTable
    let row;
    for (i = 0; i < tds.length; ++i) {
        if (tds[i].id === item.flight_id) {
            row = tds[i];
        }
    }
    tds = document.querySelectorAll('#externalFlightsTable tbody tr'), i;
    for (i = 0; i < tds.length; ++i) {
        if (tds[i].id === item.flight_id) {
            row = tds[i];
        }
    }
    markRow(row);

    //switch all markers icons' to non-marked icon
    map.eachLayer(function (layer) {
        if (layer instanceof L.Marker) {
            layer.setIcon(markerIcon);
        }
    });
    //mark current marker with marked-icon
    var layer = e.target;
    layer.setIcon(markedMarkerIcon);
    getFlightPlanByItem(item);
}

function getFlightPlanByItem(item) {
    //GET flightPlan/flight_ID
    let url = "/api/FlightPlan/"
    url = url.concat(item.flight_id);
    $.ajax({
        type: "GET",
        url: url,
        dataType: 'json',
        success: function (jdata) {
            createPolyline(jdata, polyline);
        },
        error: function () {
            alert("get error");
        }
    });
}

function updateDetails(item) {
    document.getElementById("flightID").textContent = item.flight_id;
    document.getElementById("Company_name").textContent = item.company_name;
    document.getElementById("Latitude").textContent = item.latitude;
    document.getElementById("Longitude").textContent = item.longitude;
    document.getElementById("Passengers").textContent = item.passengers;
    document.getElementById("Date_time").textContent = item.date_time;
    document.getElementById("Is_external").textContent = item.is_external;
}



function createPolyline(jdata, polyline) {
    let segments = jdata.segments;
    let longitude;
    let latitude;
    let location
    let polylineArray = [];
    for (let i = 0; i < segments.length; i++) {
        longitude = segments[i]["longitude"];
        latitude = segments[i]["latitude"];
        location = [latitude, longitude];
        polylineArray.push(location);
    }
    group.clearLayers();
    polyline = L.polyline(polylineArray, { color: 'red' }).addTo(group);
}

function createMap() {
    let map = L.map('map').setView([51.505, -0.09], 3);
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
    }).addTo(map);
    return map;
}