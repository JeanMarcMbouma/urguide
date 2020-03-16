import React, { Component } from 'react';
import { Route, BrowserRouter } from 'react-router-dom';
import {ComponentProfile} from "./ComponentProfile"
import {ComponentDeleteProfile} from "./ComponentDeleteProfile"
import {ComponentChangePassword} from "./ComponentChangePassword"
import { MenuProfile } from "./Menu"

//This component create page where you can change profile information

export class Profile extends Component {
    render () {
        return (
        <BrowserRouter>
            <MenuProfile />
            <Route path="/profile" component={ComponentProfile}/>
            <Route path="/password" component={ComponentDeleteProfile}/>
            <Route path="/deleteProfile" component={ComponentChangePassword}/>
        </BrowserRouter>
        )
    }
}