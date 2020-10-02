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
function LoginReducer(state, action) {
    var context = __assign({}, state);
    switch (action.type) {
        case "validate-login":
            context.email = action.data.email;
            context.password = action.data.password;
            context.isRemembered = action.data.isRemembered;
            context.returnUrl = action.data.returnUrl;
            var regexEmail = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/;
            var passwordRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/;
            var validEmail = regexEmail.test(context.email);
            var validpassword = passwordRegex.test(context.password);
            context.emailError = validEmail ? false : true;
            context.passwordError = validpassword ? false : true;
            context.passwordErrorMessage = context.passwordError ? "your password must contains minimum eight characters, at least one uppercase letter, one lowercase letter, one number and one special character." : '';
            if (validEmail && validpassword) {
                action.data.callback(context);
                //login(context);
            }
            return context;
    }
}
exports.default = LoginReducer;
//# sourceMappingURL=LoginReducer.js.map