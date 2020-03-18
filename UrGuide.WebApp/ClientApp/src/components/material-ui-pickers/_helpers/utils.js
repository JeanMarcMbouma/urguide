"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
function toShowDateTimePickerTabs(showTabsProps) {
    // do not show tabs for small screens
    return Boolean(showTabsProps && typeof window !== 'undefined' && window.innerHeight > 667);
}
exports.toShowDateTimePickerTabs = toShowDateTimePickerTabs;
