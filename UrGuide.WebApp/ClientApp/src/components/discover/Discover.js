import React, { Component, useState } from "react";
import {
    makeStyles,
    IconButton,
    Chip,
    Button,
    Typography,
    Avatar,
    CardHeader,
} from "@material-ui/core";
import FavoriteIcon from '@material-ui/icons/Favorite';
import ChatBubbleOutlineOutlinedIcon from '@material-ui/icons/ChatBubbleOutlineOutlined';
import Modal from 'react-bootstrap/Modal';
import {
    BrowserRouter as Router,
    Switch,
    Route,
    Link,
    useParams,
    useRouteMatch
} from "react-router-dom";
import Post from "../post/Post";
import "./DiscoverStyle.css";


class Cards extends Component {

    constructor(props) {
        super(props);
        this.state = { images: [], selectedId: null };
    }

    componentWillMount() {

        this.populateData();
    }

    render() {

        const cards = this.state.images.map((img, i) =>
          
                <div key={i} className={`col-12 col-sm-6 col-md-6 col-lg-4 col-xl-3 square-grid-item`} style={{ backgroundImage: `url(${img.href})` }} >
                <Link to={`${'/discover'}/${img.id}`}>
                    <table className="inner-container">
                        <tbody>
                        <tr>
                            <td valign="top">
                                <div className="inner-top">
                                    <CardHeader
                                        avatar={<Avatar className='user-profile' alt={img.username} src='...' />}
                                        title={<span className='user-name'>{img.username}</span>}
                                        subheader={<span className='spot-location'>{img.location}</span>}
                                    />
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td valign="bottom">
                                <div className="inner-bottom">
                                    <div className='row'>
                                        <div className='col-4 likes-div'>
                                            <h5>
                                                <IconButton>
                                                    <FavoriteIcon className='feedback-icon' fontSize="small" />
                                                </IconButton>
                                                <span className='feedback-number'>126</span>
                                            </h5>
                                        </div>
                                        <div className='col-4'>
                                            <h5>
                                                <IconButton>
                                                    <ChatBubbleOutlineOutlinedIcon className='feedback-icon' fontSize="small" />
                                                </IconButton>
                                                <span className='feedback-number'>26</span>
                                            </h5>
                                        </div>
                                    </div>
                                </div>
                            </td>
                        </tr>
                        </tbody>
                    </table>
                </Link>
              </div>
    
        );


        return (<div className='main'>
            <div className='tags-div'>
                <span className='tags-label'>Most used tags :</span>
                <Chip label="Popular" className='clicked-tag tag' clickable />
                <Chip label="Near me" className='tag unclicked-tag' clickable />
                <Chip label="Cheapest" className='tag unclicked-tag' clickable />
                <Chip label="Top Rated" className='clicked-tag tag' clickable />
                <Chip label="Historical" className='clicked-tag tag' clickable />
                <Chip label="Sport" className='tag unclicked-tag' clickable />
                <Chip label="Recent Posts" className='tag unclicked-tag' clickable />
                <Chip label="Nature" className='clicked-tag tag' clickable />
            </div>
            <div className='container-fluid'>
                <div className='row square-grid'>
                    {cards}
                </div>
            </div>
        </div>);

    }

    async populateData() {

        const data = [

            {
                id: 4,
                username: "Nadia Maya",
                href: "https://r-cf.bstatic.com/images/hotel/max1024x768/221/221741543.jpg",
                location: "Saint-Marthe Beach, French Polinesia",
                galleryId: 1000
            },
            {
                id: 5,
                username: "Kim Xinhuan",
                href: "https://i.pinimg.com/originals/29/81/94/29819463f7826507d015c57846d8a3f6.jpg",
                location: "Pekin, China",
                galleryId: 1001,
            },
            {
                id: 6,
                username: "Ipeleng Zuma",
                href: "https://www.travelanddestinations.com/wp-content/uploads/2019/06/Cape-Town-South-Africa.jpg",
                location: "Cape Town Marina,South Africa",
                galleryId: 1002,
            },
            {
                id: 2,
                username: "Stacy Riley",
                href: "https://www.intrepidtravel.com/adventures/wp-content/uploads/2017/05/FU8A0260-88x450.jpg",
                location: "Alaamhri Desert, Saudi A.",
                galleryId: 1003,
            },
            {
                id: 7,
                username: "Guillaume Leroux",
                href: "https://www.lelongweekend.com/wp-content/uploads/2018/07/N75_4248-1024x684.jpg",
                location: "Lavenders Provence, France",
                galleryId: 1004,
            },
            {
                id: 8,
                username: "Rodigro Suarez",
                href: "https://holeinthedonut.smugmug.com/DailyPhotos/HITD-Daily-Photos/i-DJm4q39/0/L/Spain-Sevilla-Street-Scene-Night-L.jpg",
                location: "Sevilla Streets, Spain",
                galleryId: 1005,
            },
            {
                id: 3,
                username: "J. Samira",
                href: "https://assets.simpleviewcms.com/simpleview/image/upload/c_limit,h_1200,q_75,w_1200/v1/clients/norway/3e0981da_3280_49d6_b879_353fe2d8c8f9_d707ed6d-2954-49db-9755-f0fbd5e08144.jpg",
                location: "Fjords Islands, Norway",
                galleryId: 1006,
            },
            {
                id: 1,
                username: "John Doe",
                href: "https://ca-times.brightspotcdn.com/dims4/default/3ebc245/2147483647/strip/true/crop/5767x3708+0+0/resize/1486x955!/quality/90/?url=https%3A%2F%2Fcalifornia-times-brightspot.s3.amazonaws.com%2Fed%2F60%2F4ae8532445e3aaea0a3cc73c2729%2Fgettyimages-1209052296.jpg",
                location: "Timesquare NYC, USA",
                galleryId: 1007,
            },


        ]

        this.setState({ images: data });
    }
}


export default function Discover() {

    let { path, url } = useRouteMatch();
        
        return (
            <>
                <div className='search-bar'>
                    <div className='row justify-content-center'>
                        <div className='col-12 col-sm-8 col-md-8 col-lg-4'>
                            <div className="search-container">
                                <input type="text" placeholder="Where are you going ?" className="searchbar" />
                                <img src="https://images-na.ssl-images-amazon.com/images/I/41gYkruZM2L.png" alt="Magnifying Glass" className="button-search" />
                            </div>
                        </div>
                    </div>
                </div>
                <Switch>
                    <Route exact path={path} >
                        <Cards />
                    </Route>
                </Switch>
               
            </>
        );
    }


//<Route path={`${path}/post/:postId/shot/:imageId`}>
//    <Post />
//</Route>