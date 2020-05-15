import React, { Component, useState } from "react";
import {
    makeStyles,
    IconButton,
    Chip,
    Button,
    Typography,
    Avatar,
    CardHeader,
} from "@material-ui/core";
import Posts from "./Posts";
import Galleries from "./Galleries";
import EditProfile from "./EditProfile";
import ChangePassword from "./ChangePassword";
import UpperSection from "./UpperSection";
import {
    BrowserRouter as Router,
    Switch,
    Route,
    Link,
    useParams,
    useRouteMatch
} from "react-router-dom";
import { CreateNewGallery } from "./CreateNewGallery";
import UserContext from "./../UserContext";
import AuthRoute from "../api-authorization/AuthRoute";


function ProfileLayout() {

    let { path, url } = useRouteMatch();

    console.log(useRouteMatch());

    return (
    
            <div className="container-fluid user-page-container">
                <div className="row">
                    <div className="col-12">
                        <UpperSection />
                    </div>
                </div>
            <Switch>
                <Route exact path={path} >
                    <Posts />
                </Route>
                <Route path={`${path}/galleries`}>
                    <Galleries />
                </Route>
                <AuthRoute path={`${path}/edit/profile`}>
                    <EditProfile />
                </AuthRoute>
                <AuthRoute path={`${path}/edit/password`}>
                    <ChangePassword />
                </AuthRoute>
                <Route path={`${path}/gallery/new`}>
                    <CreateNewGallery />
                </Route>
            </Switch>

            </div>
    );
}

export default class Profile extends Component {
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