
/**
 * Provides common shared functions and objects.
 * @constructor
 */
var lis = (function() {
    var _this = {};
    /**
     * Makes an AJAX request as specified by the options.
     * Wraps the jQuery.ajax function.
     * By default logs all errors.
     * @function ajax
     * @param {Object} options - Configuration options.
     * @returns {Promise} A Promise.
     */
    _this.ajax = function (options) {
        options = $.extend({
            type: "GET",
            dataType: "json"
        }, options);
        if (options.contentType === "application/json" && options.data && typeof (options.data) === "object") options.data = JSON.stringify(options.data);
        return $.ajax(options).fail(function (xhr, status, error) {
            console.log(error);
        });
    };

    /**
     * Default configuration for HTTP GET.
     * @function httpGet
     * @param {Object} options - Configuration options.
     * @returns {Promise} A Promise.
     */
    _this.httpGet = function (options) {
        options = $.extend({
            type: "GET",
        }, options);
        return lis.ajax(options);
    };


    /**
     * Default configuration for HTTP POST.
     * @function httpPost
     * @param {Object} options - Configuration options.
     * @returns {Promise} A Promise.
     */
    _this.httpPost = function (options) {
        options = $.extend({
            type: "POST",
            contentType: "application/json"
        }, options);
        return lis.ajax(options);
    };


    /**
     * Default configuration for HTTP PUT.
     * @function httpPut
     * @param {Object} options - Configuration options.
     * @returns {Promise} A Promise.
     */
    _this.httpPut = function (options) {
        options = $.extend({
            type: "PUT",
            contentType: "application/json"
        }, options);
        return lis.ajax(options);
    };


    /**
     * Default configuration for HTTP DELETE.
     * @function httpDelete
     * @param {Object} options - Configuration options.
     * @returns {Promise} A Promise.
     */
    _this.httpDelete = function (options) {
        options = $.extend({
            type: "DELETE",
            contentType: "application/json"
        }, options);
        return lis.ajax(options);
    };

    return _this;
})();