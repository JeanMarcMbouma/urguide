import { ChangePasswordModel, ApiException } from './../../../api'
import { HttpClientFactory } from './../../../httpclient'

async function changePassword(state: any) {

    console.log(state.user);

    const client = HttpClientFactory.getAccountClient(state.user);

    const model = new ChangePasswordModel({
        email: state.email,
        password: state.password,
        confirmPassword: state.confirmPassword,
        currentPassword: state.currentPassword,
    });

    try {

        await client.changepassword(model);
            
    }
    catch (e)  {
        state.error = (<ApiException>e).result;

        console.log(state.error);

        console.log((<ApiException>e).response);
    }
}


export default function ChangePasswordReducer(state: any, action: any) {
      
    let context = { ...state };

    switch (action.type) {
        case "changePassword":
            context.user = action.data.user;
            context.email = action.data.email;
            context.password = action.data.password;
            context.confirmPassword = action.data.confirmPassword;
            context.currentPassword = action.data.currentPassword;
            changePassword(context);

            return context;
    }     
}
