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
import { SdCard } from '@material-ui/icons';
import AddPhoto, { PhotoX } from './../../AddPhoto/AddPhoto';
import PhotoLibraryIcon from '@material-ui/icons/PhotoLibrary';
import NewPostContext from './NewPostContext';
import NewPostReducer from './NewPostReducer';
import { useAuthUser } from '../../api-authorization/AuthService';
import { PostsClient, PostUpdateModel } from '../../../api';
import { HttpClientFactory } from '../../../httpclient';

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


function Itinerary(props) {

    return (
        props.show ? <div className='itinerary_wrapper'>
            <h5>Itinerary of this tour</h5>
            <section class="itinerary">
                <div class="itinerary__block">
                    <div class="itinerary__midpoint"></div>
                    <div class="itinerary__content itinerary__content--left">
                        <h3 class="itinerary__place">Eiffel Tower</h3>
                        <p class="itinerary__text--left">
                            Celebrated my birthday with my playmates in school. What a wonderful surprise to have the same birthday as my teacher!
                                 </p>
                    </div>
                </div>
                <div class="itinerary__block">
                    <div class="itinerary__midpoint"></div>
                    <div class="itinerary__content itinerary__content--left">
                        <h3 class="itinerary__place">Musee du Louvre</h3>
                        <p class="itinerary__text--left">
                            Celebrated my birthday with my playmates in school. What a wonderful surprise to have the same birthday as my teacher!
                                       </p>
                    </div>
                </div>
                <div class="itinerary__block">
                    <div class="itinerary__midpoint"></div>
                    <div class="itinerary__content itinerary__content--left">
                        <h3 class="itinerary__place">Notre Dame</h3>
                        <p class="itinerary__text--left">
                            Celebrated my birthday with my playmates in school. What a wonderful surprise to have the same birthday as my teacher!
                                       </p>
                    </div>
                </div>
                <div class="itinerary__block">
                    <div class="itinerary__midpoint"></div>
                    <div class="itinerary__content itinerary__content--left">
                        <h3 class="itinerary__place">Champs-Elysees</h3>
                        <p class="itinerary__text--left">
                            Celebrated my birthday with my playmates in school. What a wonderful surprise to have the same birthday as my teacher!
                                       </p>
                    </div>
                </div>
                <div class="itinerary__block">
                    <div class="itinerary__midpoint"></div>
                    <div class="itinerary__content itinerary__content--left">
                        <h3 class="itinerary__place">Grand Palais</h3>
                        <p class="itinerary__text--left">
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

    const [values, setValues] = React.useState([]);

    const [infos, setPostInfos] = React.useState({
        email: '',
        description:'',
        location: '',
        date: '',
        startTime: '',
        endTime: false,
        seats: 0,
        budget: 0,
        categories:[],
        files: values,
    });

    const handleChange = prop => event => {
        setPostInfos({ ...infos, [prop]: event.target.value });
    };

    function handleChangedFile(event) {

        var filePath = URL.createObjectURL(event.target.files[0]);

        var file = {
            id:values.length,
            href:filePath
        }
       
        values.push(file);

        setValues(values);

        document.getElementById('data-sender').click();
    }

    const [posts, setPosts] = useState([]);
    const user = useAuthUser();
    const { profile } = user || {
        profile: {}
    };

    useMemo(async () => {
        const api = HttpClientFactory.getPostClient(user);
        var result = await api.last10();
        setPosts(result);
    }, [user]);


    const [Categories, setCategories] = React.useState([
        { key: 0, label: 'Sport', checked: false },
        { key: 1, label: 'Historical', checked: false },
        { key: 2, label: 'Child', checked: false },
        { key: 3, label: 'Nature', checked: false },
        { key: 4, label: 'Other', checked: false },
    ]);

    const [showComments, setShowComments] = React.useState(false);
    const [showItinerary, setShowItinerary] = React.useState(false);

    function toggleComments() {
        setShowComments(!showComments);
    }

    function toggleItinerary() {
        setShowItinerary(!showItinerary);
    }

    const [selectedDate, setSelectedDate] = React.useState(new Date());

    const handleDateChange = date => {
        setSelectedDate(date);

    };

    function setDescription() {
       
       document.getElementById('data-sender').click()
    }



    function ViewPost() {

        const [show, setShow] = useState(state.showPost);

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

        const [valueSlider, setValueSlider] = React.useState([25, 70]);

        const handleChangeSlider = (event, newValue) => {
            setValueSlider(newValue);
        };

        function valuetext(value) {
            return `${value}`;
        }

        return (<>

            {show ? <> <Button fullWidth variant="contained" type="button" onClick={() => togglePost()} >Click to hide this form</Button>
                <br />
                <br />
            </>
                 :
                <div className={`col-lg-12 p-3 mb-3 bg-white rounded new-post-card`}>
                <div className="new-post-btn" onClick={() => togglePost()}>
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
                    <div className="col-12 post-config-div">
                        <textarea className='form-control post-textarea' placeholder="Here you can write a post !" id="new-post-description"  rows="5" value="" ></textarea>
                        <br />
                        <hr />
                        <Grid container spacing={2}>
                            <Grid item xs={12} sm={6} >
                                <FormControl fullWidth className={classes.margin}>
                                    <InputLabel htmlFor="new-post-location">Location</InputLabel>
                                    <Input
                                        id="new-post-location"
                                        type="text"
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
                                        label="Excursion Start Time"
                                        value={selectedDate}
                                        onChange={handleDateChange}
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
                                        label="Excursion End Time"
                                        value={selectedDate}
                                        onChange={handleDateChange}
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
                                    defaultValue={1}
                                    getAriaValueText={valuetext}
                                    aria-labelledby="seats-slider"
                                    step={1}
                                    marks
                                    min={1}
                                    max={20}
                                    valueLabelDisplay="auto"
                                />
                            </Grid>
                            <Grid item xs={12} >
                                <Typography id="budget-slider" gutterBottom>
                                    Budget
      </Typography>
                                <Slider
                                    value={valueSlider}
                                    onChange={handleChangeSlider}
                                    aria-labelledby="budget-slider"
                                    step={5}
                                    marks={marks}
                                    min={5}
                                    max={250}
                                    valueLabelDisplay="auto"
                                    getAriaValueText={valuetext}
                                />
                            </Grid>
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
                                        }}
                                    />
                                ))
                                }
                            </Grid>
                        </Grid>
                    </div>
 
                    <div className="col-lg-12 my-2">
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
                                <span>UPLOAD PHOTOS</span>
                            </Button>
                            <button id='data-sender' className='input-file' onClick={() =>
                                dispatch({
                                    type: "update-details",
                                    data: {
                                        files: values,
                                        showPost: show,
                                        description: document.getElementById("new-post-description").value,
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
                    <br />
                    <br />
                    <div className="col-lg-12">
                        {state.isButtonEnabled ? <Button fullWidth className='btn-publish' variant="contained" color="primary" type="button" >Publish</Button>
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

