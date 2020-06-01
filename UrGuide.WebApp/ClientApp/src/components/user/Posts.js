import React, {
    useState, useContext, useMemo, useReducer, Component
} from 'react';
import { makeStyles } from '@material-ui/core/styles';
import { FaRegComment } from 'react-icons/fa';
import { AiOutlineDislike } from 'react-icons/ai';
import { AiOutlineLike } from 'react-icons/ai';
import { AiFillDislike } from 'react-icons/ai';
import { AiFillLike } from 'react-icons/ai';
import { AiOutlineStop } from 'react-icons/ai';
import { AiOutlineCheck } from 'react-icons/ai';
import { Link } from 'react-router-dom';
import {
    Card,
    CardHeader,
    CardContent,
    CardActions,
    Avatar,
    IconButton,
    InputLabel,
    Input,
    FormControl,
    InputAdornment,
    FormHelperText,
    Typography,
    ButtonGroup,
    Button,
    CardMedia,
    TextField,
    Box

} from '@material-ui/core';
import Skeleton from '@material-ui/lab/Skeleton';
import Rating from '@material-ui/lab/Rating';
import LocationOnIcon from '@material-ui/icons/LocationOn';
import PropTypes from 'prop-types';
import '../MainPage/CentralBar/CentralStyle.css';
import 'date-fns';
import Grid from '@material-ui/core/Grid';
import Slider from '@material-ui/core/Slider';
import AttachMoneyOutlinedIcon from '@material-ui/icons/AttachMoneyOutlined';
import AlarmOutlinedIcon from '@material-ui/icons/AlarmOutlined';
import clsx from "clsx";
import { withStyles } from '@material-ui/core/styles';
import { useAuthUser } from '../../components/api-authorization/AuthService';
import { useAuthContext } from '../../components/api-authorization/AuthService';
import authService from '../../components/api-authorization/AuthService';
import { HttpClientFactory } from '../../httpclient';
import "./UserStyle.css";
import FeedBackContext from './FeedbackContext';
import FeedBackReducer from './FeedBackReducer';
import { UsersClient, FeedbackModel } from '../../api';


function SkeletonCard() {
    return (
        <div className="p-3 mb-3 bg-white rounded post-card">
            <CardHeader
                avatar={<Skeleton variant="circle" width={50} height={50} />}
                title={<Skeleton variant="text" style={{ width: `160px` }} />}
                subheader={<Skeleton variant="text" style={{ width: `120px` }} />}
            />
            <CardContent>
                <div className='row'>
                    <div className='col-12' >
                        <div className='row' >
                            <div className='col-2 col-md-2' >
                                <Skeleton variant="text" style={{ marginLeft: `5px`, width: `100%` }} />
                            </div>
                            <div className='col-2 col-md-2' >
                                <Skeleton variant="text" style={{ marginLeft: `5px`, width: `100%` }} />
                            </div>
                            <div className='col-2 col-md-2' >
                                <Skeleton variant="text" style={{ marginLeft: `5px`, width: `100%` }} />
                            </div>
                        </div>
                    </div>
                    <div className='col-12'>
                        <br />
                        <Skeleton variant="text" style={{ marginLeft: `5px`, width: `180px` }} />
                        <br />
                    </div>
                    <div className='col-12'>
                        <Skeleton variant="text" style={{ marginLeft: `5px`, width: `180px` }} />
                        <br />
                    </div>
                    <div className='col-12'>
                        <Skeleton variant="text" style={{ marginLeft: `5px`, width: `180px` }} />
                        <br />
                    </div>
                    <div className='col-12'>
                        <Skeleton variant="text" style={{ marginLeft: `5px`, width: `180px` }} />
                        <br />
                    </div>
                </div>
            </CardContent>
            <CardActions className="container-fluid"  >
                <div className='row d-flex justify-content-center' style={{ width: `100%` }}>
                    <div className='col-3 col-lg-3 text-center'>
                        <Skeleton variant="text" style={{ width: `100%` }} />
                    </div>
                    <div className='col-3 col-lg-3 text-center'>
                        <Skeleton variant="text" style={{ width: `100%` }} />
                    </div>
                    <div className='col-3 col-lg-3 text-center'>
                        <Skeleton variant="text" style={{ width: `100%` }} />
                    </div>
                    <div className='col-3 col-lg-3 text-center'>
                        <Skeleton variant="text" style={{ width: `100%` }} />
                    </div>
                </div>
            </CardActions>
        </div>);
}


