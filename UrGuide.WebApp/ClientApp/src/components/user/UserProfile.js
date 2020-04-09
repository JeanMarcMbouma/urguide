import React, { Component } from 'react';
import { Route } from 'react-router-dom';
import {
    Grid
} from "@material-ui/core";
import { UpperSection } from "./UpperSection";
import { LowerSection } from "./LowerSection";
import "./UserStyle.css";

export class UserProfile extends Component {
    render() {
        return (
            <div className="container-fluid user-page-container">
                <div className="row">
                <div className="col-12">
                        <UpperSection />
                    </div>
                </div>
                <div className="row">
                    <div className="col-12 lower-section">
                        <LowerSection />
                    </div>
                </div>
        </div>
        )
    }
}