import { __rest, __assign, __extends } from 'tslib';
import React__default, { createElement, forwardRef, useCallback, createContext, Component, useContext, useRef, useEffect, cloneElement, Fragment, useState, useMemo, useDebugValue, memo } from 'react';
import { bool, string, any, arrayOf, node, func, oneOfType, object, element, number, instanceOf, oneOf } from 'prop-types';
import clsx from 'clsx';
import Typography from '@material-ui/core/Typography';
import { makeStyles, createStyles, withStyles, useTheme } from '@material-ui/core/styles';
import { fade } from '@material-ui/core/styles/colorManipulator';
import Button from '@material-ui/core/Button';
import Toolbar from '@material-ui/core/Toolbar';
import IconButton from '@material-ui/core/IconButton';
import { TransitionGroup, CSSTransition } from 'react-transition-group';
import SvgIcon from '@material-ui/core/SvgIcon';
import EventListener from 'react-event-listener';
import InputAdornment from '@material-ui/core/InputAdornment';
import TextField from '@material-ui/core/TextField';
import { Rifm } from 'rifm';
import { IconButton as IconButton$1 } from '@material-ui/core';
import DialogActions from '@material-ui/core/DialogActions';
import DialogContent from '@material-ui/core/DialogContent';
import Dialog from '@material-ui/core/Dialog';
import Popover from '@material-ui/core/Popover';
import Tab from '@material-ui/core/Tab';
import Tabs from '@material-ui/core/Tabs';
import Paper from '@material-ui/core/Paper';

var useStyles = makeStyles(function (theme) {
  var textColor = theme.palette.type === 'light' ? theme.palette.primary.contrastText : theme.palette.getContrastText(theme.palette.background.default);
  return {
    toolbarTxt: {
      color: fade(textColor, 0.54)
    },
    toolbarBtnSelected: {
      color: textColor
    }
  };
}, {
  name: 'MuiPickersToolbarText'
});

var ToolbarText = function (_a) {
  var _b;

  var selected = _a.selected,
      label = _a.label,
      _c = _a.className,
      className = _c === void 0 ? null : _c,
      other = __rest(_a, ["selected", "label", "className"]);

  var classes = useStyles();
  return createElement(Typography, __assign({
    children: label,
    className: clsx(classes.toolbarTxt, className, (_b = {}, _b[classes.toolbarBtnSelected] = selected, _b))
  }, other));
};

var ToolbarButton = function (_a) {
  var classes = _a.classes,
      _b = _a.className,
      className = _b === void 0 ? null : _b,
      label = _a.label,
      selected = _a.selected,
      variant = _a.variant,
      typographyClassName = _a.typographyClassName,
      other = __rest(_a, ["classes", "className", "label", "selected", "variant", "typographyClassName"]);

  return createElement(Button, __assign({
    className: clsx(classes.toolbarBtn, className)
  }, other), createElement(ToolbarText, {
    className: typographyClassName,
    variant: variant,
    label: label,
    selected: selected
  }));
};

process.env.NODE_ENV !== "production" ? ToolbarButton.propTypes = {
  selected: bool.isRequired,
  label: string.isRequired,
  classes: any.isRequired,
  className: string,
  innerRef: any
} : void 0;
ToolbarButton.defaultProps = {
  className: ''
};
var styles = createStyles({
  toolbarBtn: {
    padding: 0,
    minWidth: '16px',
    textTransform: 'none'
  }
});
var ToolbarButton$1 = withStyles(styles, {
  name: 'MuiPickersToolbarButton'
})(ToolbarButton);

var PickerToolbar = function (_a) {
  var children = _a.children,
      _b = _a.className,
      className = _b === void 0 ? null : _b,
      classes = _a.classes,
      other = __rest(_a, ["children", "className", "classes"]);

  return createElement(Toolbar, __assign({
    className: clsx(classes.toolbar, className)
  }, other), children);
};

process.env.NODE_ENV !== "production" ? PickerToolbar.propTypes = {
  children: arrayOf(node).isRequired,
  className: string,
  classes: any.isRequired,
  innerRef: any
} : void 0;
var styles$1 = function (theme) {
  return createStyles({
    toolbar: {
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'flex-start',
      justifyContent: 'center',
      height: 100,
      backgroundColor: theme.palette.type === 'light' ? theme.palette.primary.main : theme.palette.background.default
    }
  });
};
var PickerToolbar$1 = withStyles(styles$1, {
  name: 'MuiPickersToolbar'
})(PickerToolbar);

var useStyles$1 = makeStyles(function (theme) {
  return {
    root: {
      height: theme.spacing(5),
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      cursor: 'pointer',
      outline: 'none',
      '&:focus': {
        color: theme.palette.primary.main,
        fontWeight: theme.typography.fontWeightMedium
      }
    },
    selected: {
      margin: '10px 0',
      fontWeight: theme.typography.fontWeightMedium
    },
    disabled: {
      pointerEvents: 'none',
      color: theme.palette.text.hint
    }
  };
}, {
  name: 'MuiPickersYear'
});
var Year = function (_a) {
  var _b;

  var onSelect = _a.onSelect,
      forwardedRef = _a.forwardedRef,
      value = _a.value,
      selected = _a.selected,
      disabled = _a.disabled,
      children = _a.children,
      other = __rest(_a, ["onSelect", "forwardedRef", "value", "selected", "disabled", "children"]);

  var classes = useStyles$1();
  var handleClick = useCallback(function () {
    return onSelect(value);
  }, [onSelect, value]);
  return createElement(Typography, __assign({
    role: "button",
    component: "div",
    tabIndex: disabled ? -1 : 0,
    onClick: handleClick,
    onKeyPress: handleClick,
    color: selected ? 'primary' : undefined,
    variant: selected ? 'h5' : 'subtitle1',
    children: children,
    ref: forwardedRef,
    className: clsx(classes.root, (_b = {}, _b[classes.selected] = selected, _b[classes.disabled] = disabled, _b))
  }, other));
};
Year.displayName = 'Year';
var Year$1 = forwardRef(function (props, ref) {
  return createElement(Year, __assign({}, props, {
    forwardedRef: ref
  }));
});

var MuiPickersContext = createContext(null);

var MuiPickersUtilsProvider =
/*@__PURE__*/
function (_super) {
  __extends(MuiPickersUtilsProvider, _super);

  function MuiPickersUtilsProvider() {
    var _this = _super !== null && _super.apply(this, arguments) || this;

    _this.state = {
      utils: null
    };
    return _this;
  }

  MuiPickersUtilsProvider.getDerivedStateFromProps = function (_a) {
    var locale = _a.locale,
        libInstance = _a.libInstance,
        Utils = _a.utils;
    return {
      utils: new Utils({
        locale: locale,
        instance: libInstance
      })
    };
  };

  MuiPickersUtilsProvider.prototype.render = function () {
    return createElement(MuiPickersContext.Provider, {
      value: this.state.utils,
      children: this.props.children
    });
  };

  process.env.NODE_ENV !== "production" ? MuiPickersUtilsProvider.propTypes = {
    utils: func.isRequired,
    locale: oneOfType([object, string]),
    children: oneOfType([element.isRequired, arrayOf(element.isRequired)]).isRequired,
    moment: func
  } : void 0;
  return MuiPickersUtilsProvider;
}(Component);

var checkUtils = function (utils) {
  if (!utils) {
    // tslint:disable-next-line
    throw new Error('Can not find utils in context. You either a) forgot to wrap your component tree in MuiPickersUtilsProvider; or b) mixed named and direct file imports.  Recommendation: use named imports from the module index.');
  }
};
function useUtils() {
  var utils = useContext(MuiPickersContext);
  checkUtils(utils);
  return utils;
}

var useStyles$2 = makeStyles({
  container: {
    maxHeight: 300,
    overflowY: 'auto',
    justifyContent: 'center'
  }
}, {
  name: 'MuiPickersYearSelection'
});

var YearSelection = function (_a) {
  var date = _a.date,
      onChange = _a.onChange,
      onYearChange = _a.onYearChange,
      minDate = _a.minDate,
      maxDate = _a.maxDate,
      disablePast = _a.disablePast,
      disableFuture = _a.disableFuture,
      animateYearScrolling = _a.animateYearScrolling;
  var utils = useUtils();
  var classes = useStyles$2();
  var selectedYearRef = useRef(null);
  useEffect(function () {
    if (selectedYearRef.current && selectedYearRef.current.scrollIntoView) {
      selectedYearRef.current.scrollIntoView({
        behavior: animateYearScrolling ? 'smooth' : 'auto'
      });
    }
  }, []); // eslint-disable-line

  var currentYear = utils.getYear(date);
  var onYearSelect = useCallback(function (year) {
    var newDate = utils.setYear(date, year);

    if (onYearChange) {
      onYearChange(newDate);
    }

    onChange(newDate, true);
  }, [date, onChange, onYearChange, utils]);
  return createElement("div", {
    className: classes.container
  }, utils.getYearRange(minDate, maxDate).map(function (year) {
    var yearNumber = utils.getYear(year);
    var selected = yearNumber === currentYear;
    return createElement(Year$1, {
      key: utils.getYearText(year),
      selected: selected,
      value: yearNumber,
      onSelect: onYearSelect,
      ref: selected ? selectedYearRef : undefined,
      disabled: Boolean(disablePast && utils.isBeforeYear(year, utils.date()) || disableFuture && utils.isAfterYear(year, utils.date()))
    }, utils.getYearText(year));
  }));
};

var useStyles$3 = makeStyles(function (theme) {
  return {
    root: {
      flex: '1 0 33.33%',
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      cursor: 'pointer',
      outline: 'none',
      height: 75,
      transition: theme.transitions.create('font-size', {
        duration: '100ms'
      }),
      '&:focus': {
        color: theme.palette.primary.main,
        fontWeight: theme.typography.fontWeightMedium
      }
    },
    selected: {
      color: theme.palette.primary.main,
      fontWeight: theme.typography.fontWeightMedium
    },
    disabled: {
      pointerEvents: 'none',
      color: theme.palette.text.hint
    }
  };
}, {
  name: 'MuiPickersMonth'
});
var Month = function (_a) {
  var _b;

  var selected = _a.selected,
      onSelect = _a.onSelect,
      disabled = _a.disabled,
      value = _a.value,
      children = _a.children,
      other = __rest(_a, ["selected", "onSelect", "disabled", "value", "children"]);

  var classes = useStyles$3();
  var handleSelection = useCallback(function () {
    onSelect(value);
  }, [onSelect, value]);
  return createElement(Typography, __assign({
    role: "button",
    component: "div",
    className: clsx(classes.root, (_b = {}, _b[classes.selected] = selected, _b[classes.disabled] = disabled, _b)),
    tabIndex: disabled ? -1 : 0,
    onClick: handleSelection,
    onKeyPress: handleSelection,
    color: selected ? 'primary' : undefined,
    variant: selected ? 'h5' : 'subtitle1',
    children: children
  }, other));
};
Month.displayName = 'Month';

var useStyles$4 = makeStyles({
  container: {
    width: 310,
    display: 'flex',
    flexWrap: 'wrap',
    alignContent: 'stretch'
  }
}, {
  name: 'MuiPickersMonthSelection'
});
var MonthSelection = function (_a) {
  var disablePast = _a.disablePast,
      disableFuture = _a.disableFuture,
      minDate = _a.minDate,
      maxDate = _a.maxDate,
      date = _a.date,
      onMonthChange = _a.onMonthChange,
      onChange = _a.onChange;
  var utils = useUtils();
  var classes = useStyles$4();
  var currentMonth = utils.getMonth(date);

  var shouldDisableMonth = function (month) {
    var now = utils.date();
    var utilMinDate = utils.date(minDate);
    var utilMaxDate = utils.date(maxDate);
    var firstEnabledMonth = utils.startOfMonth(disablePast && utils.isAfter(now, utilMinDate) ? now : utilMinDate);
    var lastEnabledMonth = utils.startOfMonth(disableFuture && utils.isBefore(now, utilMaxDate) ? now : utilMaxDate);
    var isBeforeFirstEnabled = utils.isBefore(month, firstEnabledMonth);
    var isAfterLastEnabled = utils.isAfter(month, lastEnabledMonth);
    return isBeforeFirstEnabled || isAfterLastEnabled;
  };

  var onMonthSelect = useCallback(function (month) {
    var newDate = utils.setMonth(date, month);
    onChange(newDate, true);

    if (onMonthChange) {
      onMonthChange(newDate);
    }
  }, [date, onChange, onMonthChange, utils]);
  return createElement("div", {
    className: classes.container
  }, utils.getMonthArray(date).map(function (month) {
    var monthNumber = utils.getMonth(month);
    var monthText = utils.format(month, 'MMM');
    return createElement(Month, {
      key: monthText,
      value: monthNumber,
      selected: monthNumber === currentMonth,
      onSelect: onMonthSelect,
      disabled: shouldDisableMonth(month)
    }, monthText);
  }));
};

