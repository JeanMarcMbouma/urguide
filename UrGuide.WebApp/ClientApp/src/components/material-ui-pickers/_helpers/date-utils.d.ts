import { IUtils } from '@date-io/core/IUtils';
import { MaterialUiPickersDate } from '../typings/date';
import { DatePickerViewType } from '../constants/DatePickerView';
interface FindClosestDateParams {
    date: MaterialUiPickersDate;
    utils: IUtils<MaterialUiPickersDate>;
    minDate: MaterialUiPickersDate;
    maxDate: MaterialUiPickersDate;
    disableFuture: boolean;
    disablePast: boolean;
    shouldDisableDate: (date: MaterialUiPickersDate) => boolean;
}
export declare const findClosestEnabledDate: ({ date, utils, minDate, maxDate, disableFuture, disablePast, shouldDisableDate, }: FindClosestDateParams) => MaterialUiPickersDate;
export declare const isYearOnlyView: (views: DatePickerViewType[]) => boolean;
export declare const isYearAndMonthViews: (views: DatePickerViewType[]) => boolean;
export declare const getFormatByViews: (views: DatePickerViewType[], utils: IUtils<MaterialUiPickersDate>) => string;
export {};
