import React from 'react';
import { Link } from 'react-router-dom';

const Popular = () => {

    return (
        <div className='col-sm-4 col-md-4 col-lg-2 col-xl-2 rounded rightbar popular' >
            {/* <div>
                <div className="d-lg-flex p-0 mb-3 mt-3">
                    <div className='font-weight-bold title'>
                        Will be soon
                    </div>
                </div>
            </div> */}
            <div className='copyright-div'>
                <span><Link className='link' to='/terms'>Terms</Link> - <Link className='link' to='/conditions'>Conditions</Link> - <Link className='link' to='/cookies'>Cookies</Link></span>
                <br />
                <span>&copy; Urguide 2020</span>
            </div>
        </div>
    );}
export default Popular;
