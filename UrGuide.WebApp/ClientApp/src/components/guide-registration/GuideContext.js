import React, { createContext } from "react";

const GuideContext = createContext({
  email: null,
  password: null,
  confirmPassword: null,
    profilePic: null,
  picture:null,
  firstName: null,
  lastName: null,
  gender: null,
  birthday: null,
  country: null,
  city: null,
  phone: null,
  address: null,
  description: null,
  emailError: false,
  passwordError: false,
  passwordsDontMatch: false,
  profilePicError: false,
  fnameError: false,
  lnameError: false,
  phoneError: false,
  cityError: false,
  addressError: false,
  genderError: false,
  descriptionError: false,
  isChecked: true,
  step: 0,
    newly: true,

});

export default GuideContext;
