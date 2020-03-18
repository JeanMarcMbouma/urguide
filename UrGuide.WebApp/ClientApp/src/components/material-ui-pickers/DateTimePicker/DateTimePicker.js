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
var DateTimePickerRoot_1 = __importDefault(require("./DateTimePickerRoot"));
var useUtils_1 = require("../_shared/hooks/useUtils");
var utils_1 = require("../_helpers/utils");
var Wrapper_1 = require("../wrappers/Wrapper");
var usePickerState_1 = require("../_shared/hooks/usePickerState");
var prop_types_1 = require("../constants/prop-types");
var PureDateInput_1 = require("../_shared/PureDateInput");
var text_field_helper_1 = require("../_helpers/text-field-helper");
exports.DateTimePicker = function (props) {
    var allowKeyboardControl = props.allowKeyboardControl, ampm = props.ampm, animateYearScrolling = props.animateYearScrolling, autoOk = props.autoOk, autoSubmit = props.autoSubmit, dateRangeIcon = props.dateRangeIcon, disableFuture = props.disableFuture, disablePast = props.disablePast, format = props.format, forwardedRef = props.forwardedRef, initialFocusedDate = props.initialFocusedDate, invalidDateMessage = props.invalidDateMessage, labelFunc = props.labelFunc, leftArrowIcon = props.leftArrowIcon, leftArrowButtonProps = props.leftArrowButtonProps, maxDate = props.maxDate, maxDateMessage = props.maxDateMessage, minDate = props.minDate, minDateMessage = props.minDateMessage, minutesStep = props.minutesStep, onAccept = props.onAccept, onChange = props.onChange, onOpen = props.onOpen, onClose = props.onClose, onMonthChange = props.onMonthChange, onYearChange = props.onYearChange, openTo = props.openTo, renderDay = props.renderDay, rightArrowIcon = props.rightArrowIcon, rightArrowButtonProps = props.rightArrowButtonProps, shouldDisableDate = props.shouldDisableDate, showTabs = props.showTabs, timeIcon = props.timeIcon, value = props.value, variant = props.variant, other = __rest(props, ["allowKeyboardControl", "ampm", "animateYearScrolling", "autoOk", "autoSubmit", "dateRangeIcon", "disableFuture", "disablePast", "format", "forwardedRef", "initialFocusedDate", "invalidDateMessage", "labelFunc", "leftArrowIcon", "leftArrowButtonProps", "maxDate", "maxDateMessage", "minDate", "minDateMessage", "minutesStep", "onAccept", "onChange", "onOpen", "onClose", "onMonthChange", "onYearChange", "openTo", "renderDay", "rightArrowIcon", "rightArrowButtonProps", "shouldDisableDate", "showTabs", "timeIcon", "value", "variant"]);
    var utils = useUtils_1.useUtils();
    var toShowTabs = utils_1.toShowDateTimePickerTabs(showTabs);
    var _a = usePickerState_1.usePickerState(props, {
        getValidationError: function () { return text_field_helper_1.getError(value, utils, props); },
        getDefaultFormat: function () {
            return text_field_helper_1.pick12hOr24hFormat(format, ampm, {
                '12h': utils.dateTime12hFormat,
                '24h': utils.dateTime24hFormat,
            });
        },
    }), pickerProps = _a.pickerProps, inputProps = _a.inputProps, wrapperProps = _a.wrapperProps;
    return (React.createElement(Wrapper_1.Wrapper, __assign({ variant: variant, showTabs: toShowTabs, InputComponent: PureDateInput_1.PureDateInput, DateInputProps: inputProps }, wrapperProps, other),
        React.createElement(DateTimePickerRoot_1.default, __assign({}, pickerProps, { ampm: ampm, allowKeyboardControl: allowKeyboardControl, minutesStep: minutesStep, animateYearScrolling: animateYearScrolling, autoSubmit: autoSubmit, dateRangeIcon: dateRangeIcon, disableFuture: disableFuture, disablePast: disablePast, leftArrowIcon: leftArrowIcon, leftArrowButtonProps: leftArrowButtonProps, maxDate: maxDate, minDate: minDate, onMonthChange: onMonthChange, onYearChange: onYearChange, openTo: openTo, renderDay: renderDay, rightArrowIcon: rightArrowIcon, rightArrowButtonProps: rightArrowButtonProps, shouldDisableDate: shouldDisableDate, showTabs: toShowTabs, timeIcon: timeIcon }))));
};
exports.DateTimePicker.defaultProps = prop_types_1.dateTimePickerDefaultProps;
exports.default = React.forwardRef(function (props, ref) { return (React.createElement(exports.DateTimePicker, __assign({}, props, { forwardedRef: ref }))); });
