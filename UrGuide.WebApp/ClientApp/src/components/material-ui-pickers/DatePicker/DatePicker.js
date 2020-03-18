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
var DatePickerRoot_1 = __importDefault(require("./DatePickerRoot"));
var useUtils_1 = require("../_shared/hooks/useUtils");
var date_utils_1 = require("../_helpers/date-utils");
var Wrapper_1 = require("../wrappers/Wrapper");
var usePickerState_1 = require("../_shared/hooks/usePickerState");
var prop_types_1 = require("../constants/prop-types");
var PureDateInput_1 = require("../_shared/PureDateInput");
var text_field_helper_1 = require("../_helpers/text-field-helper");
exports.DatePicker = function (props) {
    var allowKeyboardControl = props.allowKeyboardControl, animateYearScrolling = props.animateYearScrolling, autoOk = props.autoOk, disableFuture = props.disableFuture, disablePast = props.disablePast, format = props.format, forwardedRef = props.forwardedRef, initialFocusedDate = props.initialFocusedDate, invalidDateMessage = props.invalidDateMessage, labelFunc = props.labelFunc, leftArrowIcon = props.leftArrowIcon, leftArrowButtonProps = props.leftArrowButtonProps, maxDate = props.maxDate, maxDateMessage = props.maxDateMessage, minDate = props.minDate, minDateMessage = props.minDateMessage, onAccept = props.onAccept, onChange = props.onChange, onMonthChange = props.onMonthChange, onYearChange = props.onYearChange, onOpen = props.onOpen, onClose = props.onClose, openTo = props.openTo, renderDay = props.renderDay, rightArrowIcon = props.rightArrowIcon, rightArrowButtonProps = props.rightArrowButtonProps, shouldDisableDate = props.shouldDisableDate, value = props.value, variant = props.variant, onlyCalendar = props.onlyCalendar, views = props.views, other = __rest(props, ["allowKeyboardControl", "animateYearScrolling", "autoOk", "disableFuture", "disablePast", "format", "forwardedRef", "initialFocusedDate", "invalidDateMessage", "labelFunc", "leftArrowIcon", "leftArrowButtonProps", "maxDate", "maxDateMessage", "minDate", "minDateMessage", "onAccept", "onChange", "onMonthChange", "onYearChange", "onOpen", "onClose", "openTo", "renderDay", "rightArrowIcon", "rightArrowButtonProps", "shouldDisableDate", "value", "variant", "onlyCalendar", "views"]);
    var utils = useUtils_1.useUtils();
    var _a = usePickerState_1.usePickerState(props, {
        getDefaultFormat: function () { return date_utils_1.getFormatByViews(views, utils); },
        getValidationError: function () { return text_field_helper_1.getError(value, utils, props); },
    }), pickerProps = _a.pickerProps, inputProps = _a.inputProps, wrapperProps = _a.wrapperProps;
    return (React.createElement(Wrapper_1.Wrapper, __assign({ variant: variant, InputComponent: PureDateInput_1.PureDateInput, DateInputProps: inputProps }, wrapperProps, other),
        React.createElement(DatePickerRoot_1.default, __assign({}, pickerProps, { onlyCalendar: onlyCalendar, allowKeyboardControl: allowKeyboardControl, animateYearScrolling: animateYearScrolling, disableFuture: disableFuture, disablePast: disablePast, leftArrowIcon: leftArrowIcon, leftArrowButtonProps: leftArrowButtonProps, maxDate: maxDate, minDate: minDate, renderDay: renderDay, rightArrowIcon: rightArrowIcon, rightArrowButtonProps: rightArrowButtonProps, shouldDisableDate: shouldDisableDate, views: views, openTo: openTo }))));
};
exports.DatePicker.defaultProps = prop_types_1.datePickerDefaultProps;
exports.default = React.forwardRef(function (props, ref) { return (React.createElement(exports.DatePicker, __assign({}, props, { forwardedRef: ref }))); });
