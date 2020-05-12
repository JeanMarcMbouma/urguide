import React, { Component, useState, useEffect } from 'react'
import { Route } from 'react-router-dom'
import { useAuthContext } from './AuthService'
import { QueryParameterNames, ApplicationPaths } from './ApiAuthorizationConstants';
import LogoutCallback from './LogoutCallback'

export default function AuthRoute(props) {
    const [ready, setReady] = useState(false);
    const { manager, isLoggedIn, user, authenticating } = useAuthContext(); 

    useEffect(() => {
        manager.isAuthenticated();
        setReady(true);
    }, [manager, user, isLoggedIn])
    var link = document.createElement("a");
    link.href = props.path;
    const returnUrl = `${link.protocol}//${link.host}${link.pathname}${link.search}${link.hash}`;
    const redirectUrl = `${ApplicationPaths.Login}?${QueryParameterNames.ReturnUrl}=${encodeURI(returnUrl)}`
    if (!ready) {
        return authenticating;
    } else {
        const { component: Component, ...rest } = props;
        return <Route {...rest}
            render={(props) => {
                if (user) {
                    return <Component {...props} />
                } else {
                    return <LogoutCallback />
                }
            }} />
    }
}
