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
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
var __generator = (this && this.__generator) || function (thisArg, body) {
    var _ = { label: 0, sent: function() { if (t[0] & 1) throw t[1]; return t[1]; }, trys: [], ops: [] }, f, y, t, g;
    return g = { next: verb(0), "throw": verb(1), "return": verb(2) }, typeof Symbol === "function" && (g[Symbol.iterator] = function() { return this; }), g;
    function verb(n) { return function (v) { return step([n, v]); }; }
    function step(op) {
        if (f) throw new TypeError("Generator is already executing.");
        while (_) try {
            if (f = 1, y && (t = op[0] & 2 ? y["return"] : op[0] ? y["throw"] || ((t = y["return"]) && t.call(y), 0) : y.next) && !(t = t.call(y, op[1])).done) return t;
            if (y = 0, t) op = [op[0] & 2, t.value];
            switch (op[0]) {
                case 0: case 1: t = op; break;
                case 4: _.label++; return { value: op[1], done: false };
                case 5: _.label++; y = op[1]; op = [0]; continue;
                case 7: op = _.ops.pop(); _.trys.pop(); continue;
                default:
                    if (!(t = _.trys, t = t.length > 0 && t[t.length - 1]) && (op[0] === 6 || op[0] === 2)) { _ = 0; continue; }
                    if (op[0] === 3 && (!t || (op[1] > t[0] && op[1] < t[3]))) { _.label = op[1]; break; }
                    if (op[0] === 6 && _.label < t[1]) { _.label = t[1]; t = op; break; }
                    if (t && _.label < t[2]) { _.label = t[2]; _.ops.push(op); break; }
                    if (t[2]) _.ops.pop();
                    _.trys.pop(); continue;
            }
            op = body.call(thisArg, _);
        } catch (e) { op = [6, e]; y = 0; } finally { f = t = 0; }
        if (op[0] & 5) throw op[1]; return { value: op[0] ? op[1] : void 0, done: true };
    }
};
Object.defineProperty(exports, "__esModule", { value: true });
var api_1 = require("./../../api");
var httpclient_1 = require("./../../httpclient");
var AuthService_1 = require("../api-authorization/AuthService");
var navigateToReturnUrl = function (returnUrl) {
    window.location.replace(returnUrl);
};
function login(state) {
    return __awaiter(this, void 0, void 0, function () {
        var returnUrl, client, loginModel, e_1;
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0:
                    returnUrl = state.returnUrl;
                    client = httpclient_1.HttpClientFactory.getClient();
                    loginModel = new api_1.LoginModel({
                        userName: state.email,
                        password: state.password,
                        persist: state.isRemembered
                    });
                    _a.label = 1;
                case 1:
                    _a.trys.push([1, 4, , 5]);
                    return [4 /*yield*/, client.login(returnUrl, loginModel)];
                case 2:
                    _a.sent();
                    return [4 /*yield*/, AuthService_1.default.completeSignIn(returnUrl)];
                case 3:
                    _a.sent();
                    return [3 /*break*/, 5];
                case 4:
                    e_1 = _a.sent();
                    state.LoginFailed = e_1.message;
                    return [3 /*break*/, 5];
                case 5: return [2 /*return*/];
            }
        });
    });
}
function LoginReducer(state, action) {
    var context = __assign({}, state);
    switch (action.type) {
        case "validate-login":
            context.email = action.data.email;
            context.password = action.data.password;
            context.isRemembered = action.data.isRemembered;
            context.returnUrl = action.data.returnUrl;
            var regexEmail = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/;
            var passwordRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/;
            var validEmail = regexEmail.test(context.email);
            var validpassword = passwordRegex.test(context.password);
            context.emailError = validEmail ? false : true;
            context.passwordError = validpassword ? false : true;
            context.passwordErrorMessage = context.passwordError ? "your password must contains minimum eight characters, at least one uppercase letter, one lowercase letter, one number and one special character." : '';
            if (validEmail && validpassword) {
                // action.data.callback(context);
                login(context);
            }
            return context;
    }
}
exports.default = LoginReducer;
//# sourceMappingURL=LoginReducer.js.map