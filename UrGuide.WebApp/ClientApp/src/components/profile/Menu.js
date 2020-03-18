import React, { Component } from 'react';
import { NavLink } from 'react-router-dom';
export class MenuProfile extends Component {
    render () {
        return(
        <div className="col-lg-3">
          <ul className="nav flex-column">
            <li className="nav-item">
                <NavLink to="/profile/profileInfo">Profile Info</NavLink>
            </li>
            <li className="nav-item">
                <NavLink to="/profile/AddGallery">Add gallery</NavLink>
            </li>
            <li className="nav-item">
                <NavLink to="/profile/passwordChange">Password Change</NavLink>
            </li>
            <li className="nav-item">
                <NavLink to="/profile/deleteProfile">Delete account</NavLink>
            </li>
          </ul>
        </div>
    )}
}