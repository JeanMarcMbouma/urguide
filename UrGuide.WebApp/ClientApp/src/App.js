import React, { Component } from 'react';
import { Route } from 'react-router';
import { Switch, Redirect } from 'react-router-dom';
import { LoginLayout } from './components/login/LoginLayout';
import { LoginPage } from './components/login/LoginPage';
import { RegisterLayout } from './components/RegisterLayout';
import Discover from "./components/discover/Discover";
import Profile from "./components/user/Profile";
import {
    ClientRegistration,
} from './components/client-registration/ClientRegistration';
import {
    GuideRegistration,
} from './components/guide-registration/GuideRegistration';
import { Layout } from './components/Layout';
import Home from './components/MainPage/Home';
import EmailConfirmation from './components/confirmation/EmailConfirmation';
import RegistrationConfirmation
    from './components/confirmation/RegistrationConfirmation';
import AuthRoute from './components/api-authorization/AuthRoute'
import { AuthContextProvider } from './components/api-authorization/AuthService'
import { ApplicationPaths } from './components/api-authorization/ApiAuthorizationConstants';
import LoginCallback from './components/api-authorization/LoginCallback';
import LogoutCallback from './components/api-authorization/LogoutCallback';

export default class App extends Component {
    constructor(props) {
        super(props);
    }
    static displayName = App.name;

    render() {

        return (
            <AuthContextProvider>
                <React.StrictMode>
                    <Switch>
                        <Route exact path="/email-confirmed" component={EmailConfirmation} />
                        <Route
                            exact
                            path="/sign-up-confirm"
                            component={RegistrationConfirmation}
                        />

                        <Route path="/sign-in">
                            <LoginLayout>
                                <Route exact path="/sign-in" component={LoginPage} />
                            </LoginLayout>
                        </Route>
                        <Route path="/(sign-up|guide/sign-up|authentication/register)">
                            <RegisterLayout>
                                <Route exact path="/sign-up" component={ClientRegistration} />
                                <Route
                                    exact
                                    path="/authentication/register"
                                    component={ClientRegistration}
                                />
                                <Route
                                    exact
                                    path="/guide/sign-up"
                                    component={GuideRegistration}
                                />
                            </RegisterLayout>
                        </Route>
                        <Route path="/(user|feed|discover)">
                            <Layout>
                                <AuthRoute path="/user" component={Profile} />
                                <Route path="/feed" component={Home} />
                                <Route path="/discover" component={Discover} />
                            </Layout>
                        </Route>
                        <Route exact path="/" render={() => <Redirect to="/feed" />} />
                        <Route exact path={ApplicationPaths.LoginCallback} component={LoginCallback} />
                        <Route exact path={ApplicationPaths.LogOutCallback} render={() => <Redirect to="/feed" />} />
                    </Switch>
                </React.StrictMode>
            </AuthContextProvider>
        );
    }
}
