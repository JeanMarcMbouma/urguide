import { string } from "prop-types";

export default function GuideReducer(state, action) {
  let context = { ...state };
  context.step = action.data.step;
  let regexEmail = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/;
  let passwordRegex = /^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$/;
  let nameRegex = /^[^-\s][\w\s-]+$/;
  //step1
  context.email = action.data.email;
  context.password = action.data.password;
  context.confirmPassword = action.data.confirmPassword;
  //step2
  context.firstName = action.data.firstName;
  context.lastName = action.data.lastName;
  context.profilePic = action.data.profilePic;
  context.gender = action.data.gender;
  context.birthday = action.data.birthday;
  context.country = action.data.country;
  context.city = action.data.city;
  context.address = action.data.address;
  context.phone = action.data.phone;
  context.description = action.data.description;
  //step3
  context.picture1 = action.data.picture1;
  context.picture2 = action.data.picture2;
  context.picture3 = action.data.picture3;
  context.picture4 = action.data.picture4;
  context.picture5 = action.data.picture5;
  context.picture6 = action.data.picture6;
  context.isChecked = action.data.isChecked;
  var description = String(context.description);

  //checking
  let validEmail = regexEmail.test(context.email);
  let validpassword = passwordRegex.test(context.password);
  let validfname = nameRegex.test(context.firstName);
  let validlname = nameRegex.test(context.lastName);
  let isDescriptionGotProperLength =
    description.length > 100 && description.length < 500 ? true : false;
  let validgender = context.gender === "null" ? false : true;
  let validcountry = nameRegex.test(context.country);
  let validcity = nameRegex.test(context.city);
  let validaddress = nameRegex.test(context.address);
  let validdesrcription = nameRegex.test(context.desrcription);

  //errors
  //step1
  context.emailError = validEmail ? false : true;
  context.passwordError = validpassword ? false : true;
  context.passwordsDontMatch =
    context.confirmPassword === context.password ? false : true;

  //step2
  context.profilePicError = context.profilePic != 0 ? false : true;
  context.fnameError = validfname ? false : true;
  context.lnameError = validlname ? false : true;
  context.genderError = validgender ? false : true;
  context.countryError = validcountry ? false : true;
  context.cityError = validcity ? false : true;
  context.phoneError = context.phone != "" ? false : true;
  context.addressError = validaddress ? false : true;
  context.descriptionError =
    isDescriptionGotProperLength && validdesrcription ? false : true;

  //step3
  context.pic1Error = action.data.picture1 != 0 ? false : true;
  context.pic2Error = action.data.picture2 != 0 ? false : true;
  context.pic3Error = action.data.picture3 != 0 ? false : true;
  context.pic4Error = action.data.picture4 != 0 ? false : true;
  context.pic5Error = action.data.picture5 != 0 ? false : true;
  context.pic6Error = action.data.picture6 != 0 ? false : true;
  context.isChecked = action.data.isChecked ? false : true;

  switch (action.type) {
    case "validate-guide":
      //step1
      if (context.step === 0) {
        context.step =
          !context.emailError &&
          !context.passwordError &&
          !context.passwordsDontMatch
            ? context.step + 1
            : context.step;

        context.newly = context.step === 1 ? true : false;

        return context;
      }
      if (context.step === 1) {
        context.step =
          !context.profilePicError &&
          !context.fnameError &&
          !context.lnameError &&
          !context.genderError &&
          !context.countryError &&
          !context.cityError &&
          !context.addressError &&
          !context.descriptionError
            ? context.step + 1
            : context.step;

        context.newly = context.step === 2 ? true : false;

        return context;
      }

    case "go-back":
      context.step = context.step > 0 ? context.step - 1 : context.step;

      context.newly = true;

      return context;

    case "submit":
      context.newly =
        context.step === 2 &&
        !context.pic1Error &&
        !context.pic2Error &&
        !context.pic3Error &&
        !context.pic4Error &&
        !context.pic5Error &&
        !context.pic6Error &&
        !context.isChecked
          ? true
          : false;
      if (context.newly) {
        alert("data sent!"); //send data here.
        return context;
      } else {
        return context;
      }
  }
}
