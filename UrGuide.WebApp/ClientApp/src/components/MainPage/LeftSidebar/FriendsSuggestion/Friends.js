import React from 'react';
import { Avatar, makeStyles } from '@material-ui/core';
import AddCircleIcon from '@material-ui/icons/AddCircle';

const useStyles = makeStyles(theme => ({
  img:{
    width:'50px',
    height:'50px'
  },
  friend:{
    display:'grid',
    gridTemplateColumns:'20% 60% 15%',
    alignItems:'center',
    padding:'5px',
    margin:'15px',
  },
  name:{
    fontSize:'14px'
  },
  email:{
    fontSize:'13',
    color:'grey'
  },
  plus:{
    color:'#FF7B77',
    width:'40px',
    height:'40px'
  }
}));

const Friends = (props) => {
  const classes = useStyles();
  return (
    <>
      <div className={(classes.friend)}>
        <Avatar className={(classes.img)} src={props.href}/>
        <div>
          <div className={(classes.name)}><b>{props.name}</b></div>
          <div className={(classes.email)}>{props.email}</div>
        </div>
        <AddCircleIcon className={(classes.plus)}/>
      </div>
    </>
  )
}

export default Friends;