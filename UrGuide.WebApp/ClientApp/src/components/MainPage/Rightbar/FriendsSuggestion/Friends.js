import React from 'react';
import {Avatar, makeStyles} from '@material-ui/core';
import Typography from '@material-ui/core/Typography';
import AddCircleIcon from '@material-ui/icons/AddCircle';

const useStyles = makeStyles (theme => ({
  text: {
    fontSize: '14px',
  },
  email:{
    fontSize:'13px'
  },
  plus: {
    color: '#FF7B77',
    width: '35px',
    height: '35px',
  },
}));

const Friends = props => {
  const classes = useStyles ();
  return (
    <div className="col-lg-12 p-2 mb-2">
      <div className="media p-0">
        <Avatar className="mr-1" src={props.href} alt="profile photo" />
        <div className="media-body">
          <Typography
            className={`mt-0 font-weight-bold ${classes.text}`}
            component="h5">
            {props.name}
          </Typography>
          <Typography
            className={`mt-0 font-weight-bold ${classes.email}`}
            color='textSecondary'
            component="p">
            {props.email}
          </Typography>
        </div>
        <AddCircleIcon className={(classes.plus)}/>
      </div>
    </div>
  );
};

export default Friends;

{
  /* <div className={(classes.friend)}>
        <Avatar className={(classes.img)} src={props.href}/>
        <div>
          <div className={(classes.name)}><b>{props.name}</b></div>
          <div className={(classes.email)}>{props.email}</div>
        </div>
        <AddCircleIcon className={(classes.plus)}/>
      </div> */
}
