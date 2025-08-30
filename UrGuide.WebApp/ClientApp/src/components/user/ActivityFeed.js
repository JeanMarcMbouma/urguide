import React, {
    useState, useContext, useReducer, useEffect
} from 'react';
import {
    Card,
    CardHeader,
    CardContent,
    Avatar,
    Typography,
    CircularProgress,
    Tabs,
    Tab,
    Box
} from '@material-ui/core';
import { Link, useParams } from 'react-router-dom';
import Rating from '@material-ui/lab/Rating';
import { useAuthContext } from '../api-authorization/AuthService';
import { HttpClientFactory } from '../../httpclient';
import { BidClient, FeedbackClient, PostsClient, SearchParameters } from '../../api';
import { useDataContext, ActionTypes } from '../../data/GlobalDataContext';
import "./UserStyle.css";
import '../MainPage/CentralBar/CentralStyle.css';

function TabPanel(props) {
    const { children, value, index, ...other } = props;

    return (
        <div
            role="tabpanel"
            hidden={value !== index}
            id={`activity-tabpanel-${index}`}
            aria-labelledby={`activity-tab-${index}`}
            {...other}
        >
            {value === index && (
                <Box p={3}>
                    {children}
                </Box>
            )}
        </div>
    );
}

function a11yProps(index) {
    return {
        id: `activity-tab-${index}`,
        'aria-controls': `activity-tabpanel-${index}`,
    };
}

export default function ActivityFeed() {
    const { userId } = useParams();
    const { user } = useAuthContext();
    const { dcReducer } = useDataContext();
    
    const [isLoading, setIsLoading] = useState(true);
    const [tabValue, setTabValue] = useState(0);
    const [userBids, setUserBids] = useState([]);
    const [userFeedback, setUserFeedback] = useState([]);
    const [userPosts, setUserPosts] = useState([]);
    
    const currentUserId = userId || (user ? user.profile.sub : null);

    useEffect(() => {
        dcReducer({ 
            type: ActionTypes.LOADINGCOMPLETED, 
            data: { completed: true, url: "/profile", profileUrl: "/activity" } 
        });

        loadActivityData();
        return () => { };
    }, [user, userId]);

    const loadActivityData = async () => {
        if (!currentUserId) return;

        setIsLoading(true);
        try {
            // Load user posts for activity
            const postsClient = HttpClientFactory.get(PostsClient, user);
            const searchParams = new SearchParameters({ term: null, pageNumber: 1 });
            const postsResult = await postsClient.all(currentUserId, searchParams);
            setUserPosts(postsResult.items || []);

            // Load user feedback
            try {
                const feedbackClient = HttpClientFactory.get(FeedbackClient, user);
                const feedbackResult = await feedbackClient.users(currentUserId, 1);
                setUserFeedback(feedbackResult.items || []);
            } catch (feedbackError) {
                console.log('Could not load feedback:', feedbackError);
                setUserFeedback([]);
            }

            // For bids, we'll get them from the user's posts
            const bidsData = [];
            for (const post of postsResult.items || []) {
                try {
                    const bidClient = HttpClientFactory.get(BidClient, user);
                    const bidHistory = await bidClient.history(post.postId);
                    bidsData.push(...bidHistory.map(bid => ({ ...bid, postTitle: post.title, postId: post.postId })));
                } catch (e) {
                    // Handle case where bid history is not accessible
                    console.log('Could not load bid history for post:', post.postId);
                }
            }
            setUserBids(bidsData);

        } catch (error) {
            console.error('Error loading activity data:', error);
        } finally {
            setIsLoading(false);
        }
    };

    const handleTabChange = (event, newValue) => {
        setTabValue(newValue);
    };

    const renderBids = () => (
        <div>
            {userBids.length > 0 ? (
                userBids.map((bid, index) => (
                    <div key={index} className="p-3 mb-3 bg-white rounded card-bid">
                        <CardHeader
                            avatar={<Avatar alt={bid.author} src={bid.authorImage} />}
                            title={<Typography variant="h6">{bid.author}</Typography>}
                            subheader={bid.created}
                        />
                        <CardContent>
                            <Typography variant="body1">
                                Bid ${bid.value} on post: <Link to={`/post/${bid.postId}`}>{bid.postTitle}</Link>
                            </Typography>
                        </CardContent>
                    </div>
                ))
            ) : (
                <Typography variant="h6" className="text-center">No bids yet.</Typography>
            )}
        </div>
    );

    const renderFeedback = () => (
        <div>
            {userFeedback.length > 0 ? (
                userFeedback.map((feedback, index) => (
                    <div key={index} className="p-3 mb-3 bg-white rounded">
                        <CardHeader
                            avatar={<Avatar alt={feedback.authorFullName} src={feedback.authorImage} />}
                            title={<Typography variant="h6">{feedback.authorFullName}</Typography>}
                            subheader={feedback.publicationDate}
                        />
                        <Rating value={feedback.rating} readOnly />
                        <CardContent>
                            <Typography variant="body1">{feedback.text}</Typography>
                        </CardContent>
                    </div>
                ))
            ) : (
                <Typography variant="h6" className="text-center">No feedback yet.</Typography>
            )}
        </div>
    );

    const renderActivity = () => (
        <div>
            {userPosts.length > 0 ? (
                userPosts.map((post, index) => (
                    <div key={index} className="p-3 mb-3 bg-white rounded">
                        <CardHeader
                            avatar={<Avatar alt={post.authorFullName} src={post.authorImage} />}
                            title={<Typography variant="h6">{post.title}</Typography>}
                            subheader={post.created}
                        />
                        <CardContent>
                            <Typography variant="body2" color="textSecondary">
                                {post.description}
                            </Typography>
                            <Typography variant="body2" style={{ marginTop: '8px' }}>
                                Location: {post.location}
                            </Typography>
                            <Typography variant="body2">
                                Price: ${post.price}
                            </Typography>
                        </CardContent>
                    </div>
                ))
            ) : (
                <Typography variant="h6" className="text-center">No posts yet.</Typography>
            )}
        </div>
    );

    if (isLoading) {
        return (
            <div className="row justify-content-center">
                <div className="col-12 text-center">
                    <CircularProgress />
                    <Typography variant="h6" style={{ marginTop: '20px' }}>
                        Loading activity feed...
                    </Typography>
                </div>
            </div>
        );
    }

    return (
        <div className="row justify-content-center">
            <div className="col-12 lower-section">
                <div className="col-12 col-sm-10 col-md-8 col-lg-8 col-xl-6 mx-auto">
                    <div className="bg-white rounded" style={{ padding: '20px' }}>
                        <Typography variant="h4" gutterBottom>
                            Activity Feed
                        </Typography>
                        
                        <Tabs 
                            value={tabValue} 
                            onChange={handleTabChange} 
                            aria-label="activity tabs"
                            variant="fullWidth"
                        >
                            <Tab label="Bids" {...a11yProps(0)} />
                            <Tab label="Feedback" {...a11yProps(1)} />
                            <Tab label="Activity" {...a11yProps(2)} />
                        </Tabs>

                        <TabPanel value={tabValue} index={0}>
                            {renderBids()}
                        </TabPanel>
                        <TabPanel value={tabValue} index={1}>
                            {renderFeedback()}
                        </TabPanel>
                        <TabPanel value={tabValue} index={2}>
                            {renderActivity()}
                        </TabPanel>
                    </div>
                </div>
            </div>
        </div>
    );
}