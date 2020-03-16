import React, { Component } from 'react';
import { NavLink } from 'react-router-dom';
export class MenuProfile extends Component {
    render () {
        return(
        <div className="col-lg-3">
          <ul class="nav flex-column">
            <li class="nav-item">
                <NavLink to="/profile">Profile Info</NavLink>
            </li>
             <li class="nav-item">
                <NavLink to="/ChangePassword">Password Change</NavLink>
            </li>
            <li class="nav-item">
                <NavLink to="/deleteProfile">Delete account</NavLink>
            </li>
        </ul>
        </div>
    )}
}