var useStyles$5 = makeStyles(function (theme) {
  return {
    day: {
      width: 36,
      height: 36,
      fontSize: theme.typography.caption.fontSize,
      margin: '0 2px',
      color: theme.palette.text.primary,
      fontWeight: theme.typography.fontWeightMedium,
      padding: 0
    },
    hidden: {
      opacity: 0,
      pointerEvents: 'none'
    },
    current: {
      color: theme.palette.primary.main,
      fontWeight: 600
    },
    isSelected: {
      color: theme.palette.primary.contrastText,
      backgroundColor: theme.palette.primary.main,
      fontWeight: theme.typography.fontWeightMedium,
      '&:hover': {
        backgroundColor: theme.palette.primary.main
      }
    },
    isDisabled: {
      pointerEvents: 'none',
      color: theme.palette.text.hint
    }
  };
}, {
  name: 'MuiPickersDay'
});
var Day = function (_a) {
  var _b;

  var children = _a.children,
      disabled = _a.disabled,
      hidden = _a.hidden,
      current = _a.current,
      selected = _a.selected,
      other = __rest(_a, ["children", "disabled", "hidden", "current", "selected"]);

  var classes = useStyles$5();
  var className = clsx(classes.day, (_b = {}, _b[classes.hidden] = hidden, _b[classes.current] = current, _b[classes.isSelected] = selected, _b[classes.isDisabled] = disabled, _b));
  return createElement(IconButton, __assign({
    className: className,
    tabIndex: hidden || disabled ? -1 : 0
  }, other), createElement(Typography, {
    variant: "body2",
    color: "inherit"
  }, children));
};
Day.displayName = 'Day';
process.env.NODE_ENV !== "production" ? Day.propTypes = {
  current: bool,
  disabled: bool,
  hidden: bool,
  selected: bool
} : void 0;
Day.defaultProps = {
  disabled: false,
  hidden: false,
  current: false,
  selected: false
};

var DayWrapper = function (_a) {
  var children = _a.children,
      value = _a.value,
      disabled = _a.disabled,
      onSelect = _a.onSelect,
      dayInCurrentMonth = _a.dayInCurrentMonth,
      other = __rest(_a, ["children", "value", "disabled", "onSelect", "dayInCurrentMonth"]);

  var handleClick = useCallback(function () {
    return onSelect(value);
  }, [onSelect, value]);
  return createElement("div", __assign({
    role: "presentation",
    onClick: dayInCurrentMonth && !disabled ? handleClick : undefined,
    onKeyPress: dayInCurrentMonth && !disabled ? handleClick : undefined
  }, other), children);
};

var animationDuration = 350;
var useStyles$6 = makeStyles(function (theme) {
  var slideTransition = theme.transitions.create('transform', {
    duration: animationDuration,
    easing: 'cubic-bezier(0.35, 0.8, 0.4, 1)'
  });
  return {
    transitionContainer: {
      display: 'block',
      position: 'relative',
      '& > *': {
        position: 'absolute',
        top: 0,
        right: 0,
        left: 0
      }
    },
    'slideEnter-left': {
      willChange: 'transform',
      transform: 'translate(100%)'
    },
    'slideEnter-right': {
      willChange: 'transform',
      transform: 'translate(-100%)'
    },
    slideEnterActive: {
      transform: 'translate(0%)',
      transition: slideTransition
    },
    slideExit: {
      transform: 'translate(0%)'
    },
    'slideExitActiveLeft-left': {
      willChange: 'transform',
      transform: 'translate(-200%)',
      transition: slideTransition
    },
    'slideExitActiveLeft-right': {
      willChange: 'transform',
      transform: 'translate(200%)',
      transition: slideTransition
    }
  };
}, {
  name: 'MuiPickersSlideTransition'
});

var SlideTransition = function (_a) {
  var children = _a.children,
      transKey = _a.transKey,
      slideDirection = _a.slideDirection,
      _b = _a.className,
      className = _b === void 0 ? null : _b;
  var classes = useStyles$6();
  var transitionClasses = {
    exit: classes.slideExit,
    enterActive: classes.slideEnterActive,
    // @ts-ignore
    enter: classes['slideEnter-' + slideDirection],
    // @ts-ignore
    exitActive: classes['slideExitActiveLeft-' + slideDirection]
  };
  return createElement(TransitionGroup, {
    className: clsx(classes.transitionContainer, className),
    childFactory: function (element) {
      return cloneElement(element, {
        classNames: transitionClasses
      });
    }
  }, createElement(CSSTransition, {
    mountOnEnter: true,
    unmountOnExit: true,
    key: transKey + slideDirection,
    timeout: animationDuration,
    classNames: transitionClasses,
    children: children
  }));
};

var ArrowLeftIcon = function (props) {
  return React__default.createElement(SvgIcon, __assign({}, props), React__default.createElement("path", {
    d: "M15.41 16.59L10.83 12l4.58-4.59L14 6l-6 6 6 6 1.41-1.41z"
  }), React__default.createElement("path", {
    fill: "none",
    d: "M0 0h24v24H0V0z"
  }));
};

var ArrowRightIcon = function (props) {
  return React__default.createElement(SvgIcon, __assign({}, props), React__default.createElement("path", {
    d: "M8.59 16.59L13.17 12 8.59 7.41 10 6l6 6-6 6-1.41-1.41z"
  }), React__default.createElement("path", {
    fill: "none",
    d: "M0 0h24v24H0V0z"
  }));
};

var useStyles$7 = makeStyles(function (theme) {
  return {
    switchHeader: {
      display: 'flex',
      justifyContent: 'space-between',
      alignItems: 'center',
      marginTop: theme.spacing(0.5),
      marginBottom: theme.spacing(1)
    },
    transitionContainer: {
      width: '100%',
      height: 20
    },
    iconButton: {
      zIndex: 2,
      backgroundColor: theme.palette.background.paper,
      '& > *': {
        // label
        backgroundColor: theme.palette.background.paper,
        '& > *': {
          // icon
          zIndex: 1,
          overflow: 'visible'
        }
      }
    },
    daysHeader: {
      display: 'flex',
      justifyContent: 'center',
      alignItems: 'center',
      maxHeight: 16
    },
    dayLabel: {
      width: 36,
      margin: '0 2px',
      textAlign: 'center',
      color: theme.palette.text.hint
    }
  };
}, {
  name: 'MuiPickersCalendarHeader'
});
var CalendarHeader = function (_a) {
  var currentMonth = _a.currentMonth,
      onMonthChange = _a.onMonthChange,
      leftArrowIcon = _a.leftArrowIcon,
      rightArrowIcon = _a.rightArrowIcon,
      leftArrowButtonProps = _a.leftArrowButtonProps,
      rightArrowButtonProps = _a.rightArrowButtonProps,
      disablePrevMonth = _a.disablePrevMonth,
      disableNextMonth = _a.disableNextMonth,
      slideDirection = _a.slideDirection;
  var utils = useUtils();
  var classes = useStyles$7();
  var theme = useTheme();
  var rtl = theme.direction === 'rtl';

  var selectNextMonth = function () {
    return onMonthChange(utils.getNextMonth(currentMonth), 'left');
  };

  var selectPreviousMonth = function () {
    return onMonthChange(utils.getPreviousMonth(currentMonth), 'right');
  };

  return createElement("div", null, createElement("div", {
    className: classes.switchHeader
  }, createElement(IconButton, __assign({}, leftArrowButtonProps, {
    disabled: disablePrevMonth,
    onClick: selectPreviousMonth,
    className: classes.iconButton
  }), rtl ? rightArrowIcon : leftArrowIcon), createElement(SlideTransition, {
    slideDirection: slideDirection,
    transKey: currentMonth.toString(),
    className: classes.transitionContainer
  }, createElement(Typography, {
    align: "center",
    variant: "body1"
  }, utils.getCalendarHeaderText(currentMonth))), createElement(IconButton, __assign({}, rightArrowButtonProps, {
    disabled: disableNextMonth,
    onClick: selectNextMonth,
    className: classes.iconButton
  }), rtl ? leftArrowIcon : rightArrowIcon)), createElement("div", {
    className: classes.daysHeader
  }, utils.getWeekdays().map(function (day, index) {
    return createElement(Typography, {
      key: index,
      variant: "caption",
      className: classes.dayLabel
    }, day);
  })));
};
CalendarHeader.displayName = 'CalendarHeader';
process.env.NODE_ENV !== "production" ? CalendarHeader.propTypes = {
  leftArrowIcon: node,
  rightArrowIcon: node,
  disablePrevMonth: bool,
  disableNextMonth: bool
} : void 0;
CalendarHeader.defaultProps = {
  leftArrowIcon: createElement(ArrowLeftIcon, null),
  rightArrowIcon: createElement(ArrowRightIcon, null),
  disablePrevMonth: false,
  disableNextMonth: false
};

var findClosestEnabledDate = function (_a) {
  var date = _a.date,
      utils = _a.utils,
      minDate = _a.minDate,
      maxDate = _a.maxDate,
      disableFuture = _a.disableFuture,
      disablePast = _a.disablePast,
      shouldDisableDate = _a.shouldDisableDate;
  var today = utils.startOfDay(utils.date());

  if (disablePast && utils.isBefore(minDate, today)) {
    minDate = today;
  }

  if (disableFuture && utils.isAfter(maxDate, today)) {
    maxDate = today;
  }

  var forward = date;
  var backward = date;

  if (utils.isBefore(date, minDate)) {
    forward = utils.date(minDate);
    backward = null;
  }

  if (utils.isAfter(date, maxDate)) {
    if (backward) {
      backward = utils.date(maxDate);
    }

    forward = null;
  }

  while (forward || backward) {
    if (forward && utils.isAfter(forward, maxDate)) {
      forward = null;
    }

    if (backward && utils.isBefore(backward, minDate)) {
      backward = null;
    }

    if (forward) {
      if (!shouldDisableDate(forward)) {
        return forward;
      }

      forward = utils.addDays(forward, 1);
    }

    if (backward) {
      if (!shouldDisableDate(backward)) {
        return backward;
      }

      backward = utils.addDays(backward, -1);
    }
  }

  return null;
};
var isYearOnlyView = function (views) {
  return views.length === 1 && views[0] === 'year';
};
var isYearAndMonthViews = function (views) {
  return views.length === 2 && views.includes('month') && views.includes('year');
};
var getFormatByViews = function (views, utils) {
  if (isYearOnlyView(views)) {
    return utils.yearFormat;
  }

  if (isYearAndMonthViews(views)) {
    return utils.yearMonthFormat;
  }

  return utils.dateFormat;
};

var withUtils = function () {
  return function (Component) {
    var WithUtils = function (props) {
      var utils = useUtils();
      return createElement(Component, __assign({
        utils: utils
      }, props));
    };

    WithUtils.displayName = "WithUtils(" + (Component.displayName || Component.name) + ")";
    return WithUtils;
  };
};

