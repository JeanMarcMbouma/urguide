import React, { Component } from "react";
import { Loader } from "../Layout";


export class LoginLayout extends Component {
  static displayName = LoginLayout.name;

  render() {
      return <div>
          <Loader/>
          {this.props.children}
      </div>;
  }
}
