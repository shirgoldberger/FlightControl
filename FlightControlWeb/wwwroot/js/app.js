var ViewModel = function () {
    var self = this; // make 'this' available to subfunctions or closures
    self.flight = ko.observableArray(); // enables data binding

    var booksUri = "/api/Flights";
    function getAllFlights(){
        $.getJSON(booksUri).done(function (data) {
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
};
ko.applyBindings(new ViewModel()); // sets up the data binding
