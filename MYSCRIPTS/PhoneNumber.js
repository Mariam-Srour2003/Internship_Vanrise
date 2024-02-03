document.getElementById('searchForm').addEventListener('submit', function (e) {
    e.preventDefault();
    var deviceSelect = document.getElementById('deviceSelect');
    var searchText = document.getElementById('searchText').value;
    var selectedDeviceId = deviceSelect.options[deviceSelect.selectedIndex].value;
    var numericPart = selectedDeviceId.replace(/\D/g, '');
    var url = '/PhoneNumber?searchdeviceid=' + numericPart + '&searchText=' + encodeURIComponent(searchText);
    window.location.href = url;
});

angular.module('phonenumberApp', ['ui.bootstrap'])
    .directive('deviceSelector', function () {
        return {
            restrict: 'E',
            scope: {
                sentObject: '='
            },
            template: `
            <div class="form-group">
                <label for="deviceSelector">Device</label>
                <select id="deviceSelector" class="form-control" ng-model="selectedDeviceId" ng-options="device.DeviceId as device.DeviceName for device in devices" ng-change="selectDevice(selectedDeviceId)">
                    <option value="">Select a device</option>
                </select>
            </div>
        `,
            controller: function ($scope, $http) {
                $http.get('/Device/GetAllDevices')
                    .then(function (response) {
                        console.log(response.data);
                        $scope.devices = response.data;
                    });
                $scope.selectedDeviceId = ''; 

                $scope.selectDevice = function (deviceId) {
                    $scope.selectedDeviceId = deviceId;
                    $scope.sentObject.getData($scope.selectedDeviceId);
                };

            }
        };
    })
    .controller('PhoneNumberController', ['$scope', '$uibModal', '$http', function ($scope, $uibModal, $http) {
        $scope.showEmptyNumberError = false;

        $scope.scopeModel = $scope.scopeModel || {};
        $scope.scopeModel.deviceSelector = {
            getData: function (selectedDeviceId) {
                $scope.selectedDeviceIds = parseInt(selectedDeviceId);

                console.log('Selected Device ID:', selectedDeviceId);
            }
        };
        $scope.selectedDeviceIds = parseInt($scope.selectedDeviceId);
        $http.get('/Device/GetAllDevices')
            .then(function (response) {
                console.log(response.data);
                $scope.devices = response.data;
            });

        $scope.cancelEdit = function (phonenumber) {
            phonenumber.editing = false;
        };
        $scope.modalPhoneNumberr = {
            number: '',
            deviceId: ''
        };

        $scope.openAddPhoneNumberModal = function () {
            $scope.modalTitle = 'Add PhoneNumber';
            $scope.modalPhoneNumber = {
                number: '',
                deviceId: ''
            };

            var modalInstance = $uibModal.open({
                templateUrl: 'addEditPhoneNumberModal.html',
                controller: 'AddEditPhoneNumberModalController',
                scope: $scope
            });

            modalInstance.result.then(function (resultPhoneNumber) {
                $http.post('/PhoneNumber/AddPhoneNumber', { newNumber: resultPhoneNumber.number, deviceid: $scope.selectedDeviceIds })
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

        $scope.openEditPhoneNumberModal = function (phonenumberId, phonenumberNumber, deviceId) {
            $scope.modalTitle = 'Edit PhoneNumber';
            $scope.modalPhoneNumber = {
                id: phonenumberId,
                number: phonenumberNumber,
                deviceId: deviceId
            };

            var modalInstance = $uibModal.open({
                templateUrl: 'addEditPhoneNumberModal.html',
                controller: 'AddEditPhoneNumberModalController',
                scope: $scope
            });

            modalInstance.result.then(function (resultPhoneNumber) {
                $http.post('/PhoneNumber/UpdatePhoneNumberNumber', { id: phonenumberId, newNumber: resultPhoneNumber.number, deviceid: $scope.selectedDeviceIds })
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
    }])
    .controller('AddEditPhoneNumberModalController', ['$scope', '$uibModalInstance', function ($scope, $uibModalInstance) {
        $scope.cancel = function () {
            $uibModalInstance.dismiss('cancel');
        };

        $scope.save = function () {
            var number = String($scope.modalPhoneNumber.number);
            if (number.trim() !== '') {
                $scope.modalPhoneNumber.number = number;
                $uibModalInstance.close($scope.modalPhoneNumber);
            } else {
                $scope.showEmptyNumberError = true;
            }
        };
    }]);