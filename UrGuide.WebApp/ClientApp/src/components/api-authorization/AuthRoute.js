import React, { Component, useState, useEffect } from 'react'
import { Route } from 'react-router-dom'
import { useAuth } from './AuthService'
import { QueryParameterNames, ApplicationPaths } from './ApiAuthorizationConstants';
import { Redirect } from 'react-router-dom';

export default function AuthRoute(props) {
    const [ready, setReady] = useState(false);
    const { manager, isLoggedIn, user, authenticating } = useAuth(); 

    useEffect(() => {
        manager.isAuthenticated();
        if (isLoggedIn)
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
                    return <Redirect to={redirectUrl} />
                }
            }} />
    }
}
