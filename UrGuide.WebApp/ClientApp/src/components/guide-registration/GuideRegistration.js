import React, { Component, useReducer, useContext } from "react";
import {
  Button,
  Link,
  Box,
  Container,
  Step,
  StepLabel,
  Stepper,
  Typography,
  makeStyles
} from "@material-ui/core";
import GuideContext from "./GuideContext";
import GuideReducer from "./GuideReducer";
import { Step1 } from "./Step1";
import { Step2 } from "./Step2";
import { Step3 } from "./Step3";
import "./GuideRegistration.css";

function Copyright() {
  return (
    <Typography variant="body2" color="textSecondary" align="center">
      {"Copyright © "}
      <Link color="inherit" href="https://material-ui.com/">
        Your Website
      </Link>{" "}
      {new Date().getFullYear()}
      {"."}
    </Typography>
  );
}

const stepperStyles = makeStyles(theme => ({
  root: {
    width: "100%",
    backgroundColor: "white"
  },
  backButton: {
    marginRight: theme.spacing(4)
  },
  instructions: {
    marginTop: theme.spacing(1),
    marginBottom: theme.spacing(1)
  }
}));

function getSteps() {
  return ["Login Details", "Personal Informations", "Setup Gallery"];
}

function getStepContent(stepIndex, state) {
  switch (stepIndex) {
    case 0:
      return (
        <div>
          <div className="active">
            <Step1
              emailError={state.emailError}
              passwordError={state.passwordError}
              passwordsDontMatch={state.passwordsDontMatch}
            />
          </div>
          <div className="not-active">
            <Step2
              profilePicError={state.profilePicError}
              fnameError={state.fnameError}
              lnameError={state.lnameError}
              birthdayError={state.birthdayError}
              cityError={state.cityError}
              phoneError={state.phoneError}
              addressError={state.addressError}
              descriptionError={state.descriptionError}
            />
          </div>
          <div className="not-active">
            <Step3
              first={state.pic1Error}
              second={state.pic2Error}
              third={state.pic3Error}
              fourth={state.pic4Error}
              fifth={state.pic5Error}
              sixth={state.pic6Error}
              consent={state.isChecked}
            />
          </div>
        </div>
      );

    case 1:
      return (
        <div>
          <div className="not-active">
            <Step1
              emailError={state.emailError}
              passwordError={state.passwordError}
              passwordsDontMatch={state.passwordsDontMatch}
              newly={state.newly}
            />
          </div>
          <div className="active">
            <Step2
              profilePicError={state.profilePicError}
              fnameError={state.fnameError}
              lnameError={state.lnameError}
              birthdayError={state.birthdayError}
              cityError={state.cityError}
              phoneError={state.phoneError}
              addressError={state.addressError}
              descriptionError={state.descriptionError}
              newly={state.newly}
            />
          </div>
          <div className="not-active">
            <Step3
              first={state.pic1Error}
              second={state.pic2Error}
              third={state.pic3Error}
              fourth={state.pic4Error}
              fifth={state.pic5Error}
              sixth={state.pic6Error}
              consent={state.isChecked}
              newly={state.newly}
            />
          </div>
        </div>
      );
    case 2:
      return (
        <div>
          <div className="not-active">
            <Step1
              emailError={state.emailError}
              passwordError={state.passwordError}
              passwordsDontMatch={state.passwordsDontMatch}
              newly={state.newly}
            />
          </div>
          <div className="not-active">
            <Step2
              profilePicError={state.profilePicError}
              fnameError={state.fnameError}
              lnameError={state.lnameError}
              birthdayError={state.birthdayError}
              cityError={state.cityError}
              phoneError={state.phoneError}
              addressError={state.addressError}
              descriptionError={state.descriptionError}
              newly={state.newly}
            />
          </div>
          <div className="active">
            <Step3
              first={state.pic1Error}
              second={state.pic2Error}
              third={state.pic3Error}
              fourth={state.pic4Error}
              fifth={state.pic5Error}
              sixth={state.pic6Error}
              consent={state.isChecked}
              newly={state.newly}
            />
          </div>
        </div>
      );
    default:
      return (
        <div>
          <div className="active">
            <Step1 />
          </div>
          <div className="not-active">
            <Step2 />
          </div>
          <div className="not-active">
            <Step3 />
          </div>
        </div>
      );
  }
}

const userStyles = makeStyles(theme => ({
  paper: {
    marginTop: theme.spacing(8),
    display: "flex",
    flexDirection: "column",
    alignItems: "center"
  },
  avatar: {
    margin: theme.spacing(1),
    backgroundColor: theme.palette.secondary.main
  },
  form: {
    width: "100%", // Fix IE 11 issue.
    marginTop: theme.spacing(3)
  },
  submit: {
    margin: theme.spacing(3, 0, 2)
  },
  formControl: {
    margin: theme.spacing(1),
    minWidth: 120
  }
}));