var Calendar =
/*@__PURE__*/
function (_super) {
  __extends(Calendar, _super);

  function Calendar() {
    var _this = _super !== null && _super.apply(this, arguments) || this;

    _this.state = {
      slideDirection: 'left',
      currentMonth: _this.props.utils.startOfMonth(_this.props.date)
    };

    _this.onDateSelect = function (day, isFinish) {
      if (isFinish === void 0) {
        isFinish = true;
      }

      var _a = _this.props,
          date = _a.date,
          utils = _a.utils;

      _this.props.onChange(utils.mergeDateAndTime(day, date), isFinish);
    };

    _this.handleChangeMonth = function (newMonth, slideDirection) {
      if (_this.props.onMonthChange) {
        _this.props.onMonthChange(newMonth);
      }

      _this.setState({
        currentMonth: newMonth,
        slideDirection: slideDirection
      });
    };

    _this.validateMinMaxDate = function (day) {
      var _a = _this.props,
          minDate = _a.minDate,
          maxDate = _a.maxDate,
          utils = _a.utils,
          disableFuture = _a.disableFuture,
          disablePast = _a.disablePast;
      var now = utils.date();
      return Boolean(disableFuture && utils.isAfterDay(day, now) || disablePast && utils.isBeforeDay(day, now) || minDate && utils.isBeforeDay(day, utils.date(minDate)) || maxDate && utils.isAfterDay(day, utils.date(maxDate)));
    };

    _this.shouldDisablePrevMonth = function () {
      var _a = _this.props,
          utils = _a.utils,
          disablePast = _a.disablePast,
          minDate = _a.minDate;
      var now = utils.date();
      var firstEnabledMonth = utils.startOfMonth(disablePast && utils.isAfter(now, utils.date(minDate)) ? now : utils.date(minDate));
      return !utils.isBefore(firstEnabledMonth, _this.state.currentMonth);
    };

    _this.shouldDisableNextMonth = function () {
      var _a = _this.props,
          utils = _a.utils,
          disableFuture = _a.disableFuture,
          maxDate = _a.maxDate;
      var now = utils.date();
      var lastEnabledMonth = utils.startOfMonth(disableFuture && utils.isBefore(now, utils.date(maxDate)) ? now : utils.date(maxDate));
      return !utils.isAfter(lastEnabledMonth, _this.state.currentMonth);
    };

    _this.shouldDisableDate = function (day) {
      var shouldDisableDate = _this.props.shouldDisableDate;
      return _this.validateMinMaxDate(day) || Boolean(shouldDisableDate && shouldDisableDate(day));
    };

    _this.moveToDay = function (day) {
      if (day && !_this.shouldDisableDate(day)) {
        _this.onDateSelect(day, false);
      }
    };

    _this.handleKeyDown = function (event) {
      var _a = _this.props,
          theme = _a.theme,
          date = _a.date,
          utils = _a.utils;

      switch (event.key) {
        case 'ArrowUp':
          _this.moveToDay(utils.addDays(date, -7));

          break;

        case 'ArrowDown':
          _this.moveToDay(utils.addDays(date, 7));

          break;

        case 'ArrowLeft':
          theme.direction === 'ltr' ? _this.moveToDay(utils.addDays(date, -1)) : _this.moveToDay(utils.addDays(date, 1));
          break;

        case 'ArrowRight':
          theme.direction === 'ltr' ? _this.moveToDay(utils.addDays(date, 1)) : _this.moveToDay(utils.addDays(date, -1));
          break;

        default:
          // if key is not handled, stop execution
          return;
      } // if event was handled prevent other side effects (e.g. page scroll)


      event.preventDefault();
    };

    _this.renderWeeks = function () {
      var _a = _this.props,
          utils = _a.utils,
          classes = _a.classes;
      var weeks = utils.getWeekArray(_this.state.currentMonth);
      return weeks.map(function (week) {
        return createElement("div", {
          key: "week-" + week[0].toString(),
          className: classes.week
        }, _this.renderDays(week));
      });
    };

    _this.renderDays = function (week) {
      var _a = _this.props,
          date = _a.date,
          renderDay = _a.renderDay,
          utils = _a.utils;
      var now = utils.date();
      var selectedDate = utils.startOfDay(date);
      var currentMonthNumber = utils.getMonth(_this.state.currentMonth);
      return week.map(function (day) {
        var disabled = _this.shouldDisableDate(day);

        var isDayInCurrentMonth = utils.getMonth(day) === currentMonthNumber;
        var dayComponent = createElement(Day, {
          disabled: disabled,
          current: utils.isSameDay(day, now),
          hidden: !isDayInCurrentMonth,
          selected: utils.isSameDay(selectedDate, day)
        }, utils.getDayText(day));

        if (renderDay) {
          dayComponent = renderDay(day, selectedDate, isDayInCurrentMonth, dayComponent);
        }

        return createElement(DayWrapper, {
          value: day,
          key: day.toString(),
          disabled: disabled,
          dayInCurrentMonth: isDayInCurrentMonth,
          onSelect: _this.onDateSelect
        }, dayComponent);
      });
    };

    return _this;
  }

  Calendar.getDerivedStateFromProps = function (nextProps, state) {
    var utils = nextProps.utils,
        nextDate = nextProps.date;

    if (!utils.isEqual(nextDate, state.lastDate)) {
      var nextMonth = utils.getMonth(nextDate);
      var lastMonth = utils.getMonth(state.lastDate || nextDate);
      return {
        lastDate: nextDate,
        currentMonth: nextProps.utils.startOfMonth(nextDate),
        // prettier-ignore
        slideDirection: nextMonth === lastMonth ? state.slideDirection : nextMonth > lastMonth ? 'left' : 'right'
      };
    }

    return null;
  };

  Calendar.prototype.componentDidMount = function () {
    var _a = this.props,
        date = _a.date,
        minDate = _a.minDate,
        maxDate = _a.maxDate,
        utils = _a.utils,
        disablePast = _a.disablePast,
        disableFuture = _a.disableFuture;

    if (this.shouldDisableDate(date)) {
      var closestEnabledDate = findClosestEnabledDate({
        date: date,
        utils: utils,
        minDate: utils.date(minDate),
        maxDate: utils.date(maxDate),
        disablePast: Boolean(disablePast),
        disableFuture: Boolean(disableFuture),
        shouldDisableDate: this.shouldDisableDate
      });
      this.onDateSelect(closestEnabledDate, false);
    }
  };

  Calendar.prototype.render = function () {
    var _a = this.state,
        currentMonth = _a.currentMonth,
        slideDirection = _a.slideDirection;
    var _b = this.props,
        classes = _b.classes,
        allowKeyboardControl = _b.allowKeyboardControl,
        leftArrowButtonProps = _b.leftArrowButtonProps,
        leftArrowIcon = _b.leftArrowIcon,
        rightArrowButtonProps = _b.rightArrowButtonProps,
        rightArrowIcon = _b.rightArrowIcon;
    return createElement(Fragment, null, allowKeyboardControl && createElement(EventListener, {
      target: "window",
      onKeyDown: this.handleKeyDown
    }), createElement(CalendarHeader, {
      currentMonth: currentMonth,
      slideDirection: slideDirection,
      onMonthChange: this.handleChangeMonth,
      leftArrowIcon: leftArrowIcon,
      leftArrowButtonProps: leftArrowButtonProps,
      rightArrowIcon: rightArrowIcon,
      rightArrowButtonProps: rightArrowButtonProps,
      disablePrevMonth: this.shouldDisablePrevMonth(),
      disableNextMonth: this.shouldDisableNextMonth()
    }), createElement(SlideTransition, {
      slideDirection: slideDirection,
      transKey: currentMonth.toString(),
      className: classes.transitionContainer
    }, createElement("div", null, this.renderWeeks())));
  };

  process.env.NODE_ENV !== "production" ? Calendar.propTypes = {
    renderDay: func,
    shouldDisableDate: func,
    allowKeyboardControl: bool
  } : void 0;
  Calendar.defaultProps = {
    minDate: new Date('1900-01-01'),
    maxDate: new Date('2100-01-01'),
    disablePast: false,
    disableFuture: false,
    allowKeyboardControl: true
  };
  return Calendar;
}(Component);
var styles$2 = function (theme) {
  return {
    transitionContainer: {
      minHeight: 36 * 6,
      marginTop: theme.spacing(1.5)
    },
    week: {
      display: 'flex',
      justifyContent: 'center'
    }
  };
};
var Calendar$1 = withStyles(styles$2, {
  name: 'MuiPickersCalendar',
  withTheme: true
})(withUtils()(Calendar));

var date = oneOfType([object, string, number, instanceOf(Date)]);
var datePickerView = oneOf(['year', 'month', 'day']);
var DomainPropTypes = {
  date: date,
  datePickerView: datePickerView
};
/* eslint-disable @typescript-eslint/no-object-literal-type-assertion */

var timePickerDefaultProps = {
  ampm: true,
  invalidDateMessage: 'Invalid Time Format'
};
var datePickerDefaultProps = {
  openTo: 'day',
  views: ['year', 'day'],
  minDate: new Date('1900-01-01'),
  maxDate: new Date('2100-01-01'),
  invalidDateMessage: 'Invalid Date Format',
  minDateMessage: 'Date should not be before minimal date',
  maxDateMessage: 'Date should not be after maximal date',
  allowKeyboardControl: true
};
var dateTimePickerDefaultProps = __assign({}, timePickerDefaultProps, datePickerDefaultProps, {
  showTabs: true,
  openTo: undefined,
  views: undefined
});

