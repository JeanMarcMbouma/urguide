import React, { Component } from 'react';
import Avatar from '@material-ui/core/Avatar';
import Typography from '@material-ui/core/Typography';
import { makeStyles } from '@material-ui/core/styles';
import Skeleton from '@material-ui/lab/Skeleton';
import './LeftStyle.css';
import { setTimeout } from 'timers';
import { HttpClientFactory } from '../../../httpclient';
 
  

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


export default class LeftBar extends Component {


        constructor(props) {
            super(props);
            this.state = { categories:[], loading: true };
    }

    componentWillMount() {
            this.populateData();
    }
   

    render() {

      const content = this.state.categories.map((category, i) =>
            <div key={i} className="col-lg-12 row w-auto p-2 mx-2 my-2 mb-4">
                <div className="col-lg-6">
                    <Avatar style={{ width: `100%`, height: `80px`, }} src={category.imageUrl} variant="rounded" />
                </div>
                <div className="col-lg-6">
                    <Typography variant="h6" component="p">{category.name}</Typography>
                    <Typography variant="subtitle1" color="textSecondary" component="p">{category.stats} excursions</Typography>
                </div>
            </div>
        );

        return (
            <div className="col-lg-3 w-auto px-2 m-0 leftbar">
                <div className={`col-lg-12 m-2 p-0 my-3`} style={{ textTransform: 'uppercase', fontSize: '12px' }} ><b>Top categories</b></div>
                { this.state.loading ? <LeftBarSkeleton />
                : content }
            </div>
        );
    }

    populateData() {
        const client = HttpClientFactory.getLookupClient();
        client.categories().then(r => {
            this.setState({ categories: r.slice(0, 4), loading: false });
        }) 
    }
}