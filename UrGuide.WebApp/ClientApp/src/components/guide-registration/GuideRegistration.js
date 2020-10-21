import React, { Component, useReducer, useContext, useEffect } from "react";
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
import { HttpClientFactory } from "../../httpclient";
import { CreateGuideModel, Client } from "../../api";
import { BlobToBase64 } from "../../helpers/fileHelpers";
import { useDataContext, ActionTypes } from "../../data/GlobalDataContext";

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
  return ["Login Details", "Personal Informations", "Terms & Policies"];
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

                      consent={state.isChecked}
                      newly={state.newly}
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


const getReturnUrl = (state) => {
    const params = new URLSearchParams(window.location.search);
    const fromQuery = params.get('ReturnUrl');
    if (fromQuery && !fromQuery.startsWith(`${window.location.origin}/`)) {
        var url = `${window.location.origin}${fromQuery}`;
        return url;
    }
    return (state && state.returnUrl) || fromQuery || `${window.location.origin}/`;
}
const navigateToReturnUrl = (returnUrl) => {
    // It's important that we do a replace here so that we remove the callback uri with the
    // fragment containing the tokens from the browser history.
    window.location.replace(returnUrl);
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



export default function GuideRegistration() {

    const { dcReducer } = useDataContext();

    useEffect(() => {

        dcReducer({ type: ActionTypes.LOADINGCOMPLETED, data: { completed: true, url: "/guide/sign-up/", } });
    }, []);


    function Context() {
        const steps = getSteps();

        const ctx = useContext(GuideContext);
        const [state, dispatch] = useReducer(GuideReducer, ctx);
        var activeStep = state.step;
        var maxWidth = activeStep != 0 ? "md" : "xs";
        var marginLeftValue = activeStep != 0 ? 15 : 0;

        async function createGuide(state) {

            dcReducer({ type: ActionTypes.LOADINGCOMPLETED, data: { completed: false, url: "/guide/sign-up/", } });
            const date = state.birthday;
            const birthday = date.toString();
            const returnUrl = getReturnUrl();
            const api = HttpClientFactory.get(Client);
                BlobToBase64(state.picture, (filename, base64Url, blobUrl) => {
                    api.newguide(returnUrl, new CreateGuideModel({
                        email: state.email,
                        address: state.address,
                        birthDay: birthday,
                        city: state.city,
                        confirmPassword: state.confirmPassword,
                        country: state.country,
                        description: state.description,
                        firstName: state.firstName,
                        gender: state.gender,
                        lastName: state.lastName,
                        password: state.password,
                        phone: state.phone,
                        profileImage: base64Url
                    })).then(() => {

                        dcReducer({ type: ActionTypes.LOADINGCOMPLETED, data: { completed: true, url: "/guide/sign-up/", } });
                        navigateToReturnUrl(`${window.location.origin}/sign-up-confirm`);
                    });
                });
        }
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
                                picture: document.getElementById("profile-pic-input").files[0],
                                firstName: document.getElementById("firstName").value,
                                lastName: document.getElementById("lastName").value,
                                gender: document.getElementsByName("gender")[0].value,
                                birthday: document.getElementById("date-picker-inline").value,
                                country: document.getElementsByName("country")[0].value,
                                city: document.getElementById("city").value,
                                phone: document.getElementById("phone").value,
                                address: document.getElementById("address").value,
                                description: document.getElementById("description").value,
                                isChecked: document.getElementById("guide-checkbox").checked,
                                step: activeStep,

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
                            type: "send-data",
                            data: {
                                email: document.getElementById("guide-email").value,
                                password: document.getElementById("guide-password").value,
                                 confirmPassword: document.getElementById("confirm-password")
                                    .value,
                                profilePic: document.getElementById("profile-pic-input").files
                                    .length,
                                picture: document.getElementById("profile-pic-input").files[0],
                                firstName: document.getElementById("firstName").value,
                                lastName: document.getElementById("lastName").value,
                                gender: document.getElementsByName("gender")[0].value,
                                birthday: document.getElementById("date-picker-inline").value,
                                country: document.getElementsByName("country")[0].value,
                                city: document.getElementById("city").value,
                                phone: document.getElementById("phone").value,
                                address: document.getElementById("address").value,
                                description: document.getElementById("description").value,
                                isChecked: document.getElementById("guide-checkbox").checked,
                                sendData: createGuide,
                                step: activeStep,
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
                                picture: document.getElementById("profile-pic-input").files[0],
                                firstName: document.getElementById("firstName").value,
                                lastName: document.getElementById("lastName").value,
                                gender: document.getElementsByName("gender")[0].value,
                                birthday: document.getElementById("date-picker-inline").value,
                                country: document.getElementsByName("country")[0].value,
                                city: document.getElementById("city").value,
                                phone: document.getElementById("phone").value,
                                address: document.getElementById("address").value,
                                description: document.getElementById("description").value,
                                isChecked: document.getElementById("guide-checkbox").checked,
                                step: activeStep,

                            }
                        })}
                    >
                        Continue
      </Button>
                );

        return (
            <div className="guide-registration-wrapper">
                <Box mt={2} mb={10}>
                    <Stepper style={{ backgroundColor:`transparent` }} activeStep={activeStep} alternativeLabel>
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
                            {getStepContent(state.step, state)}
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

    return (
        <Box mb={18}>
        <form className={userStyles.form} noValidate>
          <Context />
        </form>
      </Box>
    );
  
}
