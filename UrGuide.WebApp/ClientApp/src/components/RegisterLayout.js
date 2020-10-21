import React, { Component } from "react";
import { Box, Container } from "@material-ui/core";
import { Loader } from "./Layout";

export class RegisterLayout extends Component {
  static displayName = RegisterLayout.name;

  render() {
      return (
          <>
              <Loader/>
                  <Container maxWidth="md">
                  <div>
                      <br />
                      <br />
                      <br/>
                      {this.props.children}</div>
                  </Container>
              </>
    );
  }
}
