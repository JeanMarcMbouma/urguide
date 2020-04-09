import React from 'react';
import Posts from './Posts/Posts';
import Friends from './FriendsSuggestion/Friends';
import {makeStyles} from '@material-ui/core';
import './RightStyle.css';

const useStyles = makeStyles (theme => ({
  btn: {
    color: '#FF7B77',
    fontSize: '12px',
    textTransform: 'uppercase',
    marginLeft:'10px'
  },
  more:{
    color: '#FF7B77',
    fontSize: '12px',
    textTransform: 'uppercase',
    marginLeft:'93px'
    },
    suggestions: {
     marginTop:'20px',
    },
  title: {
    fontSize: '12px',
    textTransform:'uppercase'
    },
    popular: {
        backgroundColor: '#f7f8fa',
        height: '100%',
        width: 'auto',
        paddingBottom:'30px'
    }
}));

const Popular = props => {
  const classes = useStyles ();
  let posts = [
    {
      id: 1,
      href: 'https://i.pinimg.com/originals/df/5f/5b/df5f5b1b174a2b4b6026cc6c8f9395c1.jpg',
      title: 'Unexpected views of New York city',
      description: 'Using a something makes food very tasty and find...',
    },
    {
      id: 2,
      href: 'https://img.favpng.com/21/10/23/computer-icons-avatar-social-media-blog-font-awesome-png-favpng-jKXEv9rWhum7VbNKDbcELd6Di.jpg',
      title: 'Shannen Dohart shares updata on her...',
      description: 'This some description about the post title',
    },
  ];
  let friends = [
    {
      id: 1,
      href: 'https://cdn3.iconfinder.com/data/icons/social-media-set-1-1/256/Social_Media-11-512.png',
      name: 'Blake Scott',
      email: '@scotty',
    },
    {
      id: 1,
      href: 'https://www.clipartmax.com/png/middle/257-2572603_user-man-social-avatar-profile-icon-man-avatar-in-circle.png',
      name: 'Digby Martins',
      email: '@martins',
      },
  ];

  let friendsElement = friends.map (f => (
    <Friends href={f.href} name={f.name} email={f.email} />
  ));
  let postsElement = posts.map (p => (
    <Posts href={p.href} title={p.title} description={p.description} />
  )); 

  return (
      <div className={`col-sm-5 col-md-5 col-lg-3 col-xl-3 rounded rightbar ${classes.popular}`} >
          <div>
              <div className="d-lg-flex p-0 mb-3 mt-3">
                  <div className={`font-weight-bold ${classes.title}`}>
                      Popular posts
        </div>
                  <div className={`font-weight-bold ${classes.more}`}>
                      MORE
        </div>
              </div>
              <div>{postsElement}</div>
              <div className={classes.suggestions} >
                  <div className="d-lg-flex p-0 mb-3 mt-3">
                      <div className={`font-weight-bold ${classes.title}`}>
                          Friends Suggestions
                </div>
                  </div>
                  <div>
                      {friendsElement}
                  </div>
                  <div className={`font-weight-bold ${classes.btn}`}>
                      SEE MORE
                  </div>
           </div>
          </div>
          <div className='copyright-div'>
              <span>Terms - Conditions - Cookies</span>
              <br />
              <span>&copy; Urguide 2020</span>
          </div>
    </div>
  );
};

export default Popular;
