import React, { Component } from 'react';
//import { Container } from 'reactstrap';
import Header from './Header';
import './NavMenu.css';
export class Layout extends Component {
  static displayName = Layout.name;

  render () {
      return (
          <>
              <Header />
              <div className="container-fluid content">
                  <div className='row mb-4' >
                      <div className='col-12'>
                          {this.props.children}
                </div>
                  </div>
              </div>
         </>
    );
  }
}

