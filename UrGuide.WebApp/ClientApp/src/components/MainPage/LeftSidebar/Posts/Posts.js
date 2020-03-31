import React from 'react';
import {Avatar} from '@material-ui/core';
import Typography from '@material-ui/core/Typography';
import {makeStyles} from '@material-ui/core/styles';

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

const Posts = props => {
  const classes = useStyles ();
  return (
    <div className="col-lg-12 shadow-lg bg-white rounded p-2 mb-2">
      <div className="media p-0">
        <Avatar className="mr-3" src={props.href} alt="profile photo" />
        <div className="media-body">
          <Typography
            className={`mt-0 font-weight-bold ${classes.text}`}
            component="h4">
            {props.title}
          </Typography>
        </div>
      </div>
      <div className="col-12 p-0">
        <Typography
          className="text-justify text-truncate"
          variant="subtitle1"
          color="textSecondary"
          component="p">
          {props.description}
        </Typography>
        <div className={`font-weight-bold text-right ${classes.btn}`}>Read</div>
      </div>
    </div>
  );
};

export default Posts;

{
  /* <div className={(classes.title)}>
            <Avatar className={(classes.img)} src={props.href}/>
            <div className={(classes.text)}><b>{props.title}</b></div>
          </div> */
}
{
  /* <div className={(classes.description)}>{props.description}</div> */
}
