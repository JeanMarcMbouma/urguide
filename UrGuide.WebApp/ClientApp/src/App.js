import React, { Component } from "react";
import { Route } from "react-router";
import { Switch } from "react-router-dom";
import { LoginLayout } from "./components/login/LoginLayout";
import { LoginPage } from "./components/login/LoginPage";
import { Profile } from "./components/profile/Profile";
import { RegisterLayout } from "./components/RegisterLayout";
import { ClientRegistration } from "./components/client-registration/ClientRegistration";
import { GuideRegistration } from "./components/guide-registration/GuideRegistration";
import { Layout } from "./components/Layout";
import UserContext from './UserContext';
import Home from "./components/MainPage/Home"
import AuthorizeRoute from "./components/api-authorization/AuthorizeRoute";
import ApiAuthorizationRoutes from "./components/api-authorization/ApiAuthorizationRoutes";
import { ApplicationPaths } from './components/api-authorization/ApiAuthorizationConstants';
import EmailConfirmation from './components/confirmation/EmailConfirmation';
import RegistrationConfirmation from './components/confirmation/RegistrationConfirmation';

export default class App extends Component {
    static displayName = App.name;

    render() {
        return (
            <UserContext.Provider value={{
                email: null,
                username: 'Guest',
                isLoggedIn: false,
                token: null
            }}>
                <Switch>
                    <Route exact path="/email-confirmed" component={EmailConfirmation}></Route>
                    <Route exact path="/sign-up-confirm" component={RegistrationConfirmation}></Route>
                    <Route path={ApplicationPaths.ApiAuthorizationPrefix} component={ApiAuthorizationRoutes} />
                    <Route exact path={["/sign-in"]}>
                        <LoginLayout>
                            <Route exact path="/sign-in" component={LoginPage} />
                        </LoginLayout>
                    </Route>
                    <Route exact path={["/sign-up", "/guide/sign-up", "/authentication/register"]}>
                        <RegisterLayout>
                            <Route exact path="/sign-up" component={ClientRegistration} />
                            <Route exact path="/authentication/register" component={ClientRegistration} />
                            <Route exact path="/guide/sign-up" component={GuideRegistration} />
                        </RegisterLayout>
                    </Route>
                    <Layout>
                        <AuthorizeRoute path="/" component={Home} />
                        <AuthorizeRoute path="/profile" component={Profile} />
                    </Layout>
                </Switch>
            </UserContext.Provider>
        );
    }
}
