import React from 'react'
import Avatar from '@material-ui/core/Avatar';
import Typography from '@material-ui/core/Typography';
import { makeStyles } from '@material-ui/core/styles';
import './LeftStyle.css';
import { connect } from 'react-redux'


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

function LeftBar(props) {

    const classes = useStyles();
    return(
            <div className="col-lg-3 w-auto px-2 m-0 leftbar">
            <div className={`col-lg-12 m-2 p-0 my-3 ${classes.text}`}><b>Top categories</b></div>
            {props.Categories.map((category, i) => 
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
const mapStateToProps = (state) => {
    return {Categories: state.Categories}
}
export default connect(mapStateToProps)(LeftBar)