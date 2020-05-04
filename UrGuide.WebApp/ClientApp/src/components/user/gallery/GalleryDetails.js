import React from "react";
import {
  Grid,
  FormHelperText,
  InputLabel,
  FormControl,
  Input,
  Container
} from "@material-ui/core";
import "./Gallery.css";

export function GalleryDetails(props) {

    const [values, setValues] = React.useState({
        title: '',
        location: '',
        description: '',
    });

    const handleChangedValue = prop => event => {
        setValues({ ...values, [prop]: event.target.value });
    };

    const galleryTitleGrid = props.titleError ? (
        <Grid item xs={12} >
            <FormControl fullWidth variant="outlined" >
                <InputLabel error htmlFor="input-with-icon-adornment">
                    Title
          </InputLabel>
                <Input error id="title" value={values.title}
                    onChange={handleChangedValue("title")} />
            </FormControl>
            <FormHelperText error>Please add a title to this gallery.</FormHelperText>
        </Grid>
    ) : (
            <Grid item xs={12} >
                <FormControl fullWidth variant="outlined">
                    <InputLabel htmlFor="input-with-icon-adornment">
                        Title
          </InputLabel>
                    <Input id="title" value={values.title}
                        onChange={handleChangedValue("title")} />
                </FormControl>
            </Grid>
        );
    const galleryLocationGrid =
        props.locationError ? (
            <Grid item xs={12} >
                <FormControl fullWidth variant="outlined">
                    <InputLabel error htmlFor="input-with-icon-adornment">
                        Location
          </InputLabel>
                    <Input error id="location" value={values.location}
                        onChange={handleChangedValue("location")} />
                </FormControl>
                <FormHelperText error>Please add a location to this gallery.</FormHelperText>
            </Grid>
        ) : (
                <Grid item xs={12} >
                    <FormControl fullWidth variant="outlined">
                        <InputLabel htmlFor="input-with-icon-adornment"> Location</InputLabel>
                        <Input id="location" value={values.location}
                            onChange={handleChangedValue("location")} />
                    </FormControl>
                </Grid>
            );

    const gallerydDescriptionGrid =
        props.descriptionError ? (
            <Grid item xs={12}>
                <FormControl fullWidth variant="outlined">
                    <InputLabel error htmlFor="input-with-icon-adornment">
                        Description
          </InputLabel>
                    <Input error id="description" value={values.description}
                        onChange={handleChangedValue("description")} multiline rows={6} rowsMax={6} />
                </FormControl>
                <FormHelperText error>
                    Your gallery description must have a minimum of 100 characters and a maximum
                    of 500.
        </FormHelperText>
            </Grid>
        ) : (
                <Grid item xs={12}>
                    <FormControl fullWidth variant="outlined">
                        <InputLabel htmlFor="input-with-icon-adornment">
                            Description
          </InputLabel>
                        <Input id="description" value={values.description}
                            onChange={handleChangedValue("description")} multiline rows={6} rowsMax={6} />
                    </FormControl>
                </Grid>
            );

    return ( <Container component="main">
        <Grid container spacing={2}>
            {galleryTitleGrid}
            {galleryLocationGrid}
            {gallerydDescriptionGrid}
        </Grid>
        </Container>)
    ;
}
