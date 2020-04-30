import React, {useState, useContext} from 'react';
import {makeStyles} from '@material-ui/core/styles';
import Card from '@material-ui/core/Card';
import CardHeader from '@material-ui/core/CardHeader';
import CardContent from '@material-ui/core/CardContent';
import CardActions from '@material-ui/core/CardActions';
import Avatar from '@material-ui/core/Avatar';
import IconButton from '@material-ui/core/IconButton';
import Typography from '@material-ui/core/Typography';
import ButtonGroup from '@material-ui/core/ButtonGroup';
import {red} from '@material-ui/core/colors';
import ShareIcon from '@material-ui/icons/Share';
import PhotoIcon from '@material-ui/icons/Photo';
import VideoLibraryIcon from '@material-ui/icons/VideoLibrary';
import PersonAddIcon from '@material-ui/icons/PersonAdd';
import AddLocationIcon from '@material-ui/icons/AddLocation';
import MoreHorizIcon from '@material-ui/icons/MoreHoriz';
import Button from '@material-ui/core/Button';
import CardMedia from '@material-ui/core/CardMedia';
import TextField from '@material-ui/core/TextField';
import ChatIcon from '@material-ui/icons/Chat';
import Chip from '@material-ui/core/Chip';
import Paper from '@material-ui/core/Paper';
import TagFacesIcon from '@material-ui/icons/TagFaces';
import PropTypes from 'prop-types';
import './CentralStyle.css';
import 'date-fns';
import Grid from '@material-ui/core/Grid';
import DateFnsUtils from '@date-io/date-fns';
import {
  MuiPickersUtilsProvider,
  KeyboardTimePicker,
  KeyboardDatePicker,
} from '@material-ui/pickers';
import {withStyles} from '@material-ui/core/styles';
import {SdCard} from '@material-ui/icons';
import AddPhoto, {PhotoX} from './../../AddPhoto/AddPhoto';
import AddPhotoContext from './../../AddPhoto/AddPhotoContext';
import Modal from 'react-bootstrap/Modal'
//import { useReactOidc} from '@axa-fr/react-oidc-context';

const styles = {
  root: {
    background: props =>
      props.color === 'red'
        ? 'linear-gradient(45deg, #FE6B8B 30%, #FF8E53 90%)'
        : 'linear-gradient(45deg, #2196F3 30%, #21CBF3 90%)',
    border: 0,
    borderRadius: 3,
    boxShadow: props =>
      props.color === 'red'
        ? '0 3px 5px 2px rgba(255, 105, 135, .3)'
        : '0 3px 5px 2px rgba(33, 203, 243, .3)',
    color: 'white',
    height: 30,
    padding: '0 30px',
  },
};

function ButtonInPosts (props) {
  const {classes, color, ...other} = props;
  return <Button className={classes.root} {...other} />;
}

ButtonInPosts.propTypes = {
  classes: PropTypes.object.isRequired,
  color: PropTypes.oneOf (['blue', 'red']).isRequired,
};

const ButtonP = withStyles (styles) (ButtonInPosts);

