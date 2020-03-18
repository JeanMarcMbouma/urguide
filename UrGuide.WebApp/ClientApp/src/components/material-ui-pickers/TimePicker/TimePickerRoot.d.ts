import * as React from 'react';
import { MaterialUiPickersDate } from '../typings/date';
import { MeridiemMode } from '../DateTimePicker/components/DateTimePickerHeader';
export interface BaseTimePickerProps {
    /**
     * 12h/24h view for hour selection clock
     * @default true
     */
    ampm?: boolean;
    /**
     * Step over minutes
     * @default 1
     */
    minutesStep?: number;
    /** Show the seconds view */
    seconds?: boolean;
}
export declare const useStyles: (props?: any) => Record<"separator" | "toolbar" | "hourMinuteLabel" | "ampmSelection" | "ampmLabel" | "toolbarLeftPadding" | "ampmSelectionWithSeconds" | "hourMinuteLabelReverse", string>;
export interface TimePickerProps extends BaseTimePickerProps {
    date: MaterialUiPickersDate;
    onChange: (date: MaterialUiPickersDate, isFinished?: boolean) => void;
}
export declare function useMeridiemMode(date: MaterialUiPickersDate, ampm: boolean | undefined, onChange: (date: MaterialUiPickersDate, isFinished?: boolean | undefined) => void): {
    meridiemMode: MeridiemMode;
    handleMeridiemChange: (mode: MeridiemMode) => void;
};
declare const TimePickerRoot: React.FC<TimePickerProps>;
export default TimePickerRoot;
