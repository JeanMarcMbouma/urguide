import { createContext } from "react";

const ClientContext = createContext({
  firstName: null,
  lastName: null,
  email: null,
  password: null,
  confirmPassword: null,
  fnameError: false,
  lnameError: false,
  emailError: false,
    passwordError: false,
    passwordsDontMatch: false,
  isChecked: true
});

export default ClientContext;
