import React, { Component } from 'react'
import { Route } from 'react-router-dom'
import { useSecure } from './AuthService'


export default function AuthRoute(props) {
    var link = document.createElement("a");
    link.href = props.path;
    const { component: Component, ...rest } = props;
    const newComponent = <Component {...props} />;
    const secureComponent = useSecure(newComponent);
    return <Route {...rest}
        render={() => secureComponent} />;
}
