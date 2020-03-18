import React, { Component } from "react";
import Avatar from "@material-ui/core/Avatar";
import Button from "@material-ui/core/Button";
import CssBaseline from "@material-ui/core/CssBaseline";
import TextField from "@material-ui/core/TextField";
import FormControlLabel from "@material-ui/core/FormControlLabel";
import FormControl from "@material-ui/core/FormControl";
import Checkbox from "@material-ui/core/Checkbox";
import Link from "@material-ui/core/Link";
import Grid from "@material-ui/core/Grid";
import Box from "@material-ui/core/Box";
// import LockOutlinedIcon from '@material-ui/icons/LockOutlined';
import Typography from "@material-ui/core/Typography";
import { makeStyles } from "@material-ui/core/styles";
import Container from "@material-ui/core/Container";
import Select from "@material-ui/core/Select";
import MenuItem from "@material-ui/core/MenuItem";
import InputLabel from "@material-ui/core/InputLabel";
import Stepper from '@material-ui/core/Stepper';
import Step from '@material-ui/core/Step';
import StepLabel from '@material-ui/core/StepLabel';
import { Step1 } from "./Step1";
import { Step2 } from "./Step2";
import { Step3 } from "./Step3";
//import DateFnsUtils from "@date-io/date-fns";
//import MuiPickersUtilsProvider from "@material-ui/pickers/MuiPickersUtilsProvider";
//import KeyboardTimePicker from "@material-ui/pickers/DateTimePicker/KeyboardTimePicker";
//import KeyboardDatePicker from "@material-ui/pickers/DatePicker/DatePicker";
import "./RegisterLayout.css";

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
    width: '100%',
    backgroundColor: 'white'
  },
  backButton: {
    marginRight: theme.spacing(4),
  },
  instructions: {
    marginTop: theme.spacing(1),
    marginBottom: theme.spacing(1),
  },
}));


function getSteps() {
  return ['Login Details', 'Personal Informations', 'Setup Galery'];
}

function getStepContent(stepIndex) {
  switch (stepIndex) {
    case 0:
      return <Step1 />;
    case 1:
      return <Step2 />;
    case 2:
      return <Step3 />;
    default:
      return <Step1 />;
  }
}

const userStyles = makeStyles(theme => ({
  paper: {
    marginTop: theme.spacing(8),
    display: "flex",
    flexDirection: "column",
    alignItems: "center",
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
 
  const [activeStep, setActiveStep] = React.useState(0);
  const steps = getSteps();

  const handleNext = () => {
    setActiveStep(prevActiveStep => prevActiveStep + 1);
  };

  const handleBack = () => {
    setActiveStep(prevActiveStep => prevActiveStep - 1);
  };

  const handleReset = () => {
    setActiveStep(0);
  };

  var maxWidth = activeStep != 0 ? "md" : "xs";
  var marginLeftValue = activeStep != 0 ? 15 :0;
  var BackButton = activeStep != 0 ? <Button variant="contained"   onClick={handleBack} className={stepperStyles.backButton} >Go Back</Button> : <span></span>
  var NextButton =   activeStep === steps.length - 1 ? <Button variant="contained" color="primary" className="next-btn" id="submit-btn" style = {{marginLeft:marginLeftValue}} >Finish</Button> : <Button style = {{marginLeft:marginLeftValue}} variant="contained" color="primary" type="button" onClick={handleNext} >Continue</Button>
  
  return (
    <div className="guide-registration-wrapper" >
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
          <Box mt={4} mb={4} >
          {getStepContent(activeStep)}
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
      
        <Box mb={18} >
          <form className={userStyles.form} noValidate>
             <Context />
          </form>

        </Box>
      
    );
  }
}
