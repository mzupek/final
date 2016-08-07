/*globals angular:true*/
var buttonApp = angular.module('buttonApp', ['ngStorage', 'ngResource'])



buttonApp.directive("contenteditable", function () {
    return {
        restrict: "A",
        require: "ngModel",
        link: function (scope, element, attrs, ngModel) {

            function read() {
                ngModel.$setViewValue(element.html());
            }

            ngModel.$render = function () {
                element.html(ngModel.$viewValue || "");
            };

            element.bind("blur keyup change", function () {
                scope.$apply(read);
            });
        }
    };
});


buttonApp.directive('fileChange', [
    function () {
        return {
            link: function (scope, element, attrs) {
                element[0].onchange = function () {
                    scope[attrs['fileChange']](element[0])
                    
                }
            }

        }
    }
])


function MyCtrl($scope, $window, $location, $http, $localStorage, $sessionStorage) {
    $scope.setFile = function (element) {
        $scope.$apply(function () {
            $scope.theFile = element.files[0];
            //$scope.$apply();
            var fd = new FormData();
            fd.append('File', $scope.theFile);
            $http.post('Api/Parse/PostResumeParse', fd,
            {
                transformRequest:angular.identity,
                headers: { 'Content-Type': undefined}
            })
            .success(function (data, status, headers, config) {

                    
                    $scope.successMessage = "Your Request Was Successfully Created?";
                    $scope.showErrorMessage = false;
                    $scope.showSuccessMessage = true;
                    $scope.info = JSON.parse(data.results);
                    $scope.zip = data.zipcode;
               
                    console.log(data.results);
                    console.log(data.zipcode);
                    console.log($scope.info.Keywords);
                    console.log($scope.info.Concepts);
                    console.log($scope.info.Skills);
                    console.log($scope.info.JobTitle);
                    
                    $localStorage.query = $scope.info.JobTitle;
                    $localStorage.loc = $scope.zip;
                    $localStorage.resumeText = $scope.info.Text;

                    function onlyUnique(value, index, self) {
                        return self.indexOf(value) === index;
                    }

                      
           
                    var unique = $scope.info.Competencies.filter(onlyUnique);

                    $localStorage.CurrentResumeKeyowrds = unique;
                    mixpanel.track("UploadedResume");
                    $location.path('/One');

                    
                })           
            
        })
    };
}

var DropboxController = function ($scope, $http, $localStorage, $sessionStorage, $resource, $modal, $location) {

        $scope.dropbox = function(){

        options = {

                // Required. Called when a user selects an item in the Chooser.
                success: function(files) {
                    
                      data =
                             {
                                 url: files[0].link,
                                 
                             };

   
                    $http.post('/Api/Parse/UploadFromDropBox', data, {
        
                        headers: { 'Content-Type': "application/json" }
                    })
                            .success(function (data, status, headers, config) {


                                $scope.resume = {};
                                $scope.resume = data.resume;
                                
                                data =
                                     {
                                         resumeText: $scope.resume

                                     };


                                      $http.post('/Api/Parse/TextResumeParse', data, {
        
                                        headers: { 'Content-Type': "application/json" }
                                    })
                                    
                                    .success(function (data, status, headers, config) {
                                            
                                                $scope.successMessage = "Your Request Was Successfully Created?";
                                                $scope.showErrorMessage = false;
                                                $scope.showSuccessMessage = true;
                                                $scope.info = JSON.parse(data.results);
                                                $scope.zip = data.zipcode;
               
                                                console.log("All Data From Resume:" + data.results);
                                                //console.log(data.zipcode);
                                                //console.log($scope.info.Keywords);
                                                //console.log($scope.info.Concepts);
                                                //console.log($scope.info.Skills);
                                                //console.log($scope.info.JobTitle);
                    
                                                $localStorage.query = $scope.info.JobTitle;
                                                $localStorage.loc = $scope.zip;
                                                $localStorage.resumeText = $scope.info.Text;

                                                function onlyUnique(value, index, self) {
                                                    return self.indexOf(value) === index;
                                                }

                      
           
                                                var unique = $scope.info.Competencies.filter(onlyUnique);

                                                $localStorage.CurrentResumeKeyowrds = unique;
                                                mixpanel.track("UploadedResume");
                                                $location.path('/One');
                                     
                                     });

                            });
                },

                // Optional. Called when the user closes the dialog without selecting a file
                // and does not include any parameters.
                cancel: function() {

                },

                // Optional. "preview" (default) is a preview link to the document for sharing,
                // "direct" is an expiring link to download the contents of the file. For more
                // information about link types, see Link types below.
                linkType: "direct", // or "direct"

                // Optional. A value of false (default) limits selection to a single file, while
                // true enables multiple file selection.
                multiselect: false, // or true

                // Optional. This is a list of file extensions. If specified, the user will
                // only be able to select files with these extensions. You may also specify
                // file types, such as "video" or "images" in the list. For more information,
                // see File types below. By default, all extensions are allowed.
                extensions: ['.pdf', '.doc', '.docx'],
        };
        Dropbox.choose(options);

        }

}

