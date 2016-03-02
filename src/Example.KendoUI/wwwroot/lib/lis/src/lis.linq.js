lis.Linq = function (obj) {
    if (typeof (obj) !== "Array" && typeof (obj) !== "object" ) obj = [];

    /**
     * Filter the array with the specified expression.
     * @function where
     * @param {function} exp - A function that receives the item and returns true if it matches the expression.
     * @returns {lis.Linq} A new instance of a lis.Linq object.
     */
    obj.where = function (exp) {
        var results = [];
        if (typeof (exp) === "function") {
            $.each(this, function (i, item) {
                if (exp(item)) results.push(item);
            });
        } else { console.log("lis.Linq.where(exp) is unable to parse your expression \"" + exp + "\"."); }
        return new lis.Linq(result);
    };

    obj.any = function (exp) {
        return this.firstOrDefault(exp) !== null;
    }

    obj.single = function (exp) {
        var result = this.where(exp);
        if (result.length !== 1) throw new Error("lis.Linq.single(exp) must return only one result.");
        return result[0];
    };

    obj.singleOrDefault = function (exp) {
        var result = this.where(exp);
        if (result.length > 1) throw new Error("lis.Linq.single(exp) must return only one result.");
        else if (result.length === 0) return null;
        return result[0];
    };

    obj.first = function (exp) {
        var result = null;
        if (typeof (exp) === "function") {
            $.each(this, function (i, item) {
                if (exp(item)) {
                    result = item;
                    return false;
                }
            });
        } else if (this.length > 0) result = this[0];
        if (result === null) throw new Error("lis.Linq.first(exp) returned no results.");
        return result;
    };

    obj.firstOrDefault = function (exp) {
        var result = null;
        if (typeof (exp) === "function") {
            $.each(this, function (i, item) {
                if (exp(item)) {
                    result = item;
                    return false;
                }
            });
        } else if (this.length > 0) result = this[0];
        return result;
    };

    obj.last = function (exp) {
        var result = null;
        if (typeof (exp) === "function") {
            $.each(this.reverse(), function (i, item) {
                if (exp(item)) {
                    result = item;
                    return false;
                }
            });
        } else if (this.length > 0) result = this[this.length - 1];
        if (result === null) throw new Error("lis.Linq.first(exp) returned no results.");
        return result;
    };

    obj.lastOrDefault = function (exp) {
        var result = null;
        if (typeof (exp) === "function") {
            $.each(this.reverse(), function (i, item) {
                if (exp(item)) {
                    result = item;
                    return false;
                }
            });
        } else if (this.length > 0) result = this[this.length - 1];
        return result;
    };

    obj.skip = function (qty) {
        if (!qty) return this;
        if (qty >= this.length) return [];
        return new lis.Linq(this.slice(qty));
    }

    /**
     * Only take the specified quantity.
     * @function take
     * @param {number} qty - Quantity to return.
     * @returns {Array} An array wrapped with lis.Linq methods.
     */
    obj.take = function (qty) {
        if (!qty) return [];
        if (qty > this.length) return [];
        return new lis.Linq(this.slice(0, qty));
    }

    obj.orderBy = function (exp) {
        var result = [];
        if (typeof (exp) === "function") {
            this.sort(exp);
        } else if (typeof (exp) === "string") {
            result = this.sort(function (a, b) {
                return lis.model(a).valueOf(exp) > lis.model(b).valueOf(exp);
            });
        }
        return new lis.Linq(result);
    }

    obj.orderByDescending = function (exp) {
        var result = [];
        if (typeof (exp) === "function") {
            this.sort(exp);
        } else if (typeof (exp) === "string") {
            result = this.sort(function (a, b) {
                return lis.model(a).valueOf(exp) < lis.model(b).valueOf(exp);
            });
        }
        return new lis.Linq(result);
    }

    return obj;
};