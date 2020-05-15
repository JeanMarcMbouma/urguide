import React, {
    useState, useContext, useMemo, useReducer, Component
} from 'react';
import { makeStyles } from '@material-ui/core/styles';
import { FaRegComment } from 'react-icons/fa';
import { AiOutlineDislike } from 'react-icons/ai';
import { AiOutlineLike } from 'react-icons/ai';
import {
    Card,
    CardHeader,
    CardContent,
    CardActions,
    Avatar,
    IconButton,
    InputLabel,
    Link,
    Input,
    FormControl,
    InputAdornment,
    FormHelperText,
    Typography,
    ButtonGroup,
    Button,
    CardMedia,
    TextField,
    Chip,
    Paper,
    Switch,
    FormControlLabel,
  
} from '@material-ui/core';
import Rating from '@material-ui/lab/Rating';
import { red } from '@material-ui/core/colors';
import ShareIcon from '@material-ui/icons/Share';
import PhotoIcon from '@material-ui/icons/Photo';
import VideoLibraryIcon from '@material-ui/icons/VideoLibrary';
import PersonAddIcon from '@material-ui/icons/PersonAdd';
import AddLocationIcon from '@material-ui/icons/AddLocation';
import MoreHorizIcon from '@material-ui/icons/MoreHoriz';
import LocationOnIcon from '@material-ui/icons/LocationOn';
import RemoveCircleIcon from '@material-ui/icons/RemoveCircle';
import ChatIcon from '@material-ui/icons/Chat';
import TagFacesIcon from '@material-ui/icons/TagFaces';
import PropTypes from 'prop-types';
import './CentralStyle.css';
import 'date-fns';
import Grid from '@material-ui/core/Grid';
import Slider from '@material-ui/core/Slider';
import AttachMoneyOutlinedIcon from '@material-ui/icons/AttachMoneyOutlined';
import PeopleOutlineOutlinedIcon from '@material-ui/icons/PeopleOutlineOutlined';
import AlarmOutlinedIcon from '@material-ui/icons/AlarmOutlined';
import DateFnsUtils from '@date-io/date-fns';
import {
    MuiPickersUtilsProvider,
    KeyboardDatePicker,
    KeyboardTimePicker
} from "@material-ui/pickers";
import MomentUtils from "@date-io/moment";
import clsx from "clsx";
import { withStyles } from '@material-ui/core/styles';
import PhotoLibraryIcon from '@material-ui/icons/PhotoLibrary';
import NewPostContext from './NewPostContext';
import NewPostReducer from './NewPostReducer';
import { useAuthUser } from '../../api-authorization/AuthService';
import { useAuthContext } from '../../api-authorization/AuthService';
import authService from '../../api-authorization/AuthService';
//import { PostsClient, PostUpdateModel } from '../../../api';
import { HttpClientFactory } from '../../../httpclient';
import { PostsClient, PostCreationModel, ImageFileCreateModel, ItineraryModel } from '../../../api';

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

const useStyles = makeStyles(theme => ({
    root: {
        minHeight: "100vh"
    },
    paper: {
        display: "flex",
        flexDirection: "column",
        alignItems: "center"
    },
    avatar: {
        margin: theme.spacing(1),

    },
    form: {
        width: "100%", // Fix IE 11 issue.
        marginTop: theme.spacing(1)
    }
}));


function ButtonInPosts(props) {
    const { classes, color, ...other } = props;
    return <Button className={classes.root} {...other} />;
}

//ButtonInPosts.propTypes = {
//    classes: PropTypes.object.isRequired,
//    color: PropTypes.oneOf(['blue', 'red']).isRequired,
//};

const ButtonP = withStyles(styles)(ButtonInPosts);