var SearchResultController = function ($scope, $http, $localStorage, $sessionStorage, $resource, $modal) {

   
    $scope.query = $localStorage.query;
    $scope.loc = $localStorage.loc;
   
    data =
             {
                 search: $scope.query,
                 loc: $scope.loc
             };

   
    $http.post('/Indeed/SearchIndeed', data, {
        
        headers: { 'Content-Type': "application/json" }
    })
            .success(function (data, status, headers, config) {


                $scope.jobs = {};
                $scope.jobs = data.results;
                mixpanel.track("InitSearch");
               
                console.log($scope.jobs);
            });
    


   
    $scope.search = function ($scope) {
         // <-- here is you value from the input
       
        
            $localStorage.loc = $scope.loc;
            $localStorage.query = $scope.query;


        data =
                 {
                     search: $localStorage.query,
                     loc: $localStorage.loc
                 };


        $http.post('/Indeed/SearchIndeed', data, {

            headers: { 'Content-Type': "application/json" }
        })
                .success(function (data, status, headers, config) {
                    $scope.jobs = {};
                    $scope.jobs = data.results;
                    mixpanel.track("SelfSearch");
                   
                    console.log($scope.jobs);
                });


    };


}



var JobParseController = function ($scope, $http, $localStorage, $sessionStorage, $resource, $location, $modal, $log) {

   


    $scope.parse = function ($scope) {
        //reference to the button that triggered the function:
        
        console.log($scope.job.url);
        $localStorage.JobTitle = $scope.job.jobtitle;
        $localStorage.Location = $scope.job.formattedLocation;
        $localStorage.Company = $scope.job.company;
        $localStorage.CurrentJobUrl = $scope.job.url;
        data =
             {
                 url: $scope.job.url
                 
             };
        $http.post('/Indeed/GetJobFromIndeed', data, {

            headers: { 'Content-Type': "application/json" }
        })
            .success(function (data, status, headers, config) {
                
                $scope.jobdesc = {}
                $scope.jobdesc = data.jobdesc;
                $scope.jobtitle = {};
                $scope.jobtitle = data.jobtitle;

                console.log($scope.jobdesc);
                console.log($scope.jobtitle);
                

                jobdata =
                {
                    jobtext: data.jobdesc

                };
                $http.post('Api/Parse/PostJobParse', JSON.stringify(jobdata), {

                    headers: { 'Content-Type': "application/json" }
                })
                    .success(function (data, status, headers, config) {
                        $scope.info = JSON.parse(data.results);
                        $scope.jobskills = {}
                        $scope.jobskills = $scope.info.Competencies;
                       
                       
                        console.log($scope.jobskills);
                        

                        function onlyUnique(value, index, self) {
                            return self.indexOf(value) === index;
                        }



                        var uniqueskills = $scope.jobskills.filter(onlyUnique);
                        console.log(uniqueskills);
                        $localStorage.CurrentJobKeywords = uniqueskills;
                        $localStorage.CurrentJobTitle = $scope.jobtitle;
                        $localStorage.CurrentJobDesc = $scope.jobdesc;

                        mixpanel.track("Optimize");

                        $location.path('/Two');

                    });

            });
    };

    $scope.open = function ($scope) {

        console.log($scope.job.url);
        
        data =
             {
                 url: $scope.job.url
                 
             };
        $http.post('/Indeed/GetJobFromIndeed', data, {

            headers: { 'Content-Type': "application/json" }
        })
            .success(function (data, status, headers, config) {
                $scope.jobdesc = {}
                $scope.jobdesc = data.jobdesc;
                $scope.jobtitle = {};
                $scope.jobtitle = data.jobtitle;

                console.log($scope.jobdesc);
                console.log($scope.jobtitle);

                $scope.Djob = {

                    title: $scope.jobtitle,
                    desc: $scope.jobdesc
                }
                mixpanel.track("ViewedJob");
                var modalInstance = $modal.open({
                    templateUrl: 'myModalContent.html',
                    controller: 'ModalInstanceCtrl',
                    size: 'lg',
                    resolve: {
                        Djob: function () {
                            return $scope.Djob;
                        }
                    }
                });

            });

        

        
    };

    $scope.paste = function()
    {
        var modalInstance = $modal.open({
            templateUrl: 'PasteModalContent.html',
            controller: 'PasteModalInstanceController',
            size: 'lg'
            
        });

       
    }
}

