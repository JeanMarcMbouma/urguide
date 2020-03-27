import React, { createContext } from "react";

const LoginContext = createContext({
  email: null,
  password: null,
  emailError: false,
  passwordError: false,
  isRemembered: true
});

export default LoginContext;
