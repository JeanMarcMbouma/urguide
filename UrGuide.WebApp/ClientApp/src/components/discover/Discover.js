import React, { Component, useState, useContext, useReducer, useMemo } from "react";
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
import { BsSearch } from 'react-icons/bs';
import { AiOutlineStop } from 'react-icons/ai';
import {
    BrowserRouter as Router,
    Switch,
    Route,
    Link,
    useParams,
    useRouteMatch
} from "react-router-dom";
//import cities from "cities.json";
import cities from "./Cities";
import Post from "../post/Post";
import "./DiscoverStyle.css";
import SearchContext from "./SearchContext";
import SearchReducer from "./SearchReducer";
import { HttpClientFactory } from "../../httpclient";
import { URLSearchParams } from "url";
import { SearchParameters } from "../../api";


const mockData = [

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
        location: "Lyon, France",
        galleryId: 1003,
    },
    {
        id: 7,
        username: "Guillaume Leroux",
        href: "https://www.lelongweekend.com/wp-content/uploads/2018/07/N75_4248-1024x684.jpg",
        location: "Lyon, France",
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
        location: "Lyon, France",
        galleryId: 1007,
    },


]



export default function Discover() {

    const ctx = useContext(SearchContext);
    const [state, dispatch] = useReducer(SearchReducer, ctx);

    const [show, setShow] = useState(false);
    const [suggestions, setSuggestions] = useState([]);

    function handleChange() {
        setShow(true);
        var search = document.getElementById("search-location").value; 
        var result = cities.filter(city => city.match(`${search}`));
        result = result.slice(0, 5);
        setSuggestions(result);
    }


    useMemo(async () => {

        const api = HttpClientFactory.getPostClient();
        var model = new SearchParameters({ terms: null, nearby: true, pageNumber: 1 });
        var result = await api.search(model);
        //console.log(result);
        //state.data = result;
    }, []);

    

    async function performSearch()
    {
        setShow(false);
        var location = document.getElementById("search-location").value;

        const api = HttpClientFactory.getPostClient();
        var model = new SearchParameters({ terms: location, nearby: false, pageNumber: 1 });
        var result = await api.search(model);
        console.log(result);

       // var result = mockData.filter(rs => rs.location.match(`${location}`));
        //result = result.slice(0, 100);
        //console.log(location);
        //dispatch({
        //    type: "search",
        //    data: {
        //        data: result,
        //    }
        //});

    }

    function sendString(value) {
        document.getElementById("search-location").value = value;
    }

    
        
        return (
            <>
                <div className='search-bar'>
                    <div className='row justify-content-center'>
                        <div className='col-12 col-sm-8 col-md-8 col-lg-4'>
                            <div className="search-container">
                                <input type="text" placeholder="Where are you going ?" autoComplete="off" id="search-location" onChange={handleChange} onBlur={() => setShow(false)} className="searchbar" />
                                <img src="https://images-na.ssl-images-amazon.com/images/I/41gYkruZM2L.png" onClick={() => performSearch()} alt="Magnifying Glass" className="button-search" />
                            </div>
                            {show ? <div className="search-suggestions">
                                <div className="container-fluid">
                                    <div className="row">
                                        {suggestions.length > 0 ?
                                            suggestions.map((suggestion, i) => (
                                                <div className="col-12 suggestion-line" onMouseOver={() => sendString(suggestion)} key={i}>
                                                    <BsSearch className="suggestion-icon" /> <span className="suggestion-text">{suggestion}</span>
                                                </div>))
                                            : <div className="col-12 suggestion-line" >
                                                <AiOutlineStop className="suggestion-icon" />  <span className="suggestion-text-not-found">No location found.</span>
                                            </div>}
                                    </div>
                                </div>
                            </div> : null}
                        </div>
                    </div>
                </div>
                        <div className='main'>
                            <div className='container-fluid'>
                                <div className='row square-grid'>
                                    {state.data.map((img, i) =>

                                        (<div key={i} className={`col-12 col-sm-6 col-md-6 col-lg-6 col-xl-4 square-grid-item`} style={{ backgroundImage: `url(${img.href})` }} >
                                            <Link to={`${'/discover'}/${img.id}`}>
                                                <table className="inner-container">
                                                    <tbody>
                                                        <tr>
                                                            <td valign="top">
                                                                <div className="inner-top">
                                                                    <CardHeader
                                                                        avatar={<Avatar className='user-profile' alt={img.username} src={img.href} />}
                                                                        title={<span className='guideName' >{img.username}</span>}
                                                                        subheader={<span className='spot-location'>{img.location}</span>}
                                                                    />
                                                                </div>
                                                            </td>
                                                        </tr>
                                                    </tbody>
                                                </table>
                                            </Link>
                                        </div>))
                                        }
                                </div>
                            </div>
                        </div>
            </>
        );
    }



//<div className='tags-div'>
//    <span className='tags-label'>Most used tags :</span>
//    <Chip label="Popular" className='clicked-tag tag' clickable />
//    <Chip label="Near me" className='tag unclicked-tag' clickable />
//    <Chip label="Cheapest" className='tag unclicked-tag' clickable />
//    <Chip label="Top Rated" className='clicked-tag tag' clickable />
//    <Chip label="Historical" className='clicked-tag tag' clickable />
//    <Chip label="Nature" className='clicked-tag tag' clickable />
//</div>