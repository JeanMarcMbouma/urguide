"use strict";
var __assign = (this && this.__assign) || function () {
    __assign = Object.assign || function(t) {
        for (var s, i = 1, n = arguments.length; i < n; i++) {
            s = arguments[i];
            for (var p in s) if (Object.prototype.hasOwnProperty.call(s, p))
                t[p] = s[p];
        }
        return t;
    };
    return __assign.apply(this, arguments);
};
Object.defineProperty(exports, "__esModule", { value: true });
function ChangePasswordReducer(state, action) {
    var context = __assign({}, state);
    switch (action.type) {
        case "changePassword":
            context.user = action.data.user;
            context.email = action.data.email;
            context.password = action.data.password;
            context.confirmPassword = action.data.confirmPassword;
            context.currentPassword = action.data.currentPassword;
            var regexEmail = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/;
            var passwordRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/;
            var validEmail = regexEmail.test(context.email);
            var validpassword = passwordRegex.test(context.password);
            var validCurrentPassword = passwordRegex.test(context.currentPassword);
            context.emailError = validEmail ? false : true;
            context.passwordError = validpassword ? false : true;
            context.passwordsDontMatchError = context.password == context.confirmPassword ? false : true;
            context.currentPasswordError = validCurrentPassword ? false : true;
            //messages
            context.emailErrorMessage = context.emailError ? "invalid email address." : null;
            context.passwordErrorMessage = context.passwordError ? "your password must contains minimum eight characters, at least one uppercase letter, one lowercase letter, one number and one special character." : null;
            context.passwordsDontMatchErrorMessage = context.passwordsDontMatchError ? "The password and its confirmation do not match." : null;
            context.currentPasswordErrorMessage = context.currentPasswordError ? "your password must contains minimum eight characters, at least one uppercase letter, one lowercase letter, one number and one special character." : null;
            if (validEmail && validpassword && validCurrentPassword && !context.passwordsDontMatchError) {
                action.data.callback(context);
            }
            return context;
    }
}
exports.default = ChangePasswordReducer;
//# sourceMappingURL=ChangePasswordReducer.js.map