import React, {
    useState, useContext, useMemo, useReducer, Component
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
import {  UsersClient, FeedbackModel } from '../../api';
import "./UserStyle.css"
import FeedBackContext from './FeedbackContext';
import FeedBackReducer from './FeedBackReducer';
import { useAuthContext } from '../api-authorization/AuthService';



const labels = {
    1: 'Very poor experience.',
    2: 'It was boring.',
    3: 'It was okay.',
    4: 'It was excellent.',
    5: 'It was perfect.',
};

const MocksReviews = [
    { author: "Helen Gordon", rating:4, review: 'Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industrystandard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book.It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged.', date: '11-may-2020 12:22:54', profilePic: null, id: null, },
    { author: "Stephen Hawlys", rating:3, review:'Contrary to popular belief, Lorem Ipsum is not simply random text.', date: '04-april-2020 09:17:20', profilePic: null, id: null,},
    { author: "Rick Ross", rating: 4,review: 'There are many variations of passages of Lorem Ipsum available, but the majority have suffered alteration in some form, by injected humour, or randomised words which don look even slightly believable.', date: '19-december-2019 05:42:08', profilePic: null, id: null, },
]
function FeedBacks(props) {
    return (
        <>
            {props.reviews.length > 0 ? <h5> Reviews ({props.reviews.length})</h5> : <h5>No review</h5>}
            <br/>
            {
                
                props.reviews.map((rev, i) => (
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
       </>
    );
}


export default function Reviews(props) {

    const { manager, user } = useAuthContext();
    const [feedbacks, setFeedBacks] = useState(MocksReviews);
    const ctx = useContext(FeedBackContext);
    const [state, dispatch] = useReducer(FeedBackReducer, ctx);
    
    const [values, setValues] = useState({review:'', rating:1 });
   
    useMemo(async () => {
        if (!user)
            return;
        var client = HttpClientFactory.getClient(user);
        var data = await client.getdetails();
        //setFeedBacks(data);

    }, [user]);

    state.feedbacks = feedbacks;

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

            await client.feedback(props.userId, model);
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