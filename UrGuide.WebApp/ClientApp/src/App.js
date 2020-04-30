import React, { Component } from "react";
import { Route} from "react-router";
import { Switch, Redirect  } from "react-router-dom";
import { LoginLayout } from "./components/login/LoginLayout";
import { LoginPage } from "./components/login/LoginPage";
import { RegisterLayout } from "./components/RegisterLayout";
import { ClientRegistration } from "./components/client-registration/ClientRegistration";
import { GuideRegistration } from "./components/guide-registration/GuideRegistration";
import Post from "./components/post/Post";
import Posts from "./components/user/Posts";
import Galleries  from "./components/user/Galleries";
import EditProfile from "./components/user/EditProfile";
import ChangePassword from "./components/user/ChangePassword";
import { CreateNewGallery } from "./components/user/CreateNewGallery";
import { Layout } from "./components/Layout";
import { UserContext } from './UserContext';
import Home from "./components/MainPage/Home";
import Explorer from "./components/explorer/Explorer";
import AuthorizeRoute from "./components/api-authorization/AuthorizeRoute";
import ApiAuthorizationRoutes from "./components/api-authorization/ApiAuthorizationRoutes";
import { ApplicationPaths } from './components/api-authorization/ApiAuthorizationConstants';
import React, {Component} from 'react';
import {Route} from 'react-router';
import {Switch, Redirect} from 'react-router-dom';
import {LoginLayout} from './components/login/LoginLayout';
import {LoginPage} from './components/login/LoginPage';
import {RegisterLayout} from './components/RegisterLayout';
import {
  ClientRegistration,
} from './components/client-registration/ClientRegistration';
import {
  GuideRegistration,
} from './components/guide-registration/GuideRegistration';
import {UserProfile} from './components/user/UserProfile';
import {CreateNewGallery} from './components/gallery/CreateNewGallery';
import {Layout} from './components/Layout';
import Home from './components/MainPage/Home';
import EmailConfirmation from './components/confirmation/EmailConfirmation';
import RegistrationConfirmation from './components/confirmation/RegistrationConfirmation';
import authService from './components/api-authorization/AuthorizeService';
import RegistrationConfirmation
  from './components/confirmation/RegistrationConfirmation';

import {
  AuthenticationProvider,
  oidcLog,
  InMemoryWebStorage,
  withOidcSecure,
} from '@axa-fr/react-oidc-context';
import Loader from './components/api-authorization/loader';

export default class App extends Component {

    constructor(props) {
        super(props);
        this.state = { email: null, username:'', isLoggedIn: false,token:null, user:[] };
    }

    static displayName = Layout.name;

    componentDidMount() {
        this.populateUserData();
    }

    render() {
        return (
            <UserContext.Provider value={{
                email: this.state.email,
                username:this.state.username,
                isLoggedIn: this.state.isLoggedIn,
                token: this.state.token,
                user:this.state.user
            }}>
                <Switch>
                    <Route exact path="/email-confirmed" component={EmailConfirmation}></Route>
                    <Route exact path="/sign-up-confirm" component={RegistrationConfirmation}></Route>
                    <Route path={ApplicationPaths.ApiAuthorizationPrefix} component={ApiAuthorizationRoutes} />
                    <Route path="/sign-in">
                        <LoginLayout>
                            <Route exact path="/sign-in" component={LoginPage} />
                        </LoginLayout>
                    </Route>
                    <Route path="/(sign-up|guide/sign-up|authentication/register)">
                        <RegisterLayout>
                            <Route exact path="/sign-up" component={ClientRegistration} />
                            <Route exact path="/authentication/register" component={ClientRegistration} />
                            <Route exact path="/guide/sign-up" component={GuideRegistration} />
                        </RegisterLayout>
                    </Route>
                    <Layout>
                        <Route path="/user" component={Posts} />
                        <Route path="/galleries" component={Galleries} />
                        <Route path="/edit/profile" component={EditProfile} />
                        <Route path="/edit/password" component={ChangePassword} />
                        <Route path="/explorer" component={Explorer} />
                        <Route path="/feed" component={Home} />
                        <Route exact path='/' render={() => <Redirect to='/feed'></Redirect>}></Route>
                        <Route path="/gallery/new" component={CreateNewGallery} />
                    </Layout>
                </Switch>
            </UserContext.Provider>
        );
    }

    async populateUserData() {
        const user = await authService.getUser();

        console.log(user);

        if (user) {

            const token = await authService.getAccessToken();

            var url = '/Account/userdata/' + `${user.sub}`;
            const response = await fetch(url, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                },
                credentials: 'include',
            });

            const data = await response.json();

            this.setState({ email: data.email, username: data.firstName, isLoggedIn: true, token: token, profile: data.profile, user:data});

        }
    }
}
