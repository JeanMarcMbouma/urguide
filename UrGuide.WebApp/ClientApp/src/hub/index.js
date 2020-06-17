"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var signalr_1 = require("@microsoft/signalr");
var SignalRClient = /** @class */ (function () {
    function SignalRClient() {
    }
    SignalRClient.get = function (callback, user) {
        var connection = new signalr_1.HubConnectionBuilder().withUrl("/notify", {
            accessTokenFactory: function () { return user.access_token; }
        }).build();
        connection.on("notify", function (message, userInfo) {
            callback(userInfo, message);
        });
        return connection.start();
    };
    return SignalRClient;
}());
exports.SignalRClient = SignalRClient;
//# sourceMappingURL=index.js.map