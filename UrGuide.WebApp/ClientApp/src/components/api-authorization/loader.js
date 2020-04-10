import React from 'react'
import CircularProgress from '@material-ui/core/CircularProgress';
import { makeStyles } from '@material-ui/core/styles';

const useStyles = makeStyles(theme => ({
    positionLoader: {
        top: '45%',
        left: '45%',
        position: 'absolute',
    },
  }));

export default function Loader() {
    const styles = useStyles()
    return(
        <div className={styles.positionLoader}>
            <CircularProgress />
        </div>
    )
}