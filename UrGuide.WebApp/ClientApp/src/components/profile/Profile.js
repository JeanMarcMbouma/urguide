import React, { Component } from 'react';
import { Route } from 'react-router-dom';
import {ComponentProfileInfo} from "./ComponentProfile"
import {ComponentDeleteProfile} from "./ComponentDeleteProfile"
import {ComponentChangePassword} from "./ComponentChangePassword"
import { MenuProfile } from "./Menu"

//This component create page where you can change profile information

export class Profile extends Component {
    render () {
        return (
        <div className="row col-lg-12">
            <MenuProfile />
            <Route path="/profile/profileInfo" component={ComponentProfileInfo}/>
            <Route path="/profile/deleteProfile" component={ComponentDeleteProfile}/>
            <Route path="/profile/passwordChange" component={ComponentChangePassword}/>
        </div>
        )
    }
}