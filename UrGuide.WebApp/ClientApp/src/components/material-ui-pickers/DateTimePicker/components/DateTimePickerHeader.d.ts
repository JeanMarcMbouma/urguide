import * as React from 'react';
import { MaterialUiPickersDate } from '../../typings/date';
import { DateTimePickerViewType } from '../DateTimePickerRoot';
export declare const useStyles: (props?: any) => Record<"separator" | "toolbar" | "toolBar24h" | "hourMinuteLabel" | "dateHeader" | "timeHeader" | "ampmSelection" | "ampmLabel", string>;
export declare type MeridiemMode = 'am' | 'pm';
export interface DateTimePickerHeaderProps {
    date: MaterialUiPickersDate;
    meridiemMode: MeridiemMode;
    openView: DateTimePickerViewType;
    onOpenViewChange: (view: DateTimePickerViewType) => void;
    setMeridiemMode: (mode: MeridiemMode) => void;
    ampm?: boolean;
}
export declare const DateTimePickerHeader: React.SFC<DateTimePickerHeaderProps>;
export default DateTimePickerHeader;
