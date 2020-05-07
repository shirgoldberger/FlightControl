var ViewModel = function () {
    var self = this; // make 'this' available to subfunctions or closures
    self.flight = ko.observableArray(); // enables data binding
    var map = L.map('map').setView([51.505, -0.09], 13);

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
    }).addTo(map);

    L.marker([51.5, -0.09]).addTo(map)
        .bindPopup('first plan')
        .openPopup();
    var FlightUri = "/api/Flights";
    function getAllFlights() {
        $.getJSON(FlightUri).done(function (data) {
            self.flight(data);
        });
    }
    self.currFlight = ko.observable();
    //self.getFlightDetails = function (flight) {
    // $.getJSON(booksUri + "/" + flight.Id).done(function (data) {
    //     self.currFlight(data);
    //  });
    //}
    self.removePlace = function (flight) {
        self.currFlight(flight);
    }
    // Fetch the initial data
    getAllFlights();


    //mapp

};
ko.applyBindings(new ViewModel()); // sets up the data binding
