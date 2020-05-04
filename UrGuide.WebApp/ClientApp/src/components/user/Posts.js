import React, { Component } from "react";
import {
    makeStyles,
    IconButton,
    CardActions,
    CardHeader,
    CardContent,
    Avatar,
    Typography,
    Button

} from "@material-ui/core";
import ChatIcon from '@material-ui/icons/Chat';
import ShareIcon from '@material-ui/icons/Share';
import { withStyles } from '@material-ui/core/styles';
import PropTypes from 'prop-types';
import "./UserStyle.css";


const styles = {
    root: {
        background: props =>
            props.color === 'red'
                ? 'linear-gradient(45deg, #FE6B8B 30%, #FF8E53 90%)'
                : 'linear-gradient(45deg, #2196F3 30%, #21CBF3 90%)',
        border: 0,
        borderRadius: 3,
        boxShadow: props =>
            props.color === 'red'
                ? '0 3px 5px 2px rgba(255, 105, 135, .3)'
                : '0 3px 5px 2px rgba(33, 203, 243, .3)',
        color: 'white',
        height: 30,
        padding: '0 30px',
        margin: 8,
    },
};

function ButtonInPosts(props) {
    const { classes, color, ...other } = props;
    return <Button className={classes.root} {...other} />;
}

ButtonInPosts.propTypes = {
    classes: PropTypes.object.isRequired,
    color: PropTypes.oneOf(['blue', 'red']).isRequired,
};

const ButtonP = withStyles(styles)(ButtonInPosts);

//function FollowingCard() {


//    return (
//        <div className="container-fluid following-card">
//            <div>
//                <h6 className='text-muted'>Following (1200)</h6>
//                <br />
//            </div>
//            <div className='row'>

//                <div className='col-4 follower-div'>
//                    <div className='follower-div-photo'>
//                    </div>
//                </div>
//                <div className='col-4 follower-div'>
//                    <div className='follower-div-photo'>
//                    </div>
//                </div>
//                <div className='col-4 follower-div'>
//                    <div className='follower-div-photo'>
//                    </div>
//                </div>
//                <div className='col-4 follower-div'>
//                    <div className='follower-div-photo'>
//                    </div>
//                </div>
//                <div className='col-4 follower-div'>
//                    <div className='follower-div-photo'>
//                    </div>
//                </div>
//                <div className='col-4 follower-div'>
//                    <div className='follower-div-photo'>
//                    </div>
//                </div>
//                <div className='col-4 follower-div'>
//                    <div className='follower-div-photo'>
//                    </div>
//                </div>
//                <div className='col-4 follower-div'>
//                    <div className='follower-div-photo'>
//                    </div>
//                </div>
//                <div className='col-4 follower-div'>
//                    <div className='follower-div-photo'>
//                    </div>
//                </div>
//            </div>
//        </div>
//    );

//}


function Layout() {

    const post = (<div className="p-3 mb-3 bg-white rounded post-card">
        <CardHeader
            avatar={<Avatar alt="profile photo"  />}
            title={<Typography variant="body1" component="p">Jane Doe | Lorem Ipsum | Lorem Ipsum</ Typography>}
            subheader='12/12/2020'
        />
        <CardContent>
            <Typography variant="subtitle1" component="p">Lorem Ipsum is very cool.</Typography>
        </CardContent>
        <CardActions className="d-flex justify-content-around">
            <IconButton aria-label="share">
                <ShareIcon />
            </IconButton>
            <IconButton>
                <ChatIcon />
            </IconButton>
            <ButtonP color="red">for $22</ButtonP>
            <ButtonP color="blue">12/15</ButtonP>
        </CardActions>
    </div>);

    return (<div className='row justify-content-center'>
        <div className="col-12 col-lg-5">
            {post}
            {post}
            {post}
        </div>
    </div>);
}


export default class Posts extends Component {
    render() {
        return (
            <div className="row">
                <div className="col-12 lower-section">
                    <Layout />
                </div>
            </div>
        )
    }
}