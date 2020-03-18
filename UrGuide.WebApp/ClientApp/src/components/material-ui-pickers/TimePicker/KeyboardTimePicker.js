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
var __importStar = (this && this.__importStar) || function (mod) {
    if (mod && mod.__esModule) return mod;
    var result = {};
    if (mod != null) for (var k in mod) if (Object.hasOwnProperty.call(mod, k)) result[k] = mod[k];
    result["default"] = mod;
    return result;
};
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", { value: true });
var React = __importStar(require("react"));
var TimePickerRoot_1 = __importDefault(require("./TimePickerRoot"));
var KeyboardDateInput_1 = __importDefault(require("../_shared/KeyboardDateInput"));
var useUtils_1 = require("../_shared/hooks/useUtils");
var Wrapper_1 = require("../wrappers/Wrapper");
var prop_types_1 = require("../constants/prop-types");
var text_field_helper_1 = require("../_helpers/text-field-helper");
var useKeyboardPickerState_1 = require("../_shared/hooks/useKeyboardPickerState");
function KeyboardTimePicker(props) {
    var ampm = props.ampm, seconds = props.seconds, minutesStep = props.minutesStep, variant = props.variant, format = props.format, forwardedRef = props.forwardedRef, initialFocusedDate = props.initialFocusedDate, invalidDateMessage = props.invalidDateMessage, labelFunc = props.labelFunc, onAccept = props.onAccept, onChange = props.onChange, value = props.value, other = __rest(props, ["ampm", "seconds", "minutesStep", "variant", "format", "forwardedRef", "initialFocusedDate", "invalidDateMessage", "labelFunc", "onAccept", "onChange", "value"]);
    var utils = useUtils_1.useUtils();
    var _a = useKeyboardPickerState_1.useKeyboardPickerState(props, {
        getValidationError: function () { return text_field_helper_1.getError(value, utils, props); },
        getDefaultFormat: function () {
            return text_field_helper_1.pick12hOr24hFormat(format, ampm, {
                '12h': utils.time12hFormat,
                '24h': utils.time24hFormat,
            });
        },
    }), pickerProps = _a.pickerProps, inputProps = _a.inputProps, wrapperProps = _a.wrapperProps;
    return (React.createElement(Wrapper_1.Wrapper, __assign({ variant: variant, InputComponent: KeyboardDateInput_1.default, DateInputProps: inputProps }, wrapperProps, other),
        React.createElement(TimePickerRoot_1.default, __assign({}, pickerProps, { ampm: ampm, seconds: seconds, minutesStep: minutesStep }))));
}
exports.KeyboardTimePicker = KeyboardTimePicker;
KeyboardTimePicker.defaultProps = prop_types_1.timePickerDefaultProps;
exports.default = React.forwardRef(function (props, ref) { return (React.createElement(KeyboardTimePicker, __assign({}, props, { forwardedRef: ref }))); });