export default function CentralBar () {
  
  const uploadButton = React.createRef ();
  const [ViewPostCreating, setViewPostCreating] = useState (false);

  const [context, setContext] = React.useState ({
    files: [],
    currentFile: null,
  });

  const update = ctx => {
    setContext ({...ctx});
  };
  const {oidcUser} = {
    oidcUser: {
      profile: {
        name: 'Demo'
      }
    }
  } // useReactOidc();
  if (!oidcUser)
  {
    return (
      <></>
    )
  }
  const { profile } = oidcUser;
  
  let posts = [
    {
      name: 'Excursion around Cherkassy',
      description: 'I will show you this beautiful town',
      price: '250',
      category: 'mix',
      currentHuman: '12',
      LimitHuman: '30',
      author: 'Ivanna',
      dateStart: '12.04.20',
      profilePhoto: 'https://images.pexels.com/photos/3541390/pexels-photo-3541390.jpeg?cs=srgb&dl=close-up-photo-of-woman-wearing-red-sweater-3541390.jpg&fm=jpg',
    },
    {
      name: 'Football field in Kyiv',
      description: 'I will show you the biggest football field in Kyiv',
      category: 'sport',
      price: '450',
      currentHuman: '9',
      LimitHuman: '30',
      author: 'Lena',
      dateStart: '01.05.20',
      profilePhoto: 'https://images.pexels.com/photos/3690085/pexels-photo-3690085.jpeg?cs=srgb&dl=photo-of-woman-wearing-black-turtle-neck-top-3690085.jpg&fm=jpg',
    },
  ];

  function DatePicker () {
    const [selectedDate, setSelectedDate] = React.useState (
      new Date ('2020-04-13T14:48:54')
    );

    const handleDateChange = date => {
      setSelectedDate (date);
    };

    return (
      <MuiPickersUtilsProvider utils={DateFnsUtils}>
        <Grid container justify="space-around">
          <KeyboardDatePicker
            disableToolbar
            variant="inline"
            format="MM/dd/yyyy"
            margin="normal"
            label="Choose date"
            value={selectedDate}
            onChange={handleDateChange}
            KeyboardButtonProps={{
              'aria-label': 'change date',
            }}
          />
          <KeyboardTimePicker
            margin="normal"
            label="Choose time"
            value={selectedDate}
            onChange={handleDateChange}
            KeyboardButtonProps={{
              'aria-label': 'change time',
            }}
          />
        </Grid>
      </MuiPickersUtilsProvider>
    );
  }

  function ChipsArray () {
    const [chipData, setChipData] = React.useState ([
      {key: 0, label: 'Sport', checked: false},
      {key: 1, label: 'Historical', checked: false},
      {key: 2, label: 'Child', checked: false},
      {key: 3, label: 'Nature', checked: false},
      {key: 4, label: 'Other', checked: false},
    ]);

    return (
      <Paper className="col-lg-12 d-flex justify-content-between">
        {chipData.map (data => {
          return (
            <Chip
              key={data.key}
              color="primary"
              variant={data.checked ? 'default' : 'outlined'}
              label={data.label}
              className="my-2"
              onClick={() => {
                let newChipsData = [...chipData];
                newChipsData[data.key].checked = !newChipsData[data.key]
                  .checked;
                setChipData (newChipsData);
              }}
            />
          );
        })}
      </Paper>
    );
  }

    function ViewPost() {

        const [show, setShow] = useState(false);
    
    return (<>
            <div className={`col-lg-12 p-3 mb-3 bg-white rounded new-post-card`}>
                <div className="new-post-btn" onClick={() => setShow(true)}>
                    <span>Want to write a new post ?</span>
                </div>
            </div>
            <Modal
            size="md"
            aria-labelledby="contained-modal-title-vcenter"
            centered
            animation={true}
            show={show}
            onHide={() => setShow(false)}
        >
            <Modal.Header closeButton>
                <Modal.Title id="contained-modal-title-vcenter">
                    Modal heading
        </Modal.Title>
            </Modal.Header>
            <Modal.Body>
                    <div className="col-lg-12 d-flex justify-content-start my-4">
                        <Avatar alt="Remy Sharp" src="/static/images/avatar/1.jpg" />
                        <Typography className="m-0 px-4" variant="h6">
                            {profile.name}
                        </Typography>
                    </div>
                    <div className="col-lg-12">
                        <TextField fullWidth label="Name" variant="outlined" />
                        <TextField
                            fullWidth
                            className="my-2"
                            label="Short description"
                            multiline
                            rows="6"
                            variant="outlined"
                        />
                        <TextField label="Price" variant="outlined" />
                        <DatePicker />
                        <ChipsArray />
                        <PhotoX />
                        <AddPhoto fileInput={uploadButton} update={update} />
                    </div>
                    <div className="col-lg-12 my-2">
                        <ButtonGroup
                            fullWidth
                            size="large"
                            color="primary"
                            aria-label="large outlined primary button group"
                        >
                            <Button onClick={() => uploadButton.current.click()}>
                                <PhotoIcon />
                            </Button>
                            <Button><VideoLibraryIcon /></Button>
                            <Button><AddLocationIcon /></Button>
                            <Button><MoreHorizIcon /></Button>
                        </ButtonGroup>
                    </div>
            </Modal.Body>
            <Modal.Footer>
                <div className="col-lg-12">
                    <ButtonP fullWidth variant="outlined" color="blue">Publish</ButtonP>
                </div>
            </Modal.Footer>
        </Modal></>);
  }

  return (
    <div className="col-12 col-sm-7 col-md-7 col-lg-6 col-xl-5 timeline">
      <div className="container">
              <AddPhotoContext.Provider value={context}>
          <ViewPost />
              </AddPhotoContext.Provider>
        {posts.map ((post, i) => (
          <div key={i} className="p-3 mb-3 bg-white rounded post-card">
            <CardHeader
              avatar={<Avatar alt="profile photo" src={post.profilePhoto} />}
              title={
                <Typography variant="body1" component="p">
                  {post.author} | {post.name} | {post.category}
                </Typography>
              }
              subheader={post.dateStart}
            />
            <CardContent>
              <Typography variant="subtitle1" component="p">
                {post.description}
              </Typography>
            </CardContent>
            <CardActions className="d-flex justify-content-around">
              <IconButton aria-label="share">
                <ShareIcon />
              </IconButton>
              <IconButton>
                <ChatIcon />
              </IconButton>
              <ButtonP color="red">for {post.price}</ButtonP>
              <ButtonP color="blue">{`${post.currentHuman}/${post.LimitHuman}`}</ButtonP>
            </CardActions>
          </div>
        ))}
      </div>
    </div>
  );
}


