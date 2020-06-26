import React, {
    useState, useContext, useMemo, useReducer, Component, useEffect
} from 'react';
import {
    CardHeader,
    Avatar,
    IconButton,
    Typography,
    Button,
    Paper,
    TextField,
    CardActions,
    CardContent,
    Grid,
    CircularProgress

} from '@material-ui/core';
import { Link, useParams } from 'react-router-dom';
import { FaRegComment } from 'react-icons/fa';
import { AiOutlineDislike } from 'react-icons/ai';
import { AiOutlineLike } from 'react-icons/ai';
import { AiFillDislike } from 'react-icons/ai';
import { AiFillLike } from 'react-icons/ai';
import { AiFillCloseCircle } from 'react-icons/ai';
import { AiOutlineStop } from 'react-icons/ai';
import { AiOutlineCheck } from 'react-icons/ai';
import EventIcon from '@material-ui/icons/Event';
import ArrowForwardIosOutlinedIcon from '@material-ui/icons/ArrowForwardIosOutlined';
import ArrowBackIosOutlinedIcon from '@material-ui/icons/ArrowBackIosOutlined';
import LocationOnIcon from '@material-ui/icons/LocationOn';
import Skeleton from '@material-ui/lab/Skeleton';
import Rating from '@material-ui/lab/Rating';
import Slider from '@material-ui/core/Slider';
import AttachMoneyOutlinedIcon from '@material-ui/icons/AttachMoneyOutlined';
import PeopleOutlineOutlinedIcon from '@material-ui/icons/PeopleOutlineOutlined';
import AlarmOutlinedIcon from '@material-ui/icons/AlarmOutlined';
import "./Post.css";
import { withStyles } from '@material-ui/core/styles';
import NewBidReducer from '../MainPage/CentralBar/NewBidReducer';
import NewBidContext from '../MainPage/CentralBar/NewBidContext';
import ActionsContext from '../MainPage/CentralBar/ActionsContext';
import ActionsReducer from '../MainPage/CentralBar/ActionsReducer';
import { useAuthUser } from '../api-authorization/AuthService';
import { useAuthContext } from '../api-authorization/AuthService';
import authService from '../api-authorization/AuthService';
import { HttpClientFactory } from '../../httpclient';
import { PostsClient, ItineraryModel, BidModel, UserReactionModel, BidClient } from '../../api';


function Header(props) {
    return (<div>
        <CardHeader
            avatar={<Link to={`/user/${props.post.authorId}`}><Avatar alt={props.post.author} src={props.post.authorAvatar} /></Link>}
            title={<Typography variant="body1" component="p"><Link to='/user'>{props.post.author}</Link></ Typography>}
            subheader={`${props.post.location}, ${props.post.publicationDate}`}
        />
        </div>);

}


function PostLoading() {

    return (
        <div className="post-loading-container">
            <div className="post-loading" >
                <CircularProgress />
            </div>
        </div>
        );
}


function Itinerary(props) {


    const [itineraries, setItineraries] = useState([]);
    const [isLoading, setLoading] = useState(true);

    useEffect(() => {

        var fetch = async () => {

            if (!props.show)
                return;
            const api = HttpClientFactory.getPostClient();

            try {
                var result = await api.itineraries(props.postId);
                setItineraries(result);
                setLoading(false);
            }
            catch (e) {
                console.log(e);
            }
        }

        fetch();
        return () => { };
    }, []);


    return (
      
        isLoading && props.show  ? <><br /> <h4 className='text-center'><CircularProgress  /></h4></> :
                props.show ? <div className='itinerary_wrapper'>
                    {itineraries.length > 0 ? <h5>Itinerary of this tour</h5> : <h5>No itinerary found.</h5>}
                    <section className="itinerary">
                        {itineraries.map((itinerary, i) => (
                            <div className="itinerary__block" key={itinerary.ordinal}  >
                                <div className="itinerary__midpoint"></div>
                                <div className="itinerary__content itinerary__content--left">
                                    <h3 className="itinerary__place">{itinerary.title}</h3>
                                    <p className="itinerary__text--left">
                                        {itinerary.description}
                                    </p>
                                </div>
                            </div>
                        ))}
                    </section>
                </div> : null
       
    );

}


