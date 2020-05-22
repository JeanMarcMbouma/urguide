import React, {
    useState, useContext, useMemo, useReducer, Component
} from 'react';
import {
    CardHeader,
    Avatar,
    IconButton,
    Typography,
    Button,
    Paper,
    TextField,
    Box,
    Grid

} from '@material-ui/core';
import { Link, useParams } from 'react-router-dom';
import { FaRegComment } from 'react-icons/fa';
import { AiOutlineDislike } from 'react-icons/ai';
import { AiOutlineLike } from 'react-icons/ai';
import { AiFillDislike } from 'react-icons/ai';
import { AiFillLike } from 'react-icons/ai';
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


const labels = {
    0.5: 'Worst experience.',
    1: 'Very poor experience.',
    1.5: 'Bad experience.',
    2: 'Not realistic.',
    2.5: 'Not interesting.',
    3: 'It was okay.',
    3.5: 'It was good.',
    4: 'It was excellent.',
    4.5: 'It was almost perfect.',
    5: 'It was perfect.',
};

function NewFeedBack() {

    const [value, setValue] = React.useState(2);
    const [hover, setHover] = React.useState(-1);

    return (<form noValidate autoComplete="off" className='new-feedback'>
        <TextField fullWidth multiline rows={7} rowsMax={7} id="outlined-basic" label="Your review" variant="outlined" placeholder="Would you recommend this spot ? Write what's on your mind." />
        <br />
        <br />
        <div>
            <span>Your experience</span>
            <br />
            <Rating
                name="hover-feedback"
                value={value}
                precision={0.5}
                onChange={(event, newValue) => {
                    setValue(newValue);
                }}
                onChangeActive={(event, newHover) => {
                    setHover(newHover);
                }}
            />
            {value !== null && <Box ml={0}>{labels[hover !== -1 ? hover : value]}</Box>}
        </div>

        <br />
        <Button variant="contained" color="primary">submit review</Button>
    </form>);
}


function Itinerary(props) {


    const [itineraries, setItineraries] = useState([]);

    useMemo(async () => {
        const api = HttpClientFactory.getPostClient();
        var result = await api.itineraries(props.postId);
        setItineraries(result);

    }, []);


    return (
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



    useMemo(async () => {

        if (!props.postId) {
            return;
        }
        const api = HttpClientFactory.get(BidClient);
        var result = await api.history(props.postId);
        setBids(result);

    }, [user]);

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


    function valuetext(value) {
        return `${value}`;
    }

    return (
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
                            </div>
                        </div>))} </> : <h6>No bid yet.</h6>



            }
        </div> : null
    );
}