//<div className={`col-lg-12 p-3 mb-3 bg-white rounded new-post-card`}
//>
//    <div className="new-post-btn" onClick={() => setViewPostCreating(true)}>
//        <span>Want to write a new post ?</span>
//    </div>
//</div>
//                : <div
//    className={`col-lg-12 p-3 mb-3 bg-white rounded shadow-lg bg-white rounded`}
//>
//    <div className="col-lg-12 d-flex justify-content-between">
//        <Typography variant="h5">Create your post!</Typography>
//        <ButtonP
//            onClick={() => setViewPostCreating(false)}
//            variant="outlined"
//            color="blue"
//        >
//            X
//            </ButtonP>
//    </div>
//    <div className="col-lg-12 d-flex justify-content-start my-4">
//        <Avatar alt="Remy Sharp" src="/static/images/avatar/1.jpg" />
//        <Typography className="m-0 px-4" variant="h6">
//            {profile.name}
//        </Typography>
//    </div>
//    <div className="col-lg-12">
//        <TextField fullWidth label="Name" variant="outlined" />
//        <TextField
//            fullWidth
//            className="my-2"
//            label="Short description"
//            multiline
//            rows="6"
//            variant="outlined"
//        />
//        <TextField label="Price" variant="outlined" />
//        <DatePicker />
//        <ChipsArray />
//        <PhotoX />
//        <AddPhoto fileInput={uploadButton} update={update} />
//    </div>
//    <div className="col-lg-12 my-2">
//        <ButtonGroup
//            fullWidth
//            size="large"
//            color="primary"
//            aria-label="large outlined primary button group"
//        >
//            <Button onClick={() => uploadButton.current.click()}>
//                <PhotoIcon />
//            </Button>
//            <Button><VideoLibraryIcon /></Button>
//            <Button><AddLocationIcon /></Button>
//            <Button><MoreHorizIcon /></Button>
//        </ButtonGroup>
//    </div>
//    <div className="col-lg-12">
//        <ButtonP fullWidth variant="outlined" color="blue">Publish</ButtonP>
//    </div>
//</div>