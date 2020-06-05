import React, { useState, useEffect } from 'react';
import Avatar from '@material-ui/core/Avatar';
import Typography from '@material-ui/core/Typography';
import Skeleton from '@material-ui/lab/Skeleton';
import './LeftStyle.css';
import { HttpClientFactory } from '../../../httpclient';
import { Link } from "react-router-dom";
import { ActionTypes, useDataContext } from '../../../data/GlobalDataContext';
import { LookupClient } from '../../../api';
 
  

function LeftBarSkeleton() {

    const skeleton = <div className="col-lg-12 row w-auto p-2 mx-2 my-2 mb-4">
        <div className="col-lg-6">
            <Skeleton animation="wave" variant="rect" style={{ width: `130px`, height: `80px`, borderRadius: `8px` }} />
        </div>
        <div className="col-lg-6">
            <Skeleton variant="text" style={{ marginLeft: `10px`, width: `80px` }} />
            <Skeleton variant="text" style={{ marginLeft: `10px`, width: `160px` }} />
        </div>
</div>;

    return (<>
        {skeleton}
        {skeleton}
        {skeleton}
        {skeleton}
    </>);
}


const LeftBar = () => {
    const [categories, setCategories] = useState([]);
    const [loading, setLoading] = useState(true);

    const { dataContext, dcReducer } = useDataContext();

    useEffect(() => {
        var fetch = async () => {
            if (dataContext && dataContext.categories && dataContext.categories.length) {
                setCategories(dataContext.categories);
                setLoading(false);
                return;
            }

            const client = HttpClientFactory.get(LookupClient);
            client.categories().then(result => {
                dcReducer({ type: ActionTypes.CATEGORIES, data: result });
                setCategories(result);
                setLoading(false);
            })

        };
        fetch();
        return () => { };
    }, []);

    const content = categories.slice(0, 4).map((category, i) =>
        <Link key={i} to={`/discover/${category.name}`}>
        <div key={i} className="col-lg-12 row w-auto p-2 mx-2 my-2 mb-4">
            <div className="col-lg-6">
                <Avatar style={{ width: `100%`, height: `80px`, }} src={category.imageUrl} variant="rounded" />
            </div>
            <div className="col-lg-6">
                <Typography variant="h6" component="p">{category.name}</Typography>
                <Typography variant="subtitle1" color="textSecondary" component="p">{category.stats} excursions</Typography>
                </div>
            </div>
        </Link>
    );

    return (
        <div className="col-lg-3 w-auto px-2 m-0 leftbar">
            <div className={`col-lg-12 m-2 p-0 my-3`} style={{ textTransform: 'uppercase', fontSize: '12px' }} ><b>Top categories</b></div>
            {loading ? <LeftBarSkeleton /> : content}
        </div>
    );
}
export default LeftBar;