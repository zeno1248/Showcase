var app = angular.module('myApp', []);
app.controller('myCtrl', function ($scope, $http) {
    $scope.firstName = "John";
    $scope.lastName = "Doe";
    $scope.showCarsList = false;
    $scope.showCalendarEvents = false;

    $scope.showCars = function () {
        if ($scope.showCarsList) {
            $scope.showCarsList = false;
            return;
        } else if ($scope.cars && $scope.cars.length > 0) {
            $scope.showCarsList = true;
            return;
        }
        var getCarsRequest = {
        };
        $http.post("/Ajax/GetCars", getCarsRequest)
        .then(
            function (response) {
                $scope.cars = response.data;
                $scope.showCarsList = true;
            },
            function (response) {
                $scope.showCarsList = false;
            }
        );
    };

    $scope.getCalendarEvents = function () {
        if ($scope.showCalendarEvents) {
            $scope.showCalendarEvents = false;
            return;
        } else if ($scope.events && $scope.events.length > 0){
            $scope.showCalendarEvents = true;
            return;
        }
        $.ajax({
            url: "/Ajax/GetCalendarEvents",
            method: "post"
        }).done(function (result) {
            $scope.events = result;
            $scope.showCalendarEvents = true;
            $scope.$apply();
         }).fail(function () {
              $scope.showCalendarEvents = false;
         });
        
    };

    $scope.updateCalendar = function () {

    };

    window.appScope = $scope;
});