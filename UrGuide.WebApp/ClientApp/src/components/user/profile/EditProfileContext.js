import { createContext } from "react";

const EditProfileContext = createContext({
    id:null,
    profileImage: null,
    firstName: null,
    lastName: null,
    gender: null,
    birthDay: null,
    country: null,
    city: null,
    phoneNumber: null,
    address: null,
    description: null,
    profileImageError: false,
    fnameError: false,
    lnameError: false,
    phoneNumberError: false,
    cityError: false,
    addressError: false,
    descriptionError: false,
    requiredErrorMessage:'This field is required.',
    requiredNameErrorMessage: 'Please enter a correct name here.',
    descriptionErrorMessage: 'Your description must have a minimum of 100 characters and a maximum of 500.',
});

export default EditProfileContext;
