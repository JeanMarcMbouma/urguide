import React from "react";
import { Link } from 'react-router-dom';
import "./UserStyle.css";
import PaymentIcon from '@material-ui/icons/Payment';
import VisibilityIcon from '@material-ui/icons/Visibility';
import PersonIcon from '@material-ui/icons/Person';
import HighlightOffIcon from '@material-ui/icons/HighlightOff';


export default function EditProfileNavigation(props) {

    return (
        <div className="container-fluid edit-panel-card" >
            <div className='row justify-content-center'>
                <div className='col-11'>
                    <h5 className='text-muted'>Account Settings</h5>
                    <br />
                </div>
                {props.isGuide ? <>
                    <div className='col-11 edit-panel-link'>
                        <Link to='/profile/details' style={{ textDecoration: `none` }} tag={Link} className="text-dark" ><PersonIcon fontSize="small" /> <span className='btn-title'>Personal Information</span></Link>
                    </div>
                    <div className='col-11 edit-panel-link'>
                        <Link to='/profile/password' style={{ textDecoration: `none` }} tag={Link} className="text-dark" ><VisibilityIcon fontSize="small" /> <span className='btn-title'>Change Password</span></Link>
                    </div>
                    <div className='col-11 edit-panel-link'>
                        <Link to='/' style={{ textDecoration: `none` }} tag={Link} className="text-dark" ><PaymentIcon fontSize="small" /> <span className='btn-title'>Credit Card Details</span></Link>
                    </div>
                    <div className='col-11 edit-panel-link'>
                        <Link to='/' style={{ textDecoration: `none` }} tag={Link} className="text-dark" ><HighlightOffIcon fontSize="small" /> <span className='btn-title'>Delete My Account</span></Link>
                    </div>
                </>
                      :

                    <>
                        <div className='col-11 edit-panel-link'>
                            <Link to='/account/details' style={{ textDecoration: `none` }} tag={Link} className="text-dark" ><PersonIcon fontSize="small" /> <span className='btn-title'>Personal Information</span></Link>
                        </div>
                        <div className='col-11 edit-panel-link'>
                            <Link to='/account/password' style={{ textDecoration: `none` }} tag={Link} className="text-dark" ><VisibilityIcon fontSize="small" /> <span className='btn-title'>Change Password</span></Link>
                        </div>
                        <div className='col-11 edit-panel-link'>
                            <Link to='/' style={{ textDecoration: `none` }} tag={Link} className="text-dark" ><PaymentIcon fontSize="small" /> <span className='btn-title'>Credit Card Details</span></Link>
                        </div>
                        <div className='col-11 edit-panel-link'>
                            <Link to='/' style={{ textDecoration: `none` }} tag={Link} className="text-dark" ><HighlightOffIcon fontSize="small" /> <span className='btn-title'>Delete My Account</span></Link>
                        </div>
                    </>
                     
                    }
              
            </div>
        </div>

    );

}
