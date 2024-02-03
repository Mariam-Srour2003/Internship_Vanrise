angular.module('zoneApp', ['ui.bootstrap']).controller('ZoneController', ['$scope', '$uibModal', '$http', function ($scope, $uibModal, $http) {
    $scope.showEmptyNameError = false;
    $scope.cancelEdit = function (zone) {
        zone.editing = false;
    };

    $scope.openAddZoneModal = function () {
        $scope.modalTitle = 'Add Zone';
        $scope.modalZone = {
            name: ''
        };

        var modalInstance = $uibModal.open({
            templateUrl: 'addEditZoneModal.html',
            controller: 'AddEditZoneModalController',
            scope: $scope
        });

        modalInstance.result.then(function (resultZone) {
            $http.post('/Zone/AddZone', { newName: resultZone.name })
                .then(function (response) {
                    if (response.data.success) {
                        location.reload();
                    } else {
                    }
                })
                .catch(function (error) {
                });
        });

    };

    $scope.openEditZoneModal = function (zoneId, zoneName) {
        $scope.modalTitle = 'Edit Zone';
        $scope.modalZone = {
            id: zoneId,
            name: zoneName
        };

        var modalInstance = $uibModal.open({
            templateUrl: 'addEditZoneModal.html',
            controller: 'AddEditZoneModalController',
            scope: $scope
        });

        modalInstance.result.then(function (resultZone) {
            $http.post('/Zone/UpdateZoneName', { id: zoneId, newName: resultZone.name })
                .then(function (response) {
                    if (response.data.success) {
                        location.reload();
                    } else {
                    }
                })
                .catch(function (error) {
                });
        });
    };
}]);

angular.module('zoneApp').controller('AddEditZoneModalController', ['$scope', '$uibModalInstance', function ($scope, $uibModalInstance) {
    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };

    $scope.save = function () {
        if ($scope.modalZone.name.trim() !== '') {
            $uibModalInstance.close($scope.modalZone);
        } else {
            $scope.showEmptyNameError = true;
        }
    };
}]);
