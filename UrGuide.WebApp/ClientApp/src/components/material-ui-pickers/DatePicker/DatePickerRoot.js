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
var clsx_1 = __importDefault(require("clsx"));
var ToolbarButton_1 = __importDefault(require("../_shared/ToolbarButton"));
var PickerToolbar_1 = __importDefault(require("../_shared/PickerToolbar"));
var YearSelection_1 = __importDefault(require("./components/YearSelection"));
var MonthSelection_1 = __importDefault(require("./components/MonthSelection"));
var Calendar_1 = __importDefault(require("./components/Calendar"));
var useUtils_1 = require("../_shared/hooks/useUtils");
var styles_1 = require("@material-ui/core/styles");
var date_utils_1 = require("../_helpers/date-utils");
var prop_types_1 = require("../constants/prop-types");
exports.useStyles = styles_1.makeStyles({
    toolbarCenter: {
        flexDirection: 'row',
        alignItems: 'center',
    },
}, { name: 'MuiPickersDatePickerRoot' });
exports.DatePickerRoot = function (_a) {
    var _b;
    var date = _a.date, _c = _a.views, views = _c === void 0 ? ['year', 'day'] : _c, disablePast = _a.disablePast, disableFuture = _a.disableFuture, onChange = _a.onChange, openTo = _a.openTo, unparsedMinDate = _a.minDate, unparsedMaxDate = _a.maxDate, animateYearScrolling = _a.animateYearScrolling, leftArrowIcon = _a.leftArrowIcon, rightArrowIcon = _a.rightArrowIcon, renderDay = _a.renderDay, shouldDisableDate = _a.shouldDisableDate, allowKeyboardControl = _a.allowKeyboardControl, onMonthChange = _a.onMonthChange, onYearChange = _a.onYearChange, onlyCalendar = _a.onlyCalendar, leftArrowButtonProps = _a.leftArrowButtonProps, rightArrowButtonProps = _a.rightArrowButtonProps;
    var utils = useUtils_1.useUtils();
    var classes = exports.useStyles();
    var _d = React.useState(openTo && views.includes(openTo) ? openTo : views[0]), openView = _d[0], setOpenView = _d[1];
    var isYearOnly = React.useMemo(function () { return date_utils_1.isYearOnlyView(views); }, [views]);
    var isYearAndMonth = React.useMemo(function () { return date_utils_1.isYearAndMonthViews(views); }, [views]);
    var minDate = React.useMemo(function () { return utils.date(unparsedMinDate); }, [unparsedMinDate, utils]);
    var maxDate = React.useMemo(function () { return utils.date(unparsedMaxDate); }, [unparsedMaxDate, utils]);
    var getNextAvailableView = React.useCallback(function (nextView) {
        if (views.includes(nextView)) {
            return nextView;
        }
        return views[views.indexOf(openView) + 1];
    }, [openView, views]);
    var handleChangeAndOpenNext = React.useCallback(function (nextView) {
        return function (date, isFinish) {
            var nextViewToOpen = getNextAvailableView(nextView);
            if (isFinish && nextViewToOpen) {
                // do not close picker if needs to show next view
                onChange(date, false);
                setOpenView(nextViewToOpen);
                return;
            }
            onChange(date, isFinish);
        };
    }, [getNextAvailableView, onChange]);
    return (React.createElement(React.Fragment, null,
        !onlyCalendar && (React.createElement(PickerToolbar_1.default, { className: clsx_1.default((_b = {}, _b[classes.toolbarCenter] = isYearOnly, _b)) },
            React.createElement(ToolbarButton_1.default, { variant: isYearOnly ? 'h3' : 'subtitle1', onClick: function () { return setOpenView('year'); }, selected: openView === 'year', label: utils.getYearText(date) }),
            !isYearOnly && !isYearAndMonth && (React.createElement(ToolbarButton_1.default, { variant: "h4", onClick: function () { return setOpenView('day'); }, selected: openView === 'day', label: utils.getDatePickerHeaderText(date) })),
            isYearAndMonth && (React.createElement(ToolbarButton_1.default, { variant: "h4", onClick: function () { return setOpenView('month'); }, selected: openView === 'month', label: utils.getMonthText(date) })))),
        openView === 'year' && (React.createElement(YearSelection_1.default, { date: date, onChange: handleChangeAndOpenNext('month'), minDate: minDate, maxDate: maxDate, disablePast: disablePast, disableFuture: disableFuture, onYearChange: onYearChange, animateYearScrolling: animateYearScrolling })),
        openView === 'month' && (React.createElement(MonthSelection_1.default, { date: date, onChange: handleChangeAndOpenNext('day'), minDate: minDate, maxDate: maxDate, disablePast: disablePast, disableFuture: disableFuture, onMonthChange: onMonthChange })),
        openView === 'day' && (React.createElement(Calendar_1.default, { date: date, onChange: onChange, onMonthChange: onMonthChange, disablePast: disablePast, disableFuture: disableFuture, minDate: minDate, maxDate: maxDate, leftArrowIcon: leftArrowIcon, leftArrowButtonProps: leftArrowButtonProps, rightArrowIcon: rightArrowIcon, rightArrowButtonProps: rightArrowButtonProps, renderDay: renderDay, shouldDisableDate: shouldDisableDate, allowKeyboardControl: allowKeyboardControl }))));
};
exports.DatePickerRoot.propTypes = {
    onlyCalendar: PropTypes.bool,
    views: PropTypes.arrayOf(prop_types_1.DomainPropTypes.datePickerView),
    openTo: prop_types_1.DomainPropTypes.datePickerView,
};
exports.DatePickerRoot.defaultProps = __assign({ onlyCalendar: false }, prop_types_1.datePickerDefaultProps);
exports.default = exports.DatePickerRoot;
