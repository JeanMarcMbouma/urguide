import React, {
    useState, useContext, useMemo, useReducer, Component, useEffect
} from 'react';
import {
    CardHeader,
    Avatar,
    Button,
    TextField,
    CircularProgress,
    Box,

} from '@material-ui/core';
import { Link, useParams } from 'react-router-dom';
import { FaRegComment } from 'react-icons/fa';
import Rating from '@material-ui/lab/Rating';
import AttachMoneyOutlinedIcon from '@material-ui/icons/AttachMoneyOutlined';
import PeopleOutlineOutlinedIcon from '@material-ui/icons/PeopleOutlineOutlined';
import { withStyles } from '@material-ui/core/styles';
import { HttpClientFactory } from '../../httpclient';
import {  UsersClient, FeedbackModel, FeedbackClient } from '../../api';
import "./UserStyle.css"
import FeedBackContext from './FeedbackContext';
import FeedBackReducer from './FeedBackReducer';
import { useAuthContext } from '../api-authorization/AuthService';
import { useDataContext, ActionTypes } from "../../data/GlobalDataContext";



const labels = {
    1: 'Very poor experience.',
    2: 'It was boring.',
    3: 'It was okay.',
    4: 'It was excellent.',
    5: 'It was perfect.',
};

function FeedBacks({ reviews }) {
    return (
        <>
            {reviews.length > 0 ? <h5> Reviews ({reviews.length})</h5> : <h5>No review</h5>}
            <br/>
            {
                reviews.map((rev, i) => (
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
                            <p>{rev.text}.</p>
                        </div>
                    </div>))
            }
       </>
    );
}

export default function Reviews(props) {

    const { user } = useAuthContext();
    const ctx = useContext(FeedBackContext);
    const [state, dispatch] = useReducer(FeedBackReducer, ctx);
    const [pageNumber, setPageNumber] = useState(1);
    const initialValues = { review: '', rating: 1 };
    const [values, setValues] = useState(initialValues);
    const { dcReducer } = useDataContext();


    const client = HttpClientFactory.get(FeedbackClient, user);
    useEffect(() => {

        dcReducer({ type: ActionTypes.LOADINGCOMPLETED, data: { completed: false, url: "/profile", profileUrl: "/Reviews" } });

        client.users(props.userId || user.profile.sub, pageNumber)
            .then(r => {
                dispatch({
                    type: "loading",
                    data: {
                        feedbacks: r.items
                    }
                });

                dcReducer({ type: ActionTypes.LOADINGCOMPLETED, data: { completed: true, url: "/profile", profileUrl: "/Reviews" } });
                setPageNumber(pageNumber + 1);
            });

        return () => { };
    }, [user, pageNumber]);



    window.onscroll = async function (ev) {
        var totalPageHeight = document.body.scrollHeight;
        var scrollPoint = window.scrollY + window.innerHeight;
        if (scrollPoint >= totalPageHeight) {
            await loadMoreReviews();
        }
    };

    async function loadMoreReviews() {
        client.users(props.userId || user.profile.sub, pageNumber)
            .then(r => {
                r.items.forEach((item, index) => { state.feedbacks.push(item) });
                dispatch({
                    type: "loading",
                    data: {
                        feedbacks: state.feedbacks
                    }
                });
                setPageNumber(pageNumber + 1);
            });
    }

    const handleChange = prop => event => {
        setValues({ ...values, [prop]: event.target.value });
    };

    function handleRating(value) {
        setValues({ ...values, ["rating"]: value });
    }

    async function createReview(review) {

        if (!props)
            return;

        const client = HttpClientFactory.get(UsersClient, user);
        const config = {
            text: review.review,
            rating: review.rating,
        };
        var model = new FeedbackModel(config);
        try {

            await client.feedback(props.userId, model);
            setValues(initialValues);
            dispatch({
                type: "new-item",
                data: {
                    review: config,
                    user: user
                }
            });
        }
        catch (e) {
            console.log(e);
        }

    }

    const [hover, setHover] = React.useState(-1);

    return (<div>
        <br />
        <br/>
        <div className='row'>
            { user && props && user.profile.sub != props.userId && props.userId != null ? <div className='col-12 col-lg-6 new-feedback'>
                <h3>What people think of this guide.</h3>
                <br />
                <TextField fullWidth value={values.review} multiline rows={7} onChange={handleChange("review")} rowsMax={7} id="outlined-basic" label="Your review" variant="outlined" placeholder="Would you recommend this guide ? Write what's on your mind." />
                <br />
                {state.textError ? <><br /><span className='text-danger'>This field is required.</span><br /><br /></> : null}
                <br />
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
                <Button variant="contained" color="primary" onClick={() =>
                        dispatch({
                            type: "user-feedback",
                            data: {
                                userFeedback: values,
                                feedbacks: state.feedbacks,
                                callback: createReview,
                            }
                        }) 
                    }>submit review</Button> 
                <br />
                <br />
            </div>
                
                : 
                null
            }
            <div className='col-12 col-lg-8'>
                <FeedBacks reviews={state.feedbacks} />
            </div>
        </div>
    </div>);

}