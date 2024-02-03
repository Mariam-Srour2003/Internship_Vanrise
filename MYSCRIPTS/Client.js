angular.module('clientApp', ['ui.bootstrap'])
    .directive('zoneSelector', function () {
        return {
            restrict: 'E',
            scope: {
                sentObject: '=',
                selectedZoneId: '=' // Pass selectedZoneId to the directive
            },
            template: `
                <div class="form-group">
                    <label for="zoneSelector">Zone</label>
                    <select id="zoneSelector" class="form-control" ng-model="selectedZoneId" ng-options="zone.ZoneId as zone.ZoneName for zone in zones" ng-change="selectZone(selectedZoneId)">
                        <option value="">Select a zone</option>
                    </select>
                </div>
            `,
            controller: function ($scope, $http) {
                $http.get('/Zone/GetZones')
                    .then(function (response) {
                        console.log('this', response.data);
                        $scope.zones = response.data;
                    });

                $scope.selectZone = function (zoneId) {
                    $scope.selectedZoneId = zoneId;
                    $scope.sentObject.getData($scope.selectedZoneId);
                };
            }
        };
    })
    .directive('siteSelector', function () {
        return {
            restrict: 'E',
            scope: {
                sentObject: '=',
                selectedZoneId: '=',
                sites: '=' // Pass sites to the directive
            },
            template: `
            <div class="form-group">
                <label for="siteSelector">Site</label>
                <select id="siteSelector" class="form-control" ng-model="selectedSiteId" ng-options="site.SiteId as site.SiteName for site in sites" ng-change="selectSite(selectedSiteId)">
                    <option value="">Select a site</option>
                </select>
            </div>
        `,
            controller: function ($scope) {
                $scope.selectSite = function (siteId) {
                    $scope.selectedSiteId = siteId;
                    $scope.sentObject.getData($scope.selectedSiteId);
                };
            }
        };
    })
    .controller('ClientController', ['$scope', '$uibModal', '$http', function ($scope, $uibModal, $http) {
    $scope.showEmptyNameError = false;
        $scope.selectedZoneId = null;
        $scope.sites = []; // Initialize the sites array

        $scope.scopeModel = $scope.scopeModel || {};
        $scope.scopeModel.zoneSelector = {
            getData: function (selectedZoneId) {
                $scope.selectedZoneIds = parseInt(selectedZoneId);
                console.log('Selected Zone ID:', selectedZoneId);
                $scope.updateSites();
            }
        };
        $scope.selectedZoneIds = parseInt($scope.selectedZoneId);

        $scope.scopeModel.siteSelector = {
            getData: function (selectedSiteId) {
                $scope.selectedSiteIds = parseInt(selectedSiteId);
                console.log('Selected Site ID:', selectedSiteId);
            }
        };
        $scope.selectedSiteIds = parseInt($scope.selectedSiteId);

        $scope.updateSites = function () {
            console.log('Updating sites with zoneId:', $scope.selectedZoneIds);

            if ($scope.selectedZoneIds) {
                $http.get('/Site/GetSitesByZoneId', { params: { zoneId: $scope.selectedZoneIds } })
                    .then(function (response) {
                        console.log('Fetched Sites:', response.data);
                        $scope.sites = response.data; // Update the sites array
                    });
            } else {
                $scope.sites = []; // Clear the sites array
            }
        };

    $scope.calculateAge = function (birthday) {
        const today = new Date();
        const birthDate = new Date(birthday);
        const age = today.getFullYear() - birthDate.getFullYear();

        if (today.getMonth() < birthDate.getMonth() || (today.getMonth() === birthDate.getMonth() && today.getDate() < birthDate.getDate())) {
            age--;
        }

        return age;
    };
    $scope.cancelEdit = function (client) {
        client.editing = false;
    };

    $scope.openAddClientModal = function () {
        $scope.modalTitle = 'Add Client';
        $scope.modalClient = {
            name: '',
            type: '2',
            birthday: ''
        };

        var modalInstance = $uibModal.open({
            templateUrl: 'addEditClientModal.html',
            controller: 'AddEditClientModalController',
            scope: $scope
        });

        modalInstance.result.then(function (resultClient) {
            $http.post('/Client/AddClient', {
                newName: resultClient.name,
                Birthday: resultClient.birthday,
                Type: resultClient.type, newZone: $scope.selectedZoneIds, newSite: $scope.selectedSiteIds
            }).then(function (response) {
                if (response.data.success) {
                    location.reload();
                } else {
                }
            }).catch(function (error) {
            });
        });
    };
    $scope.openEditClientModal = function (clientId, clientName, clientType, clientBirthday) {
        $scope.modalTitle = 'Edit Client';
        $scope.modalClient = {
            id: clientId,
            name: clientName,
            type: clientType,
            birthday: clientBirthday
        };

        var modalInstance = $uibModal.open({
            templateUrl: 'addEditClientModal.html',
            controller: 'AddEditClientModalController',
            scope: $scope
        });

        modalInstance.result.then(function (resultClient) {
            $http.post('/Client/EditClient', {
                clientId: resultClient.id,
                newName: resultClient.name,
                newType: resultClient.type,
                newBirthday: resultClient.birthday, newZone: $scope.selectedZoneIds, newSite: $scope.selectedSiteIds
            }).then(function (response) {
                if (response.data.success) {
                    location.reload();
                } else {
                }
            }).catch(function (error) {

            });
        });
    };
        $scope.openReservationClientModal = function (clientId, clientName) {
            $scope.modalTitle = 'Reservation';
            $scope.modalClient = {
                clientId: clientId,
                clientName: clientName,
                phonenumberId: ''
            };
            $http.get('/PhoneNumber/GetUnreservedPhoneNumbers?clientId=' + clientId)
                .then(function (response) {
                    console.log(response.data);
                    $scope.unreservedphonenumbers = response.data;

                    var modalInstance = $uibModal.open({
                        templateUrl: 'ReservationClientModal.html',
                        controller: 'ReservationClientModalController',
                        scope: $scope 
                    });

                    modalInstance.result.then(function (resultPhoneNumber) {
                        $http.post('/Client/AddReservation', {
                            clientId: resultPhoneNumber.clientId,
                            phoneNumberId: resultPhoneNumber.phonenumberId
                        }).then(function (response) {
                            if (response.data.success) {
                                location.reload();
                            } else {
                            }
                        }).catch(function (error) {
                        });
                    });
                });
            var modalInstance = $uibModal.open({
                templateUrl: 'ReservationClientModal.html',
                controller: 'ReservationClientModalController',
                scope: $scope
            });

            modalInstance.result.then(function (resultPhoneNumber) {
                $http.post('/Client/AddReservation', {
                    clientId: resultPhoneNumber.clientId,
                    phoneNumberId: resultPhoneNumber.phonenumberId
                }).then(function (response) {
                    if (response.data.success) {
                        location.reload();
                    } else {
                    }
                }).catch(function (error) {
                });
            });
        };

    $scope.openUnReservationClientModal = function (clientId, clientName) {
        $http.get('/PhoneNumber/GetReservedPhoneNumbersForClient?clientId=' + clientId)
            .then(function (response) {
                    console.log(response.data);
                $scope.reservedPhoneNumbers = response.data;
                openModall(clientId, clientName);
            });
    };
    function openModall(clientId, clientName) {
        $scope.modalTitle = 'UnReservation';
        $scope.modalClient = {
            clientId: clientId,
            clientName: clientName,
            phonenumberId: '',
            ReservedPhoneNumbersId: []
        };




        var modalInstance = $uibModal.open({
            templateUrl: 'UnReservationClientModal.html',
            controller: 'UnReservationClientModalController',
            scope: $scope
        });

        modalInstance.result.then(function (resultPhoneNumber) {
            $http.post('/Client/Unreserve', {
                clientId: resultPhoneNumber.clientId,
                phoneNumberId: resultPhoneNumber.phonenumberId
            }).then(function (response) {
                if (response.data.success) {
                    location.reload();
                } else {
                }
            }).catch(function (error) {
            });
        });
    };


}]);


