import React, {
    useState, useContext, useReducer, Component, useEffect
} from 'react';
import { Link, useParams } from 'react-router-dom';
import AddCircleOutlineIcon from '@material-ui/icons/AddCircleOutline';
import {
    Card,
    CardHeader,
    CardContent,
    CardActions,
    Avatar,
    IconButton,
    CircularProgress,
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
import { useAuthContext } from '../../components/api-authorization/AuthService';
import { HttpClientFactory } from '../../httpclient';
import "./UserStyle.css";
import FeedBackContext from './FeedbackContext';
import FeedBackReducer from './FeedBackReducer';
import { UsersClient, FeedbackModel, FeedbackClient, PostsClient, PostModelPagedList, SearchParameters } from '../../api';


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
                </div>
            </CardContent>
            <CardActions className="container-fluid"  >
                <div className='row justify-content-center' style={{ width: `100%` }}>
                    <div className='col-10'>
                        <Skeleton variant="text" style={{ width: `100%` }} />
                    </div>
                </div>
            </CardActions>
        </div>);
}


function Itinerary(props) {


    const [itineraries, setItineraries] = useState([]);

    useEffect(() => {

        if (!props.show)
            return;
        let load = async () => {
            const api = HttpClientFactory.getPostClient();
            var result = await api.itineraries(props.postId);
            setItineraries(result);
        };

        load();

        return () => { };
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

function FeedBacks(props) {

    const { manager, user } = useAuthContext();

    async function signIn(e) {
        e.preventDefault();
        if (!user)
            await manager.signIn(window.location.href);
        return false;
    }
    const [isLoading, setLoading] = useState(true);
    const [pageNumber, setPageNumber] = useState(1);
    const ctx = useContext(FeedBackContext);
    const [state, dispatch] = useReducer(FeedBackReducer, ctx);

    const [values, setValues] = useState({ review: '', rating: 1, postId: props.postId });

    useEffect(() => {
        if (props.show === true) {
            var client = HttpClientFactory.get(FeedbackClient);
            client.posts(props.showId, 1).then(result => {
                setPageNumber(result.pageNumber);
                setLoading(false);
                state.feedbacks = result.items;
            });
        }
        return () => { };
    }, [props.show]);


    const handleChange = prop => event => {
        setValues({ ...values, [prop]: event.target.value });
    };

    function handleRating(value) {
        setValues({ ...values, ["rating"]: value });
    }

    async function createReview(review) {

        if (!props)
            return;

        const client = HttpClientFactory.get(PostsClient, user);
        var model = new FeedbackModel({
            text: review.review,
            rating: review.rating,
        });
        try {

            await client.feedback(review.postId, model);

        }
        catch (e) {
            console.log(e);
        }

    }

    const [hover, setHover] = React.useState(-1);


    return (props.show && props.postId === props.showId ? <>
        <br />
        {isLoading ? <h4 className='text-center'><CircularProgress /></h4> : <>
            {user && user.profile.sub === props.authorId ? null : <div className='col-12 col-lg-12 new-feedback'>
                <TextField fullWidth value={values.review} multiline rows={7} onChange={handleChange("review")} rowsMax={7} id="outlined-basic" label="Your review on this tour" variant="outlined" placeholder="Did you participate to this tour ? Tell people how it was." />
                <br />
                {state.textError ? <><br /><span className='text-danger'>This field is required.</span><br /><br /></> : null}
                <div>
                    <br />
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
                            user: user,
                        }
                    })
                }>submit review</Button>

                    : <Button variant="contained" color="primary" onClick={signIn}>submit review</Button>
                }
                <br />
                <br />
            </div>}

            {state.feedbacks.length > 0 ? <h5> Reviews ({state.feedbacks.length})</h5> : <h5>No review yet.</h5>}
            <br />
            {
                state.feedbacks.map((rev, i) => (
                    <div className='cmt-div' key={i} >
                        <CardHeader
                            avatar={<Avatar alt={rev.authorFullName} src={rev.authorImage} />}
                            title={
                                <h6>
                                    {rev.authorFullName}
                                </h6>
                            }
                            subheader={rev.publicationDate}
                        />
                        <Rating
                            value={rev.rating}
                            readOnly
                        />
                        <div className='comment-text'>
                            <p>{rev.text}</p>
                        </div>
                    </div>))
            }<br /> <h4 className='text-center'><IconButton onClick={async () => {

                if (props.show === true) {
                    var client = HttpClientFactory.get(FeedbackClient);
                    var page = pageNumber + 1;
                    var result = await client.posts(props.showId, page);
                    //console.log(result.pageNumber);
                    setPageNumber(result.pageNumber);
                    setLoading(false);
                    result.items.forEach((item, index) => { state.feedbacks.push(item) });
                    dispatch({
                        type: "more-feedbacks",
                        data: {
                            userFeedback: values,
                            feedbacks: state.feedbacks,
                        }
                    });
                }
            }} ><AddCircleOutlineIcon fontSize="large" /></IconButton></h4></>}
    </> : null);
}




export default function Posts() {

    const { userId } = useParams();
    const { user } = useAuthContext();

    const [isLoading, setLoading] = useState(true);
    const [data, setData] = useState({});

    useEffect(() => {

        var id = userId;

        if (user != null && id == undefined) {
            id = user.profile.sub;
        }

        let load = async () => {
            var client = HttpClientFactory.get(PostsClient, user);
            var model = new SearchParameters({ term: null, pageNumber: 1 });
            var result = await client.all(id, model);
            setData(result);
            setLoading(false);
        }

        load();
        return () => { };
    }, [user]);

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
                        {post.categories.map((category, i) => (<Link key={i} to={`/discover/${category}`}> <span className='category-tag'>{category}</span> </Link>))}
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
                <div className='row' style={{ width: `100%`, marginLeft: `2px` }} >
                    <div className='col-12'>
                        <Button onClick={() => toggleReviews(post.id)} fullWidth className="btn-reviews" >Reviews</Button>
                    </div>
                </div>
            </CardActions>
            <FeedBacks show={showReviews.show} showId={showReviews.id} postId={post.id} authorId={post.authorId} />
        </div>);
    }

    return (
        <div className="row">
            <div className="col-12 lower-section">
                <div className="col-12 col-md-6 col-lg-5 col-xl-5 timeline">
                    {isLoading ? <><SkeletonCard /><SkeletonCard /></> : data.items.map((post, i) => <SinglePost key={i} post={post} />)}
                </div>
            </div>
        </div>
    );
}
