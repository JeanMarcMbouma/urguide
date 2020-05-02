import { Client, LoginModel, ApiException } from './../../api'
const navigateToReturnUrl = (returnUrl: any) => {

    window.location.replace(returnUrl);
}

async function login(state: any) {

    const returnUrl = state.returnUrl;
    const client = new Client();
    const loginModel = new LoginModel({
        userName: state.email,
        password: state.password,
        persist: state.isRemembered
    });

    try {
        await client.login(returnUrl, loginModel);
        navigateToReturnUrl(returnUrl);
        return null;
    } catch (e) {
        state.LoginFailed = (<ApiException>e).message;
    }
}

export default function LoginReducer(state: any, action: any) {
    let context = { ...state };

    switch (action.type) {
        case "validate-login":
            context.email = action.data.email;
            context.password = action.data.password;
            context.isRemembered = action.data.isRemembered;
            context.returnUrl = action.data.returnUrl
            let regexEmail = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/;
            let passwordRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/;
            let validEmail = regexEmail.test(context.email);
            let validpassword = passwordRegex.test(context.password);
            context.emailError = validEmail ? false : true;
            context.passwordError = validpassword ? false : true;
            context.passwordErrorMessage = context.passwordError ? "your password must contains minimum eight characters, at least one uppercase letter, one lowercase letter, one number and one special character." : '';
            if (validEmail && validpassword)
            {
               login(context);
            }
            
            return context;
    }
}
