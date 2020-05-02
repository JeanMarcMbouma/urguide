import React from 'react'
import authService from './AuthService';
const LoginCallBack = () => {
    
    const url = window.location.href;
    authService.completeSignIn(url);
    return <></>;
}

export default LoginCallBack;