function Itinerary(props) {


    const [itineraries, setItineraries] = useState([]);

    useMemo(async () => {

        if (!props.show)
            return;
        const api = HttpClientFactory.getPostClient();
        var result = await api.itineraries(props.postId);
        setItineraries(result);

    }, [props.show]);


    return (
        props.show && props.showId === props.postId ? <div className='itinerary_wrapper'>
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


const labels = {
    1: 'Very poor experience.',
    2: 'It was boring.',
    3: 'It was okay.',
    4: 'It was excellent.',
    5: 'It was perfect.',
};

const MocksReviews = [
    { author: "Helen Gordon", rating: 4, review: 'Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industrystandard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book.It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged.', date: '11-may-2020 12:22:54', profilePic: null, id: null, },
    { author: "Stephen Hawlys", rating: 3, review: 'Contrary to popular belief, Lorem Ipsum is not simply random text.', date: '04-april-2020 09:17:20', profilePic: null, id: null, },
    { author: "Rick Ross", rating: 4, review: 'There are many variations of passages of Lorem Ipsum available, but the majority have suffered alteration in some form, by injected humour, or randomised words which don look even slightly believable.', date: '19-december-2019 05:42:08', profilePic: null, id: null, },
]
function FeedBacks(props) {

    const { manager, user } = useAuthContext();

    async function signIn(e) {
        e.preventDefault();
        if (!user)
            await manager.signIn(window.location.href);
        return false;
    }
    const [reviews, setReviews] = useState(MocksReviews);
    const ctx = useContext(FeedBackContext);
    const [state, dispatch] = useReducer(FeedBackReducer, ctx);

    const [values, setValues] = useState({ review: '', rating: 1, postId:props.postId });

    useMemo(async () => {
        if (!user)
            return;
        var client = HttpClientFactory.getClient(user);
        var data = await client.getdetails();
        //setFeedBacks(data);

    }, [user]);

    //state.feedbacks = reviews;

    const handleChange = prop => event => {
        setValues({ ...values, [prop]: event.target.value });
    };

    function handleRating(value) {
        setValues({ ...values, ["rating"]: value });
    }

    async function createReview(review) {

        if (!props)
            return;

        const client = HttpClientFactory.get(UsersClient);
        var model = new FeedbackModel({
            text: review.review,
            rating: review.rating,
        });
        try {

            await client.feedback(review.postId, user.profile.sub, model);
        }
        catch (e) {
            console.log(e);
        }

    }

    const [hover, setHover] = React.useState(-1);


    return (props.show && props.postId === props.showId ? <>

        {user  ? <div className='col-12 col-lg-12 new-feedback'>
            <TextField fullWidth value={values.review} multiline rows={7} onChange={handleChange("review")} rowsMax={7} id="outlined-basic" label="Your review on this tour" variant="outlined" placeholder="Did you participate to this tour ? Tell people how it was." />
            <br />
            {state.textError ? <><br /><span className='text-danger'>This field is required.</span><br /><br /></> : null}
            <div>
                <span>Your experience</span>
                <br />
                <Rating
                    name="hover-feedback"
                    value={values.rating}
                    onChange={(event, newValue) => {
                        handleRating(newValue);
                    }}
                    onChangeActive={(event, newHover) => {
                        setHover(newHover);
                    }}
                />
                {values.rating !== null && <Box ml={0}>{labels[hover !== -1 ? hover : values.rating]}</Box>}
            </div>

            <br />
            {user ? <Button variant="contained" color="primary" onClick={() =>
                dispatch({
                    type: "post-feedback",
                    data: {
                        userFeedback: values,
                        feedbacks: state.feedbacks,
                        callback: createReview,
                    }
                })
            }>submit review</Button>

                : <Button variant="contained" color="primary" onClick={signIn}>submit review</Button>
            }
            <br />
            <br />
        </div>

            :
            null
        }
        <br/>
        <br/>
        {reviews.length > 0 ? <h5> Reviews ({reviews.length})</h5> : <h5>No review</h5>}
        <br />
        {

            reviews.map((rev, i) => (
                <div className='cmt-div' key={i} >
                    <CardHeader
                        avatar={<Avatar alt={rev.author} src={rev.profilePic} />}
                        title={
                            <h6>
                                {rev.author}
                            </h6>
                        }
                        subheader={rev.date}
                    />
                    <Rating
                        value={rev.rating}
                        readOnly
                    />
                    <div className='comment-text'>
                        <p>{rev.review}.</p>
                    </div>
                </div>))
        }
    </> : null);
}

const MocksData = [{
        id: "string",
        text: "string",
    description: "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book.",
        price: "$33",
        rating:4,
        location: "string",
        likes: 0,
        dislikes: 0,
        publicationDate: "12-May-2020",
        lastEditDate: "string",
        startingBid: "string",
        lastBid: "string",
        status: "string",
        seats: 0,
        reservedSeats: 0,
        startDate: "string",
        endDate: "string",
        endTime: "string",
        startTime: "string",
        hasReserved: true,
        hasReacted: true,
        reactionType: 0,
        bidCount: 0,
        itineraryCount: 0,
        categories: [
            "string"
        ],
        images: [
            {
                imageBase64: "https://images.unsplash.com/photo-1544609442-26c059e02c7c?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&w=1000&q=80",
                name: "string1",
                id: "string1"
            },
            {
                imageBase64: "https://www.lamodeenimages.com/sites/default/files-lmi/styles/1365x768/public/images/article/homepage/full/louis-vuitton-volez-voguez-voyagez-shanghai-exhibition-center-la-mode-en-images-cover.jpg?itok=5ygYxpnS",
                name: "string2",
                id: "string2"
            },
            {
                imageBase64: "https://images.unsplash.com/photo-1503023345310-bd7c1de61c7d?ixlib=rb-1.2.1&auto=format&fit=crop&w=1000&q=80",
                name: "string3",
                id: "string3"
            }
        ],
        authorId: "string",
        author: "Jane Susan",
        authorAvatar: "data:image/jpeg;base64,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",
        isBidOptIn: true
    },]




export default function Posts() {


    function PostImages(props) {
        if (props.images.length == 1) {
            return (<div className='row'>
                <div className='col-12 unique-img' style={{ backgroundImage: `url(${props.images[0].imageBase64})` }}>
                    <Link to={`/post/${props.postId}/shot/${props.images[0].id}`} >
                        <div style={{ height: `100%`, width: `100%` }}>
                        </div>
                    </Link>
                </div>
            </div>);
        }
        if (props.images.length == 2) {
            return (<div className='row'>
                <div className='col-12 col-lg-6 post-img' style={{ backgroundImage: `url(${props.images[0].imageBase64})` }}>
                    <Link to={`/post/${props.postId}/shot/${props.images[0].id}`} >
                        <div style={{ height: `100%`, width: `100%` }}>
                        </div>
                    </Link>
                </div>
                <div className='col-12 col-lg-6 post-img' style={{ backgroundImage: `url(${props.images[1].imageBase64})` }}>
                    <Link to={`/post/${props.postId}/shot/${props.images[1].id}`} >
                        <div style={{ height: `100%`, width: `100%` }}>
                        </div>
                    </Link>
                </div>
            </div>);
        }
        if (props.images.length == 3) {
            return (<div className='row'>

                <div className='col-12 col-lg-6 post-img' style={{ backgroundImage: `url(${props.images[0].imageBase64})` }}>
                    <Link to={`/post/${props.postId}/shot/${props.images[0].id}`} >
                        <div style={{ height: `100%`, width: `100%` }}>
                        </div>
                    </Link>
                </div>
                <div className='col-12 col-lg-6 post-img' style={{ backgroundImage: `url(${props.images[1].imageBase64})` }}>
                    <Link to={`/post/${props.postId}/shot/${props.images[1].id}`} >
                        <div style={{ height: `100%`, width: `100%` }}>
                        </div>
                    </Link>
                </div>
                <div className='col-12 post-img' style={{ backgroundImage: `url(${props.images[2].imageBase64})` }}>
                    <Link to={`/post/${props.postId}/shot/${props.images[2].id}`} >
                        <div style={{ height: `100%`, width: `100%` }}>
                        </div>
                    </Link>
                </div>
            </div>);
        }
        if (props.images.length == 4) {
            return (<div className='row'>
                <div className='col-12 col-lg-6 post-img' style={{ backgroundImage: `url(${props.images[0].imageBase64})` }}>
                    <Link to={`/post/${props.postId}/shot/${props.images[0].id}`} >
                        <div style={{ height: `100%`, width: `100%` }}>
                        </div>
                    </Link>
                </div>
                <div className='col-12 col-lg-6 post-img' style={{ backgroundImage: `url(${props.images[1].imageBase64})` }}>
                    <Link to={`/post/${props.postId}/shot/${props.images[1].id}`} >
                        <div style={{ height: `100%`, width: `100%` }}>
                        </div>
                    </Link>
                </div>
                <div className='col-12 col-lg-6 post-img' style={{ backgroundImage: `url(${props.images[2].imageBase64})` }}>
                    <Link to={`/post/${props.postId}/shot/${props.images[2].id}`} >
                        <div style={{ height: `100%`, width: `100%` }}>
                        </div>
                    </Link>
                </div>
                <div className='col-12 col-lg-6 post-img' style={{ backgroundImage: `url(${props.images[3].imageBase64})` }}>
                    <Link to={`/post/${props.postId}/shot/${props.images[3].id}`} >
                        <div style={{ height: `100%`, width: `100%` }}>
                        </div>
                    </Link>
                </div>
            </div>);
        }

        return null;
    }



    const [showReviews, setShowReviews] = React.useState({ show: false, id: null });

    function toggleReviews(postId) {
        setShowReviews({ show: !showReviews.show, id: postId });
    }


    function SinglePost(props) {

      const post = props.post;

    return (<div className="p-3 mb-3 bg-white rounded post-card">
        <div className="col-12 mt-3 row">
            <CardHeader className="col-10 p-2 m-0"
                avatar={<Link to={`/g/${post.authorId}`} ><Avatar alt={post.author} src={post.authorAvatar} /></Link>}
                title={
                    <Link to={`/g/${post.authorId}`} >
                        <h6>
                            {post.author}
                        </h6>
                    </Link>
                }
                subheader={post.publicationDate}
            />
        </div>
        <CardContent>
            <div className='row'>
                <div className='col-12' >
                    <Rating value={post.rating} readOnly />
                    <br />
                    <br />
                </div>
            </div>
            <div className='row'>
                <div className='col-12' >
                    {post.categories.map((category, i) => (<Link key={i}> <span className='category-tag'>{category}</span> </Link>))}
                    <br />
                    <br />
                </div>
            </div>
            <Typography variant="subtitle1" component="p">
                {post.description}
            </Typography>
        </CardContent>
        <PostImages images={post.images} postId={post.id} />
        <CardActions  >
            <div className='row' style={{ width: `100%`, marginLeft:`2px` }} >
                <div className='col-12'>
                    <Button onClick={() => toggleReviews(post.id)} fullWidth className="btn-reviews" >Read Reviews (23)</Button>
                </div> 
            </div>
        </CardActions>
        <FeedBacks show={showReviews.show} showId={showReviews.id} postId={post.id} />
    </div>);
    }

    return (
        <div className="row">
            <div className="col-12 lower-section">
                <div className="col-12 col-md-6 col-lg-5 col-xl-5 timeline">
                    {MocksData.map((post, i) => <SinglePost key={i} post={post} />)}
                </div>
            </div>
        </div>
    );
}

//{
//    isLoading ? <><SkeletonCard /><SkeletonCard /></> :
//        MocksData.map((post, i) => <SinglePost key={i} post={post} />)
//}