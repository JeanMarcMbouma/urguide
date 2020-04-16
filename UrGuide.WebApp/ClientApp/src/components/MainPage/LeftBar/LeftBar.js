import React from 'react'
import Avatar from '@material-ui/core/Avatar';
import Typography from '@material-ui/core/Typography';
import { makeStyles } from '@material-ui/core/styles';
import './LeftStyle.css';

const useStyles = makeStyles(theme => ({
    large: {
      width: '100%',//theme.spacing(6.8),
      height: '80px',
    },
    text: {
        textTransform:'uppercase',
        fontSize:'12px',
    }
  }));

export default function LeftBar() {

    const categories = [
        {
            name:"Sport",
            href:"https://images.pexels.com/photos/270085/pexels-photo-270085.jpeg?cs=srgb&dl=athletes-audience-ball-bleachers-270085.jpg&fm=jpg",
            current: "123"
        },
        {
            name:"Historical",
            href:"https://images.pexels.com/photos/2104044/pexels-photo-2104044.jpeg?cs=srgb&dl=people-on-road-2104044.jpg&fm=jpg",
            current: "183"
        },
        {
            name:"Child",
            href:"https://images.pexels.com/photos/298825/pexels-photo-298825.jpeg?cs=srgb&dl=wood-people-creative-hand-298825.jpg&fm=jpg",
            current: "33"
        },
        {
            name:"Nature",
            href:"https://images.pexels.com/photos/3933999/pexels-photo-3933999.jpeg?cs=srgb&dl=father-and-child-near-body-of-water-3933999.jpg&fm=jpg",
            current:"89", 
        }
    ]

      const classes = useStyles();
    return(
        <div className="col-lg-3 w-auto px-2 m-0 leftbar">
            <div className={`col-lg-12 m-2 p-0 my-3 ${classes.text}`}><b>Top categories</b></div>
            {categories.map((category, i) => 
                <div key={i} className="col-lg-12 row w-auto p-2 mx-2 my-2 mb-4">
                    <div className="col-lg-6">
                        <Avatar className={classes.large} src={category.href} variant="rounded"/>
                    </div>
                    <div className="col-lg-6">
                         <Typography variant="h6" component="p">{category.name}</Typography>
                         <Typography variant="subtitle1" color="textSecondary" component="p">{category.current} excursions</Typography>
                   </div>
                </div>
                )}
        </div>
    )
}