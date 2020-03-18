"use strict";
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
var ClockType_1 = __importDefault(require("../constants/ClockType"));
var ToolbarText_1 = __importDefault(require("../_shared/ToolbarText"));
var ToolbarButton_1 = __importDefault(require("../_shared/ToolbarButton"));
var PickerToolbar_1 = __importDefault(require("../_shared/PickerToolbar"));
var TimePickerView_1 = __importDefault(require("./components/TimePickerView"));
var useUtils_1 = require("../_shared/hooks/useUtils");
var time_utils_1 = require("../_helpers/time-utils");
var styles_1 = require("@material-ui/core/styles");
exports.useStyles = styles_1.makeStyles({
    toolbar: {
        flexDirection: 'row',
        alignItems: 'center',
    },
    toolbarLeftPadding: {
        paddingLeft: 50,
    },
    separator: {
        margin: '0 4px 0 2px',
        cursor: 'default',
    },
    ampmSelection: {
        marginLeft: 20,
        marginRight: -20,
        display: 'flex',
        flexDirection: 'column',
    },
    ampmSelectionWithSeconds: {
        marginLeft: 15,
        marginRight: 10,
    },
    ampmLabel: {
        fontSize: 18,
    },
    hourMinuteLabel: {
        display: 'flex',
        justifyContent: 'flex-end',
        alignItems: 'flex-end',
    },
    hourMinuteLabelReverse: {
        display: 'flex',
        justifyContent: 'flex-end',
        alignItems: 'flex-end',
        flexDirection: 'row-reverse',
    },
}, { name: 'MuiPickersTimePicker' });
function useMeridiemMode(date, ampm, onChange) {
    var utils = useUtils_1.useUtils();
    var meridiemMode = utils.getHours(date) >= 12 ? 'pm' : 'am';
    var handleMeridiemChange = React.useCallback(function (mode) {
        var timeWithMeridiem = time_utils_1.convertToMeridiem(date, mode, Boolean(ampm), utils);
        onChange(timeWithMeridiem, false);
    }, [ampm, date, onChange, utils]);
    return { meridiemMode: meridiemMode, handleMeridiemChange: handleMeridiemChange };
}
exports.useMeridiemMode = useMeridiemMode;
var TimePickerRoot = function (_a) {
    var _b, _c;
    var date = _a.date, ampm = _a.ampm, onChange = _a.onChange, seconds = _a.seconds, minutesStep = _a.minutesStep;
    var utils = useUtils_1.useUtils();
    var classes = exports.useStyles();
    var theme = styles_1.useTheme();
    var _d = React.useState(ClockType_1.default.HOURS), openView = _d[0], setOpenView = _d[1];
    var _e = useMeridiemMode(date, ampm, onChange), meridiemMode = _e.meridiemMode, handleMeridiemChange = _e.handleMeridiemChange;
    var hourMinuteClassName = theme.direction === 'rtl' ? classes.hourMinuteLabelReverse : classes.hourMinuteLabel;
    var handleChangeAndOpenNext = React.useCallback(function (nextView) {
        return function (time, isFinish) {
            var timeWithMeridiem = time_utils_1.convertToMeridiem(time, meridiemMode, Boolean(ampm), utils);
            if (isFinish && nextView) {
                // do not close picker if needs to show next view
                onChange(timeWithMeridiem, false);
                setOpenView(nextView);
                return;
            }
            onChange(timeWithMeridiem, isFinish);
        };
    }, [ampm, meridiemMode, onChange, utils]);
    return (React.createElement(React.Fragment, null,
        React.createElement(PickerToolbar_1.default, { className: clsx_1.default(classes.toolbar, (_b = {},
                _b[classes.toolbarLeftPadding] = ampm,
                _b)) },
            React.createElement("div", { className: hourMinuteClassName },
                React.createElement(ToolbarButton_1.default, { variant: "h2", onClick: function () { return setOpenView(ClockType_1.default.HOURS); }, selected: openView === ClockType_1.default.HOURS, label: utils.getHourText(date, Boolean(ampm)) }),
                React.createElement(ToolbarText_1.default, { variant: "h2", label: ":", selected: false, className: classes.separator }),
                React.createElement(ToolbarButton_1.default, { variant: "h2", onClick: function () { return setOpenView(ClockType_1.default.MINUTES); }, selected: openView === ClockType_1.default.MINUTES, label: utils.getMinuteText(date) }),
                seconds && (React.createElement(React.Fragment, null,
                    React.createElement(ToolbarText_1.default, { variant: "h2", label: ":", selected: false, className: classes.separator }),
                    React.createElement(ToolbarButton_1.default, { variant: "h2", onClick: function () { return setOpenView(ClockType_1.default.SECONDS); }, selected: openView === ClockType_1.default.SECONDS, label: utils.getSecondText(date) })))),
            ampm && (React.createElement("div", { className: clsx_1.default(classes.ampmSelection, (_c = {},
                    _c[classes.ampmSelectionWithSeconds] = seconds,
                    _c)) },
                React.createElement(ToolbarButton_1.default, { disableRipple: true, variant: "subtitle1", selected: meridiemMode === 'am', typographyClassName: classes.ampmLabel, label: utils.getMeridiemText('am'), onClick: function () { return handleMeridiemChange('am'); } }),
                React.createElement(ToolbarButton_1.default, { disableRipple: true, variant: "subtitle1", selected: meridiemMode === 'pm', typographyClassName: classes.ampmLabel, label: utils.getMeridiemText('pm'), onClick: function () { return handleMeridiemChange('pm'); } })))),
        React.createElement(TimePickerView_1.default, { ampm: ampm, date: date, type: openView, minutesStep: minutesStep, onHourChange: handleChangeAndOpenNext(ClockType_1.default.MINUTES), onMinutesChange: handleChangeAndOpenNext(seconds ? ClockType_1.default.SECONDS : null), onSecondsChange: handleChangeAndOpenNext(null) })));
};
TimePickerRoot.propTypes = {
    ampm: PropTypes.bool,
    seconds: PropTypes.bool,
    minutesStep: PropTypes.number,
};
TimePickerRoot.defaultProps = {
    ampm: true,
    seconds: false,
    minutesStep: 1,
};
exports.default = TimePickerRoot;
