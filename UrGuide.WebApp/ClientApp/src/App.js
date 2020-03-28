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
<<<<<<< HEAD
import Home from "./components/MainPage/Home"
=======
import Home from './components/MainPage/Home';

>>>>>>> af79794e9d4cc2ef7a17b71320063e79a7b80b61
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
<<<<<<< HEAD
      <Switch>
        <Route exact path={["/"]}>
          <LoginLayout>
            <Route exact path="/" component={LoginPage} />
          </LoginLayout>
        </Route>
        <Route exact path={["/sign-up", "/guide/sign-up"]}>
          <RegisterLayout>
            <Route exact path="/sign-up" component={ClientRegistration} />
            <Route exact path="/guide/sign-up" component={GuideRegistration} />
          </RegisterLayout>
        </Route>
        <Route path={["/home","/profile"]}>
          <Layout>
            <Route path="/home" component={Home} />
            <Route path="/profile" component={Profile} />
          </Layout>
        </Route>
      </Switch>
=======
        <Switch>
          <Route exact path={["/", "/home"]}>
            <LoginLayout>
              <Route path="/home" component={Home} />
              <Route exact path="/" component={LoginPage} />
            </LoginLayout>
          </Route>
          <Route exact path={["/sign-up", "/guide/sign-up"]}>
            <RegisterLayout>
              <Route exact path="/sign-up" component={ClientRegistration} />
              <Route exact path="/guide/sign-up" component={GuideRegistration} />
            </RegisterLayout>
          </Route>
          <Route path={["/counter", "/fetch-data", "/profile", "/home"]}>
            <Layout>
              <Route path="/counter" component={Counter} />
              <Route path="/profile" component={Profile} />
              <AuthorizeRoute path="/fetch-data" component={FetchData} />
            </Layout>
          </Route>
        </Switch>
>>>>>>> af79794e9d4cc2ef7a17b71320063e79a7b80b61
      </UserContext.Provider>
    );
  }
}
