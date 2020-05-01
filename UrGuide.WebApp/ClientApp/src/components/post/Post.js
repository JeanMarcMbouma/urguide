import React, { Component, useState } from "react";
import {
    makeStyles,
    IconButton,
    Button,
    Paper,
    Grid,
    CssBaseline,
    TextField,
    Box
} from "@material-ui/core";
import Card from '@material-ui/core/Card';
import Rating from '@material-ui/lab/Rating';
import CardHeader from '@material-ui/core/CardHeader';
import CardContent from '@material-ui/core/CardContent';
import CardActions from '@material-ui/core/CardActions';
import Avatar from '@material-ui/core/Avatar';
import Typography from '@material-ui/core/Typography';
import ShareIcon from '@material-ui/icons/Share';
import ChatIcon from '@material-ui/icons/Chat';
import FavoriteIcon from '@material-ui/icons/Favorite';
import ArrowForwardIosOutlinedIcon from '@material-ui/icons/ArrowForwardIosOutlined';
import ArrowBackIosOutlinedIcon from '@material-ui/icons/ArrowBackIosOutlined';
import LocationOnIcon from '@material-ui/icons/LocationOn';
import ChatBubbleOutlineOutlinedIcon from '@material-ui/icons/ChatBubbleOutlineOutlined';
import MoreHorizIcon from '@material-ui/icons/MoreHoriz';
import { Link, useParams } from 'react-router-dom';
import "./Post.css";


function Header() {
    return (<div>
        <CardHeader
            avatar={<Link to='/user'><Avatar alt="profile photo" src={'...'} /></Link>}
            title={<Typography variant="body1" component="p"><Link to='/user'>{'John Stephens'}</Link>  |  <Link to='/'>Follow</Link></ Typography>}
            subheader={'Trailer Park 921, London UK, 12/10/2020'}
        />
        </div>);

}

function Reviews() {
    return (<div className="row">
        <div className="col-12 rating-box">
            <Box component="fieldset" mb={3} mt={3} borderColor="transparent">
                <div className='text-center'>
                    <span className='rating-number'>4.5</span>
                    <h1 className='text-center' ><Rating name="half-rating-read" defaultValue={4.5} precision={0.5} readOnly /></h1>
                    <span>(17,500)</span>
                </div>
            </Box>
            <br />
            <div className='description-div'>
                <span className='text-muted'>Description</span>
                <br />
                <br />
                <p className='photo-description'>
                    Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged.
                                </p>
            </div>
        </div>
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

function Comment() {

    return (<div className='cmt' >
        <div className='row'>
            <div className='col-12'>
                <div className='row'>
                    <div className='col-12' >
                        <CardHeader
                            avatar={<Link to='/user'><Avatar alt="profile photo" src={'...'} /></Link>}
                            title={<Typography variant="body1" component="p"><Link to='/user'>{'John Stephens'}</Link></ Typography>}
                            subheader={'12/10/2019, 10:34 AM'}
                        />
                    </div>
                </div>
            </div>
        </div>
        <div className='container'>
            <div className='row justify-content-center'>
                <div className='col-12 comment-div' >
                    <p className='cmt-txt'>Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged.</p>
                </div>
            </div>
            <div className='row likes-row justify-content-end'>
                <div className='col-6 col-md-4' >
                    <div className='row'>
                        <div className='col-3 col-md-2'>
                            <IconButton>
                                <FavoriteIcon fontSize="small" />
                            </IconButton>
                            <span className='likes text-muted'>126</span>
                        </div>
                        <div className='col-3 col-md-2' >
                            <IconButton>
                                <ChatBubbleOutlineOutlinedIcon fontSize="small" />
                            </IconButton>
                        </div>
                    </div>
                </div>

            </div>
        </div>

    </div>);
}

export default function Post() {

    let { id } = useParams();

    return (
        <div className="post-container">
            <div className='container'>
                <div className="row">
                    <div className="col-12 card-header">
                        <Header />
                    </div>
                    <div className="col-12 col-lg-8 card-photo">
                        <div className="row">
                            <div className="col-12 item-photo">
                            </div>
                            <div className="col-12 nav-box">
                                <div className="row justify-content-between">
                                    <div className="col-2 col-md-1 col-lg-1">
                                        <IconButton>
                                            <ArrowBackIosOutlinedIcon />
                                        </IconButton>
                                    </div>
                                    <div className="col-2 col-md-1 col-lg-1">
                                        <IconButton>
                                            <ArrowForwardIosOutlinedIcon />
                                        </IconButton>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div className="col-12 col-lg-4 reviews-card">
                        <Reviews />
                    </div>
                    <div className="col-12 col-lg-8 feedbacks-card">
                        <NewFeedBack />
                        <div className='comments'>
                            <h6>Reviews (128)</h6>
                            <br />
                            <Comment />
                            <Comment />
                            <Comment />
                            <br />
                            <br />
                            <h3 className="text-center">
                                <Button color="default" variant="outlined">LOAD MORE REVIEWS</Button>
                            </h3>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
 }

