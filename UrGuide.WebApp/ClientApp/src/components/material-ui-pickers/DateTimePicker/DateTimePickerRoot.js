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
var PropTypes = __importStar(require("prop-types"));
var DateTimePickerView_1 = __importDefault(require("./components/DateTimePickerView"));
var Calendar_1 = __importDefault(require("../DatePicker/components/Calendar"));
var DateTimePickerTabs_1 = __importDefault(require("./components/DateTimePickerTabs"));
var YearSelection_1 = __importDefault(require("../DatePicker/components/YearSelection"));
var TimePickerView_1 = __importDefault(require("../TimePicker/components/TimePickerView"));
var DateTimePickerHeader_1 = __importDefault(require("./components/DateTimePickerHeader"));
var useUtils_1 = require("../_shared/hooks/useUtils");
var time_utils_1 = require("../_helpers/time-utils");
var TimePickerRoot_1 = require("../TimePicker/TimePickerRoot");
var DateTimePickerRoot = function (_a) {
    var date = _a.date, minDate = _a.minDate, maxDate = _a.maxDate, showTabs = _a.showTabs, disablePast = _a.disablePast, disableFuture = _a.disableFuture, leftArrowIcon = _a.leftArrowIcon, leftArrowButtonProps = _a.leftArrowButtonProps, rightArrowIcon = _a.rightArrowIcon, rightArrowButtonProps = _a.rightArrowButtonProps, dateRangeIcon = _a.dateRangeIcon, timeIcon = _a.timeIcon, renderDay = _a.renderDay, ampm = _a.ampm, minutesStep = _a.minutesStep, shouldDisableDate = _a.shouldDisableDate, animateYearScrolling = _a.animateYearScrolling, allowKeyboardControl = _a.allowKeyboardControl, ViewContainerComponent = _a.ViewContainerComponent, onChange = _a.onChange, onMonthChange = _a.onMonthChange, onYearChange = _a.onYearChange;
    var utils = useUtils_1.useUtils();
    var _b = React.useState('date'), openView = _b[0], setOpenView = _b[1];
    var _c = TimePickerRoot_1.useMeridiemMode(date, ampm, onChange), meridiemMode = _c.meridiemMode, handleMeridiemChange = _c.handleMeridiemChange;
    var handleChangeAndOpenNext = React.useCallback(function (nextView) {
        return function (time, isFinish) {
            var timeWithMeridiem = time_utils_1.convertToMeridiem(time, meridiemMode, Boolean(ampm), utils);
            if (isFinish && nextView) {
                // do not close picker if needs to show next view
                onChange(timeWithMeridiem, false);
                setOpenView(nextView);
                return;
            }
            onChange(timeWithMeridiem, Boolean(isFinish));
        };
    }, [ampm, meridiemMode, onChange, utils]);
    var Container = ViewContainerComponent;
    var ViewContainerComponentProps = typeof ViewContainerComponent === 'string' ? {} : { openView: openView, onChange: onChange };
    return (React.createElement(React.Fragment, null,
        React.createElement(DateTimePickerHeader_1.default, { date: date, openView: openView, meridiemMode: meridiemMode, setMeridiemMode: handleMeridiemChange, onOpenViewChange: setOpenView, ampm: ampm }),
        showTabs && (React.createElement(DateTimePickerTabs_1.default, { view: openView, onChange: setOpenView, dateRangeIcon: dateRangeIcon, timeIcon: timeIcon })),
        React.createElement(Container, __assign({}, ViewContainerComponentProps),
            React.createElement(DateTimePickerView_1.default, { selected: openView === 'year' },
                React.createElement(YearSelection_1.default, { date: date, minDate: utils.date(minDate), maxDate: utils.date(maxDate), onYearChange: onYearChange, onChange: handleChangeAndOpenNext('date'), disablePast: disablePast, disableFuture: disableFuture, animateYearScrolling: animateYearScrolling })),
            React.createElement(DateTimePickerView_1.default, { selected: openView === 'date' },
                React.createElement(Calendar_1.default, { allowKeyboardControl: allowKeyboardControl, date: date, minDate: utils.date(minDate), maxDate: utils.date(maxDate), onChange: handleChangeAndOpenNext('hours'), disablePast: disablePast, disableFuture: disableFuture, leftArrowIcon: leftArrowIcon, leftArrowButtonProps: leftArrowButtonProps, rightArrowIcon: rightArrowIcon, rightArrowButtonProps: rightArrowButtonProps, renderDay: renderDay, shouldDisableDate: shouldDisableDate, onMonthChange: onMonthChange })),
            React.createElement(DateTimePickerView_1.default, { selected: openView === 'hours' || openView === 'minutes' },
                React.createElement(TimePickerView_1.default, { date: date, ampm: ampm, type: openView, onHourChange: handleChangeAndOpenNext('minutes'), onMinutesChange: handleChangeAndOpenNext(null), onSecondsChange: function () { }, minutesStep: minutesStep })))));
};
DateTimePickerRoot.propTypes = {
    autoSubmit: PropTypes.bool,
    openTo: PropTypes.oneOf(['year', 'date', 'hours', 'minutes']),
    showTabs: PropTypes.bool,
    ViewContainerComponent: PropTypes.oneOfType([PropTypes.string, PropTypes.func, PropTypes.object]),
    minutesStep: PropTypes.number,
};
DateTimePickerRoot.defaultProps = {
    autoSubmit: true,
    showTabs: true,
    ampm: true,
    minutesStep: 1,
    minDate: new Date('1900-01-01'),
    maxDate: new Date('2100-01-01'),
    openTo: 'date',
    ViewContainerComponent: 'div',
};
exports.default = DateTimePickerRoot;
