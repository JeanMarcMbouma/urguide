"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var useUtils_1 = require("./useUtils");
var text_field_helper_1 = require("../../_helpers/text-field-helper");
var react_1 = require("react");
var valueToDate = function (utils, _a) {
    var value = _a.value, initialFocusedDate = _a.initialFocusedDate;
    var initialDate = value || initialFocusedDate || utils.date();
    var date = utils.date(initialDate);
    return date && utils.isValid(date) ? date : utils.date();
};
function useDateValues(props, options) {
    var utils = useUtils_1.useUtils();
    var date = valueToDate(utils, props);
    var acceptedDateRef = react_1.useRef(date);
    var format = props.format || options.getDefaultFormat();
    return { acceptedDateRef: acceptedDateRef, date: date, format: format };
}
function makeControlledOpenProps(props) {
    return {
        isOpen: props.open,
        setIsOpen: function (newIsOpen) {
            return newIsOpen ? props.onOpen && props.onOpen() : props.onClose && props.onClose();
        },
    };
}
/* eslint-disable react-hooks/rules-of-hooks */
function useOpenState(props) {
    if (props.open !== undefined && props.open !== null) {
        return makeControlledOpenProps(props);
    }
    var _a = react_1.useState(false), isOpen = _a[0], setIsOpenState = _a[1];
    // prettier-ignore
    var setIsOpen = react_1.useCallback(function (newIsOpen) {
        setIsOpenState(newIsOpen);
        return newIsOpen
            ? props.onOpen && props.onOpen()
            : props.onClose && props.onClose();
    }, [props]);
    return { isOpen: isOpen, setIsOpen: setIsOpen };
}
/* eslint-enable react-hooks/rules-of-hooks */
function usePickerState(props, options) {
    var utils = useUtils_1.useUtils();
    var _a = useOpenState(props), isOpen = _a.isOpen, setIsOpen = _a.setIsOpen;
    var _b = useDateValues(props, options), acceptedDateRef = _b.acceptedDateRef, date = _b.date, format = _b.format;
    react_1.useEffect(function () {
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
        onClick: function () { return setIsOpen(true); },
        inputValue: text_field_helper_1.getDisplayDate(date, format, utils, props.value === null, props),
    };
    // prettier-ignore
    var acceptDate = react_1.useCallback(function (acceptedDate) {
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
        onAccept: function () { return acceptDate(date); },
        onClear: function () { return acceptDate(null); },
        onSetToday: react_1.useCallback(function () { return props.onChange(utils.date()); }, [props, utils]),
        onDismiss: react_1.useCallback(function () {
            setIsOpen(false);
            props.onChange(acceptedDateRef.current);
        }, [setIsOpen, props, acceptedDateRef]),
    };
    var pickerProps = {
        date: date,
        onChange: react_1.useCallback(function (newDate, isFinish) {
            if (isFinish === void 0) { isFinish = true; }
            props.onChange(newDate);
            if (isFinish && props.autoOk) {
                acceptDate(newDate);
            }
        }, [props, acceptDate]),
    };
    var pickerState = { pickerProps: pickerProps, inputProps: inputProps, wrapperProps: wrapperProps };
    react_1.useDebugValue(pickerState);
    return pickerState;
}
exports.usePickerState = usePickerState;
