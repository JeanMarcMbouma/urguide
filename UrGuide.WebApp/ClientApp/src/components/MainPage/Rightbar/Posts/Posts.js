import React from 'react';
import {Avatar, Card} from '@material-ui/core';
import Typography from '@material-ui/core/Typography';
import {makeStyles} from '@material-ui/core/styles';
import { Link } from 'react-router-dom';
const useStyles = makeStyles (theme => ({
  text: {
    fontSize: '15px',
  },
  btn: {
    color: '#FF7B77',
    fontSize: '12px',
    textTransform: 'uppercase'
  },
}));

const Posts = ({post}) => {
  const classes = useStyles ();
  return (
    <Card style={{width:`350px`}} className="col-lg-12 bg-white rounded p-2 mb-2">
      <div className="media p-0">
              <Avatar className="mr-3" src={post.authorImage} alt="profile photo" />
        <div className="media-body">
          <Typography
            className={`mt-0 font-weight-bold ${classes.text}`}
            component="h4">
            {post.author}
          </Typography>
        </div>
      </div>
      <div className="col-12 p-0">
        <Typography
          className="text-justify text-truncate"
          variant="subtitle1"
          color="textSecondary"
                  component="p">
                  {post.description}
              </Typography>
              {
                  post.images.length ? <div className={`text-right`}>
                  <Link to={`/post/${post.id}/shot/${post.images[0].id}`} >
                      <span className={`font-weight-bold ${classes.btn}`} > Read</span>
                 </Link>
                  </div> : <></>
              }
              
      </div>
      </Card>
  );
};

export default Posts;
