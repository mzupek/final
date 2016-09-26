var searchApp = angular.module('searchApp', [])
searchApp.filter('highlight', function ($sce, $localStorage, $sessionStorage, $window) {
    return function (str, termsToHighlight, termsToNot, Score ) {
        
        console.log(termsToNot.length);
        console.log(termsToHighlight.length);
        console.log(Score);
        //console.log(str);
        
        var str = str.replace(/<[\/]{0,1}(i)[^><]*>/ig, "");

        //console.log("clean:" +str);

        angular.forEach(termsToNot, function (item, index) {
            var regexp;
            //console.log(item);
            

            regexp = new RegExp("(\\b" + item + "\\b)", 'ig');

            var final =str.match(regexp);
            
            if (final != null) {
                //console.log("match: " + item);
                termsToHighlight.push(item);
                var i = termsToNot.indexOf(item);
                termsToNot.splice(i, 1);
                var total = termsToNot.length + termsToHighlight.length;
                Score = Math.ceil(termsToHighlight.length / total * 100);
                console.log("score:" + Score);
                
            }

        });

        angular.forEach(termsToHighlight, function (item, index) {
            var regexp;
            //console.log(item);
            regexp = new RegExp("(\\b" + item + "\\b)", 'ig');

            var final = str.match(regexp);

            if (final === null) {
                //console.log("match: " + item);
                termsToNot.push(item);
                var i = termsToHighlight.indexOf(item);
                termsToHighlight.splice(i, 1);
                var total = termsToNot.length + termsToHighlight.length;
                Score = Math.ceil(termsToHighlight.length / total * 100);
                //console.log("score:" + Score);
               
            }

        });

        termsToHighlight.sort(function (a, b) {
            return b.length - a.length;
        });

        

        if (termsToHighlight.length === 0)
        {
            $localStorage.resumeText = str;
            return $sce.trustAsHtml(str);
        }
        else {

            
            $localStorage.resumeText = str;
            var _savedSelection = $window.rangy.saveSelection();
            $window.rangy.restoreSelection(_savedSelection);
            var regex = new RegExp('(' + termsToHighlight.join('|') + ')', 'ig');
            return $sce.trustAsHtml(str.replace(regex, '<span class="match" contenteditable="false">$&</span>'));
        }
        
        
        
    };
});