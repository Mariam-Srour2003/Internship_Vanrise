angular.module('siteApp', ['ui.bootstrap']).controller('SiteController', ['$scope', '$uibModal', '$http', function ($scope, $uibModal, $http) {
    $scope.showEmptyNameError = false;
    $scope.cancelEdit = function (site) {
        site.editing = false;
    };

    $scope.openAddSiteModal = function () {
        $scope.modalTitle = 'Add Site';
        $scope.modalSite = {
            name: ''
        };

        var modalInstance = $uibModal.open({
            templateUrl: 'addEditSiteModal.html',
            controller: 'AddEditSiteModalController',
            scope: $scope
        });

        modalInstance.result.then(function (resultSite) {
            $http.post('/Site/AddSite', { newName: resultSite.name })
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

    $scope.openEditSiteModal = function (siteId, siteName) {
        $scope.modalTitle = 'Edit Site';
        $scope.modalSite = {
            id: siteId,
            name: siteName
        };

        var modalInstance = $uibModal.open({
            templateUrl: 'addEditSiteModal.html',
            controller: 'AddEditSiteModalController',
            scope: $scope
        });

        modalInstance.result.then(function (resultSite) {
            $http.post('/Site/UpdateSiteName', { id: siteId, newName: resultSite.name })
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

angular.module('siteApp').controller('AddEditSiteModalController', ['$scope', '$uibModalInstance', function ($scope, $uibModalInstance) {
    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };

    $scope.save = function () {
        if ($scope.modalSite.name.trim() !== '') {
            $uibModalInstance.close($scope.modalSite);
        } else {
            $scope.showEmptyNameError = true;
        }
    };
}]);
