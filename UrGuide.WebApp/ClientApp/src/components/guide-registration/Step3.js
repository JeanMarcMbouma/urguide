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
  function handleChange(event) {
    var file = URL.createObjectURL(event.target.files[0]);
    var elementId = String(event.target.id);
    elementId = elementId.substring(0, elementId.length - 2);

    document.getElementById(elementId).style.backgroundImage =
      "url('" + file + "')";
  }

  const firstPicGrid =
    props.first && !props.newly ? (
      <Grid item xs={12} sm={6} md={4}>
        <Box>
          <div
            className="photo-box text-center"
            id="pic-1"
            onClick={e => document.getElementById("pic-1-i").click()}
          >
            <span>Upload an image here</span>
            <input
              className="input-file"
              id="pic-1-i"
              type="file"
              accept=".png,.jpg"
              onChange={handleChange}
            />
          </div>
        </Box>
        <FormHelperText error>please upload an image here.</FormHelperText>
      </Grid>
    ) : (
      <Grid item xs={12} sm={6} md={4}>
        <Box>
          <div
            className="photo-box text-center"
            id="pic-1"
            onClick={e => document.getElementById("pic-1-i").click()}
          >
            <span>Upload an image here</span>
            <input
              className="input-file"
              id="pic-1-i"
              type="file"
              accept=".png,.jpg"
              onChange={handleChange}
            />
          </div>
        </Box>
      </Grid>
    );

  const secondPicGrid =
    props.second && !props.newly ? (
      <Grid item xs={12} sm={6} md={4}>
        <Box>
          <div
            className="photo-box text-center"
            id="pic-2"
            onClick={e => document.getElementById("pic-2-i").click()}
          >
            <span>Upload an image here</span>
            <input
              className="input-file"
              id="pic-2-i"
              type="file"
              accept=".png,.jpg"
              onChange={handleChange}
            />
          </div>
        </Box>
        <FormHelperText error>please upload an image here.</FormHelperText>
      </Grid>
    ) : (
      <Grid item xs={12} sm={6} md={4}>
        <Box>
          <div
            className="photo-box text-center"
            id="pic-2"
            onClick={e => document.getElementById("pic-2-i").click()}
          >
            <span>Upload an image here</span>
            <input
              className="input-file"
              id="pic-2-i"
              type="file"
              accept=".png,.jpg"
              onChange={handleChange}
            />
          </div>
        </Box>
      </Grid>
    );

  const thirdPicGrid =
    props.third && !props.newly ? (
      <Grid item xs={12} sm={6} md={4}>
        <Box>
          <div
            className="photo-box text-center"
            id="pic-3"
            onClick={e => document.getElementById("pic-3-i").click()}
          >
            <span>Upload an image here</span>
            <input
              className="input-file"
              id="pic-3-i"
              type="file"
              accept=".png,.jpg"
              onChange={handleChange}
            />
          </div>
        </Box>
        <FormHelperText error>please upload an image here.</FormHelperText>
      </Grid>
    ) : (
      <Grid item xs={12} sm={6} md={4}>
        <Box>
          <div
            className="photo-box text-center"
            id="pic-3"
            onClick={e => document.getElementById("pic-3-i").click()}
          >
            <span>Upload an image here</span>
            <input
              className="input-file"
              id="pic-3-i"
              type="file"
              accept=".png,.jpg"
              onChange={handleChange}
            />
          </div>
        </Box>
      </Grid>
    );

  const fourthPicGrid =
    props.fourth && !props.newly ? (
      <Grid item xs={12} sm={6} md={4}>
        <Box>
          <div
            className="photo-box text-center"
            id="pic-4"
            onClick={e => document.getElementById("pic-4-i").click()}
          >
            <span>Upload an image here</span>
            <input
              className="input-file"
              id="pic-4-i"
              type="file"
              accept=".png,.jpg"
              onChange={handleChange}
            />
          </div>
        </Box>
        <FormHelperText error>please upload an image here.</FormHelperText>
      </Grid>
    ) : (
      <Grid item xs={12} sm={6} md={4}>
        <Box>
          <div
            className="photo-box text-center"
            id="pic-4"
            onClick={e => document.getElementById("pic-4-i").click()}
          >
            <span>Upload an image here</span>
            <input
              className="input-file"
              id="pic-4-i"
              type="file"
              accept=".png,.jpg"
              onChange={handleChange}
            />
          </div>
        </Box>
      </Grid>
    );

  const fifthPicGrid =
    props.fifth && !props.newly ? (
      <Grid item xs={12} sm={6} md={4}>
        <Box>
          <div
            className="photo-box text-center"
            id="pic-5"
            onClick={e => document.getElementById("pic-5-i").click()}
          >
            <span>Upload an image here</span>
            <input
              className="input-file"
              id="pic-5-i"
              type="file"
              accept=".png,.jpg"
              onChange={handleChange}
            />
          </div>
        </Box>
        <FormHelperText error>please upload an image here.</FormHelperText>
      </Grid>
    ) : (
      <Grid item xs={12} sm={6} md={4}>
        <Box>
          <div
            className="photo-box text-center"
            id="pic-5"
            onClick={e => document.getElementById("pic-5-i").click()}
          >
            <span>Upload an image here</span>
            <input
              className="input-file"
              id="pic-5-i"
              type="file"
              accept=".png,.jpg"
              onChange={handleChange}
            />
          </div>
        </Box>
      </Grid>
    );

  const sixthPicGrid =
    props.sixth && !props.newly ? (
      <Grid item xs={12} sm={6} md={4}>
        <Box>
          <div
            className="photo-box text-center"
            id="pic-6"
            onClick={e => document.getElementById("pic-6-i").click()}
          >
            <span>Upload an image here</span>
            <input
              className="input-file"
              id="pic-6-i"
              type="file"
              accept=".png,.jpg"
              onChange={handleChange}
            />
          </div>
        </Box>
        <FormHelperText error>please upload an image here.</FormHelperText>
      </Grid>
    ) : (
      <Grid item xs={12} sm={6} md={4}>
        <Box>
          <div
            className="photo-box text-center"
            id="pic-6"
            onClick={e => document.getElementById("pic-6-i").click()}
          >
            <span>Upload an image here</span>
            <input
              className="input-file"
              id="pic-6-i"
              type="file"
              accept=".png,.jpg"
              onChange={handleChange}
            />
          </div>
        </Box>
      </Grid>
    );

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
              3.Now upload photos of places you'd want your guests to visit.
            </h6>
          </div>
        </Box>
      </Grid>
      <Grid container spacing={2}>
        {firstPicGrid}
        {secondPicGrid}
        {thirdPicGrid}
        {fourthPicGrid}
        {fifthPicGrid}
        {sixthPicGrid}
        {consent}
      </Grid>
    </Container>
  );
}
