import React, { createContext } from "react";

const LoginContext = createContext({
  email: null,
  password: null,
  emailError: false,
    passwordError: false,
    passwordErrorMessage: null,
  LoginFailed:'',
  isRemembered: true,
  returnUrl: null,
});

export default LoginContext;
