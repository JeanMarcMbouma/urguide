export default async function UserReducer(state, action)  {
    let context = {...state};

    switch (action.type) {
        case 'login':
            // await fetch(url);
            context.username = action.data;
            context.isLoggedIn = true;
            return context;

        case 'logout':
            if(context.username === action.data){
                context.username = 'Guest';
                context.isLoggedIn = false;
            }
            return context;

    }


}