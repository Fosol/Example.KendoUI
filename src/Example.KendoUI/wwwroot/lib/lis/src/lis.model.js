lis.model = function (obj) {
    obj.valueOf = function (path) {
        if (typeof (path) !== "string") return;
        var parts = path.split(".");
        return valueOf(this, path);
    };

    function valueOf(obj, parts) {
        if (parts.length === 0) return;
        else if (parts.length === 1) return obj[parts[0]];
        else return valueOf(obj[parts[0]], parts.slice(1));
    }

    return obj;
};