import * as React from 'react';
import { Omit } from '@material-ui/core';
import { MaterialUiPickersDate } from '../typings/date';
import { BaseDatePickerProps } from '../DatePicker/DatePickerRoot';
import { BaseTimePickerProps } from '../TimePicker/TimePickerRoot';
export declare type DateTimePickerViewType = 'year' | 'date' | 'hours' | 'minutes';
export declare type BaseDateTimePickerProps = Omit<BaseTimePickerProps, 'seconds'> & Omit<BaseDatePickerProps, 'onlyCalendar' | 'views' | 'openTo'> & {
    autoSubmit?: boolean;
    /** Show or hide date/time tabs (hidden automatically on small screens) */
    showTabs?: boolean;
    /** Initial view to show when datetime picker is open */
    openTo?: 'year' | 'date' | 'hours' | 'minutes';
    /** Date tab icon */
    dateRangeIcon?: React.ReactNode;
    /** Time tab icon */
    timeIcon?: React.ReactNode;
    /**
     * View container that wraps DateTimePicker views
     * @type {React.Component}
     */
    ViewContainerComponent?: string | React.ComponentType<{} | {
        openView: DateTimePickerViewType;
        onChange: () => void;
    }>;
};
export interface DateTimePickerProps extends BaseDateTimePickerProps {
    date: MaterialUiPickersDate;
    onChange: (date: MaterialUiPickersDate, isFinished?: boolean) => void;
}
declare const DateTimePickerRoot: React.FC<DateTimePickerProps>;
export default DateTimePickerRoot;
