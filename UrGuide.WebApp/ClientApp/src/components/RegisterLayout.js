import React, { Component } from "react";
import { Box, Container } from "@material-ui/core";

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
