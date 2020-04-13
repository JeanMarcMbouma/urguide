import React , { useState , useContext } from 'react';
import { makeStyles } from '@material-ui/core/styles';
import UserContext from '../../../UserContext'
import Card from '@material-ui/core/Card';
import CardHeader from '@material-ui/core/CardHeader';
import CardContent from '@material-ui/core/CardContent';
import CardActions from '@material-ui/core/CardActions';
import Avatar from '@material-ui/core/Avatar';
import IconButton from '@material-ui/core/IconButton';
import Typography from '@material-ui/core/Typography';
import ButtonGroup from '@material-ui/core/ButtonGroup';
import { red } from '@material-ui/core/colors';
import ShareIcon from '@material-ui/icons/Share';
import PhotoIcon from '@material-ui/icons/Photo';
import VideoLibraryIcon from '@material-ui/icons/VideoLibrary';
import PersonAddIcon from '@material-ui/icons/PersonAdd';
import AddLocationIcon from '@material-ui/icons/AddLocation';
import MoreHorizIcon from '@material-ui/icons/MoreHoriz';
import Button from '@material-ui/core/Button';
import CardMedia from '@material-ui/core/CardMedia';
import TextField from '@material-ui/core/TextField';
import ChatIcon from '@material-ui/icons/Chat';
import PropTypes from 'prop-types';
import './CentralStyle.css';
import { withStyles } from '@material-ui/core/styles';
import { SdCard } from '@material-ui/icons';

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

export default function CentralBar() {

    const user = useContext(UserContext)
    let posts = [
    {
        name:"Excursion around Cherkassy",
        description:"I will show you this beautiful town",
        price:"250",
        category:"mix",
        currentHuman:"12",
        LimitHuman:"30",
        author:"Ivanna",
        dateStart:"12.04.20",
        profilePhoto:"https://images.pexels.com/photos/3541390/pexels-photo-3541390.jpeg?cs=srgb&dl=close-up-photo-of-woman-wearing-red-sweater-3541390.jpg&fm=jpg",
    },
    {
        name:"Football field in Kyiv",
        description:"I will show you the biggest football field in Kyiv",
        category:"sport",
        price:"450",
        currentHuman:"9",
        LimitHuman:"30",
        author:"Lena",
        dateStart:"01.05.20",
        profilePhoto:"https://images.pexels.com/photos/3690085/pexels-photo-3690085.jpeg?cs=srgb&dl=photo-of-woman-wearing-black-turtle-neck-top-3690085.jpg&fm=jpg",
    },
    ]



    function ViewPost() {

        const [ViewPostCreating, setViewPostCreating] = useState('button');

        if (ViewPostCreating==='button') {
            return (
            <div onClick={() => setViewPostCreating('post')} className={`col-lg-12 p-3 mb-3 bg-white rounded new-post-card`} >
                <div className='new-post-btn' >
                    <span>Want to write a new post ?</span>
                </div>
            </div>)
        } else {
            return(
            <div className={`col-lg-12 p-3 mb-3 bg-white rounded shadow-lg bg-white rounded`}>
                <div className="col-lg-12 row d-flex justify-content-between">
                    <Typography variant="h5">Create your post!</Typography>
                    <ButtonP onClick={() => setViewPostCreating('button')} variant="outlined" color="blue">X</ButtonP>
                </div>
                <div className="col-lg-12 row my-4">
                    <Avatar alt="Remy Sharp" src="/static/images/avatar/1.jpg" />
                    <Typography className="mx-4" variant="h6">{user.username}</Typography>
                </div>
                <div className="col-lg-12 row">
                    <TextField
                        fullWidth
                        label="Short description"
                        multiline
                        rows="6"
                        variant="outlined"
                />
                </div>
                <div className="col-lg-12 row my-2">
                    <ButtonGroup fullWidth size="large" color="primary" aria-label="large outlined primary button group">
                        <Button><PhotoIcon /></Button>
                        <Button><VideoLibraryIcon /></Button>
                        <Button><PersonAddIcon /></Button>
                        <Button><AddLocationIcon /></Button>
                        <Button><ShareIcon /></Button>
                        <Button><MoreHorizIcon /></Button>
                    </ButtonGroup>
                </div>
                <div className="col-lg-12 row">
                    <ButtonP fullWidth variant="outlined" color="blue" href="#outlined-buttons">Publish</ButtonP>
                </div>
            </div>)
        }
    }



    return (
        <div className="col-12 col-sm-7 col-md-7 col-lg-6 col-xl-5 timeline">
            <div className='container'>

                <ViewPost />
                {posts.map((post, i) =>
                    <div key={i} className="p-3 mb-3 bg-white rounded post-card">
                        <CardHeader
                            avatar={<Avatar alt="profile photo" src={post.profilePhoto} />}
                            title={<Typography variant="body1" component="p">{post.author} | {post.name} | {post.category}</ Typography>}
                            subheader={post.dateStart}
                        />
                        <CardContent>
                            <Typography variant="subtitle1" component="p">{post.description}</Typography>
                        </CardContent>
                        <CardActions className="d-flex justify-content-around">
                            <IconButton aria-label="share">
                                <ShareIcon />
                            </IconButton>
                            <IconButton>
                                <ChatIcon />
                            </IconButton>
                            <ButtonP color="red">for {post.price}</ButtonP>
                            <ButtonP color="blue">{`${post.currentHuman}/${post.LimitHuman}`}</ButtonP>
                        </CardActions>
                    </div>
                )}
            </div>
        </div>
    )
}