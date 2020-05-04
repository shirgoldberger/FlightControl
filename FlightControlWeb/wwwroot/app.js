var ViewModel = function () {
    var self = this; // make 'this' available to subfunctions or closures
    self.Flight = ko.observableArray(); // enables data binding
    var booksUri = "/api/Flights";
    function getAllBooks() {
        $.getJSON(booksUri).done(function (data) {
            self.Flight(data);
        });
    }
    // Fetch the initial data
    getAllBooks();
};
ko.applyBindings(new ViewModel()); 