export default function Post() {

    let { id } = useParams();
    const [post, setPost] = useState(
        {
            id: "097EE6C5-ED1F-4CD5-8862-62D90C3C69F8",
            text: "string",
            description: "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in",
            price: "$10 - $50",
            rating: "string",
           location: "St-Louis, Louisiana",
            likes: 0,
            dislikes: 0,
           publicationDate: "19-May-2020",
            lastEditDate: "string",
           startingBid: "string",
            lastBid: "string",
            status: "string",
            seats: 0,
           reservedSeats: 0,
            startDate: "11/12/2020",
            endDate: "11/12/2020",
            endTime: "13:00",
            startTime: "15:00",
            hasReserved: true,
            hasReacted: true,
            reactionType: 0,
            bidCount: 0,
            itineraryCount: 0,
            categories: [
                "Amusement",
                "Historical"
            ],
            images: [
                {
                    imageBase64: "https://www.visitcalifornia.com/sites/default/files/VC_California101_VeniceBeach_Stock_RF_638340372_1280x640.jpg",
                    name: "venice.jpg",
                    id:1
                },
                {
                    imageBase64: "https://previews.123rf.com/images/crisograf/crisograf1610/crisograf161000065/63683101-new-york-etats-unis-manhattan-times-square-foules-et-de-la-circulation-dans-la-soir%C3%A9e-avec-des-th%C3%A9%C3%A2tres-.jpg",
                    name: "venice.jpg",
                    id: 2
                },
                {
                    imageBase64: "https://media.timeout.com/images/105383118/630/472/image.jpg",
                    name: "venice.jpg",
                    id: 3
                }
            ],
            authorId: "dfe470e0-d3bb-40bf-b119-64309098432c",
            author: "John Doe",
            authorAvatar: "string",
            isBidOptIn: true
        });
    const actionCtx = useContext(ActionsContext);
    const [actionsState, dispatchAction] = useReducer(ActionsReducer, actionCtx);

    const { manager, user } = useAuthContext();

    const { profile } = user || {
        profile: {}
    };

    const [isLoading, setLoading] = React.useState(true);

    function Description(props) {
        return (<>
            <div className="container-fluid">
                <div className='row'>
                    <div className='col-12' >
                        <br />
                        <br />
                        {props.post.categories.map((category, i) => (<Link key={i}> <span className='category-tag'>{category}</span> </Link>))}
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
                        <span><AlarmOutlinedIcon /> Time : <b>{`${props.post.startDate}, from ${props.post.startTime} to ${props.post.endTime}`}</b></span>
                        <br />
                        <br />
                    </div>
                </div>
                <Typography variant="subtitle1" component="p">
                    {props.post.description}
                </Typography>
                <br />
                <div className='row d-flex justify-content-center' style={{ width: `100%` }}>

                    {props.user ?
                        <>  <div className='col-3 col-lg-3 text-center'>
                            {props.post.reactionType == 2 ? <IconButton className='like_div' onClick={() =>
                                dispatchAction({
                                    type: "like-action",
                                    data: {
                                        post: props.post,
                                        posts: actionsState.posts,
                                        callback: handleReaction
                                    }
                                })
                            } >
                                <AiFillLike className='liked_icon' />
                            </IconButton> :
                                <IconButton className='like_div' onClick={() =>
                                    dispatchAction({
                                        type: "like-action",
                                        data: {
                                            post: props.post,
                                            posts: actionsState.posts,
                                            callback: handleReaction
                                        }
                                    })
                                } >
                                    <AiOutlineLike />
                                </IconButton>}
                            <span className='text-center'>{props.post.likes}</span>
                        </div>
                            <div className='col-3 col-lg-3 text-center'>
                                {post.reactionType === 4 ? <IconButton className='dislike_div' onClick={() =>
                                    dispatchAction({
                                        type: "dislike-action",
                                        data: {
                                            post: post,
                                            posts: actionsState.posts,
                                            callback: handleReaction
                                        }
                                    })
                                }>
                                    <AiFillDislike className='disliked_icon' />
                                </IconButton> : <IconButton onClick={() =>
                                    dispatchAction({
                                        type: "dislike-action",
                                        data: {
                                            post: props.post,
                                            posts: actionsState.posts,
                                            callback: handleReaction
                                        }
                                    })
                                } >
                                        <AiOutlineDislike />
                                    </IconButton>}
                                <span className='text-center' >{props.post.dislikes}</span>
                            </div></> :

                        <><div className='col-3 col-lg-3 text-center'>

                            <IconButton className='like_div' onClick={signIn} >
                                <AiOutlineLike />
                            </IconButton>
                            <span className='text-center'>{props.post.likes}</span>
                        </div>
                            <div className='col-3 col-lg-3 text-center'>
                                <IconButton className='dislike_div' onClick={signIn} >
                                    <AiOutlineDislike />
                                </IconButton>
                                <span className='text-center' >{props.post.dislikes}</span>
                            </div></>
                    }
                    {
                        props.post.isBidOptIn ? <div className='col-3 col-lg-3 text-center'>
                            <IconButton onClick={toggleComments}>
                                <FaRegComment />
                            </IconButton>
                            <span className='text-center' >{props.post.bidCount}</span>
                        </div> : null
                    }
                    <div className='col-3 col-lg-3 text-center'>
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


    useMemo(async () => {
        const api = HttpClientFactory.get(PostsClient, user);
        try {
            var result = await api.posts(id);
            setPost(result);
            actionsState.post = result;
            setLoading(false);
        } catch (e) {
            console.log(e);
        }

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
        setShowComments(!showComments);
    }

    function toggleItinerary() {
        setShowItinerary(!showItinerary);
    }

    const [index, setIndex] = React.useState(0);

    function navigateForwardGallery(index) {
        var num = index + 1;
        if (num === post.images.length)
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

            var num = post.images.length - 1;
            setIndex(num);
        }
        else
        {
            var num = index - 1;
            setIndex(num);
        }
    }


    return (
        <div className="post-container">
            <div >
                <div className="row">
                    <div className="col-12 card-header">
                        <Header user={user} post={post} />
                    </div>
                    <div className="col-12 col-lg-8 card-photo">
                        <div className="row">
                            <div className="col-12 item-photo" style={{ backgroundImage: `url(${post.images[index].imageBase64})` }}>
                            </div>
                            {
                                post.images.length > 1 ? <div className="col-12 nav-box">
                                    <div className="row justify-content-between">
                                        <div className="col-2 col-md-1 col-lg-1">
                                            <IconButton onClick={() => navigateBackGallery(index)}>
                                                <ArrowBackIosOutlinedIcon />
                                            </IconButton>
                                        </div>
                                        <div className="col-2 col-md-1 col-lg-1">
                                            <IconButton onClick={() => navigateForwardGallery(index)} >
                                                <ArrowForwardIosOutlinedIcon />
                                            </IconButton>
                                        </div>
                                    </div>
                                </div> : null
                            }
                        </div>
                    </div>
                    <div className="col-12 col-lg-4 reviews-card">
                        <Description user={user} post={post} />
                    </div>
                    <div className="col-12 col-lg-8 feedbacks-card">
                        
                    </div>
                </div>
            </div>
        </div>
    );
 }

