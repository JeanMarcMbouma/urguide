import React, { useState, useEffect, useContext } from 'react';
import { Avatar, CardHeader, CircularProgress } from '@material-ui/core';
import { HttpClientFactory } from '../httpclient';
import { NotificationsClient } from '../api';
import { Link } from "react-router-dom";
import "./NavMenu.css";
import NotificationsReducer from './NotificationsReducer';
import { useReducer } from 'react';
import NotificationsContext from './NotificationsContext';


function Notification(props) {

    return (<li className="notification_li">
        <Link to={props.notification.referenceLink}>
                <CardHeader
                avatar={<Avatar alt={'P'} src={props.notification.authorImage} />}
                title={props.notification.read ? <span className="text-muted" >{props.notification.content}</span> : <b>{props.notification.content}</b>}
                subheader={props.notification.read ? <span>{props.notification.created}</span> : <span style={{ color:`#2c6ef2`}}>{props.notification.created}</span>}
            />
            </Link>
          </li>);
}

function Loading() {
    return (<div className="loading-icon"><h6 className="text-center"><CircularProgress ></CircularProgress></h6></div>);
}

export default function NotificationsBox(props)
{

    const ctx = useContext(NotificationsContext);
    const [state, dispatch] = useReducer(NotificationsReducer, ctx);

    const[isLoading, setLoading] = useState(true);
    
    useEffect(() => {
        var fetch = async () => {

            if (props.show === false || props.user === null)
                return;
            const client = HttpClientFactory.get(NotificationsClient, props.user);
            try {

                var result = await client.all(1);
               // console.log(result);
                dispatch({
                    type: "all",
                    data: {
                      
                        itemsCount: 1,  //result.itemsCountk
                        pageNumber: result.pageNumber,
                        items: [{ content: 'Lorem Ipsum is simply dummy text of the printing and typesetting industry.', created: '12-May-2020', read: false, isSystem: true }],//result.items,
                    }
                });
                setLoading(false);
            }
            catch (e) {
                console.log(e);
            }
        };
        fetch();
        return () => { };
    }, [props.user]);

        return (<div className="notification_dd">
            <div className='notification_label'>
                <h5>Notifications</h5>
            </div>
            <ul className="notification_ul">
                {isLoading ? <Loading /> : state.itemsCount > 0 ? state.items.map((notif, i) => (<Notification notification={notif} />)) : <div style={{ marginLeft: `-20px` }}><br /><h5 className='text-center text-muted'>No notifications yet.</h5></div>}
            </ul>
        </div>);

    }




