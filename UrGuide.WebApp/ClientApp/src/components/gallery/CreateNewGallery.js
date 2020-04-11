import React, { Component, useContext, useReducer } from 'react';
import { Route } from 'react-router-dom';
import {
    Grid, Button, IconButton, Box, TextField, Link
} from "@material-ui/core";
import PhotoLibraryIcon from '@material-ui/icons/PhotoLibrary';
import RemoveCircleIcon from '@material-ui/icons/RemoveCircle';
import KeyboardBackspaceIcon from '@material-ui/icons/KeyboardBackspace';
import { GalleryDetails } from "./GalleryDetails";
import GalleryContext from "./GalleryContext";
import GalleryReducer from "./GalleryReducer";
import "./Gallery.css";

function Gallery() {


    const ctx = useContext(GalleryContext);
    const [state, dispatch] = useReducer(GalleryReducer, ctx);
    var currentFile = null;

    let data = state.files;

   
    function handleChange(event) {
        var file = URL.createObjectURL(event.target.files[0]);

        currentFile = {
            id: data.length,
            href: file,
            description: '',
            name:`gallery-${data.length}`
        };

        document.getElementById('data-sender').click();

    }
    

    let Cards = data.map((f, i) => (
        <div className='col-12 col-sm-6 col-md-12 col-lg-6 col-xl-4' key={f.id}>
           
            <div className='card file-card' >
                <div className='thumbnail' style={{ backgroundImage: `url(${f.href})` }}>
                    <div className='cancel-file'>
                        <IconButton onClick={() =>
                            dispatch({
                                type: "remove-file",
                                data: {
                                    idToRemove:f.id,
                                    files: state.files,
                                }
                            })
                        }>
                            <RemoveCircleIcon />
                        </IconButton>
                    </div>
                </div>
                <div className="card-body">
                    <TextField id={f.name} fullWidth label="Description (optional)"  variant="outlined"  multiline rows={4} rowsMax={4} />
                </div>
            </div>
        </div>
    ));

    return (
        <div className="row justify-content-between">
            <div className="col-12 col-md-6 col-lg-5 col-xl-4">
                <div>
                    <Link style={{ textDecoration: `none`, fontSize: `20px`, fontWeight: `bolder` }} href="/user" color="primary">
                        <KeyboardBackspaceIcon fontSize="small" /> Back
                               </Link>
                    <span style={{ fontSize: `20px`, fontWeight: `bold` }}> | Create New Gallery.</span>
                 </div>
                <br/>
                <div className='details-card'>
                    <Grid item xs={12}>
                        <Box mb={5} mt={3}>
                            <div>
                                <span className='text-danger'>{state.emptyGalleryMessage}</span>
                            </div>
                        </Box>
                    </Grid>
                    <Grid item xs={12}>
                        <input type="file" className='input-file' id='file-init' accept=".png,.jpg"
                            onChange={handleChange} />
                        <button id='data-sender' className='input-file' onClick={() =>
                            dispatch({
                                type: "validate-files",
                                data: {
                                    currentFile: currentFile,
                                    files: state.files,
                                }
                            })
                        } >
                        </button>

                        <Button
                            fullWidth
                            variant="contained"
                            color="default"
                            onClick={e => document.getElementById('file-init').click()}
                        >
                            <PhotoLibraryIcon />
                            <span>UPLOAD PHOTOS OR VIDEOS</span>
                        </Button>
                    </Grid>
                    <br />
                    <br />
                    <GalleryDetails titleError={state.titleError} locationError={state.locationError} descriptionError={state.descriptionError} />
                    <Grid item xs={12}>
                        <div className="submit-gallery-btn-div">
                            <Button
                                fullWidth
                                variant="contained"
                                color="primary"
                                onClick={() =>
                                    dispatch({
                                        type: "validate-gallery",
                                        data: {
                                            title: document.getElementById("title").value,
                                            location: document.getElementById("location").value,
                                            description: document.getElementById("description").value,
                                            files: state.files,
                                        }
                                    })
                                }
                            >
                                SUBMIT
                   </Button>
                        </div>
                    </Grid>
                </div>
        </div>
            <div className="col-12 col-md-6 col-lg-7 col-xl-8 gallery-content">
                <div className='row'>
                    {Cards}
                </div>
            </div>
        </div>
        );

}
export class CreateNewGallery extends Component {
    render() {
        return (
            <div className="container-fluid gallery">
               <Gallery />   
            </div>
        )
    }
}
