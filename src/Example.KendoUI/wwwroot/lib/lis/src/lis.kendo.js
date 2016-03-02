lis.kendo = (function () {
    var _this = { data: {} };
    var handler = (function (ajax) {
        function response(e) { return _this.success(e).error(e); }
        function success(e) { ajax.done(function () { e.success.apply(this, arguments); }); return _this; }
        function error(e) { ajax.fail(function () { e.error.apply(this, arguments); }); return _this; }
        var _this = {
            response: response,
            success: success,
            error: error,
            done: ajax.done,
            fail: ajax.fail,
            always: ajax.always
        };
        return _this;
    });

    _this.data.read = function (options) {
        return handler(lis.httpGet(options));
    };

    _this.data.create = function (options) {
        return handler(lis.httpPost(options));
    };

    _this.data.update = function (options) {
        return handler(lis.httpPut(options));
    };

    _this.data.destroy = function (options) {
        return handler(lis.httpDelete(options));
    };

    return _this;
})();