function Comments(props) {

    const { manager, user } = useAuthContext();

    const { profile } = user || {
        profile: {}
    };

    async function signIn(e) {
        e.preventDefault();
        if (!user)
            await manager.signIn(window.location.href);
        return false;
    }

    const ctx = useContext(NewBidContext);
    const [state, dispatch] = useReducer(NewBidReducer, ctx);

    const marks = [
        {
            value: 5,
            label: '$5',
        },

        {
            value: 250,
            label: '$250',
        },
    ];

    const [bids, setBids] = useState([]);
    const [isLoading, setLoading] = useState(true);


    useEffect(() => {

        var fetch = async () => {

            if (!props.postId || !props.show) {
                return;
            }
            const api = HttpClientFactory.get(BidClient);

            try {
                var result = await api.history(props.postId);
                setBids(result);
                setLoading(false);
            }
            catch (e) {
                console.log(e);
            }
        }

        fetch();
        return () => { };
    }, []);

    const [bid, setBid] = React.useState(25);
    const handleChangeBid = (event, newValue) => {
        setBid(newValue);

    };

    async function createNewBid(state) {

        console.log(user);
        if (!state.postId) {
            return;
        }
        const client = HttpClientFactory.get(BidClient, user);

        const model = new BidModel({
            postId: state.postId,
            value: state.value,
        });

        try {

            await client.newbid(state.postId, model);
            var result = await client.history(state.postId);
            setBids(result);
        }
        catch (e) {
            console.log(e);
        }

    }

    async function acceptBid(event) {


        if (!props.postId) {
            return;
        }
        const client = HttpClientFactory.get(BidClient, user);
        try {

            var target = event.target;
            target.style.color = '#1dc76f';
            target.style.fontWeight = '600';
            target.innerHTML = '<b>Accepted</b>';
            await client.accept(props.postId);

        }
        catch (e) {
            console.log(e);
        }

    }
    async function rejectBid(event) {

        if (!props.postId) {
            return;
        }
        const client = HttpClientFactory.get(BidClient, user);
        try {

            var target = event.target;
            target.style.color = '#d4144f';
            target.style.fontWeight = '600';
            target.innerHTML = '<b>Rejected</b>';
            await client.reject(props.postId);

        }
        catch (e) {
            console.log(e);
        }

    }


    function valuetext(value) {
        return `${value}`;
    }

    return (
        isLoading && props.show ? <><br /> <h4 className='text-center'><CircularProgress /></h4></> :
            props.show ? <div className='comments' >
            <div noValidate autoComplete="off" className='new-bid'>
                <Grid item xs={12} >
                    <Typography id="bid-slider" gutterBottom>
                        How much would you bid on this tour ?
      </Typography>
                    <Slider
                        defaultValue={bid}
                        getAriaValueText={valuetext}
                        aria-labelledby="bid-slider"
                        step={1}
                        marks={marks}
                        min={5}
                        max={250}
                        value={bid}
                        onChange={handleChangeBid}
                        valueLabelDisplay="auto"
                    />
                </Grid>
                {user ? <Button variant="contained" color="primary"
                    onClick={() =>
                        dispatch({
                            type: "new-bid",
                            data: {
                                postId: props.postId,
                                value: bid,
                                callback: createNewBid,
                            }
                        })
                    }
                >
                    Bid now
                    </Button> : <Button variant="contained" color="primary"
                        onClick={signIn}
                    >
                        Bid now
                    </Button>}
            </div>
            {
                bids.length > 0 ? <>
                    <h6>Bids History ({bids.length})</h6>
                    <br />
                    {bids.map((bid, i) => (
                        <div className='cmt-div' key={i} >
                            <CardHeader
                                avatar={<Avatar alt={bid.author} src={bid.authorImage} />}
                                title={
                                    <h6>
                                        {bid.author}
                                    </h6>
                                }
                                subheader={bid.created}
                            />
                            <div className='comment-text'>
                                <p>{bid.author} made a proposal of {bid.value}.</p>


                                {user && profile.sub === props.post.authorId ?
                                    <div className='row'>
                                        <div className='col-5 col-md-5'>
                                            <span onClick={(e) => acceptBid(e)} className='accept'>Accept  <AiOutlineCheck /></span>
                                        </div>
                                        <div className='col-5 col-md-5'>
                                            <span className='reject' onClick={(e) => rejectBid(e)} >Reject   <AiOutlineStop /></span>

                                        </div>
                                    </div>
                                    : null

                                }
                            </div>
                        </div>))} </> : <><h6>No bid yet.</h6><br/><br/></>



            }
        </div> : null
    );
}




