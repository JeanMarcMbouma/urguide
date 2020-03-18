import * as React from 'react';
import { DateTimePickerViewType } from '../DateTimePickerRoot';
export interface DateTimePickerTabsProps {
    view: DateTimePickerViewType;
    onChange: (view: DateTimePickerViewType) => void;
    dateRangeIcon: React.ReactNode;
    timeIcon: React.ReactNode;
}
export declare const useStyles: (props?: any) => Record<"tabs", string>;
export declare const DateTimePickerTabs: React.SFC<DateTimePickerTabsProps>;
export default DateTimePickerTabs;
