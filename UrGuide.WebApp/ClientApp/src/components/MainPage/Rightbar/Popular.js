import React, { useEffect, useState } from 'react';
import Posts from './Posts/Posts';
import { Card, Avatar, Typography } from '@material-ui/core';
import Skeleton from '@material-ui/lab/Skeleton';
import './RightStyle.css';
import { PostsClient } from '../../../api';
import { HttpClientFactory } from '../../../httpclient';
import { ActionTypes, useDataContext } from '../../../data/GlobalDataContext';
import { Link } from 'react-router-dom';



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

const Popular = () => {
    const [posts, setPosts] = useState([]);
    const [loading, setLoading] = useState(true);

    const { dataContext, dcReducer } = useDataContext();

    useEffect(() => {
        var fetch = async () => {
            if (dataContext && dataContext.top && dataContext.top.length) {
                setPosts(dataContext.top);
                setLoading(false);
                return;
            }

            let client = HttpClientFactory.get(PostsClient);

            client.top10().then(posts => {
                dcReducer({ type: ActionTypes.TOP, data: posts });
                setPosts(posts);
                setLoading(false);
            });

        };
        fetch();
        return () => { };
    }, []);

   
    let postsElement = posts.map((p, i) => (
        <Posts key={i} post={p} />
    ));


    return (
        <div className='col-sm-4 col-md-4 col-lg-3 col-xl-3 rounded rightbar popular' >
            <div>
                <div className="d-lg-flex p-0 mb-3 mt-3">
                    <div className='font-weight-bold title'>
                        Popular posts
        </div>
                </div>
                <div>
                    { loading ? <PostsSkeleton /> : postsElement}
                </div>

            </div>
            <div className='copyright-div'>
                <span><Link className='link' to='/terms'>Terms</Link> - <Link className='link' to='/conditions'>Conditions</Link> - <Link className='link' to='/cookies'>Cookies</Link></span>
                <br />
                <span>&copy; Urguide 2020</span>
            </div>
        </div>
    );
}
export default Popular;