var useStyles$8 = makeStyles({
  toolbarCenter: {
    flexDirection: 'row',
    alignItems: 'center'
  }
}, {
  name: 'MuiPickersDatePickerRoot'
});
var DatePickerRoot = function (_a) {
  var _b;

  var date = _a.date,
      _c = _a.views,
      views = _c === void 0 ? ['year', 'day'] : _c,
      disablePast = _a.disablePast,
      disableFuture = _a.disableFuture,
      onChange = _a.onChange,
      openTo = _a.openTo,
      unparsedMinDate = _a.minDate,
      unparsedMaxDate = _a.maxDate,
      animateYearScrolling = _a.animateYearScrolling,
      leftArrowIcon = _a.leftArrowIcon,
      rightArrowIcon = _a.rightArrowIcon,
      renderDay = _a.renderDay,
      shouldDisableDate = _a.shouldDisableDate,
      allowKeyboardControl = _a.allowKeyboardControl,
      onMonthChange = _a.onMonthChange,
      onYearChange = _a.onYearChange,
      onlyCalendar = _a.onlyCalendar,
      leftArrowButtonProps = _a.leftArrowButtonProps,
      rightArrowButtonProps = _a.rightArrowButtonProps;
  var utils = useUtils();
  var classes = useStyles$8();

  var _d = useState(openTo && views.includes(openTo) ? openTo : views[0]),
      openView = _d[0],
      setOpenView = _d[1];

  var isYearOnly = useMemo(function () {
    return isYearOnlyView(views);
  }, [views]);
  var isYearAndMonth = useMemo(function () {
    return isYearAndMonthViews(views);
  }, [views]);
  var minDate = useMemo(function () {
    return utils.date(unparsedMinDate);
  }, [unparsedMinDate, utils]);
  var maxDate = useMemo(function () {
    return utils.date(unparsedMaxDate);
  }, [unparsedMaxDate, utils]);
  var getNextAvailableView = useCallback(function (nextView) {
    if (views.includes(nextView)) {
      return nextView;
    }

    return views[views.indexOf(openView) + 1];
  }, [openView, views]);
  var handleChangeAndOpenNext = useCallback(function (nextView) {
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
  return createElement(Fragment, null, !onlyCalendar && createElement(PickerToolbar$1, {
    className: clsx((_b = {}, _b[classes.toolbarCenter] = isYearOnly, _b))
  }, createElement(ToolbarButton$1, {
    variant: isYearOnly ? 'h3' : 'subtitle1',
    onClick: function () {
      return setOpenView('year');
    },
    selected: openView === 'year',
    label: utils.getYearText(date)
  }), !isYearOnly && !isYearAndMonth && createElement(ToolbarButton$1, {
    variant: "h4",
    onClick: function () {
      return setOpenView('day');
    },
    selected: openView === 'day',
    label: utils.getDatePickerHeaderText(date)
  }), isYearAndMonth && createElement(ToolbarButton$1, {
    variant: "h4",
    onClick: function () {
      return setOpenView('month');
    },
    selected: openView === 'month',
    label: utils.getMonthText(date)
  })), openView === 'year' && createElement(YearSelection, {
    date: date,
    onChange: handleChangeAndOpenNext('month'),
    minDate: minDate,
    maxDate: maxDate,
    disablePast: disablePast,
    disableFuture: disableFuture,
    onYearChange: onYearChange,
    animateYearScrolling: animateYearScrolling
  }), openView === 'month' && createElement(MonthSelection, {
    date: date,
    onChange: handleChangeAndOpenNext('day'),
    minDate: minDate,
    maxDate: maxDate,
    disablePast: disablePast,
    disableFuture: disableFuture,
    onMonthChange: onMonthChange
  }), openView === 'day' && createElement(Calendar$1, {
    date: date,
    onChange: onChange,
    onMonthChange: onMonthChange,
    disablePast: disablePast,
    disableFuture: disableFuture,
    minDate: minDate,
    maxDate: maxDate,
    leftArrowIcon: leftArrowIcon,
    leftArrowButtonProps: leftArrowButtonProps,
    rightArrowIcon: rightArrowIcon,
    rightArrowButtonProps: rightArrowButtonProps,
    renderDay: renderDay,
    shouldDisableDate: shouldDisableDate,
    allowKeyboardControl: allowKeyboardControl
  }));
};
process.env.NODE_ENV !== "production" ? DatePickerRoot.propTypes = {
  onlyCalendar: bool,
  views: arrayOf(DomainPropTypes.datePickerView),
  openTo: DomainPropTypes.datePickerView
} : void 0;
DatePickerRoot.defaultProps = __assign({
  onlyCalendar: false
}, datePickerDefaultProps);

var KeyboardIcon = function (props) {
  return React__default.createElement(SvgIcon, __assign({}, props), React__default.createElement("path", {
    d: "M17 12h-5v5h5v-5zM16 1v2H8V1H6v2H5c-1.11 0-1.99.9-1.99 2L3 19c0 1.1.89 2 2 2h14c1.1 0 2-.9 2-2V5c0-1.1-.9-2-2-2h-1V1h-2zm3 18H5V8h14v11z"
  }), React__default.createElement("path", {
    fill: "none",
    d: "M0 0h24v24H0z"
  }));
};

var getDisplayDate = function (value, format, utils, isEmpty, _a) {
  var invalidLabel = _a.invalidLabel,
      emptyLabel = _a.emptyLabel,
      labelFunc = _a.labelFunc;
  var date = utils.date(value);

  if (labelFunc) {
    return labelFunc(isEmpty ? null : date, invalidLabel);
  }

  if (isEmpty) {
    return emptyLabel || '';
  }

  return utils.isValid(date) ? utils.format(date, format) : invalidLabel;
};
var getError = function (value, utils, _a // DateTimePicker doesn't support month in openTo
) {
  var maxDate = _a.maxDate,
      minDate = _a.minDate,
      disablePast = _a.disablePast,
      disableFuture = _a.disableFuture,
      maxDateMessage = _a.maxDateMessage,
      minDateMessage = _a.minDateMessage,
      invalidDateMessage = _a.invalidDateMessage;
  var parsedValue = utils.date(value); // if null - do not show error

  if (value === null) {
    return '';
  }

  if (!utils.isValid(value)) {
    return invalidDateMessage;
  }

  if (maxDate && utils.isAfter(parsedValue, utils.endOfDay(utils.date(maxDate))) || disableFuture && utils.isAfter(parsedValue, utils.endOfDay(utils.date()))) {
    return maxDateMessage;
  }

  if (minDate && utils.isBefore(parsedValue, utils.startOfDay(utils.date(minDate))) || disablePast && utils.isBefore(parsedValue, utils.startOfDay(utils.date()))) {
    return minDateMessage;
  }

  return '';
};
function pick12hOr24hFormat(userFormat, ampm, formats) {
  if (ampm === void 0) {
    ampm = true;
  }

  if (userFormat) {
    return userFormat;
  }

  return ampm ? formats['12h'] : formats['24h'];
}
function makeMaskFromFormat(format, numberMaskChar) {
  return format.replace(/[a-z]/gi, numberMaskChar);
}
var maskedDateFormatter = function (mask, numberMaskChar, refuse) {
  return function (value) {
    var result = '';
    var parsed = value.replace(refuse, '');
    var i = 0;
    var n = 0;

    while (i < mask.length) {
      var maskChar = mask[i];

      if (maskChar === numberMaskChar && n < parsed.length) {
        var parsedChar = parsed[n];
        result += parsedChar;
        n += 1;
      } else {
        result += maskChar;
      }

      i += 1;
    }

    return result;
  };
};

var refuse = /[^\dap]+/gi;

var KeyboardDateInput = function (_a) {
  var inputValue = _a.inputValue,
      inputVariant = _a.inputVariant,
      validationError = _a.validationError,
      KeyboardButtonProps = _a.KeyboardButtonProps,
      InputAdornmentProps = _a.InputAdornmentProps,
      onClick = _a.onClick,
      onChange = _a.onChange,
      mask = _a.mask,
      _b = _a.maskChar,
      maskChar = _b === void 0 ? '_' : _b,
      format = _a.format,
      other = __rest(_a, ["inputValue", "inputVariant", "validationError", "KeyboardButtonProps", "InputAdornmentProps", "onClick", "onChange", "mask", "maskChar", "format"]);

  var inputMask = mask || makeMaskFromFormat(format, maskChar); // prettier-ignore

  var formatter = useCallback(maskedDateFormatter(inputMask, maskChar, refuse), [mask, maskChar]);
  var position = InputAdornmentProps && InputAdornmentProps.position ? InputAdornmentProps.position : 'end';
  return createElement(Rifm, {
    value: inputValue,
    onChange: onChange,
    refuse: refuse,
    format: formatter
  }, function (_a) {
    var _b;

    var onChange = _a.onChange,
        value = _a.value;
    return createElement(TextField, __assign({
      error: Boolean(validationError),
      helperText: validationError
    }, other, {
      value: value,
      onChange: onChange,
      variant: inputVariant,
      InputProps: (_b = {}, _b[position + "Adornment"] = createElement(InputAdornment, __assign({
        position: position
      }, InputAdornmentProps), createElement(IconButton$1, __assign({}, KeyboardButtonProps, {
        onClick: onClick
      }), createElement(KeyboardIcon, null))), _b)
    }));
  });
};

var DIALOG_WIDTH = 310;
var DIALOG_WIDTH_WIDER = 325;

var ModalDialog = function (_a) {
  var _b, _c, _d;

  var children = _a.children,
      classes = _a.classes,
      onKeyDownInner = _a.onKeyDownInner,
      onAccept = _a.onAccept,
      onDismiss = _a.onDismiss,
      onClear = _a.onClear,
      onSetToday = _a.onSetToday,
      okLabel = _a.okLabel,
      cancelLabel = _a.cancelLabel,
      clearLabel = _a.clearLabel,
      todayLabel = _a.todayLabel,
      clearable = _a.clearable,
      showTodayButton = _a.showTodayButton,
      showTabs = _a.showTabs,
      wider = _a.wider,
      other = __rest(_a, ["children", "classes", "onKeyDownInner", "onAccept", "onDismiss", "onClear", "onSetToday", "okLabel", "cancelLabel", "clearLabel", "todayLabel", "clearable", "showTodayButton", "showTabs", "wider"]);

  return createElement(Dialog, __assign({
    role: "dialog",
    onClose: onDismiss,
    classes: {
      paper: clsx(classes.dialogRoot, (_b = {}, _b[classes.dialogRootWider] = wider, _b[classes.dialogWithTabs] = showTabs, _b))
    }
  }, other), createElement(EventListener, {
    target: "window",
    onKeyDown: onKeyDownInner
  }), createElement(DialogContent, {
    children: children,
    className: clsx(classes.dialog, (_c = {}, _c[classes.dialogWithTabs] = showTabs, _c))
  }), createElement(DialogActions, {
    classes: {
      root: clsx((_d = {}, _d[classes.withAdditionalAction] = clearable || showTodayButton, _d))
    }
  }, clearable && createElement(Button, {
    color: "primary",
    onClick: onClear
  }, clearLabel), !clearable && showTodayButton && createElement(Button, {
    color: "primary",
    onClick: onSetToday
  }, todayLabel), cancelLabel && createElement(Button, {
    color: "primary",
    onClick: onDismiss
  }, cancelLabel), okLabel && createElement(Button, {
    color: "primary",
    onClick: onAccept
  }, okLabel)));
};
ModalDialog.displayName = 'ModalDialog';
var dialogHeight = 405;
var dialogHeightWithTabs = 455;
var styles$3 = createStyles({
  dialogRoot: {
    minWidth: DIALOG_WIDTH,
    minHeight: dialogHeight
  },
  dialogRootWider: {
    minWidth: DIALOG_WIDTH_WIDER
  },
  dialog: {
    minHeight: dialogHeight,
    overflow: 'hidden',
    '&:first-child': {
      padding: 0
    }
  },
  dialogWithTabs: {
    minHeight: dialogHeightWithTabs
  },
  withAdditionalAction: {
    // set justifyContent to default value to fix IE11 layout bug
    // see https://github.com/dmtrKovalenko/material-ui-pickers/pull/267
    justifyContent: 'flex-start',
    '& > *:first-child': {
      marginRight: 'auto'
    }
  }
});
var ModalDialog$1 = withStyles(styles$3, {
  name: 'MuiPickersModal'
})(ModalDialog);

var ModalWrapper = function (_a) {
  var open = _a.open,
      children = _a.children,
      okLabel = _a.okLabel,
      cancelLabel = _a.cancelLabel,
      clearLabel = _a.clearLabel,
      todayLabel = _a.todayLabel,
      showTodayButton = _a.showTodayButton,
      clearable = _a.clearable,
      DialogProps = _a.DialogProps,
      showTabs = _a.showTabs,
      wider = _a.wider,
      InputComponent = _a.InputComponent,
      DateInputProps = _a.DateInputProps,
      onClear = _a.onClear,
      onAccept = _a.onAccept,
      onDismiss = _a.onDismiss,
      onSetToday = _a.onSetToday,
      other = __rest(_a, ["open", "children", "okLabel", "cancelLabel", "clearLabel", "todayLabel", "showTodayButton", "clearable", "DialogProps", "showTabs", "wider", "InputComponent", "DateInputProps", "onClear", "onAccept", "onDismiss", "onSetToday"]);

  var handleKeyDown = useCallback(function (event) {
    switch (event.key) {
      case 'Enter':
        onAccept();
        break;

      default:
        return;
      // if key is not handled, stop execution
    } // if event was handled prevent other side effects


    event.preventDefault();
  }, [onAccept]);
  return createElement(Fragment, null, createElement(InputComponent, __assign({}, other, DateInputProps)), createElement(ModalDialog$1, __assign({
    wider: wider,
    showTabs: showTabs,
    open: open,
    onKeyDownInner: handleKeyDown,
    onClear: onClear,
    onAccept: onAccept,
    onDismiss: onDismiss,
    onSetToday: onSetToday,
    clearLabel: clearLabel,
    todayLabel: todayLabel,
    okLabel: okLabel,
    cancelLabel: cancelLabel,
    clearable: clearable,
    showTodayButton: showTodayButton,
    children: children
  }, DialogProps)));
};
process.env.NODE_ENV !== "production" ? ModalWrapper.propTypes = {
  okLabel: node,
  cancelLabel: node,
  clearLabel: node,
  clearable: bool,
  todayLabel: node,
  showTodayButton: bool,
  DialogProps: object
} : void 0;
ModalWrapper.defaultProps = {
  okLabel: 'OK',
  cancelLabel: 'Cancel',
  clearLabel: 'Clear',
  todayLabel: 'Today',
  clearable: false,
  showTodayButton: false
};

var useStyles$9 = makeStyles({
  popoverPaper: {
    width: DIALOG_WIDTH,
    paddingBottom: 8
  },
  popoverPaperWider: {
    width: DIALOG_WIDTH_WIDER
  }
}, {
  name: 'MuiPickersInlineWrapper'
});
var InlineWrapper = function (_a) {
  var _b;

  var open = _a.open,
      wider = _a.wider,
      children = _a.children,
      PopoverProps = _a.PopoverProps,
      onClear = _a.onClear,
      onDismiss = _a.onDismiss,
      onSetToday = _a.onSetToday,
      onAccept = _a.onAccept,
      showTabs = _a.showTabs,
      DateInputProps = _a.DateInputProps,
      InputComponent = _a.InputComponent,
      other = __rest(_a, ["open", "wider", "children", "PopoverProps", "onClear", "onDismiss", "onSetToday", "onAccept", "showTabs", "DateInputProps", "InputComponent"]);

  var ref = useRef();
  var classes = useStyles$9();
  var handleKeyDown = useCallback(function (event) {
    switch (event.key) {
      case 'Enter':
        onAccept();
        break;

      default:
        return;
      // if key is not handled, stop execution
    } // if event was handled prevent other side effects


    event.preventDefault();
  }, [onAccept]);
  return createElement(Fragment, null, open && createElement(EventListener, {
    target: "window",
    onKeyDown: handleKeyDown
  }), createElement(InputComponent, __assign({
    inputRef: ref
  }, other, DateInputProps)), createElement(Popover, __assign({
    open: open,
    onClose: onAccept,
    anchorEl: ref.current,
    classes: {
      paper: clsx(classes.popoverPaper, (_b = {}, _b[classes.popoverPaperWider] = wider, _b))
    },
    anchorOrigin: {
      vertical: 'bottom',
      horizontal: 'right'
    },
    transformOrigin: {
      vertical: 'top',
      horizontal: 'right'
    },
    children: children
  }, PopoverProps)));
};
process.env.NODE_ENV !== "production" ? InlineWrapper.propTypes = {
  onOpen: func,
  onClose: func,
  PopoverProps: object
} : void 0;

function getWrapperFromVariant(variant) {
  switch (variant) {
    case 'inline':
      return InlineWrapper;

    default:
      return ModalWrapper;
  }
}
var Wrapper = function (_a) {
  var variant = _a.variant,
      props = __rest(_a, ["variant"]);

  var Component = getWrapperFromVariant(variant);
  return React__default.createElement(Component, __assign({}, props));
};

var valueToDate = function (utils, _a) {
  var value = _a.value,
      initialFocusedDate = _a.initialFocusedDate;
  var initialDate = value || initialFocusedDate || utils.date();
  var date = utils.date(initialDate);
  return date && utils.isValid(date) ? date : utils.date();
};

function useDateValues(props, options) {
  var utils = useUtils();
  var date = valueToDate(utils, props);
  var acceptedDateRef = useRef(date);
  var format = props.format || options.getDefaultFormat();
  return {
    acceptedDateRef: acceptedDateRef,
    date: date,
    format: format
  };
}

function makeControlledOpenProps(props) {
  return {
    isOpen: props.open,
    setIsOpen: function (newIsOpen) {
      return newIsOpen ? props.onOpen && props.onOpen() : props.onClose && props.onClose();
    }
  };
}
/* eslint-disable react-hooks/rules-of-hooks */


function useOpenState(props) {
  if (props.open !== undefined && props.open !== null) {
    return makeControlledOpenProps(props);
  }

  var _a = useState(false),
      isOpen = _a[0],
      setIsOpenState = _a[1]; // prettier-ignore


  var setIsOpen = useCallback(function (newIsOpen) {
    setIsOpenState(newIsOpen);
    return newIsOpen ? props.onOpen && props.onOpen() : props.onClose && props.onClose();
  }, [props]);
  return {
    isOpen: isOpen,
    setIsOpen: setIsOpen
  };
}
/* eslint-enable react-hooks/rules-of-hooks */


function usePickerState(props, options) {
  var utils = useUtils();

  var _a = useOpenState(props),
      isOpen = _a.isOpen,
      setIsOpen = _a.setIsOpen;

  var _b = useDateValues(props, options),
      acceptedDateRef = _b.acceptedDateRef,
      date = _b.date,
      format = _b.format;

  useEffect(function () {
    if (!isOpen) {
      // if value was changed in closed state treat it as accepted
      acceptedDateRef.current = date;
    }
  }, [acceptedDateRef, date, isOpen, props.value]);
  var validationError = options.getValidationError();

  if (validationError && props.onError) {
    props.onError(validationError, props.value);
  }

  var inputProps = {
    validationError: validationError,
    onClick: function () {
      return setIsOpen(true);
    },
    inputValue: getDisplayDate(date, format, utils, props.value === null, props)
  }; // prettier-ignore

  var acceptDate = useCallback(function (acceptedDate) {
    acceptedDateRef.current = acceptedDate;
    setIsOpen(false);
    props.onChange(acceptedDate);

    if (props.onAccept) {
      props.onAccept(acceptedDate);
    }
  }, [acceptedDateRef, setIsOpen, props]);
  var wrapperProps = {
    format: format,
    open: isOpen,
    onAccept: function () {
      return acceptDate(date);
    },
    onClear: function () {
      return acceptDate(null);
    },
    onSetToday: useCallback(function () {
      return props.onChange(utils.date());
    }, [props, utils]),
    onDismiss: useCallback(function () {
      setIsOpen(false);
      props.onChange(acceptedDateRef.current);
    }, [setIsOpen, props, acceptedDateRef])
  };
  var pickerProps = {
    date: date,
    onChange: useCallback(function (newDate, isFinish) {
      if (isFinish === void 0) {
        isFinish = true;
      }

      props.onChange(newDate);

      if (isFinish && props.autoOk) {
        acceptDate(newDate);
      }
    }, [props, acceptDate])
  };
  var pickerState = {
    pickerProps: pickerProps,
    inputProps: inputProps,
    wrapperProps: wrapperProps
  };
  useDebugValue(pickerState);
  return pickerState;
}

function parseInputString(value, utils, format) {
  try {
    return utils.parse(value, format);
  } catch (_a) {
    return null;
  }
}

function useKeyboardPickerState(props, options) {
  var utils = useUtils();
  var format = props.format || options.getDefaultFormat();

  var _a = useState(getDisplayDate(props.value, format, utils, props.value === null, props)),
      innerInputValue = _a[0],
      setInnerInputValue = _a[1];

  var dateValue = props.inputValue ? parseInputString(props.inputValue, utils, format) : props.value;
  useEffect(function () {
    if (props.value === null || utils.isValid(props.value)) {
      setInnerInputValue(getDisplayDate(props.value, format, utils, props.value === null, props));
    }
  }, [format, props, props.value, utils]);

  function handleChange(date) {
    var dateString = utils.format(date, format);
    props.onChange(date, dateString);
  }

  var _b = usePickerState(__assign({}, props, {
    value: dateValue,
    onChange: handleChange
  }), options),
      innerInputProps = _b.inputProps,
      wrapperProps = _b.wrapperProps,
      pickerProps = _b.pickerProps;

  var inputProps = __assign({}, innerInputProps, {
    format: wrapperProps.format,
    inputValue: props.inputValue || innerInputValue,
    onChange: function (value) {
      setInnerInputValue(value);
      var date = value === '' ? null : utils.parse(value, wrapperProps.format);
      props.onChange(date, value);
    }
  });

  return {
    inputProps: inputProps,
    wrapperProps: wrapperProps,
    pickerProps: pickerProps
  };
}

function KeyboardDatePicker(props) {
  var allowKeyboardControl = props.allowKeyboardControl,
      animateYearScrolling = props.animateYearScrolling,
      autoOk = props.autoOk,
      disableFuture = props.disableFuture,
      disablePast = props.disablePast,
      format = props.format,
      forwardedRef = props.forwardedRef,
      initialFocusedDate = props.initialFocusedDate,
      invalidDateMessage = props.invalidDateMessage,
      labelFunc = props.labelFunc,
      leftArrowIcon = props.leftArrowIcon,
      leftArrowButtonProps = props.leftArrowButtonProps,
      maxDate = props.maxDate,
      maxDateMessage = props.maxDateMessage,
      minDate = props.minDate,
      minDateMessage = props.minDateMessage,
      onAccept = props.onAccept,
      onChange = props.onChange,
      onClose = props.onClose,
      onlyCalendar = props.onlyCalendar,
      onMonthChange = props.onMonthChange,
      onOpen = props.onOpen,
      onYearChange = props.onYearChange,
      openTo = props.openTo,
      renderDay = props.renderDay,
      rightArrowIcon = props.rightArrowIcon,
      rightArrowButtonProps = props.rightArrowButtonProps,
      shouldDisableDate = props.shouldDisableDate,
      value = props.value,
      variant = props.variant,
      views = props.views,
      other = __rest(props, ["allowKeyboardControl", "animateYearScrolling", "autoOk", "disableFuture", "disablePast", "format", "forwardedRef", "initialFocusedDate", "invalidDateMessage", "labelFunc", "leftArrowIcon", "leftArrowButtonProps", "maxDate", "maxDateMessage", "minDate", "minDateMessage", "onAccept", "onChange", "onClose", "onlyCalendar", "onMonthChange", "onOpen", "onYearChange", "openTo", "renderDay", "rightArrowIcon", "rightArrowButtonProps", "shouldDisableDate", "value", "variant", "views"]);

  var utils = useUtils();

  var _a = useKeyboardPickerState(props, {
    getDefaultFormat: function () {
      return getFormatByViews(views, utils);
    },
    getValidationError: function () {
      return getError(value, utils, props);
    }
  }),
      pickerProps = _a.pickerProps,
      inputProps = _a.inputProps,
      wrapperProps = _a.wrapperProps;

  return createElement(Wrapper, __assign({
    variant: variant,
    InputComponent: KeyboardDateInput,
    DateInputProps: inputProps
  }, wrapperProps, other), createElement(DatePickerRoot, __assign({}, pickerProps, {
    allowKeyboardControl: allowKeyboardControl,
    animateYearScrolling: animateYearScrolling,
    disableFuture: disableFuture,
    disablePast: disablePast,
    leftArrowIcon: leftArrowIcon,
    leftArrowButtonProps: leftArrowButtonProps,
    maxDate: maxDate,
    minDate: minDate,
    onlyCalendar: onlyCalendar,
    openTo: openTo,
    renderDay: renderDay,
    rightArrowIcon: rightArrowIcon,
    rightArrowButtonProps: rightArrowButtonProps,
    shouldDisableDate: shouldDisableDate,
    views: views
  })));
}
KeyboardDatePicker.defaultProps = datePickerDefaultProps;
var KeyboardDatePicker$1 = forwardRef(function (props, ref) {
  return createElement(KeyboardDatePicker, __assign({}, props, {
    forwardedRef: ref
  }));
});

var PureDateInput = function (_a) {
  var inputValue = _a.inputValue,
      inputVariant = _a.inputVariant,
      validationError = _a.validationError,
      InputProps = _a.InputProps,
      other = __rest(_a, ["inputValue", "inputVariant", "validationError", "InputProps"]);

  var PureDateInputProps = useMemo(function () {
    return __assign({}, InputProps, {
      readOnly: true
    });
  }, [InputProps]);
  return createElement(TextField, __assign({
    error: Boolean(validationError),
    helperText: validationError
  }, other, {
    // do not overridable
    value: inputValue,
    variant: inputVariant,
    InputProps: PureDateInputProps
  }));
};
PureDateInput.displayName = 'PureDateInput';

var DatePicker = function (props) {
  var allowKeyboardControl = props.allowKeyboardControl,
      animateYearScrolling = props.animateYearScrolling,
      autoOk = props.autoOk,
      disableFuture = props.disableFuture,
      disablePast = props.disablePast,
      format = props.format,
      forwardedRef = props.forwardedRef,
      initialFocusedDate = props.initialFocusedDate,
      invalidDateMessage = props.invalidDateMessage,
      labelFunc = props.labelFunc,
      leftArrowIcon = props.leftArrowIcon,
      leftArrowButtonProps = props.leftArrowButtonProps,
      maxDate = props.maxDate,
      maxDateMessage = props.maxDateMessage,
      minDate = props.minDate,
      minDateMessage = props.minDateMessage,
      onAccept = props.onAccept,
      onChange = props.onChange,
      onMonthChange = props.onMonthChange,
      onYearChange = props.onYearChange,
      onOpen = props.onOpen,
      onClose = props.onClose,
      openTo = props.openTo,
      renderDay = props.renderDay,
      rightArrowIcon = props.rightArrowIcon,
      rightArrowButtonProps = props.rightArrowButtonProps,
      shouldDisableDate = props.shouldDisableDate,
      value = props.value,
      variant = props.variant,
      onlyCalendar = props.onlyCalendar,
      views = props.views,
      other = __rest(props, ["allowKeyboardControl", "animateYearScrolling", "autoOk", "disableFuture", "disablePast", "format", "forwardedRef", "initialFocusedDate", "invalidDateMessage", "labelFunc", "leftArrowIcon", "leftArrowButtonProps", "maxDate", "maxDateMessage", "minDate", "minDateMessage", "onAccept", "onChange", "onMonthChange", "onYearChange", "onOpen", "onClose", "openTo", "renderDay", "rightArrowIcon", "rightArrowButtonProps", "shouldDisableDate", "value", "variant", "onlyCalendar", "views"]);

  var utils = useUtils();

  var _a = usePickerState(props, {
    getDefaultFormat: function () {
      return getFormatByViews(views, utils);
    },
    getValidationError: function () {
      return getError(value, utils, props);
    }
  }),
      pickerProps = _a.pickerProps,
      inputProps = _a.inputProps,
      wrapperProps = _a.wrapperProps;

  return createElement(Wrapper, __assign({
    variant: variant,
    InputComponent: PureDateInput,
    DateInputProps: inputProps
  }, wrapperProps, other), createElement(DatePickerRoot, __assign({}, pickerProps, {
    onlyCalendar: onlyCalendar,
    allowKeyboardControl: allowKeyboardControl,
    animateYearScrolling: animateYearScrolling,
    disableFuture: disableFuture,
    disablePast: disablePast,
    leftArrowIcon: leftArrowIcon,
    leftArrowButtonProps: leftArrowButtonProps,
    maxDate: maxDate,
    minDate: minDate,
    renderDay: renderDay,
    rightArrowIcon: rightArrowIcon,
    rightArrowButtonProps: rightArrowButtonProps,
    shouldDisableDate: shouldDisableDate,
    views: views,
    openTo: openTo
  })));
};
DatePicker.defaultProps = datePickerDefaultProps;
var DatePicker$1 = forwardRef(function (props, ref) {
  return createElement(DatePicker, __assign({}, props, {
    forwardedRef: ref
  }));
});

var ClockType;

(function (ClockType) {
  ClockType["HOURS"] = "hours";
  ClockType["MINUTES"] = "minutes";
  ClockType["SECONDS"] = "seconds";
})(ClockType || (ClockType = {}));

var ClockType$1 = ClockType;

var ClockPointer =
/*@__PURE__*/
function (_super) {
  __extends(ClockPointer, _super);

  function ClockPointer() {
    var _this = _super !== null && _super.apply(this, arguments) || this;

    _this.state = {
      toAnimateTransform: false,
      previousType: undefined
    };

    _this.getAngleStyle = function () {
      var _a = _this.props,
          value = _a.value,
          isInner = _a.isInner,
          type = _a.type;
      var max = type === ClockType$1.HOURS ? 12 : 60;
      var angle = 360 / max * value;

      if (type === ClockType$1.HOURS && value > 12) {
        angle -= 360; // round up angle to max 360 degrees
      }

      return {
        height: isInner ? '26%' : '40%',
        transform: "rotateZ(" + angle + "deg)"
      };
    };

    return _this;
  }

  ClockPointer.prototype.render = function () {
    var _a, _b;

    var _c = this.props,
        classes = _c.classes,
        hasSelected = _c.hasSelected;
    return createElement("div", {
      style: this.getAngleStyle(),
      className: clsx(classes.pointer, (_a = {}, _a[classes.animateTransform] = this.state.toAnimateTransform, _a))
    }, createElement("div", {
      className: clsx(classes.thumb, (_b = {}, _b[classes.noPoint] = hasSelected, _b))
    }));
  };

  ClockPointer.getDerivedStateFromProps = function (nextProps, state) {
    if (nextProps.type !== state.previousType) {
      return {
        toAnimateTransform: true,
        previousType: nextProps.type
      };
    }

    return {
      toAnimateTransform: false,
      previousType: nextProps.type
    };
  };

  return ClockPointer;
}(Component);
var styles$4 = function (theme) {
  return createStyles({
    pointer: {
      width: 2,
      backgroundColor: theme.palette.primary.main,
      position: 'absolute',
      left: 'calc(50% - 1px)',
      bottom: '50%',
      transformOrigin: 'center bottom 0px'
    },
    animateTransform: {
      transition: theme.transitions.create(['transform', 'height'])
    },
    thumb: {
      width: 4,
      height: 4,
      backgroundColor: theme.palette.primary.contrastText,
      borderRadius: '100%',
      position: 'absolute',
      top: -21,
      left: -15,
      border: "14px solid " + theme.palette.primary.main,
      boxSizing: 'content-box'
    },
    noPoint: {
      backgroundColor: theme.palette.primary.main
    }
  });
};
var ClockPointer$1 = withStyles(styles$4, {
  name: 'MuiPickersClockPointer'
})(ClockPointer);

var center = {
  x: 260 / 2,
  y: 260 / 2
};
var basePoint = {
  x: center.x,
  y: 0
};
var cx = basePoint.x - center.x;
var cy = basePoint.y - center.y;

var rad2deg = function (rad) {
  return rad * 57.29577951308232;
};

var getAngleValue = function (step, offsetX, offsetY) {
  var x = offsetX - center.x;
  var y = offsetY - center.y;
  var atan = Math.atan2(cx, cy) - Math.atan2(x, y);
  var deg = rad2deg(atan);
  deg = Math.round(deg / step) * step;
  deg %= 360;
  var value = Math.floor(deg / step) || 0;
  var delta = Math.pow(x, 2) + Math.pow(y, 2);
  var distance = Math.sqrt(delta);
  return {
    value: value,
    distance: distance
  };
};

var getHours = function (offsetX, offsetY, ampm) {
  // tslint:disable-next-line
  var _a = getAngleValue(30, offsetX, offsetY),
      value = _a.value,
      distance = _a.distance;

  value = value || 12;

  if (!ampm) {
    if (distance < 90) {
      value += 12;
      value %= 24;
    }
  } else {
    value %= 12;
  }

  return value;
};
var getMinutes = function (offsetX, offsetY, step) {
  if (step === void 0) {
    step = 1;
  }

  var angleStep = step * 6;
  var value = getAngleValue(angleStep, offsetX, offsetY).value;
  value = value * step % 60;
  return value;
};
var convertToMeridiem = function (time, meridiem, ampm, utils) {
  if (ampm) {
    var currentMeridiem = utils.getHours(time) >= 12 ? 'pm' : 'am';

    if (currentMeridiem !== meridiem) {
      var hours = meridiem === 'am' ? utils.getHours(time) - 12 : utils.getHours(time) + 12;
      return utils.setHours(time, hours);
    }
  }

  return time;
};

var Clock =
/*@__PURE__*/
function (_super) {
  __extends(Clock, _super);

  function Clock() {
    var _this = _super !== null && _super.apply(this, arguments) || this;

    _this.isMoving = false;

    _this.handleTouchMove = function (e) {
      _this.isMoving = true;

      _this.setTime(e);
    };

    _this.handleTouchEnd = function (e) {
      if (_this.isMoving) {
        _this.setTime(e, true);

        _this.isMoving = false;
      }
    };

    _this.handleMove = function (e) {
      e.preventDefault();
      e.stopPropagation(); // MouseEvent.which is deprecated, but MouseEvent.buttons is not supported in Safari

      var isButtonPressed = typeof e.buttons === 'undefined' ? e.nativeEvent.which === 1 : e.buttons === 1;

      if (isButtonPressed) {
        _this.setTime(e.nativeEvent, false);
      }
    };

    _this.handleMouseUp = function (e) {
      if (_this.isMoving) {
        _this.isMoving = false;
      }

      _this.setTime(e.nativeEvent, true);
    };

    _this.hasSelected = function () {
      var _a = _this.props,
          type = _a.type,
          value = _a.value;

      if (type === ClockType$1.HOURS) {
        return true;
      }

      return value % 5 === 0;
    };

    return _this;
  }

  Clock.prototype.setTime = function (e, isFinish) {
    if (isFinish === void 0) {
      isFinish = false;
    }

    var offsetX = e.offsetX,
        offsetY = e.offsetY;

    if (typeof offsetX === 'undefined') {
      var rect = e.target.getBoundingClientRect();
      offsetX = e.changedTouches[0].clientX - rect.left;
      offsetY = e.changedTouches[0].clientY - rect.top;
    }

    var value = this.props.type === ClockType$1.SECONDS || this.props.type === ClockType$1.MINUTES ? getMinutes(offsetX, offsetY, this.props.minutesStep) : getHours(offsetX, offsetY, Boolean(this.props.ampm));
    this.props.onChange(value, isFinish);
  };

  Clock.prototype.render = function () {
    var _a = this.props,
        classes = _a.classes,
        value = _a.value,
        children = _a.children,
        type = _a.type,
        ampm = _a.ampm;
    var isPointerInner = !ampm && type === ClockType$1.HOURS && (value < 1 || value > 12);
    return createElement("div", {
      className: classes.container
    }, createElement("div", {
      className: classes.clock
    }, createElement("div", {
      role: "menu",
      tabIndex: -1,
      className: classes.squareMask,
      onTouchMove: this.handleTouchMove,
      onTouchEnd: this.handleTouchEnd,
      onMouseUp: this.handleMouseUp,
      onMouseMove: this.handleMove
    }), createElement("div", {
      className: classes.pin
    }), createElement(ClockPointer$1, {
      type: type,
      value: value,
      isInner: isPointerInner,
      hasSelected: this.hasSelected()
    }), children));
  };

  process.env.NODE_ENV !== "production" ? Clock.propTypes = {
    type: oneOf(Object.keys(ClockType$1).map(function (key) {
      return ClockType$1[key];
    })).isRequired,
    value: number.isRequired,
    onChange: func.isRequired,
    children: arrayOf(node).isRequired,
    ampm: bool,
    minutesStep: number,
    innerRef: any
  } : void 0;
  Clock.defaultProps = {
    ampm: false,
    minutesStep: 1
  };
  return Clock;
}(Component);
var styles$5 = function (theme) {
  return createStyles({
    container: {
      display: 'flex',
      justifyContent: 'center',
      alignItems: 'flex-end',
      margin: theme.spacing(4) + "px 0 " + theme.spacing(1) + "px"
    },
    clock: {
      backgroundColor: 'rgba(0,0,0,.07)',
      borderRadius: '50%',
      height: 260,
      width: 260,
      position: 'relative',
      pointerEvents: 'none',
      zIndex: 1
    },
    squareMask: {
      width: '100%',
      height: '100%',
      position: 'absolute',
      pointerEvents: 'auto',
      outline: 'none',
      touchActions: 'none',
      userSelect: 'none',
      '&:active': {
        cursor: 'move'
      }
    },
    pin: {
      width: 6,
      height: 6,
      borderRadius: '50%',
      backgroundColor: theme.palette.primary.main,
      position: 'absolute',
      top: '50%',
      left: '50%',
      transform: 'translate(-50%, -50%)'
    }
  });
};
var Clock$1 = withStyles(styles$5, {
  name: 'MuiPickersClock'
})(Clock);

var positions = {
  0: [0, 40],
  1: [55, 19.6],
  2: [94.4, 59.5],
  3: [109, 114],
  4: [94.4, 168.5],
  5: [54.5, 208.4],
  6: [0, 223],
  7: [-54.5, 208.4],
  8: [-94.4, 168.5],
  9: [-109, 114],
  10: [-94.4, 59.5],
  11: [-54.5, 19.6],
  12: [0, 5],
  13: [36.9, 49.9],
  14: [64, 77],
  15: [74, 114],
  16: [64, 151],
  17: [37, 178],
  18: [0, 188],
  19: [-37, 178],
  20: [-64, 151],
  21: [-74, 114],
  22: [-64, 77],
  23: [-37, 50]
};
var useStyles$a = makeStyles(function (theme) {
  var size = theme.spacing(4);
  return {
    clockNumber: {
      width: size,
      height: size,
      userSelect: 'none',
      position: 'absolute',
      left: "calc(50% - " + size / 2 + "px)",
      display: 'inline-flex',
      justifyContent: 'center',
      alignItems: 'center',
      borderRadius: '50%',
      color: theme.palette.type === 'light' ? theme.palette.text.primary : theme.palette.text.hint
    },
    selected: {
      color: theme.palette.primary.contrastText
    }
  };
}, {
  name: 'MuiPickersClockNumber'
});
var ClockNumber = function (_a) {
  var _b;

  var selected = _a.selected,
      label = _a.label,
      index = _a.index,
      isInner = _a.isInner;
  var classes = useStyles$a();
  var className = clsx(classes.clockNumber, (_b = {}, _b[classes.selected] = selected, _b));
  var transformStyle = useMemo(function () {
    var position = positions[index];
    return {
      transform: "translate(" + position[0] + "px, " + position[1] + "px"
    };
  }, [index]);
  return createElement(Typography, {
    component: "span",
    className: className,
    variant: isInner ? 'body2' : 'body1',
    style: transformStyle,
    children: label
  });
};

var getHourNumbers = function (_a) {
  var ampm = _a.ampm,
      utils = _a.utils,
      date = _a.date;
  var currentHours = utils.getHours(date);
  var hourNumbers = [];
  var startHour = ampm ? 1 : 0;
  var endHour = ampm ? 12 : 23;

  var isSelected = function (hour) {
    if (ampm) {
      if (hour === 12) {
        return currentHours === 12 || currentHours === 0;
      }

      return currentHours === hour || currentHours - 12 === hour;
    }

    return currentHours === hour;
  };

  for (var hour = startHour; hour <= endHour; hour += 1) {
    var label = hour.toString();

    if (hour === 0) {
      label = '00';
    }

    var props = {
      index: hour,
      label: utils.formatNumber(label),
      selected: isSelected(hour),
      isInner: !ampm && (hour === 0 || hour > 12)
    };
    hourNumbers.push(createElement(ClockNumber, __assign({
      key: hour
    }, props)));
  }

  return hourNumbers;
};
var getMinutesNumbers = function (_a) {
  var value = _a.value,
      utils = _a.utils;
  var f = utils.formatNumber;
  return [createElement(ClockNumber, {
    label: f('00'),
    selected: value === 0,
    index: 12,
    key: 12
  }), createElement(ClockNumber, {
    label: f('05'),
    selected: value === 5,
    index: 1,
    key: 1
  }), createElement(ClockNumber, {
    label: f('10'),
    selected: value === 10,
    index: 2,
    key: 2
  }), createElement(ClockNumber, {
    label: f('15'),
    selected: value === 15,
    index: 3,
    key: 3
  }), createElement(ClockNumber, {
    label: f('20'),
    selected: value === 20,
    index: 4,
    key: 4
  }), createElement(ClockNumber, {
    label: f('25'),
    selected: value === 25,
    index: 5,
    key: 5
  }), createElement(ClockNumber, {
    label: f('30'),
    selected: value === 30,
    index: 6,
    key: 6
  }), createElement(ClockNumber, {
    label: f('35'),
    selected: value === 35,
    index: 7,
    key: 7
  }), createElement(ClockNumber, {
    label: f('40'),
    selected: value === 40,
    index: 8,
    key: 8
  }), createElement(ClockNumber, {
    label: f('45'),
    selected: value === 45,
    index: 9,
    key: 9
  }), createElement(ClockNumber, {
    label: f('50'),
    selected: value === 50,
    index: 10,
    key: 10
  }), createElement(ClockNumber, {
    label: f('55'),
    selected: value === 55,
    index: 11,
    key: 11
  })];
};

var TimePickerView = function (_a) {
  var type = _a.type,
      onHourChange = _a.onHourChange,
      onMinutesChange = _a.onMinutesChange,
      onSecondsChange = _a.onSecondsChange,
      ampm = _a.ampm,
      date = _a.date,
      minutesStep = _a.minutesStep;
  var utils = useUtils();

  var getViewProps = function () {
    switch (type) {
      case ClockType$1.HOURS:
        return {
          value: utils.getHours(date),
          children: getHourNumbers({
            date: date,
            utils: utils,
            ampm: Boolean(ampm)
          }),
          onChange: function (value, isFinish) {
            var updatedTime = utils.setHours(date, value);
            onHourChange(updatedTime, isFinish);
          }
        };

      case ClockType$1.MINUTES:
        var minutesValue = utils.getMinutes(date);
        return {
          value: minutesValue,
          children: getMinutesNumbers({
            value: minutesValue,
            utils: utils
          }),
          onChange: function (value, isFinish) {
            var updatedTime = utils.setMinutes(date, value);
            onMinutesChange(updatedTime, isFinish);
          }
        };

      case ClockType$1.SECONDS:
        var secondsValue = utils.getSeconds(date);
        return {
          value: secondsValue,
          children: getMinutesNumbers({
            value: secondsValue,
            utils: utils
          }),
          onChange: function (value, isFinish) {
            var updatedTime = utils.setSeconds(date, value);
            onSecondsChange(updatedTime, isFinish);
          }
        };

      default:
        throw new Error('You must provide the type for TimePickerView');
    }
  };

  var viewProps = useMemo(getViewProps, [date, type, utils]);
  return createElement(Clock$1, __assign({
    type: type,
    ampm: ampm,
    minutesStep: minutesStep
  }, viewProps));
};
TimePickerView.displayName = 'TimePickerView';
process.env.NODE_ENV !== "production" ? TimePickerView.propTypes = {
  date: object.isRequired,
  onHourChange: func.isRequired,
  onMinutesChange: func.isRequired,
  onSecondsChange: func.isRequired,
  ampm: bool,
  minutesStep: number,
  type: oneOf(Object.keys(ClockType$1).map(function (key) {
    return ClockType$1[key];
  })).isRequired
} : void 0;
TimePickerView.defaultProps = {
  ampm: true,
  minutesStep: 1
};
var TimePickerView$1 = memo(TimePickerView);

var useStyles$b = makeStyles({
  toolbar: {
    flexDirection: 'row',
    alignItems: 'center'
  },
  toolbarLeftPadding: {
    paddingLeft: 50
  },
  separator: {
    margin: '0 4px 0 2px',
    cursor: 'default'
  },
  ampmSelection: {
    marginLeft: 20,
    marginRight: -20,
    display: 'flex',
    flexDirection: 'column'
  },
  ampmSelectionWithSeconds: {
    marginLeft: 15,
    marginRight: 10
  },
  ampmLabel: {
    fontSize: 18
  },
  hourMinuteLabel: {
    display: 'flex',
    justifyContent: 'flex-end',
    alignItems: 'flex-end'
  },
  hourMinuteLabelReverse: {
    display: 'flex',
    justifyContent: 'flex-end',
    alignItems: 'flex-end',
    flexDirection: 'row-reverse'
  }
}, {
  name: 'MuiPickersTimePicker'
});
function useMeridiemMode(date, ampm, onChange) {
  var utils = useUtils();
  var meridiemMode = utils.getHours(date) >= 12 ? 'pm' : 'am';
  var handleMeridiemChange = useCallback(function (mode) {
    var timeWithMeridiem = convertToMeridiem(date, mode, Boolean(ampm), utils);
    onChange(timeWithMeridiem, false);
  }, [ampm, date, onChange, utils]);
  return {
    meridiemMode: meridiemMode,
    handleMeridiemChange: handleMeridiemChange
  };
}

var TimePickerRoot = function (_a) {
  var _b, _c;

  var date = _a.date,
      ampm = _a.ampm,
      onChange = _a.onChange,
      seconds = _a.seconds,
      minutesStep = _a.minutesStep;
  var utils = useUtils();
  var classes = useStyles$b();
  var theme = useTheme();

  var _d = useState(ClockType$1.HOURS),
      openView = _d[0],
      setOpenView = _d[1];

  var _e = useMeridiemMode(date, ampm, onChange),
      meridiemMode = _e.meridiemMode,
      handleMeridiemChange = _e.handleMeridiemChange;

  var hourMinuteClassName = theme.direction === 'rtl' ? classes.hourMinuteLabelReverse : classes.hourMinuteLabel;
  var handleChangeAndOpenNext = useCallback(function (nextView) {
    return function (time, isFinish) {
      var timeWithMeridiem = convertToMeridiem(time, meridiemMode, Boolean(ampm), utils);

      if (isFinish && nextView) {
        // do not close picker if needs to show next view
        onChange(timeWithMeridiem, false);
        setOpenView(nextView);
        return;
      }

      onChange(timeWithMeridiem, isFinish);
    };
  }, [ampm, meridiemMode, onChange, utils]);
  return createElement(Fragment, null, createElement(PickerToolbar$1, {
    className: clsx(classes.toolbar, (_b = {}, _b[classes.toolbarLeftPadding] = ampm, _b))
  }, createElement("div", {
    className: hourMinuteClassName
  }, createElement(ToolbarButton$1, {
    variant: "h2",
    onClick: function () {
      return setOpenView(ClockType$1.HOURS);
    },
    selected: openView === ClockType$1.HOURS,
    label: utils.getHourText(date, Boolean(ampm))
  }), createElement(ToolbarText, {
    variant: "h2",
    label: ":",
    selected: false,
    className: classes.separator
  }), createElement(ToolbarButton$1, {
    variant: "h2",
    onClick: function () {
      return setOpenView(ClockType$1.MINUTES);
    },
    selected: openView === ClockType$1.MINUTES,
    label: utils.getMinuteText(date)
  }), seconds && createElement(Fragment, null, createElement(ToolbarText, {
    variant: "h2",
    label: ":",
    selected: false,
    className: classes.separator
  }), createElement(ToolbarButton$1, {
    variant: "h2",
    onClick: function () {
      return setOpenView(ClockType$1.SECONDS);
    },
    selected: openView === ClockType$1.SECONDS,
    label: utils.getSecondText(date)
  }))), ampm && createElement("div", {
    className: clsx(classes.ampmSelection, (_c = {}, _c[classes.ampmSelectionWithSeconds] = seconds, _c))
  }, createElement(ToolbarButton$1, {
    disableRipple: true,
    variant: "subtitle1",
    selected: meridiemMode === 'am',
    typographyClassName: classes.ampmLabel,
    label: utils.getMeridiemText('am'),
    onClick: function () {
      return handleMeridiemChange('am');
    }
  }), createElement(ToolbarButton$1, {
    disableRipple: true,
    variant: "subtitle1",
    selected: meridiemMode === 'pm',
    typographyClassName: classes.ampmLabel,
    label: utils.getMeridiemText('pm'),
    onClick: function () {
      return handleMeridiemChange('pm');
    }
  }))), createElement(TimePickerView$1, {
    ampm: ampm,
    date: date,
    type: openView,
    minutesStep: minutesStep,
    onHourChange: handleChangeAndOpenNext(ClockType$1.MINUTES),
    onMinutesChange: handleChangeAndOpenNext(seconds ? ClockType$1.SECONDS : null),
    onSecondsChange: handleChangeAndOpenNext(null)
  }));
};

process.env.NODE_ENV !== "production" ? TimePickerRoot.propTypes = {
  ampm: bool,
  seconds: bool,
  minutesStep: number
} : void 0;
TimePickerRoot.defaultProps = {
  ampm: true,
  seconds: false,
  minutesStep: 1
};

function KeyboardTimePicker(props) {
  var ampm = props.ampm,
      seconds = props.seconds,
      minutesStep = props.minutesStep,
      variant = props.variant,
      format = props.format,
      forwardedRef = props.forwardedRef,
      initialFocusedDate = props.initialFocusedDate,
      invalidDateMessage = props.invalidDateMessage,
      labelFunc = props.labelFunc,
      onAccept = props.onAccept,
      onChange = props.onChange,
      value = props.value,
      other = __rest(props, ["ampm", "seconds", "minutesStep", "variant", "format", "forwardedRef", "initialFocusedDate", "invalidDateMessage", "labelFunc", "onAccept", "onChange", "value"]);

  var utils = useUtils();

  var _a = useKeyboardPickerState(props, {
    getValidationError: function () {
      return getError(value, utils, props);
    },
    getDefaultFormat: function () {
      return pick12hOr24hFormat(format, ampm, {
        '12h': utils.time12hFormat,
        '24h': utils.time24hFormat
      });
    }
  }),
      pickerProps = _a.pickerProps,
      inputProps = _a.inputProps,
      wrapperProps = _a.wrapperProps;

  return createElement(Wrapper, __assign({
    variant: variant,
    InputComponent: KeyboardDateInput,
    DateInputProps: inputProps
  }, wrapperProps, other), createElement(TimePickerRoot, __assign({}, pickerProps, {
    ampm: ampm,
    seconds: seconds,
    minutesStep: minutesStep
  })));
}
KeyboardTimePicker.defaultProps = timePickerDefaultProps;
var KeyboardTimePicker$1 = forwardRef(function (props, ref) {
  return createElement(KeyboardTimePicker, __assign({}, props, {
    forwardedRef: ref
  }));
});

var TimePicker = function (props) {
  var ampm = props.ampm,
      seconds = props.seconds,
      minutesStep = props.minutesStep,
      autoOk = props.autoOk,
      format = props.format,
      forwardedRef = props.forwardedRef,
      initialFocusedDate = props.initialFocusedDate,
      labelFunc = props.labelFunc,
      invalidDateMessage = props.invalidDateMessage,
      onAccept = props.onAccept,
      onChange = props.onChange,
      value = props.value,
      variant = props.variant,
      other = __rest(props, ["ampm", "seconds", "minutesStep", "autoOk", "format", "forwardedRef", "initialFocusedDate", "labelFunc", "invalidDateMessage", "onAccept", "onChange", "value", "variant"]);

  var utils = useUtils();

  var _a = usePickerState(props, {
    getValidationError: function () {
      return getError(value, utils, props);
    },
    getDefaultFormat: function () {
      return pick12hOr24hFormat(format, ampm, {
        '12h': utils.time12hFormat,
        '24h': utils.time24hFormat
      });
    }
  }),
      pickerProps = _a.pickerProps,
      inputProps = _a.inputProps,
      wrapperProps = _a.wrapperProps;

  return createElement(Wrapper, __assign({
    variant: variant,
    InputComponent: PureDateInput,
    DateInputProps: inputProps
  }, wrapperProps, other), createElement(TimePickerRoot, __assign({}, pickerProps, {
    ampm: ampm,
    seconds: seconds,
    minutesStep: minutesStep
  })));
};
TimePicker.defaultProps = timePickerDefaultProps;
var TimePicker$1 = forwardRef(function (props, ref) {
  return createElement(TimePicker, __assign({}, props, {
    forwardedRef: ref
  }));
});

var DateTimePickerView = function (_a) {
  var selected = _a.selected,
      children = _a.children;

  if (!selected) {
    return null;
  }

  return createElement("div", {
    children: children
  });
};

var TimeIcon = function (props) {
  return React__default.createElement(SvgIcon, __assign({}, props), React__default.createElement("path", {
    d: "M11.99 2C6.47 2 2 6.48 2 12s4.47 10 9.99 10C17.52 22 22 17.52 22 12S17.52 2 11.99 2zM12 20c-4.42 0-8-3.58-8-8s3.58-8 8-8 8 3.58 8 8-3.58 8-8 8z"
  }), React__default.createElement("path", {
    fill: "none",
    d: "M0 0h24v24H0z"
  }), React__default.createElement("path", {
    d: "M12.5 7H11v6l5.25 3.15.75-1.23-4.5-2.67z"
  }));
};

var DateRangeIcon = function (props) {
  return React__default.createElement(SvgIcon, __assign({}, props), React__default.createElement("path", {
    d: "M9 11H7v2h2v-2zm4 0h-2v2h2v-2zm4 0h-2v2h2v-2zm2-7h-1V2h-2v2H8V2H6v2H5c-1.11 0-1.99.9-1.99 2L3 20c0 1.1.89 2 2 2h14c1.1 0 2-.9 2-2V6c0-1.1-.9-2-2-2zm0 16H5V9h14v11z"
  }), React__default.createElement("path", {
    fill: "none",
    d: "M0 0h24v24H0z"
  }));
};

var viewToTabIndex = function (openView) {
  if (openView === 'date' || openView === 'year') {
    return 'date';
  }

  return 'time';
};

var tabIndexToView = function (tab) {
  if (tab === 'date') {
    return 'date';
  }

  return 'hours';
};

var useStyles$c = makeStyles(function (theme) {
  // prettier-ignore
  var tabsBackground = theme.palette.type === 'light' ? theme.palette.primary.main : theme.palette.background.default;
  return {
    tabs: {
      color: theme.palette.getContrastText(tabsBackground),
      backgroundColor: tabsBackground
    }
  };
}, {
  name: 'MuiPickerDTTabs'
});
var DateTimePickerTabs = function (_a) {
  var view = _a.view,
      onChange = _a.onChange,
      dateRangeIcon = _a.dateRangeIcon,
      timeIcon = _a.timeIcon;
  var classes = useStyles$c();
  var theme = useTheme();
  var indicatorColor = theme.palette.type === 'light' ? 'secondary' : 'primary';

  var handleChange = function (e, value) {
    if (value !== viewToTabIndex(view)) {
      onChange(tabIndexToView(value));
    }
  };

  return createElement(Paper, null, createElement(Tabs, {
    variant: "fullWidth",
    value: viewToTabIndex(view),
    onChange: handleChange,
    className: classes.tabs,
    indicatorColor: indicatorColor
  }, createElement(Tab, {
    value: "date",
    icon: createElement(Fragment, null, dateRangeIcon)
  }), createElement(Tab, {
    value: "time",
    icon: createElement(Fragment, null, timeIcon)
  })));
};
process.env.NODE_ENV !== "production" ? DateTimePickerTabs.propTypes = {
  view: string.isRequired,
  dateRangeIcon: node.isRequired,
  timeIcon: node.isRequired
} : void 0;
DateTimePickerTabs.defaultProps = {
  dateRangeIcon: createElement(DateRangeIcon, null),
  timeIcon: createElement(TimeIcon, null)
};

var useStyles$d = makeStyles(function (theme) {
  return {
    toolbar: {
      flexDirection: 'row',
      alignItems: 'center',
      paddingLeft: 16,
      paddingRight: 16,
      justifyContent: 'space-around'
    },
    toolBar24h: {
      paddingLeft: 32
    },
    separator: {
      margin: '0 4px 0 2px',
      cursor: 'default'
    },
    hourMinuteLabel: {
      top: 10,
      position: 'relative',
      display: 'flex',
      justifyContent: 'flex-end',
      alignItems: 'flex-end',
      flexDirection: theme.direction === 'rtl' ? 'row-reverse' : 'row'
    },
    dateHeader: {
      height: 60,
      minWidth: 110,
      marginRight: 4,
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'flex-start',
      justifyContent: 'flex-start'
    },
    timeHeader: {
      height: 65,
      minWidth: 155,
      display: 'flex',
      justifyContent: 'center',
      alignItems: 'flex-end'
    },
    ampmSelection: {
      top: 11,
      position: 'relative',
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'center',
      marginLeft: 10,
      marginRight: -10
    },
    ampmLabel: {
      fontSize: 18
    }
  };
}, {
  name: 'MuiPickerDTHeader'
});
var DateTimePickerHeader = function (_a) {
  var _b;

  var date = _a.date,
      openView = _a.openView,
      meridiemMode = _a.meridiemMode,
      onOpenViewChange = _a.onOpenViewChange,
      setMeridiemMode = _a.setMeridiemMode,
      ampm = _a.ampm;
  var utils = useUtils();
  var classes = useStyles$d();
  return createElement(PickerToolbar$1, {
    className: clsx(classes.toolbar, (_b = {}, _b[classes.toolBar24h] = !ampm, _b))
  }, createElement("div", {
    className: classes.dateHeader
  }, createElement(ToolbarButton$1, {
    variant: "subtitle1",
    onClick: function () {
      return onOpenViewChange('year');
    },
    selected: openView === 'year',
    label: utils.getYearText(date)
  }), createElement(ToolbarButton$1, {
    variant: "h4",
    onClick: function () {
      return onOpenViewChange('date');
    },
    selected: openView === 'date',
    label: utils.getDateTimePickerHeaderText(date)
  })), createElement("div", {
    className: classes.timeHeader
  }, createElement("div", {
    className: classes.hourMinuteLabel
  }, createElement(ToolbarButton$1, {
    variant: "h3",
    onClick: function () {
      return onOpenViewChange('hours');
    },
    selected: openView === 'hours',
    label: utils.getHourText(date, ampm)
  }), createElement(ToolbarText, {
    variant: "h3",
    label: ":",
    selected: false,
    className: classes.separator
  }), createElement(ToolbarButton$1, {
    variant: "h3",
    onClick: function () {
      return onOpenViewChange('minutes');
    },
    selected: openView === 'minutes',
    label: utils.getMinuteText(date)
  })), ampm && createElement("div", {
    className: classes.ampmSelection
  }, createElement(ToolbarButton$1, {
    variant: "subtitle1",
    typographyClassName: classes.ampmLabel,
    selected: meridiemMode === 'am',
    label: utils.getMeridiemText('am'),
    onClick: function () {
      return setMeridiemMode('am');
    }
  }), createElement(ToolbarButton$1, {
    variant: "subtitle1",
    typographyClassName: classes.ampmLabel,
    selected: meridiemMode === 'pm',
    label: utils.getMeridiemText('pm'),
    onClick: function () {
      return setMeridiemMode('pm');
    }
  }))));
};

var DateTimePickerRoot = function (_a) {
  var date = _a.date,
      minDate = _a.minDate,
      maxDate = _a.maxDate,
      showTabs = _a.showTabs,
      disablePast = _a.disablePast,
      disableFuture = _a.disableFuture,
      leftArrowIcon = _a.leftArrowIcon,
      leftArrowButtonProps = _a.leftArrowButtonProps,
      rightArrowIcon = _a.rightArrowIcon,
      rightArrowButtonProps = _a.rightArrowButtonProps,
      dateRangeIcon = _a.dateRangeIcon,
      timeIcon = _a.timeIcon,
      renderDay = _a.renderDay,
      ampm = _a.ampm,
      minutesStep = _a.minutesStep,
      shouldDisableDate = _a.shouldDisableDate,
      animateYearScrolling = _a.animateYearScrolling,
      allowKeyboardControl = _a.allowKeyboardControl,
      ViewContainerComponent = _a.ViewContainerComponent,
      onChange = _a.onChange,
      onMonthChange = _a.onMonthChange,
      onYearChange = _a.onYearChange;
  var utils = useUtils();

  var _b = useState('date'),
      openView = _b[0],
      setOpenView = _b[1];

  var _c = useMeridiemMode(date, ampm, onChange),
      meridiemMode = _c.meridiemMode,
      handleMeridiemChange = _c.handleMeridiemChange;

  var handleChangeAndOpenNext = useCallback(function (nextView) {
    return function (time, isFinish) {
      var timeWithMeridiem = convertToMeridiem(time, meridiemMode, Boolean(ampm), utils);

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
  var ViewContainerComponentProps = typeof ViewContainerComponent === 'string' ? {} : {
    openView: openView,
    onChange: onChange
  };
  return createElement(Fragment, null, createElement(DateTimePickerHeader, {
    date: date,
    openView: openView,
    meridiemMode: meridiemMode,
    setMeridiemMode: handleMeridiemChange,
    onOpenViewChange: setOpenView,
    ampm: ampm
  }), showTabs && createElement(DateTimePickerTabs, {
    view: openView,
    onChange: setOpenView,
    dateRangeIcon: dateRangeIcon,
    timeIcon: timeIcon
  }), createElement(Container, __assign({}, ViewContainerComponentProps), createElement(DateTimePickerView, {
    selected: openView === 'year'
  }, createElement(YearSelection, {
    date: date,
    minDate: utils.date(minDate),
    maxDate: utils.date(maxDate),
    onYearChange: onYearChange,
    onChange: handleChangeAndOpenNext('date'),
    disablePast: disablePast,
    disableFuture: disableFuture,
    animateYearScrolling: animateYearScrolling
  })), createElement(DateTimePickerView, {
    selected: openView === 'date'
  }, createElement(Calendar$1, {
    allowKeyboardControl: allowKeyboardControl,
    date: date,
    minDate: utils.date(minDate),
    maxDate: utils.date(maxDate),
    onChange: handleChangeAndOpenNext('hours'),
    disablePast: disablePast,
    disableFuture: disableFuture,
    leftArrowIcon: leftArrowIcon,
    leftArrowButtonProps: leftArrowButtonProps,
    rightArrowIcon: rightArrowIcon,
    rightArrowButtonProps: rightArrowButtonProps,
    renderDay: renderDay,
    shouldDisableDate: shouldDisableDate,
    onMonthChange: onMonthChange
  })), createElement(DateTimePickerView, {
    selected: openView === 'hours' || openView === 'minutes'
  }, createElement(TimePickerView$1, {
    date: date,
    ampm: ampm,
    type: openView,
    onHourChange: handleChangeAndOpenNext('minutes'),
    onMinutesChange: handleChangeAndOpenNext(null),
    onSecondsChange: function () {},
    minutesStep: minutesStep
  }))));
};

process.env.NODE_ENV !== "production" ? DateTimePickerRoot.propTypes = {
  autoSubmit: bool,
  openTo: oneOf(['year', 'date', 'hours', 'minutes']),
  showTabs: bool,
  ViewContainerComponent: oneOfType([string, func, object]),
  minutesStep: number
} : void 0;
DateTimePickerRoot.defaultProps = {
  autoSubmit: true,
  showTabs: true,
  ampm: true,
  minutesStep: 1,
  minDate: new Date('1900-01-01'),
  maxDate: new Date('2100-01-01'),
  openTo: 'date',
  ViewContainerComponent: 'div'
};

function toShowDateTimePickerTabs(showTabsProps) {
  // do not show tabs for small screens
  return Boolean(showTabsProps && typeof window !== 'undefined' && window.innerHeight > 667);
}

function KeyboardDateTimePicker(props) {
  var allowKeyboardControl = props.allowKeyboardControl,
      ampm = props.ampm,
      animateYearScrolling = props.animateYearScrolling,
      autoOk = props.autoOk,
      autoSubmit = props.autoSubmit,
      dateRangeIcon = props.dateRangeIcon,
      disableFuture = props.disableFuture,
      disablePast = props.disablePast,
      format = props.format,
      forwardedRef = props.forwardedRef,
      initialFocusedDate = props.initialFocusedDate,
      invalidDateMessage = props.invalidDateMessage,
      labelFunc = props.labelFunc,
      leftArrowIcon = props.leftArrowIcon,
      leftArrowButtonProps = props.leftArrowButtonProps,
      maxDate = props.maxDate,
      maxDateMessage = props.maxDateMessage,
      minDate = props.minDate,
      onOpen = props.onOpen,
      onClose = props.onClose,
      minDateMessage = props.minDateMessage,
      minutesStep = props.minutesStep,
      onAccept = props.onAccept,
      onChange = props.onChange,
      onMonthChange = props.onMonthChange,
      onYearChange = props.onYearChange,
      openTo = props.openTo,
      renderDay = props.renderDay,
      rightArrowIcon = props.rightArrowIcon,
      rightArrowButtonProps = props.rightArrowButtonProps,
      shouldDisableDate = props.shouldDisableDate,
      showTabs = props.showTabs,
      timeIcon = props.timeIcon,
      value = props.value,
      variant = props.variant,
      other = __rest(props, ["allowKeyboardControl", "ampm", "animateYearScrolling", "autoOk", "autoSubmit", "dateRangeIcon", "disableFuture", "disablePast", "format", "forwardedRef", "initialFocusedDate", "invalidDateMessage", "labelFunc", "leftArrowIcon", "leftArrowButtonProps", "maxDate", "maxDateMessage", "minDate", "onOpen", "onClose", "minDateMessage", "minutesStep", "onAccept", "onChange", "onMonthChange", "onYearChange", "openTo", "renderDay", "rightArrowIcon", "rightArrowButtonProps", "shouldDisableDate", "showTabs", "timeIcon", "value", "variant"]);

  var utils = useUtils();
  var toShowTabs = toShowDateTimePickerTabs(showTabs);

  var _a = useKeyboardPickerState(props, {
    getValidationError: function () {
      return getError(value, utils, props);
    },
    getDefaultFormat: function () {
      return pick12hOr24hFormat(format, ampm, {
        '12h': utils.dateTime12hFormat,
        '24h': utils.dateTime24hFormat
      });
    }
  }),
      pickerProps = _a.pickerProps,
      inputProps = _a.inputProps,
      wrapperProps = _a.wrapperProps;

  return createElement(Wrapper, __assign({
    variant: variant,
    showTabs: showTabs,
    InputComponent: KeyboardDateInput,
    DateInputProps: inputProps
  }, wrapperProps, other), createElement(DateTimePickerRoot, __assign({}, pickerProps, {
    ampm: ampm,
    allowKeyboardControl: allowKeyboardControl,
    minutesStep: minutesStep,
    animateYearScrolling: animateYearScrolling,
    autoSubmit: autoSubmit,
    dateRangeIcon: dateRangeIcon,
    disableFuture: disableFuture,
    disablePast: disablePast,
    leftArrowIcon: leftArrowIcon,
    leftArrowButtonProps: leftArrowButtonProps,
    maxDate: maxDate,
    minDate: minDate,
    onMonthChange: onMonthChange,
    onYearChange: onYearChange,
    openTo: openTo,
    renderDay: renderDay,
    rightArrowIcon: rightArrowIcon,
    rightArrowButtonProps: rightArrowButtonProps,
    shouldDisableDate: shouldDisableDate,
    showTabs: toShowTabs,
    timeIcon: timeIcon
  })));
}
KeyboardDateTimePicker.defaultProps = dateTimePickerDefaultProps;
var KeyboardDateTimePicker$1 = forwardRef(function (props, ref) {
  return createElement(KeyboardDateTimePicker, __assign({}, props, {
    forwardedRef: ref
  }));
});

var DateTimePicker = function (props) {
  var allowKeyboardControl = props.allowKeyboardControl,
      ampm = props.ampm,
      animateYearScrolling = props.animateYearScrolling,
      autoOk = props.autoOk,
      autoSubmit = props.autoSubmit,
      dateRangeIcon = props.dateRangeIcon,
      disableFuture = props.disableFuture,
      disablePast = props.disablePast,
      format = props.format,
      forwardedRef = props.forwardedRef,
      initialFocusedDate = props.initialFocusedDate,
      invalidDateMessage = props.invalidDateMessage,
      labelFunc = props.labelFunc,
      leftArrowIcon = props.leftArrowIcon,
      leftArrowButtonProps = props.leftArrowButtonProps,
      maxDate = props.maxDate,
      maxDateMessage = props.maxDateMessage,
      minDate = props.minDate,
      minDateMessage = props.minDateMessage,
      minutesStep = props.minutesStep,
      onAccept = props.onAccept,
      onChange = props.onChange,
      onOpen = props.onOpen,
      onClose = props.onClose,
      onMonthChange = props.onMonthChange,
      onYearChange = props.onYearChange,
      openTo = props.openTo,
      renderDay = props.renderDay,
      rightArrowIcon = props.rightArrowIcon,
      rightArrowButtonProps = props.rightArrowButtonProps,
      shouldDisableDate = props.shouldDisableDate,
      showTabs = props.showTabs,
      timeIcon = props.timeIcon,
      value = props.value,
      variant = props.variant,
      other = __rest(props, ["allowKeyboardControl", "ampm", "animateYearScrolling", "autoOk", "autoSubmit", "dateRangeIcon", "disableFuture", "disablePast", "format", "forwardedRef", "initialFocusedDate", "invalidDateMessage", "labelFunc", "leftArrowIcon", "leftArrowButtonProps", "maxDate", "maxDateMessage", "minDate", "minDateMessage", "minutesStep", "onAccept", "onChange", "onOpen", "onClose", "onMonthChange", "onYearChange", "openTo", "renderDay", "rightArrowIcon", "rightArrowButtonProps", "shouldDisableDate", "showTabs", "timeIcon", "value", "variant"]);

  var utils = useUtils();
  var toShowTabs = toShowDateTimePickerTabs(showTabs);

  var _a = usePickerState(props, {
    getValidationError: function () {
      return getError(value, utils, props);
    },
    getDefaultFormat: function () {
      return pick12hOr24hFormat(format, ampm, {
        '12h': utils.dateTime12hFormat,
        '24h': utils.dateTime24hFormat
      });
    }
  }),
      pickerProps = _a.pickerProps,
      inputProps = _a.inputProps,
      wrapperProps = _a.wrapperProps;

  return createElement(Wrapper, __assign({
    variant: variant,
    showTabs: toShowTabs,
    InputComponent: PureDateInput,
    DateInputProps: inputProps
  }, wrapperProps, other), createElement(DateTimePickerRoot, __assign({}, pickerProps, {
    ampm: ampm,
    allowKeyboardControl: allowKeyboardControl,
    minutesStep: minutesStep,
    animateYearScrolling: animateYearScrolling,
    autoSubmit: autoSubmit,
    dateRangeIcon: dateRangeIcon,
    disableFuture: disableFuture,
    disablePast: disablePast,
    leftArrowIcon: leftArrowIcon,
    leftArrowButtonProps: leftArrowButtonProps,
    maxDate: maxDate,
    minDate: minDate,
    onMonthChange: onMonthChange,
    onYearChange: onYearChange,
    openTo: openTo,
    renderDay: renderDay,
    rightArrowIcon: rightArrowIcon,
    rightArrowButtonProps: rightArrowButtonProps,
    shouldDisableDate: shouldDisableDate,
    showTabs: toShowTabs,
    timeIcon: timeIcon
  })));
};
DateTimePicker.defaultProps = dateTimePickerDefaultProps;
var DateTimePicker$1 = forwardRef(function (props, ref) {
  return createElement(DateTimePicker, __assign({}, props, {
    forwardedRef: ref
  }));
});

export { Calendar$1 as Calendar, Clock$1 as Clock, DatePicker$1 as DatePicker, DateTimePicker$1 as DateTimePicker, Day, KeyboardDatePicker$1 as KeyboardDatePicker, KeyboardDateTimePicker$1 as KeyboardDateTimePicker, KeyboardTimePicker$1 as KeyboardTimePicker, MuiPickersContext, MuiPickersUtilsProvider, TimePicker$1 as TimePicker, TimePickerView$1 as TimePickerView, useKeyboardPickerState, usePickerState, useUtils };
//# sourceMappingURL=material-ui-pickers.esm.js.map
