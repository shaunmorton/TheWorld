(function() {

    "use strict";

    //get module
    angular.module("app-trips")
        .controller("tripsController", tripsController);

    function tripsController($http) {

        var vm = this;
        vm.trips = [];
        vm.newTrip = {};
        vm.errorMessage = "";
        vm.isBusy = true;

        $http.get("/api/trips")
            .then(function(res) {
                    //success
                    angular.copy(res.data, vm.trips);
                },
                function(error) {
                    //failure
                    vm.errorMessage = "Failed to load data: " + error;
                })
            .finally(function() {
                vm.isBusy = false;
            });


        vm.addTrip = function() {
            vm.isBusy = true;
            vm.errorMessage = "";

            $http.post("/api/trips", vm.newTrip)
                .then(function(res) {
                        //success
                        vm.trips.push(res.data);
                        vm.newTrip = {};
                    },
                    function() {
                        //failure
                        vm.errorMessage = "Failed to save new trip";
                    })
                .finally(function() {
                    vm.isBusy = false;
                });
        };
    }

})();