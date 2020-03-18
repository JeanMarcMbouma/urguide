/// <reference types="react" />
/// <reference types="styled-jsx" />
import { BasePickerProps } from '../../typings/BasePicker';
export interface HookOptions {
    getDefaultFormat: () => string;
    getValidationError: () => React.ReactNode;
}
export declare function usePickerState(props: BasePickerProps, options: HookOptions): {
    pickerProps: {
        date: import("../../typings/date").MaterialUiPickersDate;
        onChange: (newDate: import("../../typings/date").MaterialUiPickersDate, isFinish?: any) => void;
    };
    inputProps: {
        validationError: import("react").ReactNode;
        onClick: () => void;
        inputValue: string;
    };
    wrapperProps: {
        format: string;
        open: boolean;
        onAccept: () => void;
        onClear: () => void;
        onSetToday: () => void;
        onDismiss: () => void;
    };
};