const MockPosts = [
    {
        id: 0,
        author: 'Jane Doe',
        authorId: 0,
        location:'Miami Beach, FL',
        authorAvatar: "https://i.pinimg.com/originals/df/5f/5b/df5f5b1b174a2b4b6026cc6c8f9395c1.jpg",
        description: 'I am looking for a simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industrys standard dummy text ever since the 1500s',
        categories: ['sport', 'other'],
        publicationDate:'11/08/2020',
        Date: '11/11/2020',
        EndTime: "05:00 PM",
        StartTime: "03:30 AM",
        seats: 1,
        budget:60,
        images: [{
            imageBase6: "https://besttoppers.com/wp-content/uploads/2011/12/petra.jpg",
            name: "mock1.png",
            id: 0
        },
            {
                imageBase6: "https://media.tacdn.com/media/attractions-splice-spp-674x446/07/3c/cb/03.jpg",
                name: "mock2.png",
                id: 1
            },
            {
                imageBase6: "https://i2.wp.com/frenchmoments.eu/wp-content/uploads/2015/12/Eiffel-Tower-5-December-2015-01-%C2%A9-French-Moments.jpg?resize=702%2C471&ssl=1",
                name: "mock3.png",
                id: 2
            },
            {
                imageBase6: "https://www.touropia.com/gfx/b/2013/02/sacre_coeur.jpg",
                name: "mock1.png",
                id: 0
            },
        ]
    },
]

function CreateItinerary(props) {


    return (
       <div className='itinerary_wrapper'>
            <section className="itinerary">
                {props.itineraries.map((itinerary, i) => (<div key={itinerary.id} className="itinerary__block">
                    <div className="itinerary__midpoint"></div>
                    <div className="itinerary__content itinerary__content--left">
                        <h3 className="itinerary__place">{itinerary.title}</h3>
                        <p className="itinerary__text--left">
                            {itinerary.description}
                        </p>
                    </div>
                </div>))}
            </section>
        </div> 
    );

}

function Itinerary(props) {

    return (
        props.show ? <div className='itinerary_wrapper'>
            <h5>Itinerary of this tour</h5>
            <section className="itinerary">
                <div className="itinerary__block">
                    <div className="itinerary__midpoint"></div>
                    <div className="itinerary__content itinerary__content--left">
                        <h3 className="itinerary__place">Eiffel Tower</h3>
                        <p className="itinerary__text--left">
                            Celebrated my birthday with my playmates in school. What a wonderful surprise to have the same birthday as my teacher!
                                 </p>
                    </div>
                </div>
                <div className="itinerary__block">
                    <div className="itinerary__midpoint"></div>
                    <div className="itinerary__content itinerary__content--left">
                        <h3 className="itinerary__place">Musee du Louvre</h3>
                        <p className="itinerary__text--left">
                            Celebrated my birthday with my playmates in school. What a wonderful surprise to have the same birthday as my teacher!
                                       </p>
                    </div>
                </div>
                <div className="itinerary__block">
                    <div className="itinerary__midpoint"></div>
                    <div className="itinerary__content itinerary__content--left">
                        <h3 className="itinerary__place">Notre Dame</h3>
                        <p className="itinerary__text--left">
                            Celebrated my birthday with my playmates in school. What a wonderful surprise to have the same birthday as my teacher!
                                       </p>
                    </div>
                </div>
                <div className="itinerary__block">
                    <div className="itinerary__midpoint"></div>
                    <div className="itinerary__content itinerary__content--left">
                        <h3 className="itinerary__place">Champs-Elysees</h3>
                        <p className="itinerary__text--left">
                            Celebrated my birthday with my playmates in school. What a wonderful surprise to have the same birthday as my teacher!
                                       </p>
                    </div>
                </div>
                <div className="itinerary__block">
                    <div className="itinerary__midpoint"></div>
                    <div className="itinerary__content itinerary__content--left">
                        <h3 className="itinerary__place">Grand Palais</h3>
                        <p className="itinerary__text--left">
                            Celebrated my birthday with my playmates in school. What a wonderful surprise to have the same birthday as my teacher!
                                       </p>
                    </div>
                </div>
            </section>
        </div> : null
    );

}


function Comments(props) {


        return (
            props.show ? <div className='comments' >
                <form noValidate autoComplete="off" className='new-bid'>
                    <TextField fullWidth multiline rows={6} rowsMax={6} id="outlined-basic" variant="outlined" placeholder="Make a bid on this post !" />
                    <br />
                    <br />
                    <Button variant="contained" color="primary">Bid now</Button>
                </form>
                <div className='cmt-div'>
                    <CardHeader
                        avatar={<Avatar alt="profile photo" src={props.post.authorAvatar} />}
                        title={
                            <h6>
                                {props.post.author}
                            </h6>
                        }
                        subheader={props.post.publicationDate}
                    />
                    <div className='comment-text'>
                        <p>
                           simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s.
                                    </p>
                    </div>
                </div>
                <div className='cmt-div'>
                    <CardHeader
                        avatar={<Avatar alt="profile photo" src={props.post.authorAvatar} />}
                        title={
                            <h6>
                                {props.post.author} {<Rating name="half-rating-read" className='user-rating' defaultValue={4.5} precision={0.5} readOnly />}
                            </h6>
                        }
                        subheader={props.post.publicationDate}
                    />
                    <div className='comment-text'>
                        <p>I am happy to pay a fixed priced and my maximum budget is $30 USD.</p>
                    </div>
                </div>
            </div> : null
        );
}

