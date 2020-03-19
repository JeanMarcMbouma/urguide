import React, { Component } from "react";
import { Route } from "react-router";
import { Switch } from "react-router-dom";
import { LoginLayout } from "./components/LoginLayout";
import { LoginPage } from "./components/LoginPage";
import { RegisterLayout } from "./components/RegisterLayout";
import { ClientRegistration } from "./components/ClientRegistration";
import { GuideRegistration } from "./components/GuideRegistration";
import { Layout } from "./components/Layout";
import { FetchData } from "./components/FetchData";
import { Counter } from "./components/Counter";
import AuthorizeRoute from "./components/api-authorization/AuthorizeRoute";
import ApiAuthorizationRoutes from "./components/api-authorization/ApiAuthorizationRoutes";
import { ApplicationPaths } from "./components/api-authorization/ApiAuthorizationConstants";
import { Profile } from "./components/profile/Profile";

export default class App extends Component {
  static displayName = App.name;

  render() {
    return (
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
        <Route path={["/profile"]}>
          <Layout>
            <Route exact path="/profile" component={Profile} />
          </Layout>
        </Route>
      </Switch>
    );

    //  <Layout>
    //   <Route exact path='/' component={Home} />
    //  <Route path='/counter' component={Counter} />
    //  <AuthorizeRoute path='/fetch-data' component={FetchData} />
    //  <Route path={ApplicationPaths.ApiAuthorizationPrefix} component={ApiAuthorizationRoutes} />
    //</Layout>
    //);
  }
}
