import React, {  useState, useEffect, useReducer,  } from "react";
import {
    Grid,
    FormHelperText,
    InputLabel,
    FormControl,
    Input,
    Container,
    makeStyles
  } from "@material-ui/core";
import { useAuthContext } from '../api-authorization/AuthService';
import Alert from '@material-ui/lab/Alert';
import Skeleton from '@material-ui/lab/Skeleton';
import { AiOutlineSetting } from 'react-icons/ai';
import { MdVisibility } from 'react-icons/md';
import { MdAddAPhoto } from 'react-icons/md';
import { MdDelete } from 'react-icons/md';
import { MdModeEdit } from 'react-icons/md';
import { Link, useParams  } from 'react-router-dom';
import { HttpClientFactory } from './../../httpclient';
import "./UserStyle.css";
import Button from '@material-ui/core/Button';
import { CatalogsClient, UpdateImageCatalogModel, ImageFileCreateModel, ImagesClient, UpdateClient } from "../../api";
import { Dropdown } from "react-bootstrap";
import { BlobToBase64 } from "../../helpers/fileHelpers";
import { GalleryDetails } from "./gallery/GalleryDetails";
import GalleryReducer from "./gallery/GalleryReducer";


function GalleryCard(props) {

    const [status, setStatus] = useState({ message: '', code: 0 });

    async function deleteGallery() {

        if (!props.user) {
            return;
        }
        const client = HttpClientFactory.get(CatalogsClient, props.user);
        client.remove(props.gallery.catalogId).then(() => {
            props.deleteCb(props.gallery);
        }).catch(() => {
            setStatus({ message: 'Oops ! something went wrong.', code: 400 });
        });
    }

    const [gallery, setGallery] = useState(props.gallery);
    const [editing, setEditing] = useState(false);

    function handleChange(event) {
        const client = HttpClientFactory.get(UpdateClient, props.user);

        const blob = event.target.files[0];
        BlobToBase64(blob, (fileName, base64Url, blobUrl) => {
            var newFile = {
                name: fileName,
                imageBase64: base64Url,
            };
            
            const model = new ImageFileCreateModel(newFile);
            client.addimage(gallery.catalogId, model).then(function (image) {
                let images = gallery.files.slice();
                images.push(image);
                setGallery({ ...gallery, files: images });
                setStatus({ message: 'Save successfully!', code: 200 });
            }).catch(() => {
                setStatus({ message: 'Oops ! something went wrong.', code: 400 });
            });
        });
        event.preventDefault();
        return false;
    }

    function saveChanges(model) {
        const galleryAPI = HttpClientFactory.get(CatalogsClient, props.user);

        galleryAPI.update(gallery.catalogId, new UpdateImageCatalogModel({
            catalogId: gallery.catalogId,
            name: model.title,
            description: model.description
        })).then(() => {
            setEditing(false);
            setGallery({ ...gallery, name: model.title, description: model.description});
            setStatus({ message: 'Saved successfully', code: 200 });
        }).catch((e) => {
            setStatus({ message: 'Oops ! something went wrong.', code: 400 });
        });
    }
    const fileInit = React.createRef();

    const [state, dispatch] = useReducer(GalleryReducer, {...gallery});

    function revertChanges() {
        dispatch({
            type: 'revert-changes',
            data: gallery
        });
        setEditing(false);
    }

    function handleAddImage(e) {
        fileInit.current.click();
        e.preventDefault();
        return false;
    }
    
    return !gallery ? <></> : (<div className="container gallery-card">
        <input type="file" className='input-file' ref={fileInit} accept=".png,.jpg"
            onChange={handleChange} />
        <button id='data-sender' className='input-file'  >
        </button>
        <div className='row justify-content-end'>
            {!props.visitor ?
                <div className='col-3 col-lg-1'>
                    <Dropdown>
                        <Dropdown.Toggle className='dropdown-button' >
                            <AiOutlineSetting className='cog-icon' />
                        </Dropdown.Toggle>

                        <Dropdown.Menu>
                            <Dropdown.Item onClick={handleAddImage} ><span className='md-icon' ><MdAddAPhoto /></span>Add photo</Dropdown.Item>
                            <Dropdown.Item onClick={() => setEditing(!editing)} ><span className='md-icon'><MdModeEdit /></span>  Edit details </Dropdown.Item>
                            <Dropdown.Item onClick={deleteGallery}><span className='md-icon' ><MdDelete /></span>  Delete gallery</Dropdown.Item>
                            {gallery.files.length ? <Dropdown.Item><Link to={`/gallery/${gallery.catalogId}/shot/${gallery.files[0].id}`} ><span className='md-icon'><MdVisibility /></span>  See details</Link></Dropdown.Item> : <></>}
                        </Dropdown.Menu>
                    </Dropdown>
                </div> : null}

        </div>
        <br />
        <div className='row justify-content-start'>
            <div className='col-12'>
                {status.code == 200 ? <Alert severity="success">{status.message}</Alert> : null}
                {status.code == 400 ? <Alert severity="error">{status.message}</Alert> : null}
                <br />
                <br />
                {editing
                    ? <form noValidate autoComplete="off">
                        <GalleryDetails title={state.name}
                            description={state.description}
                            titleError={state.titleError}
                            descriptionError={state.descriptionError}
                        />
                        <Button className="col-lg-2 m-3" onClick={() => dispatch({
                            type: 'update-gallery',
                            data: {
                                title: document.getElementById("title").value,
                                name: document.getElementById("title").value,
                                description: document.getElementById("description").value,
                                files: [],
                                callback: saveChanges
                            }
                        })} variant="contained" color="primary" type="button">Save changes</Button>
                        <Button className="col-lg-2 m-3" onClick={revertChanges} variant="contained" color="primary" type="button">Cancel</Button>
                    </form>
                    : <>
                        <h4>{gallery.name}</h4>
                        <p>{gallery.description}</p>
                    </>}
            </div>
            {gallery.files.map((img, i) => (
                <div key={i} className='col-12 col-sm-6 col-md-4 col-xl-3 photo-div'>
                    <Link to={`/gallery/${gallery.catalogId}/shot/${img.id}`} >  <div className='photo' style={{ backgroundImage: `url(${img.imageBase64})` }}></div></Link>
                </div>))}
        </div>
    </div>);
           
}


