import React, { Component, useState, useMemo } from "react";
import EditProfile from "./EditProfile";
import ChangePassword from "./ChangePassword";
import {
    BrowserRouter as Router,
    Switch,
    Route,
    useRouteMatch,
} from "react-router-dom";
import UserContext from "./../UserContext";
import AuthRoute from "../api-authorization/AuthRoute";



function ProfileLayout() {

    let { path } = useRouteMatch();

    return (
    
        <div className="container-fluid user-page-container">
            <Switch>
                <AuthRoute path={`${path}/details`}>
                    <EditProfile isGuide={false} />
                </AuthRoute>
                <AuthRoute path={`${path}/password`}>
                    <ChangePassword isGuide={false} />
                </AuthRoute>
            </Switch>

            </div>
    );
}

export default class ClientDetails extends Component {
    constructor(props) {
        super(props);
        this.state = {
            data: [],
            error: null,
            isLoaded: false,
        };
    
    }


    render() {
        return (<UserContext.Provider value={{ userData: this.state.data }} >
            <ProfileLayout />
        </UserContext.Provider>);
    }

}