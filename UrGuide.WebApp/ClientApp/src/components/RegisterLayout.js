import React, { Component } from "react";
import Box from "@material-ui/core/Box";
import Container from "@material-ui/core/Container";

export class RegisterLayout extends Component {
  static displayName = RegisterLayout.name;

  render() {
    return (
      <Container maxWidth="md">
        <Box mt={5}>
          <div>{this.props.children}</div>
        </Box>
      </Container>
    );
  }
}
