import React from "react";
import {
  Grid,
  Box,
  FormHelperText,
  Container,
  Checkbox,
  CssBaseline,
  FormControlLabel
} from "@material-ui/core";
import "./GuideRegistration.css";

export function Step3(props) {
  //function handleChange(event) {
  //  var file = URL.createObjectURL(event.target.files[0]);
  //  var elementId = String(event.target.id);
  //  elementId = elementId.substring(0, elementId.length - 2);

  //  document.getElementById(elementId).style.backgroundImage =
  //    "url('" + file + "')";
  //}


    const terms = (<><Grid item xs={12}>
        <Box mt={5}>
            <h6>1. Lorem Ipsum is simply dummy text of the printing and typesetting industry.</h6>
            <p>
              Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum
          </p>
                </Box>
    </Grid>
        <Grid item xs={12}>
            <Box mt={5}>
                <h6>2. Lorem Ipsum is simply dummy text of the printing and typesetting industry.</h6>
                <p>
                    Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum
          </p>
            </Box>
        </Grid>
        <Grid item xs={12}>
            <Box mt={5}>
                <h6>3. Lorem Ipsum is simply dummy text of the printing and typesetting industry.</h6>
                <p>
                    Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum
          </p>
            </Box>
        </Grid>
        <Grid item xs={12}>
            <Box mt={5}>
                <h6>4. Lorem Ipsum is simply dummy text of the printing and typesetting industry.</h6>
                <p>
                    Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum
          </p>
            </Box>
        </Grid>
    </>);

  const consent =
    props.consent && !props.newly ? (
      <Grid item xs={12}>
        <Box mt={5}>
          <FormControlLabel
            control={
              <Checkbox
                value="allowExtraEmails"
                id="guide-checkbox"
                color="primary"
              />
            }
            label="I agree with the Terms and Conditons."
          />
        </Box>
        <FormHelperText error>
          please check to agree with the Terms and Conditons for signing up.
        </FormHelperText>
      </Grid>
    ) : (
      <Grid item xs={12}>
        <Box mt={5}>
          <FormControlLabel
            control={
              <Checkbox
                value="allowExtraEmails"
                id="guide-checkbox"
                color="primary"
              />
            }
            label="I agree with the Terms and Conditons."
          />
        </Box>
      </Grid>
    );

  return (
    <Container component="main">
      <CssBaseline />
      <Grid item xs={12}>
        <Box mb={5}>
          <div>
            <h6>
              3.Before using Urguide you must carefully read our terms and policies.
            </h6>
          </div>
        </Box>
      </Grid>
      <Grid container spacing={2}>
              {terms}
        {consent}
      </Grid>
    </Container>
  );
}