function GallerySkeleton() {

    return (
        <div className="container gallery-card">
            <div className='row justify-content-end'>
                    <div className='col-3 col-lg-3'>
                    
                    </div>
                </div>
            <br />
            <div className='row justify-content-start'>
                <div className='col-12'>
                    <Skeleton variant="text" style={{ width: `100%` }} />
                    <Skeleton variant="text" style={{ width: `60%` }} />
                </div>
                <div  className='col-12 col-sm-6 col-md-4 col-xl-3 photo-div'>
                    <Skeleton variant="rect" style={{ height: `190px`, width: `100%` }} />
                </div>
                <div className='col-12 col-sm-6 col-md-4 col-xl-3 photo-div'>
                    <Skeleton variant="rect" style={{ height: `190px`, width: `100%` }} />
                </div>
                <div className='col-12 col-sm-6 col-md-4 col-xl-3 photo-div'>
                    <Skeleton variant="rect" style={{ height: `190px`, width: `100%` }} />
                </div>
                <div className='col-12 col-sm-6 col-md-4 col-xl-3 photo-div'>
                    <Skeleton variant="rect" style={{ height: `190px`, width: `100%` }} />
                </div>
            </div>
        </div>
    );

}



export default function Galleries() {

    let { userId } = useParams();

    const { user } = useAuthContext();

    const { profile } = user;

    const [model, setModel] = useState({ galleries: [], loading: true });

    useEffect(() => {
        let fetch = async () => {

            if (!userId && !profile) {
                return;
            }
            if (userId != null) {
                let client = HttpClientFactory.get(CatalogsClient);

                client.all(userId).then(catalogs => {
                    setModel({ galleries: catalogs, loading: false });
                });
            }
            else {
                let client = HttpClientFactory.get(CatalogsClient);

                client.all(profile.sub).then(catalogs => {
                    setModel({ galleries: catalogs, loading: false });
                });
            }
        };
        fetch();
        return () => { };
    }, [userId, profile]);

    const deleteGallery = (gallery) => {
        var g = model.galleries;
        var index = g.indexOf(gallery);
        if (index !== -1) {
            g.splice(index, 1);
            setModel({ ...model, galleries: g });
        }
    }

    return (
        <div className="row">
            <div className="col-12 lower-section">
                <div className='row justify-content-center'>
                    {userId != null ? <div className="col-12 col-lg-10">
                        {model.loading ? <GallerySkeleton /> : model.galleries.map((gallery, i) => (<GalleryCard key={i} gallery={gallery} userId={userId} visitor={true} user={null}/>))}
                    </div> : <div className="col-12 col-lg-10">
                            {model.loading ? <GallerySkeleton /> : model.galleries.map((gallery, i) => (<GalleryCard key={i} gallery={gallery} userId={profile.sub} visitor={false} user={user} deleteCb={deleteGallery}/>))}
                        </div> }
                </div>
            </div>
        </div>
    );
       
       
}

