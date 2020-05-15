

export default function ChangePasswordReducer(state: any, action: any) {

  
      
    let context = { ...state };

    switch (action.type) {
        case "changePassword":
        
            context.user = action.data.user;
            context.email = action.data.email;
            context.password = action.data.password;
            context.confirmPassword = action.data.confirmPassword;
            context.currentPassword = action.data.currentPassword;

            let regexEmail = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/;
            let passwordRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/;

            let validEmail = regexEmail.test(context.email);
            let validpassword = passwordRegex.test(context.password);
            let validCurrentPassword = passwordRegex.test(context.currentPassword);
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
