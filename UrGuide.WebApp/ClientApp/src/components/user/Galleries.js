import React, {  useState, useMemo,  } from "react";
import {
    makeStyles,
    IconButton,
    Button,
} from "@material-ui/core";
import Alert from '@material-ui/lab/Alert';
import Skeleton from '@material-ui/lab/Skeleton';
import { AiOutlineSetting } from 'react-icons/ai';
import { MdVisibility } from 'react-icons/md';
import { MdAddAPhoto } from 'react-icons/md';
import { MdDelete } from 'react-icons/md';
import { MdModeEdit } from 'react-icons/md';
import { Link, useParams  } from 'react-router-dom';
import { useAuthUser } from "../api-authorization/AuthService";
import { HttpClientFactory } from './../../httpclient';
import "./UserStyle.css";
import { CatalogsClient, CreateImageCatalogModel, ImageFileCreateModel, ImagesClient, UpdateClient } from "../../api";
import { Dropdown } from "react-bootstrap";
import { BlobToBase64 } from "../../helpers/fileHelpers";


const buttonStyles = makeStyles(theme => ({
    root: {
        '& > *': {
            margin: theme.spacing(1),
        },
        title: {
            marginLeft:'10px',
            fontSize:'14px',
        },
    },
}));




function GalleryCard(props) {

    const [status, setStatus] = useState({ message: '', code: 0 });

    async function deleteGallery() {

        if (!props.user) {
            return;
        }
        const client = HttpClientFactory.get(CatalogsClient, props.user);

        try {

            await client.remove(props.gallery.catalogId);
            window.location.replace(`${window.location.origin}/profile/galleries`);

        }
        catch (e) {
            console.log(e);
        }
    }

    const [gallery, setGallery] = useState(props.gallery);

    //async function addPhotoToGallery(state) {

    //    if (!props.user) {
    //        return;
    //    }
    //    const client = HttpClientFactory.get(UpdateClient, props.user);
    //    const api = HttpClientFactory.get(CatalogsClient);

    //    const model = new ImageFileCreateModel({
    //        imageBase64: state.imageBase64,
    //        name: state.name,
    //    });

    //    try {

    //        await client.addimage(gallery.catalogId, model);
    //        var result = await api.retrieve(gallery.catalogId);
    //        setGallery(result);
    //        setStatus({ message: 'photo succesfully added to this gallery !', code: 200 });
    //    }
    //    catch (e) {
    //        console.log(e);
    //        setStatus({ message: 'Oops ! something went wrong.', code: 400 });
    //    }
    //}


    async function removePhotoToGallery(state) {

        if (!props.user) {
            return;
        }
        const client = HttpClientFactory.get(ImagesClient, props.user);

        try {

            await client.remove(state.catalogId, state.imageId);

        }
        catch (e) {
            console.log(e);
        }
    }

    const [photo, setPhoto] = useState({});

    function handleChange(event) {

        const blob = event.target.files[0];
        BlobToBase64(blob, (fileName, base64Url, blobUrl) => {
            var newFile = {
                name: fileName,
                imageBase64: base64Url,
            };
            setPhoto(newFile);
            document.getElementById('data-sender').click();
        });

    }



    return !gallery ? <></> : (
        <>
            <div className="container gallery-card">
                <input type="file" className='input-file' id='file-init' accept=".png,.jpg"
                    onChange={handleChange} />
                <button id='data-sender' className='input-file'  >
                </button>
                <div className='row justify-content-end'>
                    {  !props.visitor  ? <div className='col-3 col-lg-1'>
                        <Dropdown>
                            <Dropdown.Toggle className='dropdown-button' >
                                <AiOutlineSetting className='cog-icon' />
                            </Dropdown.Toggle>

                            <Dropdown.Menu>
                                <Dropdown.Item onClick={e => document.getElementById('file-init').click()} ><span className='md-icon' ><MdAddAPhoto /></span> Add photo</Dropdown.Item>
                                <Dropdown.Item><span className='md-icon'><MdModeEdit /></span>  Edit details</Dropdown.Item>
                                <Dropdown.Item onClick={deleteGallery}><span className='md-icon' ><MdDelete /></span>  Delete gallery</Dropdown.Item>
                                { gallery.files.length ? <Dropdown.Item><Link to={`/gallery/${gallery.catalogId}/shot/${gallery.files[0].id}`} ><span className='md-icon'><MdVisibility /></span>  See details</Link></Dropdown.Item> : <></>}
                                
                            </Dropdown.Menu>
                        </Dropdown>
                    </div> : null }
                   
                </div>
                <br />
                <div className='row justify-content-start'>
                    <div className='col-12'>
                        {status.code == 200 ? <Alert severity="success">{status.message}</Alert> : null}
                        {status.code == 400 ? <Alert severity="error">{status.message}</Alert> : null}
                        <br />
                        <br />
                        <h4>{props.gallery.name}</h4>
                        <p>{props.gallery.description}</p>
                    </div>
                    {gallery.files.map((img, i) => (<div key={i} className='col-12 col-sm-6 col-md-4 col-xl-3 photo-div'>
                        <Link to={`/gallery/${gallery.catalogId}/shot/${img.id}`} >  <div className='photo' style={{ backgroundImage: `url(${img.imageBase64})` }}></div></Link>
                    </div>))}
                </div>
            </div></>
    );
           
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

    const user = useAuthUser();

    const { profile } = user || {
        profile: {}
    };

    const [model, setModel] = useState({ galleries: [], loading: true });

    useMemo(async () => {

        if (userId != null) {
            let client = HttpClientFactory.get(CatalogsClient);

            client.all(userId).then(catalogs => {
                setModel({ galleries: catalogs, loading: false });
            });
        }
        else
        {
            let client = HttpClientFactory.get(CatalogsClient);

            client.all(profile.sub).then(catalogs => {
                setModel({ galleries: catalogs, loading: false });
            });
        }
     

    }, [user]);


    return (
        <div className="row">
            <div className="col-12 lower-section">
                <div className='row justify-content-center'>
                    {userId != null ? <div className="col-12 col-lg-10">
                        {model.loading ? <GallerySkeleton /> : model.galleries.map((gallery, i) => (<GalleryCard key={i} gallery={gallery} userId={userId} visitor={true} user={null}  />))}
                    </div> : <div className="col-12 col-lg-10">
                            {model.loading ? <GallerySkeleton /> : model.galleries.map((gallery, i) => (<GalleryCard key={i} gallery={gallery} userId={profile.sub} visitor={false} user={user} />))}
                        </div> }
                </div>
            </div>
        </div>
    );
       
       
}