const navigateToReturnUrl = returnUrl => {

    window.location.replace(returnUrl);
}

export default function CentralBar() {

    const classes = useStyles();


    const uploadButton = React.createRef();
    const [ViewPostCreating, setViewPostCreating] = useState(false);

    const [context, setContext] = React.useState({
        files: [],
        currentFile: null,
    });

    const update = ctx1 => {
        setContext({ ...ctx1 });
    };

    const ctx = useContext(NewPostContext);
    const [state, dispatch] = useReducer(NewPostReducer, ctx);


    const [posts, setPosts] = useState([]);
    const user = useAuthUser();
    const { profile } = user || {
        profile: {}
    };

    useMemo(async () => {
        const api = HttpClientFactory.getPostClient();
        var result = await api.last10();
        setPosts(result);
    }, []);


    const [showComments, setShowComments] = React.useState(false);
    const [showItinerary, setShowItinerary] = React.useState(false);

    function toggleComments() {
        setShowComments(!showComments);
    }

    function toggleItinerary() {
        setShowItinerary(!showItinerary);
    }


    function ViewPost() {

 

        async function createNewPost(state) {

            const client = HttpClientFactory.getPostClient(user);

            const model = new PostCreationModel({
                text: state.text,
                description: state.description,
                geoLocation: state.geoLocation,
                startTime: state.startTime,
                endTime: state.EndTime,
                seats: state.seats,
                unitPrice: state.priceRange,
                categories: state.categories,
                images: state.files.map(i => new ImageFileCreateModel({...i})),
                itineraries: state.itineraries.map(i => new ItineraryModel({...i})),
                bidOptIn: state.bidOptIn,
            });

            try {

                await client.create(model);
                const returnUrl = authService.getReturnUrl();
                navigateToReturnUrl(returnUrl);
               
            }
            catch (e) {
                console.log(e);
            }

        }

        const [show, setShow] = useState(state.showPost);
        const [Categories, setCategories] = React.useState([
            { key: 0, label: 'Sport', checked: false },
            { key: 1, label: 'Historical', checked: false },
            { key: 2, label: 'Child', checked: false },
            { key: 3, label: 'Nature', checked: false },
            { key: 4, label: 'Other', checked: false },
        ]);

        
        const [cats, setCats] = React.useState([]);
        const [values, setValues] = React.useState([]);
        const [itineraryError, setItineraryError] = React.useState(false);
        const [btnEnabled, setBtnEnabled] = React.useState(false);
        const [selectedDate, setSelectedDate] = React.useState(new Date());
        const [selectedStartTime, setSelectedStartTime] = React.useState(new Date());
        const [selectedEndTime, setSelectedEndTime] = React.useState(new Date());
        const [infos, setPostInfos] = React.useState({

            text: '',
            description: '',
            geoLocation: '',
            date:selectedDate,
            startTime: getDate(new Date(), true),
            endTime: getDate(new Date(), true),
            seats: 1,
            unitPrice: [25, 70],
            categories: cats,
            files: values,
            bidOptIn:true,
        });

        function getDate(time, isValid) {

            if (!isValid) {
                time = new Date();
            }

            var date = selectedDate;

            var Time = new Date(date.getFullYear(), date.getMonth(), date.getDate(), time.getHours(), time.getMinutes(), time.getSeconds());
            return Time;
        }

        function handleDescription(event) {
            setPostInfos({ ...infos, ['description']: event.target.value });
          
            if (infos.description.length >= 10 && infos.geoLocation.length >= 4)
            {
                setBtnEnabled(true);
            }
       
        }

        function handleGeoLocation(event){
            setPostInfos({ ...infos, ['geoLocation']: event.target.value });
     
            if (infos.description.length >= 10 && infos.geoLocation.length >= 4) {
                setBtnEnabled(true);
            }
        
        }

        function addItinerary() {

            var place = document.getElementById("itinerary-point").value;
            var description = document.getElementById("itinerary-point-description").value;

            if (place.length < 4 || description.length < 4) {
                setItineraryError(true);
                return false;
            }
            var itin = {
                id: (state.itineraries.length + 1),
                ordinal: (state.itineraries.length + 1),
                title: place,
                description: description
            }

            state.itineraries.push(itin);
            document.getElementById('data-sender').click();
        }

        function handleChangedFile(event) {
            const blob = event.target.files[0];
            var filePath = URL.createObjectURL(blob);
            var reader = new window.FileReader();
            reader.readAsDataURL(blob);
            reader.onloadend = function () {
                const base64data = reader.result;
                console.log(base64data);
                var file = {
                    id: (state.files.length + 1),
                    href: filePath,
                    name: filePath,
                    imageBase64: base64data,
                }

                state.files.push(file);
                //setValues(values);
                document.getElementById('data-sender').click();
            }
            
        }

        function togglePost()
        {
            setShow(!show);
        }

        const marks = [
            {
                value: 5,
                label: '$5',
            },

            {
                value: 250,
                label: '$250',
            },
        ];


        const [seats, setSeats] = React.useState(1);
        const handleChangeSeats = (event, newValue) => {
            setSeats(newValue);
            setPostInfos({ ...infos, ['seats']: newValue });
  
        };
        const [unitPrice, setUnitPrice] = React.useState([25, 70]);
        const handleChangeUnitPrice = (event, newValue) => {
            setUnitPrice(newValue);
            setPostInfos({ ...infos, ['unitPrice']: newValue });
            
        };

        const handleBidOptIn = (event) => {
            setPostInfos({ ...infos, ['bidOptIn']: event.target.checked });
        };
       
        const handleDateChange = date => {
            setSelectedDate(date._d);
            setPostInfos({ ...infos, ['date']: date._d });
        };
        const handleStartTimeChange = date => {
            setSelectedStartTime(date);
            setPostInfos({ ...infos, ['startTime']: getDate(date._d, date._isValid) });
          
        };
       
        const handleEndTimeChange = date => {
            setSelectedEndTime(date);
            setPostInfos({ ...infos, ['endTime']: getDate(date._d, date._isValid) });
        };

        function valuetext(value) {
            return `${value}`;
        }

        const { manager } = useAuthContext();

        function signIn(e) {
            e.preventDefault();
            manager.signIn(window.location.href);
            return false;
        }

        return (<>

            {show ? <> <Button fullWidth variant="contained" type="button" onClick={() => togglePost()} >Click to hide this form</Button>
                <br />
                <br />
            </>
                 :
                 user ? <div className = {`col-lg-12 p-3 mb-3 bg-white rounded new-post-card`}>
                  <div className="new-post-btn" onClick={() => togglePost()}>
                <span>Want to write a new post ?</span>
            </div>
                </div> : <div className={`col-lg-12 p-3 mb-3 bg-white rounded new-post-card`}>
                        <div className="new-post-btn" onClick={signIn} >
                            <span>Want to write a new post ?</span>
                        </div>
                    </div>

}
          

            {show ?
                <Card className='new-post-form' >
                    <div className="col-lg-12 d-flex justify-content-start my-4">
                        <Avatar alt="Remy Sharp" src="/static/images/avatar/1.jpg" />
                        <Typography className="m-0 px-4" variant="h6">
                            {profile.given_name}
                        </Typography>
                    </div>

                    <div className="col-lg-12 my-2 post-config-div">
                        <br />
                        <br />
                        <Grid item xs={12} >
                            <br />
                            <br />
                            <h6>1. Add places you'll be visiting on this tour.</h6>
                            <br />
                            <br />
                            <FormControl fullWidth className={classes.margin}>
                                <InputLabel htmlFor="itinerary-point">Place to visit</InputLabel>
                                <Input
                                    id="itinerary-point"
                                    type="text"
                                    endAdornment={<InputAdornment position="end">
                                        <LocationOnIcon />
                                    </InputAdornment>}

                                />
                            </FormControl>
                        </Grid>
                        <Grid item xs={12} >
                            <FormControl fullWidth className={classes.margin}>
                                <InputLabel htmlFor="itinerary-point-description">About the place</InputLabel>
                                <Input
                                    id="itinerary-point-description"
                                    type="text"
                                    multiline rows={6} rowsMax={6}


                                />
                            </FormControl>
                            <br />
                            {itineraryError ? <FormHelperText error>
                                The place and description fields are required.
                                </FormHelperText> : null}
                        </Grid>
                        <Grid item xs={12} sm={6} >
                            <br />
                            <Button

                                variant="contained"
                                color='primary'
                                onClick={addItinerary}
                            >
                                Add
                                    </Button>
                        </Grid>
                        <br />
                        <br />
                        <Grid item xs={12}>
                            <CreateItinerary itineraries={state.itineraries} />
                        </Grid>
                       
                        <h6>2. Add max 4 pictures of places you'll be visiting.</h6>
                        <br />
                        <br />
                        <Grid item xs={12}>
                            <input type="file" className='input-file' id='file-init' accept=".png,.jpg"
                                onChange={handleChangedFile} />
                            <Button
                                fullWidth
                                variant="contained"
                                color="default"
                                onClick={e => document.getElementById('file-init').click()}
                            >
                                <PhotoLibraryIcon />
                                <span>UPLOAD PICTURES</span>
                            </Button>
                            <button id='data-sender' className='input-file' onClick={() =>
                                dispatch({
                                    type: "update-details",
                                    data: {
                                        files: state.files,
                                        showPost: show,
                                        itineraries: state.itineraries,
                                    }
                                })
                            } >
                            </button>
                        </Grid>
                    </div>
                    <br />
                    <div className='container'>
                        <div className="row uploaded-files">
                            {state.files.map((path) => (<div className='col-12 col-lg-6' key={path.id}>
                                <div className='card file-card' >
                                    <div className='thumbnail' style={{ backgroundImage: `url(${path.href})` }}>
                                        <div className='cancel-file'>
                                            <IconButton onClick={() =>
                                                dispatch({
                                                    type: "remove-file",
                                                    data: {
                                                        idToRemove: path.id,
                                                        files: state.files,
                                                    }
                                                })
                                            }>
                                                <RemoveCircleIcon />
                                            </IconButton>
                                        </div>
                                    </div>
                                </div>
                            </div>))}
                        </div>
                    </div>
                    <div className="col-12">
                        <br />
                        <br />
                        <h6>3. Add some informations about the tour.</h6>
                        <br />
                        <br />
                        <textarea className='form-control post-textarea' placeholder="Here you can write a post !" id="new-post-description"
                            value={infos.description}
                            onChange={(e) => handleDescription(e)}
                            rows="5"

                        >
                        </textarea>
                        <br />
                        <hr />
                        <Grid container spacing={2}>
                            <Grid item xs={12} sm={6} >
                                <FormControl fullWidth className={classes.margin}>
                                    <InputLabel htmlFor="new-post-location">Location</InputLabel>
                                    <Input
                                        id="new-post-location"
                                        type="text"
                                        value={infos.geoLocation}
                                        onChange={(e) => handleGeoLocation(e)}
                                    
                                        endAdornment={<InputAdornment position="end">
                                            <LocationOnIcon />
                                        </InputAdornment>}

                                    />
                                </FormControl>
                            </Grid>
                            <Grid item xs={12} sm={6} >
                                <MuiPickersUtilsProvider utils={MomentUtils}>
                                    <KeyboardDatePicker
                                        disableToolbar
                                        variant="inline"
                                        fullWidth
                                        margin="normal"
                                        format="MM/DD/YYYY"
                                        id="date-picker-inline"
                                        label="Date"
                                        value={selectedDate}
                                        onChange={handleDateChange}
                                        KeyboardButtonProps={{
                                            "aria-label": "change date"
                                        }}
                                        style={{ marginTop: 0 }}
                                    />
                                </MuiPickersUtilsProvider>
                            </Grid>

                            <Grid item xs={12} sm={6} >
                                <MuiPickersUtilsProvider utils={MomentUtils}>
                                    <KeyboardTimePicker
                                        margin="normal"
                                        id="start-time-picker"
                                        label="Tour Start Time"
                                        value={selectedStartTime}
                                        onChange={handleStartTimeChange}
                                        KeyboardButtonProps={{
                                            'aria-label': 'change time',
                                        }}
                                    />
                                </MuiPickersUtilsProvider>
                            </Grid>
                            <Grid item xs={12} sm={6} >
                                <MuiPickersUtilsProvider utils={MomentUtils}>
                                    <KeyboardTimePicker
                                        margin="normal"
                                        id="end-time-picker"
                                        label="Tour End Time"
                                        value={selectedEndTime}
                                        onChange={handleEndTimeChange}
                                        KeyboardButtonProps={{
                                            'aria-label': 'change time',
                                        }}
                                    />
                                </MuiPickersUtilsProvider>
                            </Grid>
                            <Grid item xs={12} >
                                <Typography id="seats-slider" gutterBottom>
                                    Seats
      </Typography>
                                <Slider
                                    defaultValue={seats}
                                    getAriaValueText={valuetext}
                                    aria-labelledby="seats-slider"
                                    step={1}
                                    marks
                                    min={1}
                                    max={20}
                                    value={seats}
                                    onChange={handleChangeSeats}
                                    valueLabelDisplay="auto"
                                />
                            </Grid>
                            <Grid item xs={12} >
                                <Typography id="budget-slider" gutterBottom>
                                    Budget
      </Typography>
                                <Slider
                                    value={unitPrice}
                                    onChange={handleChangeUnitPrice}
                                    aria-labelledby="budget-slider"
                                    step={5}
                                    marks={marks}
                                    min={5}
                                    max={250}
                                    valueLabelDisplay="auto"
                                    getAriaValueText={valuetext}
                                />
                            </Grid>
                            
                        </Grid>
                        <Grid item xs={12}>
                            <br />
                            
                            <FormControlLabel control={<Switch
                                checked={infos.bidOptIn}
                                onChange={handleBidOptIn}
                                color="primary"
                                name="BidOptIn"
                                inputProps={{ 'aria-label': 'primary checkbox' }}
                            />} label="Is this post bidable ?" />
                        </Grid>
                    </div>
 
                    <br />
                    <br />
                    <div className="col-lg-12">
                        <Grid item xs={12}>
                            {
                                Categories.map((data) =>
                                    (
                                        <Chip key={data.key}
                                            color="primary"
                                            variant={data.checked ? 'default' : 'outlined'}
                                            label={data.label}
                                            className="chip"
                                            onClick={() => {
                                                let category = [...Categories];
                                                category[data.key].checked = !category[data.key]
                                                    .checked;
                                                setCategories(category);

                                                if (category[data.key].checked == true) {
                                                    cats.push(category[data.key].label);
                                                    setCats(cats);
                                                    setPostInfos({ ...infos, ['categories']: cats });
                                            
                                                }
                                            }}
                                        />
                                    ))
                            }
                        </Grid>
                        <br />
                        <br />
                        {btnEnabled ? <Button fullWidth className='btn-publish' variant="contained" color="primary" type="button"
                            onClick={() =>
                                dispatch({
                                    type: "create-post",
                                    data: {
                                        description: infos.description,
                                        geoLocation: infos.geoLocation,
                                        date:infos.date,
                                        startTime: infos.startTime,
                                        endTime: infos.endTime,
                                        seats: infos.seats,
                                        unitPrice: infos.unitPrice,
                                        itineraries: state.itineraries,
                                        files: state.files,
                                        categories: infos.categories,
                                        bidOptIn: infos.bidOptIn,
                                        callback: createNewPost,
                                        showPost:true,
                                    }
                                })
                            }
                        >
                            Publish
                            </Button>
                            :
                            <Button fullWidth className='btn-disabled' variant="contained"  type="button" disabled >Publish</Button>
                        }
                       
                    </div>
                </Card> : null }
        </>);
    }

    function PostImages(props)
    {
        if (props.images.length == 1)
        {
            return (<div className='row'>
                <div className='col-12 unique-img' style={{ backgroundImage: `url(${props.images[0].imageBase6})` }}>
                </div>
            </div>);
        }
        if (props.images.length == 2) {
            return (<div className='row'>
                <div className='col-12 col-lg-6 post-img' style={{ backgroundImage: `url(${props.images[0].imageBase6})` }}>
                </div>
                <div className='col-12 col-lg-6 post-img' style={{ backgroundImage: `url(${props.images[1].imageBase6})` }}>
                </div>
            </div>);
        }
        if (props.images.length == 3) {
            return (<div className='row'>
                <div className='col-12 col-lg-6 post-img' style={{ backgroundImage: `url(${props.images[0].imageBase6})` }}>
                </div>
                <div className='col-12 col-lg-6 post-img' style={{ backgroundImage: `url(${props.images[1].imageBase6})` }}>
                </div>
                <div className='col-12 post-img' style={{ backgroundImage: `url(${props.images[2].imageBase6})` }}>
                </div>
            </div>);
        }
        if (props.images.length == 4) {
            return (<div className='row'>
                <div className='col-12 col-lg-6 post-img' style={{ backgroundImage: `url(${props.images[0].imageBase6})` }}>
                </div>
                <div className='col-12 col-lg-6 post-img' style={{ backgroundImage: `url(${props.images[1].imageBase6})` }}>
                </div>
                <div className='col-12 col-lg-6 post-img' style={{ backgroundImage: `url(${props.images[2].imageBase6})` }}>
                </div>
                <div className='col-12 col-lg-6 post-img' style={{ backgroundImage: `url(${props.images[3].imageBase6})` }}>
                </div>
            </div>);
        }

        return null;
    }

    return (
        <div className="col-12 col-sm-7 col-md-7 col-lg-6 col-xl-5 timeline">
            <div >
                <ViewPost />
                {MockPosts.map((post, i) => (
                    <div key={i} className="p-3 mb-3 bg-white rounded post-card">
                        <CardHeader
                            avatar={<Avatar alt="profile photo" src={post.authorAvatar} />}
                            title={
                                    <h6>
                                    {post.author}
                                    </h6>
                            }
                            subheader={post.publicationDate}
                        />
                        <CardContent>
                            <div className='row'>
                                <div className='col-12' >
                                    {post.categories.map((category, i) => (<Link key={i}> <span className='category-tag'>{category}</span> </Link>))}
                                    <br />
                                    <br />
                                </div>
                                <div className='col-12'>
                                    <span><LocationOnIcon /> Place : <b>{'Place du Trocadero, Paris, France'}</b></span>
                                    <br />
                                    <br />
                                </div>
                                <div className='col-12'>
                                    <span><AttachMoneyOutlinedIcon /> Budget : <b>{'$20 - $120'}</b></span>
                                    <br />
                                    <br />
                                </div>
                                <div className='col-12'>
                                    <span><PeopleOutlineOutlinedIcon /> Seats : <b>{20}</b></span>
                                    <br />
                                    <br />
                                </div>
                                <div className='col-12'>
                                    <span><AlarmOutlinedIcon /> Time : <b>{'11/15/2020, from 04PM to 08PM'}</b></span>
                                    <br />
                                    <br />
                                </div>
                            </div>
                            <Typography variant="subtitle1" component="p">
                                {post.description}
                            </Typography>
                        </CardContent>
                        <PostImages images={post.images} />
                        <CardActions className="container-fluid"  >
                            <div className='row d-flex justify-content-center' style={{width:`100%`}}>
                                <div className='col-3 col-lg-3 text-center'>
                                    <IconButton>
                                        <AiOutlineLike />
                                    </IconButton>
                                    <span className='text-center'>102</span>
                                </div>
                                <div className='col-3 col-lg-3 text-center'>
                                    <IconButton>
                                        <AiOutlineDislike />
                                    </IconButton>
                                    <span className='text-center' >17</span>
                                </div>
                                <div className='col-3 col-lg-3 text-center'>
                                    <IconButton onClick={() => toggleComments() }>
                                        <FaRegComment />
                                    </IconButton>  
                                    <span className='text-center' >56</span>
                                </div>
                                <div className='col-3 col-lg-3 text-center'>
                                    <IconButton onClick={() => toggleItinerary() }>
                                        <LocationOnIcon />
                                    </IconButton>
                                    <span className='text-center' >5</span>
                                </div>
                            </div>  
                        </CardActions>
                        <Itinerary show={showItinerary} />
                        <Comments post={post} show={showComments} />
                    </div>
                ))}
            </div>
        </div>
    );
}

