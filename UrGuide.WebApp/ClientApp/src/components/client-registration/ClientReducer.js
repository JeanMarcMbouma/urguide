export default function ClientReducer(state, action) {
  let context = { ...state };

  switch (action.type) {
    case "validate":
      context.firstName = action.data.firstName;
      context.lastName = action.data.lastName;
      context.email = action.data.email;
      context.password = action.data.password;
      context.isChecked = action.data.isChecked;
      let nameRegex = /^[^-\s][\w\s-]+$/;
      let regexEmail = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/;
      let passwordRegex = /^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$/;
      let validfname = nameRegex.test(context.firstName);
      let validlname = nameRegex.test(context.lastName);
      let validEmail = regexEmail.test(context.email);
      let validpassword = passwordRegex.test(context.password);
      context.emailError = validEmail ? false : true;
      context.fnameError = validfname ? false : true;
      context.lnameError = validlname ? false : true;
      context.passwordError = validpassword ? false : true;
      return context;
  }
}
