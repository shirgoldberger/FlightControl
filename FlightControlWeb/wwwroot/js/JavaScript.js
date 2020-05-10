$(document).ready(function () {
    let map = createMap();
    g(map);

})

function g(map) {
    //key = flight_id, value = marker on map
    const plains = new Map()
    let markerIcon = L.icon({
        iconUrl: '/pic/plain.png',

        iconSize: [38, 20], // size of the icon
        shadowSize: [50, 64], // size of the shadow
        iconAnchor: [22, 94], // point of the icon which will correspond to marker's location
        shadowAnchor: [4, 62],  // the same for the shadow
        popupAnchor: [-3, -76] // point from which the popup should open relative to the iconAnchor
    });

    setInterval(function () {
        $.ajax({
            type: "GET",
            url: "/api/Flights",
            dataType: 'json',
            success: function (jdata) {
                jdata.forEach(function (item, i) {
                    //current location of the flight
                    let lat = parseFloat(item.latitude);
                    let long = parseFloat(item.longitude);
                    //the flight is already exist
                    if (plains.has(item.flight_id)) {
                        let flightMarker = plains.get(item.flight_id);
                        //set latitude, longitude.
                        flightMarker.setLatLng([lat, long]);
                    //new flight
                    } else {
                        //create a new marker on map.
                        let marker = L.marker([lat, long], { icon: markerIcon }).addTo(map)
                            .openPopup().on('click', onClick);
                        plains.set(item.flight_id, marker);
                        appendItem(item);
                    }
                });
            },
            error: errorCallback

            //add external flights here
        });
    }, 1000);
}

function errorCallback(jdata) {
    alert("Error");
}

//create a new row at the initial flights
function appendItem(item) {
    let tableRef  = document.getElementById("myFlightsTable").getElementsByTagName('tbody')[0];
    let row = tableRef.insertRow();
    row.id.replace(item.flight_id);
    let cell1 = row.insertCell(0);
    let cell2 = row.insertCell(1);
    let cell3 = row.insertCell(2);
    cell1.innerHTML = item.flight_id;
    cell2.innerHTML = item.company_name;
    cell3.innerHTML = item.date_time;
}



function onClick(e) {
    var coord = e.latlng.toString().split(',');
    var lat = coord[0].split('(');
    var long = coord[1].split(')');
    $.ajax({
        type: "GET",
        url: "/api/Flights",
        dataType: 'json',
        success: function (jdata) {
            updatePlan(lat[1], long[0],jdata)
        },
        error: errorCallback
    });
}


function updatePlan(lat, long, jdata) {
    var plan;
    jdata.forEach(function (item, i) {
        if (item.longitude == long && item.latitude == lat) {
            plan = item;
        }
    });
    alert(plan.flight_id);
    document.getElementById("flightID").textContent = plan.flight_id;
    document.getElementById("Company_name").textContent = plan.company_name;
    document.getElementById("Latitude").textContent = plan.latitude;
    document.getElementById("Longitude").textContent = plan.longitude;
    document.getElementById("Passengers").textContent = plan.passengers;
    document.getElementById("Date_time").textContent = plan.date_time;
    document.getElementById("Is_external").textContent = plan.is_external;
}

function createMap() {
    var map = L.map('map').setView([51.505, -0.09], 3);
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
    }).addTo(map);
    return map;
}