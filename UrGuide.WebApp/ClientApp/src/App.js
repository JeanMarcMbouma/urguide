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
  constructor (props) {
    super (props);
    const baseUrl = 'https://localhost:5001';
    this.configuration = {
      client_id: 'UrGuide.WebApp',
      redirect_uri: `${baseUrl}/authentication/login-callback`,
      response_type: 'code',
      post_logout_redirect_uri: baseUrl,
      scope: 'openid profile',
      authority: '/',
      silent_redirect_uri: `${baseUrl}/authentication/silent_callback`,
      automaticSilentRenew: true,
      loadUserInfo: true,
    };
    console.log (this.configuration);
  }
  static displayName = App.name;

  render () {
    return (
      <AuthenticationProvider
        configuration={this.configuration}
        loggerLevel={oidcLog.DEBUG}
        isEnabled={true}
        callbackComponentOverride={Home}
        UserStore={InMemoryWebStorage}
        authenticating={Loader}
      >
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
          <Route path="/(user|feed|gallery/new)">
            <Layout>

              <Route path="/user" component={withOidcSecure (UserProfile)} />
              <Route path="/feed" component={Home} />

              <Route
                path="/gallery/new"
                component={withOidcSecure (CreateNewGallery)}
              />
            </Layout>
          </Route>
          <Route exact path="/" render={() => <Redirect to="/feed" />} />
        </Switch>
      </AuthenticationProvider>
    );
  }
}
