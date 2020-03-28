import React from 'react';
import Posts from './Posts/Posts';
import Friends from './FriendsSuggestion/Friends';
import { makeStyles } from '@material-ui/core';

const useStyles = makeStyles(theme => ({
  body:{
    margin:'0 0 10px 10px',
    maxWidth:'23%',
    border:'1px solid white',
    boxShadow:'0 0 15px rgba(0,0,0,0.2)',
    backgroundColor:'white',
    borderRadius:'20px',
    float:'right'
  },
  title:{
    display:'grid',
    gridTemplateColumns:'81% 15%',
    textTransform:'uppercase',
    fontSize:'12px',
    margin:'25px'
  },
  title_friend:{
    display:'grid',
    gridTemplateColumns:'85% 15%',
    textTransform:'uppercase',
    fontSize:'12px',
    margin:'25px'
  },
  btn:{
    color:'#FF7B77'
  }
}));

const Popular = (props) => {
  const classes = useStyles();
  let posts = [
    {
      id:1, 
      href:'https://i.pinimg.com/originals/df/5f/5b/df5f5b1b174a2b4b6026cc6c8f9395c1.jpg', 
      title:'Unexpected views of New York city', 
      description:'Using a something makes food very tasty and find...'
    },
    {
      id:2, 
      href:'https://img.favpng.com/21/10/23/computer-icons-avatar-social-media-blog-font-awesome-png-favpng-jKXEv9rWhum7VbNKDbcELd6Di.jpg', 
      title:'Shannen Dohart shares updata on her...', 
      description:'This some description about the post title'
    },
    {
      id:3, 
      href:'https://cdn4.vectorstock.com/i/1000x1000/42/83/avatar-social-media-isolated-icon-design-vector-10704283.jpg', 
      title:'Lawmaker.Border wall threaters', 
      description:'This some description about the post title'
    }
  ]
  let friends = [
    {id:1,
     href:'https://cdn3.iconfinder.com/data/icons/social-media-set-1-1/256/Social_Media-11-512.png', 
     name:'Blake Scott', 
     email:'@scotty'
    },
    {id:1, 
    href:'https://www.clipartmax.com/png/middle/257-2572603_user-man-social-avatar-profile-icon-man-avatar-in-circle.png', 
    name:'Digby Martins', 
    email:'@martins'
    }
  ]

  let friendsElement = friends.map(f => <Friends href={f.href} name={f.name} email={f.email}/>)
  let postsElement = posts.map(p => <Posts href={p.href} title={p.title} description={p.description}/> )

  return (
    <>
      <div className={(classes.body)}>
        <div className={(classes.title)}>
          <div><b>Popular posts</b></div>
          <div className={(classes.btn)}><b>More</b></div>
        </div>
        <div>{postsElement}</div>
        <div className={(classes.title_friend)}>
          <div><b>Friends Suggestions</b></div>
          <div className={(classes.btn)}><b>All</b></div>
        </div>
        <div>{friendsElement}</div>
      </div>
    </>
  );
}

export default Popular;