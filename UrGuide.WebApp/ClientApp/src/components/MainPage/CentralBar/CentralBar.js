import React from 'react';
import { makeStyles } from '@material-ui/core/styles';
import Card from '@material-ui/core/Card';
import CardHeader from '@material-ui/core/CardHeader';
import CardContent from '@material-ui/core/CardContent';
import CardActions from '@material-ui/core/CardActions';
import Avatar from '@material-ui/core/Avatar';
import IconButton from '@material-ui/core/IconButton';
import Typography from '@material-ui/core/Typography';
import { red } from '@material-ui/core/colors';
import ShareIcon from '@material-ui/icons/Share';
import Button from '@material-ui/core/Button';
import CardMedia from '@material-ui/core/CardMedia';
import TextField from '@material-ui/core/TextField';
import ChatIcon from '@material-ui/icons/Chat';
import PropTypes from 'prop-types';
import { withStyles } from '@material-ui/core/styles';

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

export default function CentralBar() {

    let posts = [
    {
        name:"Excursion around Cherkassy",
        description:"I will show you this beautiful town",
        price:"250",
        photo:"https://images.pexels.com/photos/3442567/pexels-photo-3442567.jpeg?cs=srgb&dl=aerial-photo-of-a-city-3442567.jpg&fm=jpg",
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
        photo:"https://images.pexels.com/photos/2787267/pexels-photo-2787267.jpeg?cs=srgb&dl=vehicles-on-road-beside-sea-2787267.jpg&fm=jpg",
        currentHuman:"9",
        LimitHuman:"30",
        author:"Lena",
        dateStart:"01.05.20",
        profilePhoto:"https://images.pexels.com/photos/3690085/pexels-photo-3690085.jpeg?cs=srgb&dl=photo-of-woman-wearing-black-turtle-neck-top-3690085.jpg&fm=jpg",
    },
    ]
    
    return (
        <div className="col-lg-6">
            {posts.map(post =>
                <Card className="shadow-lg p-3 mb-3 bg-white rounded">
                    <CardHeader
                        avatar={<Avatar alt="profile photo" src={post.profilePhoto} />}
                        title={post.author}
                        subheader={post.dateStart}/>
                    <CardMedia
                        image={post.photo}
                        title="Paella dish"
                    />
                    <CardContent>
                        <Typography variant="h6" component="p">{post.name}</Typography>
                        <Typography variant="subtitle1" color="textSecondary" component="p">{post.category}</Typography>
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
                </Card>
            )}
        </div>
    )
}