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
var __rest = (this && this.__rest) || function (s, e) {
    var t = {};
    for (var p in s) if (Object.prototype.hasOwnProperty.call(s, p) && e.indexOf(p) < 0)
        t[p] = s[p];
    if (s != null && typeof Object.getOwnPropertySymbols === "function")
        for (var i = 0, p = Object.getOwnPropertySymbols(s); i < p.length; i++) if (e.indexOf(p[i]) < 0)
            t[p[i]] = s[p[i]];
    return t;
};
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", { value: true });
var react_1 = __importDefault(require("react"));
var ModalWrapper_1 = __importDefault(require("./ModalWrapper"));
var InlineWrapper_1 = __importDefault(require("./InlineWrapper"));
function getWrapperFromVariant(variant) {
    switch (variant) {
        case 'inline':
            return InlineWrapper_1.default;
        default:
            return ModalWrapper_1.default;
    }
}
exports.getWrapperFromVariant = getWrapperFromVariant;
exports.Wrapper = function (_a) {
    var variant = _a.variant, props = __rest(_a, ["variant"]);
    var Component = getWrapperFromVariant(variant);
    return react_1.default.createElement(Component, __assign({}, props));
};
