angular.module('ZoneSelectorModule', [])
    .directive('ZoneSelector', ['$http', function ($http) {
        return {
            restrict: 'E',
            scope: {
                zoneId: '='
            },
            template: `
                <div>
                    <select ng-model="zoneId" ng-change="getZones()">
                        <option value="" disabled>Select a zone</option>
                        <option ng-repeat="zone in zones" value="{{ zone.ZoneId }}">{{ zone.ZoneName }}</option>
                    </select>
                </div>

            `,
            link: function (scope) {
                scope.getZones = function () {
                    if (scope.zoneId) {
                        $http.get('/Zone/GetZones')
                            .then(function (response) {
                                scope.zones = response.data;
                            })
                            .catch(function (error) {
                                console.error('Error fetching numbers:', error);
                            });
                    }
                };

                scope.zoneSelected = function () {
                    console.log('Selected number ID:', scope.selectedNumber);
                };
                scope.getZones();
            }
        };
    }]);
