
export default function ResetPasswordReducer(state, action) {
    let context = { ...state };

    switch (action.type) {
        case "confirm-password":
            context.newPassword = action.data.newPassword;
            context.confirmPassword = action.data.confirmPassword
            let passwordRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/;
            let validpassword = passwordRegex.test(context.newPassword);
            context.passwordError = validpassword ? false : true;
            context.passwordErrorMessage = context.passwordError ? "your password must contains minimum eight characters, at least one uppercase letter, one lowercase letter, one number and one special character." : '';
            if (validpassword) {
                action.data.callback(context);
            }

            return context;
    }
}