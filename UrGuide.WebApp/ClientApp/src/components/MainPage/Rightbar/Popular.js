import React, { Component } from 'react';
import Posts from './Posts/Posts';
import Friends from './FriendsSuggestion/Friends';
import { Card, Avatar, Typography } from '@material-ui/core';
import Skeleton from '@material-ui/lab/Skeleton';
import './RightStyle.css';
import { PostsClient } from '../../../api';
import { HttpClientFactory } from '../../../httpclient';



function PostsSkeleton() {

    const post = <Card style={{ width: `350px` }} className="col-lg-12 bg-white rounded p-2 mb-2">
        <div className="media p-0">
            <Skeleton variant="circle" width={40} height={40} />
            <div className="media-body">
                <Skeleton variant="text" style={{ marginLeft: `20px`, width: `200px` }} />
            </div>
        </div>
        <div className="col-12 p-0">
            <br />
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

      this.populateData();
    }


    render() {


        let friendsElement = this.state.suggestions.map((f, i) => (
            <Friends key={i} href={f.href} name={f.name} email={f.email} />
        ));
        let postsElement = this.state.posts.map((p, i) => (
            <Posts key={i} href={p.authorAvatar} title={p.author} description={p.description} images={p.images} postId={p.id} />
        ));


        return (
            <div className='col-sm-5 col-md-5 col-lg-3 col-xl-3 rounded rightbar popular' >
                <div>
                    <div className="d-lg-flex p-0 mb-3 mt-3">
                        <div className='font-weight-bold title'>
                            Popular posts
        </div>
                    </div>
                    <div>
                        {this.state.loading ? <PostsSkeleton /> : postsElement}
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

    populateData() {

        let client = HttpClientFactory.get(PostsClient);

        client.top10().then(posts => {
            this.setState({ posts: posts, loading: false });
        });

      
    }
}
