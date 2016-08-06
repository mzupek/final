var LoginController = function ($scope, $routeParams, $location, LoginFactory) {
    $scope.loginForm = {
        emailAddress: '',
        password: '',
        rememberMe: false,
        returnUrl: $routeParams.returnUrl,
        loginFailure: false
    };

    $scope.login = function () {
        var result = LoginFactory($scope.loginForm.emailAddress, $scope.loginForm.password, $scope.loginForm.rememberMe);
        
        result.then(function(result) {
            console.log(result.success);
            if (result.success) {
               
                    $location.path('/Start');
                      
            } else {
                $scope.loginForm.loginFailure = true;
            }
        });
    }
}

LoginController.$inject = ['$scope', '$routeParams', '$location', 'LoginFactory'];