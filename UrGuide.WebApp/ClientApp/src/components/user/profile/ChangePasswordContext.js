import React, { createContext } from "react";

const ChangePasswordContext = createContext({
    user:null,
    email: null,
    password: null,
    confirmPassword: null,
    currentPassword: null,
    emailError: false,
    passwordError: false,
    passwordsDontMatchError: false,
    currentPasswordError: false,
    emailErrorMessage:null,
    passwordErrorMessage: null,
    passwordsDontMatchErrorMessage: null,
    currentPasswordErrorMessage : null,
    error: null,
    status:0,
});

export default ChangePasswordContext;