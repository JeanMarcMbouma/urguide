import React, { useState } from 'react';
import GridList from '@material-ui/core/GridList';
import GridListTile from '@material-ui/core/GridListTile';
import { makeStyles } from '@material-ui/core/styles';

const uri = "https://picsum.photos/v2/list/?limit=9";

export function Gallery () {
    const [images, setImages] = useState([])

     fetch(uri)
            .then(result => result.json())
         .then(jsonImages => setImages(jsonImages))

    const useStyles = makeStyles(theme => ({
        root: {
            display: 'flex',
            flexWrap: 'wrap',
            justifyContent: 'space-around',
            overflow: 'hidden',
            backgroundColor: theme.palette.background.paper,
        },
        gridList: {
            height: 500,
        },
    }));

    const classes = useStyles();

        return (
        <div className="col-lg-9">
        <GridList cellHeight={200} className = {classes.gridList} cols={3}>
            {images.map(tile => (
                <GridListTile key={tile.id} cols={tile.cols || 1}>
                   <img src={tile.download_url} alt={tile.title} />
                </GridListTile>))}
        </GridList>
        </div>
    )
}