export default function LoginReducer(state, action) {
  let context = { ...state };

  switch (action.type) {
    case "validate-login":
      context.email = action.data.email;
      context.password = action.data.password;
      context.isRemembered = action.data.isRemembered;
      let regexEmail = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/;
      let passwordRegex = /^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$/;
      let validEmail = regexEmail.test(context.email);
      let validpassword = passwordRegex.test(context.password);
      context.emailError = validEmail ? false : true;
      context.passwordError = validpassword ? false : true;
      return context;
  }
}