var OptimizeController = function ($scope, $http, $localStorage, $sessionStorage, $resource, $sce, $filter, $modal, $log) {

    $scope.currentJobUrl = $localStorage.CurrentJobUrl;
    $scope.resumeText = $localStorage.resumeText;
    $scope.html = $sce.trustAsHtml($scope.resumeText);
    console.log("html:" + $scope.html);
    $scope.resumeKeys = $localStorage.CurrentResumeKeyowrds;
    $scope.jobKeys = $localStorage.CurrentJobKeywords;
    $scope.Keep = _.intersection($scope.jobKeys, $scope.resumeKeys);
    $scope.Missing = _.difference($scope.jobKeys, $scope.resumeKeys);

    $scope.Score = Math.ceil($scope.Keep.length / $scope.jobKeys.length * 100);
    //$scope.resumeText = $filter('highlight')($scope.resumeText, $scope.Keep, $scope.Missing, $scope.Score);

    $scope.JobTitle = $localStorage.JobTitle;
    $scope.Location = $localStorage.Location;
    $scope.Company = $localStorage.Company;

    $scope.currentJobTitle = $localStorage.CurrentJobTitle;
    $scope.currentJobDesc = $localStorage.CurrentJobDesc;

    $scope.track = function ($scope) {

        $scope.resumeText = $filter('highlight')($scope.resumeText, $scope.Keep, $scope.Missing, $scope.Score);
       
        $scope.final = $scope.Keep.length + $scope.Missing.length;
        $scope.Score = Math.ceil($scope.Keep.length / $scope.final * 100);
        $scope.range = rangy.saveSelection();
        rangy.restoreSelection($scope.range);
        mixpanel.track("Rescore");


    };

    $scope.viewjob = function($scope)
    {

        $scope.Cjob = {

            title: $scope.currentJobTitle,
            desc: $scope.currentJobDesc
        }
        mixpanel.track("ViewJobContext");
        var modalInstance = $modal.open({
            templateUrl: 'jobModalContent.html',
            controller: 'jobModalInstanceController',
            size: 'lg',
            resolve: {
                Cjob: function () {
                    return $scope.Cjob;
                }
            }

        });
    }


   
  

}


//var ModalDemoCtrl = function ($scope, $modal, $log) {

//    $scope.items = ['item1', 'item2', 'item3'];

//    $scope.open = function (size) {

//        var modalInstance = $modal.open({
//            templateUrl: 'myModalContent.html',
//            controller: 'ModalInstanceCtrl',
//            size: size,
//            resolve: {
//                items: function () {
//                    return $scope.items;
//                }
//            }
//        });

//        modalInstance.result.then(function (selectedItem) {
//            $scope.selected = selectedItem;
//        }, function () {
//            $log.info('Modal dismissed at: ' + new Date());
//        });
//    };
//}

// Please note that $modalInstance represents a modal window (instance) dependency.
// It is not the same as the $modal service used above.

var ModalInstanceCtrl = function ($scope, $modalInstance, Djob) {

    //$scope.items = items;
    //$scope.selected = {
    //    item: $scope.items[0]
    //};

    $scope.MJtitle = Djob.title;
    $scope.MJdesc = Djob.desc;

    //$scope.ok = function () {
    //    $modalInstance.close($scope.selected.item);
    //};

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
}

var PasteModalInstanceController = function ($scope, $modalInstance, $http, $localStorage, $location)
{

    //$scope.ok = function () {
    //    $modalInstance.close($scope.selected.item);
    //};

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };

    $scope.op = function (jd) {

        console.log("jd:" + jd);
        $localStorage.Location = "";
        $localStorage.Company = "";
        jobdata =
               {
                   jobtext: jd

               };
        $http.post('Api/Parse/PostJobParse', JSON.stringify(jobdata), {

            headers: { 'Content-Type': "application/json" }
        })
            .success(function (data, status, headers, config) {
                $scope.jobskills = {}
                $scope.jobskills = data.skills;
                $scope.vals = {};
                $scope.vals = data.singleValues;

                console.log($scope.jobskills);
                console.log($scope.vals.jobTitle);

                function onlyUnique(value, index, self) {
                    return self.indexOf(value) === index;
                }



                var uniqueskills = $scope.jobskills.filter(onlyUnique);
                console.log(uniqueskills);
                $localStorage.CurrentJobKeywords = uniqueskills;
                $localStorage.CurrentJobTitle = $scope.vals.jobTitle;
                $localStorage.CurrentJobDesc = jd;
                mixpanel.track("PastedJob");

                $modalInstance.dismiss('cancel');
                $location.path('/Two');

            });

    }

}

var jobModalInstanceController = function ($scope, $modalInstance, Cjob) {

    //$scope.ok = function () {
    //    $modalInstance.close($scope.selected.item);
    //};

    $scope.CJtitle = Cjob.title;
    $scope.CJdesc = Cjob.desc;

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };

}