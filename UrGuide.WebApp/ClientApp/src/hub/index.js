"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var signalr_1 = require("@microsoft/signalr");
var SignalRClient = /** @class */ (function () {
    function SignalRClient() {
    }
    SignalRClient.get = function (callback) {
        var connection = new signalr_1.HubConnectionBuilder().withUrl("/notify").build();
        connection.on("notify", function (userId, message) {
            callback(userId, message);
            console.log(message);
        });
        return connection.start();
    };
    return SignalRClient;
}());
exports.SignalRClient = SignalRClient;
//# sourceMappingURL=index.js.map