angular.module('clientApp').controller('AddEditClientModalController', ['$scope', '$uibModalInstance', function ($scope, $uibModalInstance) {
    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };
    $scope.save = function () {
        var age = $scope.modalClient.birthday ? $scope.calculateAge($scope.modalClient.birthday) : null;

        if ($scope.modalClient.name.trim() !== '' && (age === null || age >= 18)) {
            $uibModalInstance.close($scope.modalClient);
        } else {
            if (age !== null && age < 18) {
                $scope.ageValidationError = "Age must be 18 or older.";
            }
            $scope.showEmptyNameError = $scope.modalClient.name.trim() === '';
        }
    };
}]);

angular.module('clientApp').controller('ReservationClientModalController', ['$scope', '$uibModalInstance', function ($scope, $uibModalInstance) {
    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };

    $scope.save = function () {
        if ($scope.modalClient.phonenumberId !== '') {
            var resultPhoneNumber = {
                clientId: $scope.modalClient.clientId,
                phonenumberId: $scope.modalClient.phonenumberId
            };
            $uibModalInstance.close(resultPhoneNumber);
        } else {
        }
    };
}]);


angular.module('clientApp').controller('UnReservationClientModalController', ['$scope', '$uibModalInstance', '$http', function ($scope, $uibModalInstance, $http) {
    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };

    $http.get('/PhoneNumber/GetReservedPhoneNumbersForClient?clientId=' + $scope.modalClient.clientId)
        .then(function (response) {
            if (response.data.success) {
                $scope.reservedPhoneNumbers = response.data;
            } else {
                console.error(response.data.message);
            }
        })
        .catch(function (error) {
            console.error(error);
        });


    $scope.save = function () {
        if ($scope.modalClient.phonenumberId !== '') {
            $uibModalInstance.close($scope.modalClient);
        } else {
        }
    };
}]);
