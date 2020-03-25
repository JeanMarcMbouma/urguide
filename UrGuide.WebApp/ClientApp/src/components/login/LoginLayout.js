import React, { Component } from "react";
export class LoginLayout extends Component {
  static displayName = LoginLayout.name;

  render() {
    return <div>{this.props.children}</div>;
  }
}
