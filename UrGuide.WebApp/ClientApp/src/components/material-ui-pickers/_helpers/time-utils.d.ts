import { IUtils } from '@date-io/core/IUtils';
import { MaterialUiPickersDate } from '../typings/date';
import { MeridiemMode } from '../DateTimePicker/components/DateTimePickerHeader';
export declare const getHours: (offsetX: number, offsetY: number, ampm: boolean) => number;
export declare const getMinutes: (offsetX: number, offsetY: number, step?: number) => number;
export declare const convertToMeridiem: (time: MaterialUiPickersDate, meridiem: MeridiemMode, ampm: boolean, utils: IUtils<MaterialUiPickersDate>) => MaterialUiPickersDate;
