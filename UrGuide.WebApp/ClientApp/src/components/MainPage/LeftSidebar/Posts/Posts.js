import React from 'react';
import { Avatar } from '@material-ui/core';import 
{ makeStyles } from "@material-ui/core/styles";

const useStyles = makeStyles(theme => ({
  img:{
    width:'40px',
    height:'40px'
  },
  post:{
    boxShadow:'0 0 15px rgba(0,0,0,0.2)',
    padding:'15px 15px 25px 15px',
    margin:'25px',
    maxWidth:'85%',
    borderRadius:'20px'
  },
  text:{
    fontSize:'15px'
  },
  title:{
    display:'grid',
    gridTemplateColumns:'20% 75%'
  },
  description:{
    marginTop:'5px',
    display:'grid',
    gridTemplateColumns:'85%',
    fontSize:'13px',
    color:'grey'
  },
  btn:{
    color:'#FF7B77',
    fontSize:'12px',
    textTransform:'uppercase',
    float:'right'
  },
}));

const Posts = (props) => { 
  const classes = useStyles();
  return (
    <>
      <div className={(classes.post)}>
          <div className={(classes.title)}>
            <Avatar className={(classes.img)} src={props.href}/>
            <div className={(classes.text)}><b>{props.title}</b></div>
          </div>
          <div>
            <div className={(classes.description)}>{props.description}</div>
            <div className={(classes.btn)}><b>Read</b></div>
          </div>
      </div>
    </>
  );
}

export default Posts;