angular.module('deviceApp', ['ui.bootstrap'])
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

    .controller('DeviceController', ['$scope', '$uibModal', '$http', function ($scope, $uibModal, $http) {
        $scope.showEmptyNameError = false;
        $scope.selectedZoneId = null;
        $scope.selectedSiteId = null;
        $scope.sites = []; 

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
                        $scope.sites = response.data; 
                    });
            } else {
                $scope.sites = [];
            }
        };

        $scope.cancelEdit = function (device) {
            device.editing = false;
        };

        $scope.openAddDeviceModal = function () {
            $scope.modalTitle = 'Add Device';
            $scope.modalDevice = {
                name: '',
                zoneId: ''
            };

            var modalInstance = $uibModal.open({
                templateUrl: 'addEditDeviceModal.html',
                controller: 'AddEditDeviceModalController',
                scope: $scope
            });

            modalInstance.result.then(function (resultDevice) {
                $http.post('/Device/AddDevice', { newName: resultDevice.name, zoneId: $scope.selectedZoneIds, siteId: $scope.selectedSiteIds })
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

        $scope.openEditDeviceModal = function (deviceId, deviceName, ZoneName) {
            $scope.modalTitle = 'Edit Device';
            $scope.modalDevice = {
                id: deviceId,
                name: deviceName,
                ZoneName: ZoneName
            };

            var modalInstance = $uibModal.open({
                templateUrl: 'addEditDeviceModal.html',
                controller: 'AddEditDeviceModalController',
                scope: $scope
            });

            modalInstance.result.then(function (resultDevice) {
                $http.post('/Device/UpdateDeviceName', { id: deviceId, newName: resultDevice.name, newZone: $scope.selectedZoneIds, newSite: $scope.selectedSiteIds })
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
angular.module('deviceApp').controller('AddEditDeviceModalController', ['$scope', '$uibModalInstance', function ($scope, $uibModalInstance) {
    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };

    $scope.save = function () {
        if ($scope.modalDevice.name.trim() !== '') {
            $uibModalInstance.close($scope.modalDevice);
        } else {
            $scope.showEmptyNameError = true;
        }
    };
}]);
