import * as PropTypes from 'prop-types';
import { DatePickerProps } from '../DatePicker/DatePicker';
export declare type ParsableDate = object | string | number | Date | null | undefined;
export declare const DomainPropTypes: {
    date: PropTypes.Requireable<string | number | object>;
    datePickerView: PropTypes.Requireable<string>;
};
export declare const timePickerDefaultProps: import("../TimePicker/TimePicker").TimePickerProps;
export declare const datePickerDefaultProps: DatePickerProps;
export declare const dateTimePickerDefaultProps: import("../DateTimePicker/DateTimePicker").DateTimePickerProps;
