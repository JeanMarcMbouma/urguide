import * as React from 'react';
import { OutterCalendarProps } from './components/Calendar';
import { MaterialUiPickersDate } from '../typings/date';
import { ParsableDate } from '../constants/prop-types';
export interface BaseDatePickerProps extends OutterCalendarProps {
    /**
     * Show only calendar, without toolbar
     * @default false
     */
    onlyCalendar?: boolean;
    /**
     * Min selectable date
     * @default Date(1900-01-01)
     */
    minDate?: ParsableDate;
    /**
     * Max selectable date
     * @default Date(2100-01-01)
     */
    maxDate?: ParsableDate;
    /**
     * Disable past dates
     * @default false
     */
    disablePast?: boolean;
    /**
     * Disable future dates
     * @default false
     */
    disableFuture?: boolean;
    /**
     * To animate scrolling to current year (with scrollIntoView)
     * @default false
     */
    animateYearScrolling?: boolean;
    /**
     * Array of views to show. Order year -> month -> day
     * @default ["day", "year"]
     */
    views?: ('year' | 'month' | 'day')[];
    /**
     * Initial view to show when date picker is open
     * @default props.views[0]
     */
    openTo?: 'year' | 'month' | 'day';
    /** Callback firing on year change */
    onYearChange?: (date: MaterialUiPickersDate) => void;
}
export interface DatePickerRootProps extends BaseDatePickerProps {
    date: MaterialUiPickersDate;
    onChange: (date: MaterialUiPickersDate, isFinished?: boolean) => void;
}
export declare const useStyles: (props?: any) => Record<"toolbarCenter", string>;
export declare const DatePickerRoot: React.FC<DatePickerRootProps>;
export default DatePickerRoot;
