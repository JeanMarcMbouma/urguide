import React, {createContext} from 'react';

const UserContext = createContext({
    email: null, 
    username: null, 
    isLoggedIn: false,
    token: null
});

export default UserContext;