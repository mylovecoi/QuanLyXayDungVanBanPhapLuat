/******/ (() => { // webpackBootstrap
var __webpack_exports__ = {};
/*!************************************************************!*\
  !*** ../demo1/src/js/pages/crud/forms/widgets/autosize.js ***!
  \************************************************************/
// Class definition

var KTAutosize = function () {

    // Private functions
    var demos = function () {
        // basic demo
        var demo1 = $('#kt_autosize_1');
        var demo2 = $('#kt_autosize_2');
        var demo3 = $('.autosize');

        autosize(demo1);

        autosize(demo2);
        autosize(demo3);
        autosize.update(demo2);
    }

    return {
        // public functions
        init: function() {
            demos();
        }
    };
}();

jQuery(document).ready(function() {
    KTAutosize.init();
});

/******/ })()
;
//# sourceMappingURL=autosize.js.map