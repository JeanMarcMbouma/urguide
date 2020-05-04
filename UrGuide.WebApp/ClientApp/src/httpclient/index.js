"use strict";
var __assign = (this && this.__assign) || function () {
    __assign = Object.assign || function(t) {
        for (var s, i = 1, n = arguments.length; i < n; i++) {
            s = arguments[i];
            for (var p in s) if (Object.prototype.hasOwnProperty.call(s, p))
                t[p] = s[p];
        }
        return t;
    };
    return __assign.apply(this, arguments);
};
Object.defineProperty(exports, "__esModule", { value: true });
var api_1 = require("../api");
var originalFetch = fetch;
var Http = /** @class */ (function () {
    function Http(user) {
        this.user = user;
    }
    Http.prototype.fetch = function (url, init) {
        if (this.user && this.user.access_token) {
            var headers = init.headers;
            var options = __assign(__assign({}, init), { headers: __assign(__assign({}, headers), { Authorization: this.user.token_type + " " + this.user.access_token }) });
            return originalFetch(url, options);
        }
        return originalFetch(url, init);
    };
    return Http;
}());
var HttpClientFactory = /** @class */ (function () {
    function HttpClientFactory() {
    }
    HttpClientFactory.getPostClient = function (user) {
        return new api_1.PostsClient("", new Http(user));
    };
    HttpClientFactory.getCatalogClient = function (user) {
        return new api_1.CatalogsClient("", new Http(user));
    };
    HttpClientFactory.getAccountClient = function (user) {
        return new api_1.AccountClient("", new Http(user));
    };
    HttpClientFactory.getClient = function () {
        return new api_1.Client("", new Http());
    };
    return HttpClientFactory;
}());
exports.HttpClientFactory = HttpClientFactory;
//# sourceMappingURL=index.js.map