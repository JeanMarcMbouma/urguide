import React, { Component, useContext, useReducer } from 'react';
import {
    Grid, Button, IconButton, Box, Avatar, Typography
} from "@material-ui/core";
import PhotoLibraryIcon from '@material-ui/icons/PhotoLibrary';
import RemoveCircleIcon from '@material-ui/icons/RemoveCircle';
import { GalleryDetails } from "../../components/user/gallery/GalleryDetails";
import GalleryContext from "../../components/user/gallery/GalleryContext";
import GalleryReducer from "../../components/user/gallery/GalleryReducer";
import "../user/gallery/Gallery.css";
import { useAuthUser } from '../api-authorization/AuthService';
import { HttpClientFactory } from '../../httpclient';
import { CatalogsClient, CreateImageCatalogModel, ImageFileCreateModel } from '../../api';
import { BlobToBase64 } from '../../helpers/fileHelpers';

function Gallery() {

    const user = useAuthUser();

    async function createGallery(state) {

        if (!user) {
            return;
        }
        const client = HttpClientFactory.get(CatalogsClient, user);

        const model = new CreateImageCatalogModel({
            name: state.title,
            files: state.files.map(i => new ImageFileCreateModel({ ...i })),
        });

        try {

            await client.create(model);
            window.location.replace(`${window.location.origin}/profile/galleries`);

        }
        catch (e) {
            console.log(e);
        }
    }


    const ctx = useContext(GalleryContext);
    const [state, dispatch] = useReducer(GalleryReducer, ctx);
    var currentFile = null;

    let data = state.files;

   
    function handleChange(event) {
       
        const blob = event.target.files[0];
        BlobToBase64(blob, (fileName, base64Url, blobUrl) => {
            var newFile = {
                index: data.length,
                href: blobUrl,
                name: fileName,
                imageBase64: base64Url,
                prop: `gallery${data.length}`
            }
            state.files.push(newFile);
            document.getElementById('data-sender').click();
        });
       
    }

    let Cards = data.map((f, i) => (
        <div className='col-12 col-sm-6 col-md-12 col-lg-6' key={i}>
           
            <div className='card file-card' >
                <div className='thumbnail' style={{ backgroundImage: `url(${f.href})` }}>
                    <div className='cancel-file'>
                        <IconButton onClick={() =>
                            dispatch({
                                type: "remove-file",
                                data: {
                                    idToRemove:f.index,
                                    files: state.files,
                                }
                            })
                        }>
                            <RemoveCircleIcon />
                        </IconButton>
                    </div>
                </div>
            </div>
        </div>
    ));

    return (
        <div className="row justify-content-between">
            <div className="col-12 col-md-6 col-lg-5 col-xl-4">
                <div className='details-card'>
                    <Grid item xs={12}>
                    <div className="col-lg-12 d-flex justify-content-start my-4">
                        <Avatar alt={user.profile.given_name} src={user.profile.picture} />
                        <Typography className="m-0 px-4" variant="h6">
                            {user.profile.given_name}
                        </Typography>
                        </div>
                    </Grid>
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
                                        type: "create-gallery",
                                        data: {
                                            title: document.getElementById("title").value,
                                            description: document.getElementById("description").value,
                                            files: state.files,
                                            callback: createGallery
                                        }
                                    })
                                }
                            >
                                CREATE GALLERY
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
            <div className="row">
                <div className="col-12 lower-section gallery">
                    <Gallery />
                </div>
            </div>
        )
    }
}
