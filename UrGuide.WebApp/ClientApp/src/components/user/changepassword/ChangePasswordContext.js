import React, { createContext } from "react";

const ChangePasswordContext = createContext({
    email: null,
    password: null,
    confirmPassword: null,
    currentPassword: null,
    emailError: false,
    passwordError: false,
    confirmPasswordError: false,
    currentPasswordError: false,
    emailErrorMessage:null,
    passwordErrorMessage: null,
    passwordDontMatchError: null,
    error:null,
});

export default ChangePasswordContext;