function Context() {
  const steps = getSteps();

  var activeStep = 0;

  const ctx = useContext(GuideContext);
  const [state, dispatch] = useReducer(GuideReducer, ctx);
  activeStep = state.step;
  var maxWidth = activeStep != 0 ? "md" : "xs";
  var marginLeftValue = activeStep != 0 ? 15 : 0;

  var BackButton =
    activeStep != 0 ? (
      <Button
        variant="contained"
        onClick={() =>
          dispatch({
            type: "go-back",
            data: {
              email: document.getElementById("guide-email").value,
              password: document.getElementById("guide-password").value,
              confirmPassword: document.getElementById("confirm-password")
                .value,
              profilePic: document.getElementById("profile-pic-input").files
                .length,
              firstName: document.getElementById("firstName").value,
              lastName: document.getElementById("lastName").value,
              gender: document.getElementById("gender").value,
              birthday: document.getElementById("date-picker-inline").value,
              country: document.getElementById("country").value,
              city: document.getElementById("city").value,
              phone: document.getElementById("phone").value,
              address: document.getElementById("address").value,
              description: document.getElementById("description").value,
              picture1: document.getElementById("pic-1-i").files.length,
              picture2: document.getElementById("pic-2-i").files.length,
              picture3: document.getElementById("pic-3-i").files.length,
              picture4: document.getElementById("pic-4-i").files.length,
              picture5: document.getElementById("pic-5-i").files.length,
              picture6: document.getElementById("pic-6-i").files.length,
              isChecked: document.getElementById("guide-checkbox").checked,
              step: activeStep
            }
          })
        }
        className={stepperStyles.backButton}
      >
        Go Back
      </Button>
    ) : (
      <span></span>
    );
  var NextButton =
    activeStep === steps.length - 1 ? (
      <Button
        variant="contained"
        color="primary"
        className="next-btn"
        id="submit-btn"
        style={{ marginLeft: marginLeftValue }}
        onClick={() =>
          dispatch({
            type: "submit",
            data: {
              email: document.getElementById("guide-email").value,
              password: document.getElementById("guide-password").value,
              confirmPassword: document.getElementById("confirm-password")
                .value,
              profilePic: document.getElementById("profile-pic-input").files
                .length,
              firstName: document.getElementById("firstName").value,
              lastName: document.getElementById("lastName").value,
              gender: document.getElementById("gender").value,
              birthday: document.getElementById("date-picker-inline").value,
              country: document.getElementById("country").value,
              city: document.getElementById("city").value,
              phone: document.getElementById("phone").value,
              address: document.getElementById("address").value,
              description: document.getElementById("description").value,
              picture1: document.getElementById("pic-1-i").files.length,
              picture2: document.getElementById("pic-2-i").files.length,
              picture3: document.getElementById("pic-3-i").files.length,
              picture4: document.getElementById("pic-4-i").files.length,
              picture5: document.getElementById("pic-5-i").files.length,
              picture6: document.getElementById("pic-6-i").files.length,
              isChecked: document.getElementById("guide-checkbox").checked,
              step: activeStep
            }
          })
        }
      >
        Finish
      </Button>
    ) : (
      <Button
        style={{ marginLeft: marginLeftValue }}
        variant="contained"
        color="primary"
        type="button"
        onClick={() => dispatch({
          type: "validate-guide",
          data: {
            email: document.getElementById("guide-email").value,
            password: document.getElementById("guide-password").value,
            confirmPassword: document.getElementById("confirm-password").value,
            profilePic: document.getElementById("profile-pic-input").files.length,
            firstName: document.getElementById("firstName").value,
            lastName: document.getElementById("lastName").value,
            gender: document.getElementById("gender").value,
            birthday: document.getElementById("date-picker-inline").value,
            country: document.getElementById("country").value,
            city: document.getElementById("city").value,
            phone: document.getElementById("phone").value,
            address: document.getElementById("address").value,
            description: document.getElementById("description").value,
            picture1: document.getElementById("pic-1-i").files.length,
            picture2: document.getElementById("pic-2-i").files.length,
            picture3: document.getElementById("pic-3-i").files.length,
            picture4: document.getElementById("pic-4-i").files.length,
            picture5: document.getElementById("pic-5-i").files.length,
            picture6: document.getElementById("pic-6-i").files.length,
            isChecked: document.getElementById("guide-checkbox").checked,
            step: activeStep
          }
        })}
      >
        Continue
      </Button>
    );

  return (
    <div className="guide-registration-wrapper">
      <Box mt={2} mb={10}>
        <Stepper activeStep={activeStep} alternativeLabel>
          {steps.map(label => (
            <Step key={label}>
              <StepLabel>{label}</StepLabel>
            </Step>
          ))}
        </Stepper>
      </Box>
      <div>
        <Container maxWidth={maxWidth}>
          <Box mt={4} mb={4}>
            {getStepContent(activeStep, state)}
          </Box>
          <Box ml={3}>
            {BackButton}
            {NextButton}
          </Box>
        </Container>
      </div>
    </div>
  );
}

export class GuideRegistration extends Component {
  static displayName = GuideRegistration.name;

  render() {
    return (
      <Box mb={18}>
        <form className={userStyles.form} noValidate>
          <Context />
        </form>
      </Box>
    );
  }
}
