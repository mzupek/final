var jobminxApp = angular.module('jobminxApp', ['ngRoute', 'ngAnimate', 'buttonApp', 'ngStorage', 'ngResource', 'ngSanitize', 'searchApp', 'ui.bootstrap', 'angular-progress-arc', 'textAngular', 'cgBusy']);

jobminxApp.controller('LandingPageController', LandingPageController);
jobminxApp.controller('LoginController', LoginController);
jobminxApp.controller('RegisterController', RegisterController);
jobminxApp.controller('MyCtrl', MyCtrl);
jobminxApp.controller('SearchResultController', SearchResultController);
jobminxApp.controller('OptimizeController', OptimizeController); 
jobminxApp.controller('JobParseController', JobParseController);
jobminxApp.controller('DropboxController', DropboxController);
jobminxApp.controller('jobModalInstanceController', jobModalInstanceController);
jobminxApp.controller('ModalInstanceCtrl', ModalInstanceCtrl); 
jobminxApp.controller('PasteModalInstanceController', PasteModalInstanceController);
jobminxApp.controller('DriveController', DriveController);
//jobminxApp.factory('AuthHttpResponseInterceptor', AuthHttpResponseInterceptor);
//jobminxApp.factory('LoginFactory', LoginFactory);
//jobminxApp.factory('RegistrationFactory', RegistrationFactory);

var configFunction = function ($routeProvider, $httpProvider, $locationProvider) {

   //$locationProvider.hashPrefix('!').html5Mode(true);
    //$locationProvider.html5Mode({
    //    enabled: true,
    //    requireBase: false,
    //});
    $routeProvider.
        
        when('/One', {
            templateUrl: 'Scripts/App/One.html'
        })
        .when('/Two', {
            templateUrl: 'Scripts/App/Two.html'
        })
         .when('/Start', {
             templateUrl: 'Scripts/App/Start.html'
        })
        .otherwise({
            templateUrl: 'Scripts/App/Start.html'
        });

    

    //$httpProvider.interceptors.push('AuthHttpResponseInterceptor');
}
configFunction.$inject = ['$routeProvider', '$httpProvider', '$locationProvider'];

jobminxApp.config(configFunction);

