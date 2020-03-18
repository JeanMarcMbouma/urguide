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
var clsx_1 = __importDefault(require("clsx"));
var ToolbarText_1 = __importDefault(require("../../_shared/ToolbarText"));
var PickerToolbar_1 = __importDefault(require("../../_shared/PickerToolbar"));
var ToolbarButton_1 = __importDefault(require("../../_shared/ToolbarButton"));
var styles_1 = require("@material-ui/core/styles");
var useUtils_1 = require("../../_shared/hooks/useUtils");
exports.useStyles = styles_1.makeStyles(function (theme) { return ({
    toolbar: {
        flexDirection: 'row',
        alignItems: 'center',
        paddingLeft: 16,
        paddingRight: 16,
        justifyContent: 'space-around',
    },
    toolBar24h: {
        paddingLeft: 32,
    },
    separator: {
        margin: '0 4px 0 2px',
        cursor: 'default',
    },
    hourMinuteLabel: {
        top: 10,
        position: 'relative',
        display: 'flex',
        justifyContent: 'flex-end',
        alignItems: 'flex-end',
        flexDirection: theme.direction === 'rtl' ? 'row-reverse' : 'row',
    },
    dateHeader: {
        height: 60,
        minWidth: 110,
        marginRight: 4,
        display: 'flex',
        flexDirection: 'column',
        alignItems: 'flex-start',
        justifyContent: 'flex-start',
    },
    timeHeader: {
        height: 65,
        minWidth: 155,
        display: 'flex',
        justifyContent: 'center',
        alignItems: 'flex-end',
    },
    ampmSelection: {
        top: 11,
        position: 'relative',
        display: 'flex',
        flexDirection: 'column',
        alignItems: 'center',
        marginLeft: 10,
        marginRight: -10,
    },
    ampmLabel: {
        fontSize: 18,
    },
}); }, { name: 'MuiPickerDTHeader' });
exports.DateTimePickerHeader = function (_a) {
    var _b;
    var date = _a.date, openView = _a.openView, meridiemMode = _a.meridiemMode, onOpenViewChange = _a.onOpenViewChange, setMeridiemMode = _a.setMeridiemMode, ampm = _a.ampm;
    var utils = useUtils_1.useUtils();
    var classes = exports.useStyles();
    return (React.createElement(PickerToolbar_1.default, { className: clsx_1.default(classes.toolbar, (_b = {}, _b[classes.toolBar24h] = !ampm, _b)) },
        React.createElement("div", { className: classes.dateHeader },
            React.createElement(ToolbarButton_1.default, { variant: "subtitle1", onClick: function () { return onOpenViewChange('year'); }, selected: openView === 'year', label: utils.getYearText(date) }),
            React.createElement(ToolbarButton_1.default, { variant: "h4", onClick: function () { return onOpenViewChange('date'); }, selected: openView === 'date', label: utils.getDateTimePickerHeaderText(date) })),
        React.createElement("div", { className: classes.timeHeader },
            React.createElement("div", { className: classes.hourMinuteLabel },
                React.createElement(ToolbarButton_1.default, { variant: "h3", onClick: function () { return onOpenViewChange('hours'); }, selected: openView === 'hours', label: utils.getHourText(date, ampm) }),
                React.createElement(ToolbarText_1.default, { variant: "h3", label: ":", selected: false, className: classes.separator }),
                React.createElement(ToolbarButton_1.default, { variant: "h3", onClick: function () { return onOpenViewChange('minutes'); }, selected: openView === 'minutes', label: utils.getMinuteText(date) })),
            ampm && (React.createElement("div", { className: classes.ampmSelection },
                React.createElement(ToolbarButton_1.default, { variant: "subtitle1", typographyClassName: classes.ampmLabel, selected: meridiemMode === 'am', label: utils.getMeridiemText('am'), onClick: function () { return setMeridiemMode('am'); } }),
                React.createElement(ToolbarButton_1.default, { variant: "subtitle1", typographyClassName: classes.ampmLabel, selected: meridiemMode === 'pm', label: utils.getMeridiemText('pm'), onClick: function () { return setMeridiemMode('pm'); } }))))));
};
exports.default = exports.DateTimePickerHeader;
