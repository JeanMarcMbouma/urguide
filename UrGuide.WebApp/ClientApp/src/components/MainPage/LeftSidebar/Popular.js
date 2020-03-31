import React from 'react';
import Posts from './Posts/Posts';
import Friends from './FriendsSuggestion/Friends';
import {makeStyles} from '@material-ui/core';

const useStyles = makeStyles (theme => ({
  btn: {
    color: '#FF7B77',
    fontSize: '12px',
    textTransform: 'uppercase',
    marginLeft:'65px'
  },
  more:{
    color: '#FF7B77',
    fontSize: '12px',
    textTransform: 'uppercase',
    marginLeft:'93px'
  },
  title: {
    fontSize: '12px',
    textTransform:'uppercase'
  },
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
    {
      id: 3,
      href: 'https://cdn4.vectorstock.com/i/1000x1000/42/83/avatar-social-media-isolated-icon-design-vector-10704283.jpg',
      title: 'Lawmaker.Border wall threaters',
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
    <div className="col-lg-3 bg-white rounded shadow-lg">
      <div className="d-lg-flex p-0 mb-3 mt-3">
        <div className={`font-weight-bold ${classes.title}`}>
          Popular posts
        </div>
        <div className={`font-weight-bold ${classes.more}`}>
          More
        </div>
      </div>
      <div>{postsElement}</div>
      <div className="d-lg-flex p-0 mb-3 mt-3">
        <div className={`font-weight-bold ${classes.title}`}>
          Friends Suggestions
        </div>
        <div className={`font-weight-bold ${classes.btn}`}>
          All
        </div>
      </div>
      {friendsElement}
    </div>
  );
};

export default Popular;
