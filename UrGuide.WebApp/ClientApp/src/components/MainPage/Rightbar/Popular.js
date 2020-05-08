import React, { Component } from 'react';
import Posts from './Posts/Posts';
import Friends from './FriendsSuggestion/Friends';
import { Card, Avatar, Typography } from '@material-ui/core';
import Skeleton from '@material-ui/lab/Skeleton';
import './RightStyle.css';



function PostsSkeleton() {

    const post = <Card style={{width:`350px`}} className="col-lg-12 bg-white rounded p-2 mb-2">
        <div className="media p-0">
            <Skeleton variant="circle" width={40} height={40} />
            <div className="media-body">
                    <Skeleton variant="text" style={{ marginLeft:`20px`, width: `200px` }} />
            </div>
        </div>
        <div className="col-12 p-0">
        <br/>
        <Skeleton variant="text" style={{ marginLeft: `10px`, width: `280px` }} />
        </div>
    </Card>;

    return (<>{post}{post}</>);
}

function SuggestionsSkeleton() {

    const suggestions = (<div className="col-lg-12 p-2 mb-2">
        <div className="media p-0">
            <Skeleton variant="circle" width={35} height={35} />
            <div className="media-body">
                <Skeleton variant="text" style={{ marginLeft: `20px`, width: `100px` }} />
                <Skeleton variant="text" style={{ marginLeft: `20px`, width: `60px` }} />
            </div>
            <Skeleton variant="circle" width={30} height={30} />
        </div>
    </div>);

    return (<>{suggestions}{suggestions}</>)
}

export default class Popular extends Component {


    constructor(props) {
        super(props);
        this.state = { posts: [], suggestions: [], loading: true };
    }

    componentWillMount() {

        const timer = setTimeout(() => {
            this.populateData();

        }, 9000);

        return () => clearTimeout(timer);
    }


render()  {


    let friendsElement = this.state.suggestions.map((f, i) => (
        <Friends key={i} href={f.href} name={f.name} email={f.email} />
    ));
    let postsElement = this.state.posts.map((p, i) => (
        <Posts key={i} href={p.href} title={p.title} description={p.description} />
    ));


  return (
    <div className='col-sm-5 col-md-5 col-lg-3 col-xl-3 rounded rightbar popular' >
      <div>
        <div className="d-lg-flex p-0 mb-3 mt-3">
          <div className='font-weight-bold title'>
            Popular posts
        </div>
          <div className='font-weight-bold more'>
           {this.state.loading ? <></> : <span>MORE</span>}
        </div>
              </div>
              <div>{this.state.loading ? <PostsSkeleton /> : postsElement}
              </div>

      </div>
      <div className='copyright-div'>
        <span>Terms - Conditions - Cookies</span>
        <br />
        <span>&copy; Urguide 2020</span>
      </div>
    </div>
    );
}

async populateData() {


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

    this.setState({ posts: posts, suggestions:friends, loading: false });
}
}
