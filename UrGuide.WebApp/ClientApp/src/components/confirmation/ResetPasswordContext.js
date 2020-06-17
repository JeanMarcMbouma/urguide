import React, { createContext } from "react";

const ResetPasswordContext = createContext({
  password: null,
  newPassword: null,
  confirmPassword: null,
  passwordError: false,
  passwordErrorMessage: null,
  returnUrl: null,
});

export default ResetPasswordContext;