export default function Post() {

    let { postId, imageId } = useParams();
    const [post, setPost] = useState({});
    const actionCtx = useContext(ActionsContext);
    const [state, dispatchAction] = useReducer(ActionsReducer, actionCtx);

    const { manager, user } = useAuthContext();

    const { profile } = user || {
        profile: {}
    };

    const [isLoading, setLoading] = React.useState(true);

   

    async function handleReaction(state) {

        const client = HttpClientFactory.getPostClient(user);

        const model = new UserReactionModel({
            postId: state.post.id,
            like: state.like,
        });

        try {

            await client.reaction(state.post.id, model);

        }
        catch (e) {
            console.log(e);
        }

    }

    function setDefaultIndex(arr, fromIndex, toIndex) {
        var element = arr[fromIndex];
        arr.splice(fromIndex, 1);
        arr.splice(toIndex, 0, element);
    }

    useEffect(() => {
        var fetch = async () => {
            const api = HttpClientFactory.get(PostsClient, user);
            try {
                var result = await api.retrieve(postId);
                result.images.forEach((img, index) => {

                    if (img.id === imageId) {
                        setDefaultIndex(result.images, index, 0);
                    }
                });
                dispatchAction({
                    type: "set-single-post",
                    data: {
                        post:result,
                    }
                })
                setLoading(false);
            } catch (e) {
                console.log(e);
            }

        }

        fetch();
        return () => { };
    }, [user]);

    async function signIn(e) {
        e.preventDefault();
        if (!user)
            await manager.signIn(window.location.href);
        return false;
    }

    const [showComments, setShowComments] = React.useState(false);
    const [showItinerary, setShowItinerary] = React.useState(false);

    function toggleComments() {
        if (showItinerary && !showComments) {

            setShowItinerary(!showItinerary);
        }
        setShowComments(!showComments);
    }

    function toggleItinerary() {

        if (showComments && !showItinerary) {

            setShowComments(!showComments);
        }
        setShowItinerary(!showItinerary);
    }

    function Description(props) {
        return (<>
            <div className='container-fluid description'>
                <div className='row'>
                    <div className='col-12' >
                        <CardHeader
                            avatar={<Link to={`/g/${props.post.authorId}`} ><Avatar alt={props.post.author} src={props.post.authorAvatar} /> </Link>}
                            title={
                                <h6>
                                    <Link to={`/g/${props.post.authorId}`} >{props.post.author}</Link>
                                </h6>
                            }
                            subheader={props.post.publicationDate}
                        />
                        <br />
                        <br />

                    </div>
                    <div className='col-12'>
                        <span><LocationOnIcon /> Place : <b>{props.post.location}</b></span>
                        <br />
                        <br />
                    </div>
                    <div className='col-12'>
                        <span><AttachMoneyOutlinedIcon /> Budget : <b>{props.post.startingBid}</b></span>
                        <br />
                        <br />
                    </div>
                    <div className='col-12'>
                        <span><PeopleOutlineOutlinedIcon /> Seats : <b>{props.post.seats}</b></span>
                        <br />
                        <br />
                    </div>
                    <div className='col-12'>
                        <span><EventIcon /> Date : <b>{props.post.startDate}</b></span>
                        <br />
                        <br />
                    </div>
                    <div className='col-12'>
                        <span><AlarmOutlinedIcon /> Time : <b>{`from ${props.post.startTime} to ${props.post.endTime}`}</b></span>
                        <br />
                        <br />
                    </div>
                </div>
                <Typography variant="subtitle1" component="p">
                    {props.post.description}
                </Typography>
                <br />
                <div className='row d-flex reactions-panel' >

                    {props.user ?
                        <>  <div className='col-3 col-sm-2 col-md-2 col-lg-3 col-xl-3 text-center'>
                            {props.post.reactionType == 2 ? <IconButton className='like_div' onClick={() =>
                                dispatchAction({
                                    type: "single-like-action",
                                    data: {
                                        post: props.post,
                                        callback: handleReaction
                                    }
                                })
                            } >
                                <AiFillLike className='liked_icon' />
                            </IconButton> :
                                <IconButton className='like_div' onClick={() =>
                                    dispatchAction({
                                        type: "single-like-action",
                                        data: {
                                            post: props.post,
                                            callback: handleReaction
                                        }
                                    })
                                } >
                                    <AiOutlineLike />
                                </IconButton>}
                            <span className='text-center'>{props.post.likes}</span>
                        </div>
                            <div className='col-3 col-sm-2 col-md-2 col-lg-3 col-xl-3 text-center'>
                                {post.reactionType === 4 ? <IconButton className='dislike_div' onClick={() =>
                                    dispatchAction({
                                        type: "single-dislike-action",
                                        data: {
                                            post: post,
                                            callback: handleReaction
                                        }
                                    })
                                }>
                                    <AiFillDislike className='disliked_icon' />
                                </IconButton> : <IconButton onClick={() =>
                                    dispatchAction({
                                        type: "single-dislike-action",
                                        data: {
                                            post: props.post,
                                            callback: handleReaction
                                        }
                                    })
                                } >
                                        <AiOutlineDislike />
                                    </IconButton>}
                                <span className='text-center' >{props.post.dislikes}</span>
                            </div></> :

                        <><div className='col-3 col-sm-2 col-md-2 col-lg-3 col-xl-3 text-center'>

                            <IconButton className='like_div' onClick={signIn} >
                                <AiOutlineLike />
                            </IconButton>
                            <span className='text-center'>{props.post.likes}</span>
                        </div>
                            <div className='col-3 col-sm-2 col-md-2 col-lg-3 col-xl-3 text-center'>
                                <IconButton className='dislike_div' onClick={signIn} >
                                    <AiOutlineDislike />
                                </IconButton>
                                <span className='text-center' >{props.post.dislikes}</span>
                            </div></>
                    }
                    {
                        props.post.isBidOptIn ? <div className='col-3 col-sm-2 col-md-2 col-lg-3 col-xl-3 text-center'>
                            <IconButton onClick={toggleComments}>
                                <FaRegComment />
                            </IconButton>
                            <span className='text-center' >{props.post.bidCount}</span>
                        </div> : null
                    }
                    <div className='col-3 col-sm-2 col-md-2 col-lg-3 col-xl-3 text-center'>
                        <IconButton onClick={toggleItinerary}>
                            <LocationOnIcon />
                        </IconButton>
                        <span className='text-center' >{props.post.itineraryCount}</span>
                    </div>
                </div>
                <Itinerary show={showItinerary} postId={props.post.id} />
                <Comments post={props.post} show={showComments} postId={props.post.id} />
            </div>
        </>
        );

    }


    const [index, setIndex] = React.useState(0);

    function navigateForwardGallery(index) {
        var num = index + 1;
        if (num === state.post.images.length)
        {
            setIndex(0);
        }
        else
        {
            setIndex(num);
        }
    }
    function navigateBackGallery(index) {
        
        if (index === 0) {

            var num = state.post.images.length - 1;
            setIndex(num);
        }
        else
        {
            var num = index - 1;
            setIndex(num);
        }
    }

    function goBack() {
        window.history.back();
    }


    return (
        isLoading ? <PostLoading /> : <div className="post-container">
            <div>
                <div className="row">
                    <div className="col-12 col-lg-8 col-xl-9 main-section">
                        <div className="col-12 card-photo">
                            <div className="row">
                                
                                <div className="col-12 item-photo" style={{ backgroundImage: `url(${state.post.images[index].imageBase64})` }}>
                                    <div className='close-page-icon-div'>
                                        <IconButton onClick={() => goBack()}>
                                            <AiFillCloseCircle className='close-page-icon' />
                                        </IconButton>
                                </div>
                                    {
                                        state.post.images.length > 1 ? <div className="container-fluid nav-box">
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
                    <div className="col-12 col-lg-4 col-xl-3 description-section">
                            <Description user={user} post={state.post} />
                    </div>
                </div>
            </div>
        </div>
        );
 }
