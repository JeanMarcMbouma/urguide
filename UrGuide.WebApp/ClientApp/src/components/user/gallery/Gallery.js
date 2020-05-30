import React, {
    useState, useMemo 
} from 'react';
import {

    IconButton,
    CircularProgress,
    Avatar,
    CardHeader

} from '@material-ui/core';
import { Link, useParams } from 'react-router-dom';
import { AiFillCloseCircle } from 'react-icons/ai';
import ArrowForwardIosOutlinedIcon from '@material-ui/icons/ArrowForwardIosOutlined';
import ArrowBackIosOutlinedIcon from '@material-ui/icons/ArrowBackIosOutlined';
import "../../post/Post.css";
import { HttpClientFactory } from '../../../httpclient';
import { useAuthContext } from '../../api-authorization/AuthService';
import { CatalogsClient } from '../../../api';



function GalleryLoading() {

    return (
        <div className="post-loading-container">
            <div className="post-loading" >
                <CircularProgress />
            </div>
        </div>
    );
}



export default function Gallery() {

    let { catalogId, imageId } = useParams();
    const [catalog, setCatalog] = useState({});

    const [isLoading, setLoading] = React.useState(true);

    function setDefaultIndex(arr, fromIndex, toIndex) {
        var element = arr[fromIndex];
        arr.splice(fromIndex, 1);
        arr.splice(toIndex, 0, element);
    }


    const { user } = useAuthContext();

    const { profile } = user || {
        profile: {}
    };


    useMemo(async () => {
        const api = HttpClientFactory.get(CatalogsClient);
        try {
            var result = await api.retrieve(catalogId);
            result.files.forEach((img, index) => {

                if (img.id === imageId) {
                    setDefaultIndex(result.files, index, 0);
                }
            });
            setCatalog(result);
            setLoading(false);
        } catch (e) {
            console.log(e);
        }

    }, [user]);


    const [index, setIndex] = React.useState(0);

    function navigateForwardGallery(index) {
        var num = index + 1;
        if (num === catalog.files.length) {
            setIndex(0);
        }
        else {
            setIndex(num);
        }
    }
    function navigateBackGallery(index) {

        if (index === 0) {

            var num = catalog.files.length - 1;
            setIndex(num);
        }
        else {
            var num = index - 1;
            setIndex(num);
        }
    }

    function goBack() {
        window.history.back();
    }

    return (
        isLoading ? <GalleryLoading /> : <div className="post-container-gallery" style={{ backgroundImage: `url(${catalog.files[index].imageBase64})` }}>
                <div className="row">
                    <div className="col-12">
                     
                       <div className="item-photo-gallery" >
                               <div className='close-page-icon-div'>
                                        <IconButton onClick={() => goBack()}>
                                            <AiFillCloseCircle className='close-page-icon' />
                            </IconButton>
                        </div>
                        <div className="author-div">
                            <CardHeader
                                avatar={<Link to={`/g/${catalog.authorId}`} ><Avatar className='author-avatar' alt={catalog.author} src={catalog.authorAvatar} /> </Link>}
                                title={
                                    <h5>
                                        <Link className='authorName' to={`/g/${catalog.authorId}`} >{catalog.author}</Link>
                                    </h5>
                                }

                            />
                        </div>
                                    {
                                        catalog.files.length > 1 ? <div className="container-fluid nav-box">
                                            <div className="row justify-content-between">
                                                <div className="col-2 col-md-1 col-lg-1">
                                                    <IconButton className='nav-btn-div' onClick={() => navigateBackGallery(index)}>
                                                        <ArrowBackIosOutlinedIcon />
                                                    </IconButton>
                                                </div>
                                                <div className="col-2 col-md-1 col-lg-1">
                                                    <IconButton className='nav-btn-div' onClick={() => navigateForwardGallery(index)} >
                                                        <ArrowForwardIosOutlinedIcon />
                                                    </IconButton>
                                                </div>
                                            </div>
                                        </div> : null
                                    }
                                </div>
                            </div>
                        </div>
                    </div>
    );
}
