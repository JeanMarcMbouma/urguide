import React, { Component, useState, useContext, useReducer, useMemo, useEffect } from "react";
import {
    makeStyles,
    IconButton,
    Chip,
    Button,
    Typography,
    Avatar,
    CardHeader,
    CircularProgress,
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



export default function Discover() {

    const { cat } = useParams();
    const ctx = useContext(SearchContext);
    const [state, dispatch] = useReducer(SearchReducer, ctx);

    const [show, setShow] = useState(false);
    const [isLoading, setLoading] = useState(true);
    const [suggestions, setSuggestions] = useState([]);

    function handleChange() {
        setShow(true);
        var search = document.getElementById("search-location").value; 
        var result = cities.filter(city => city.match(`${search}`));
        result = result.slice(0, 5);
        setSuggestions(result);
    }


    useEffect(() => {

        let doWork = async () => {

            const api = HttpClientFactory.getPostClient();
            if(cat === "nearme") {

            var model = new SearchParameters({ term: null, nearby: true, pageNumber: 1 });
            var result = await api.search(model);
            var items = result.items.filter(i => i.images.length > 0);
            dispatch({
                type: "near-me",
                data: {
                    itemsCount: result.itemsCount,
                    pageNumber: result.pageNumber,
                    items: items,
                }
            });
        }
        else
        {
            var model = new SearchParameters({ term: cat, nearby: false, pageNumber: 1 });
            var result = await api.search(model);
            var items = result.items.filter(i => i.images.length > 0);
            dispatch({
                type: "search",
                data: {
                    itemsCount: result.itemsCount,
                    pageNumber: result.pageNumber,
                    items: items,
                }
            });
        }

            setLoading(false);
        }
        doWork();

        return () => { };
    }, []);

    

    async function performSearch()
    {
        setLoading(true);
        setShow(false);
        var location = document.getElementById("search-location").value;
        const api = HttpClientFactory.getPostClient();
        var model = new SearchParameters({ term: location, nearby: false, pageNumber: 1 });
        var result = await api.search(model);
        var items = result.items.filter(i => i.images.length > 0);
        dispatch({
            type: "search",
            data: {
                itemsCount: result.itemsCount,
                pageNumber: result.pageNumber,
                items: items,
            }
        });
        setLoading(false);

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
                            {isLoading ? <div className="col-12 loading-icon"><h4 className="text-center"><CircularProgress /></h4></div> :

                             state.items.length > 0 ? 

                                        state.items.map((post, i) =>

                                            (<div key={i} className={`col-12 col-sm-6 col-md-6 col-lg-6 col-xl-4 square-grid-item`} style={{ backgroundImage: `url(${post.images[0].imageBase64})` }} >
                                                <Link to={`/post/${post.id}/shot/${post.images[0].id}`}>
                                                    <table className="inner-container">
                                                        <tbody>
                                                            <tr>
                                                                <td valign="top">
                                                                    <div className="inner-top">
                                                                        <Link to={`/g/${post.authorId}`}>
                                                                            <CardHeader
                                                                                avatar={<Avatar className='user-profile' alt={post.author} src={post.authorAvatar} />}
                                                                                title={<span className='guideName' >{post.author}</span>}
                                                                                subheader={<span className='spot-location'>{post.location}</span>}
                                                                            />
                                                                        </Link>
                                                                    </div>
                                                                </td>
                                                            </tr>
                                                        </tbody>
                                                    </table>
                                                </Link>
                                            </div>))
                                        
                                        : <div className="col-12 loading-icon"><h4 className="text-center">No content found.</h4></div>               
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