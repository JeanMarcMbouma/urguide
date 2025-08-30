import React, { useState, useEffect } from 'react';
import './CreateTourRequest.css';
import { TourRequestClient, CreateTourRequestModel, LookupClient } from '../../api';
import { HttpClientFactory } from '../../httpclient';
import { useAuthContext } from '../api-authorization/AuthService';

const CreateTourRequest = () => {
    const { user } = useAuthContext();
    const [formData, setFormData] = useState({
        title: '',
        description: '',
        preferredDate: '',
        maxParticipants: 1,
        maxBudget: 0,
        tags: '',
        regionId: ''
    });
    
    const [regions, setRegions] = useState([]);
    const [loading, setLoading] = useState(false);
    const [errors, setErrors] = useState({});
    const [submitMessage, setSubmitMessage] = useState('');

    useEffect(() => {
        // Fetch available regions from the backend
        const fetchRegions = async () => {
            try {
                const httpClient = HttpClientFactory.get(null);
                const lookupClient = new LookupClient(null, httpClient);
                const regionsData = await lookupClient.regions();
                setRegions(regionsData || []);
            } catch (error) {
                console.error('Failed to fetch regions:', error);
                // Fallback to empty array if API call fails
                setRegions([]);
            }
        };

        fetchRegions();
    }, []);

    const handleInputChange = (e) => {
        const { name, value } = e.target;
        setFormData(prev => ({
            ...prev,
            [name]: value
        }));
        
        // Clear error for this field
        if (errors[name]) {
            setErrors(prev => ({
                ...prev,
                [name]: ''
            }));
        }
    };

    const validateForm = () => {
        const newErrors = {};
        
        if (!formData.title.trim()) {
            newErrors.title = 'Title is required';
        } else if (formData.title.length > 200) {
            newErrors.title = 'Title cannot exceed 200 characters';
        }
        
        if (!formData.description.trim()) {
            newErrors.description = 'Description is required';
        } else if (formData.description.length > 1000) {
            newErrors.description = 'Description cannot exceed 1000 characters';
        }
        
        if (!formData.preferredDate) {
            newErrors.preferredDate = 'Preferred date is required';
        } else {
            const selectedDate = new Date(formData.preferredDate);
            const today = new Date();
            today.setHours(0, 0, 0, 0);
            if (selectedDate <= today) {
                newErrors.preferredDate = 'Preferred date must be in the future';
            }
        }
        
        if (formData.maxParticipants < 1) {
            newErrors.maxParticipants = 'Maximum participants must be at least 1';
        } else if (formData.maxParticipants > 50) {
            newErrors.maxParticipants = 'Maximum participants cannot exceed 50';
        }
        
        if (formData.maxBudget < 0) {
            newErrors.maxBudget = 'Budget cannot be negative';
        }
        
        if (!formData.regionId) {
            newErrors.regionId = 'Please select a region';
        }
        
        if (formData.tags.length > 500) {
            newErrors.tags = 'Tags cannot exceed 500 characters';
        }
        
        return newErrors;
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        
        const validationErrors = validateForm();
        if (Object.keys(validationErrors).length > 0) {
            setErrors(validationErrors);
            return;
        }
        
        setLoading(true);
        setErrors({});
        setSubmitMessage('');
        
        try {
            // Create the API client
            const client = HttpClientFactory.get(TourRequestClient, user);
            
            // Format the data for the API
            const requestData = new CreateTourRequestModel({
                title: formData.title,
                description: formData.description,
                preferredDate: new Date(formData.preferredDate),
                maxParticipants: parseInt(formData.maxParticipants),
                maxBudget: parseFloat(formData.maxBudget),
                tags: formData.tags,
                regionId: formData.regionId
            });
            
            // Make API call to create tour request
            const result = await client.createTourRequest(requestData);
            setSubmitMessage('Tour request created successfully!');
            
            // Reset form
            setFormData({
                title: '',
                description: '',
                preferredDate: '',
                maxParticipants: 1,
                maxBudget: 0,
                tags: '',
                regionId: ''
            });
        } catch (error) {
            console.error('Error creating tour request:', error);
            
            if (error.status === 400 && error.result && error.result.errors) {
                setSubmitMessage('Please fix the errors and try again.');
                // Handle server validation errors
                const serverErrors = {};
                if (Array.isArray(error.result.errors)) {
                    error.result.errors.forEach(err => {
                        serverErrors.general = err;
                    });
                }
                setErrors(serverErrors);
            } else if (error.status === 401) {
                setSubmitMessage('You must be logged in to create a tour request.');
            } else {
                setSubmitMessage('Failed to create tour request. Please try again.');
            }
        } finally {
            setLoading(false);
        }
    };

    const selectedRegion = regions.find(r => r.regionId === formData.regionId);
    const currencySymbol = selectedRegion ? getCurrencySymbol(selectedRegion.currencyId) : '$';

    return (
        <div className="create-tour-request">
            <div className="container">
                <h1>Create a Tour Request</h1>
                <p className="description">
                    Tell local guides what kind of tour experience you're looking for. 
                    Specify your desired location and budget to get the best proposals.
                </p>
                
                <form onSubmit={handleSubmit} className="tour-request-form">
                    <div className="form-group">
                        <label htmlFor="title">Tour Title *</label>
                        <input
                            type="text"
                            id="title"
                            name="title"
                            value={formData.title}
                            onChange={handleInputChange}
                            placeholder="e.g., Historical City Walking Tour"
                            maxLength="200"
                            required
                        />
                        {errors.title && <span className="error">{errors.title}</span>}
                    </div>

                    <div className="form-group">
                        <label htmlFor="description">Description *</label>
                        <textarea
                            id="description"
                            name="description"
                            value={formData.description}
                            onChange={handleInputChange}
                            placeholder="Describe what you'd like to see and do during your tour..."
                            rows="4"
                            maxLength="1000"
                            required
                        />
                        <small className="char-count">{formData.description.length}/1000 characters</small>
                        {errors.description && <span className="error">{errors.description}</span>}
                    </div>

                    <div className="form-row">
                        <div className="form-group">
                            <label htmlFor="regionId">Location *</label>
                            <select
                                id="regionId"
                                name="regionId"
                                value={formData.regionId}
                                onChange={handleInputChange}
                                required
                            >
                                <option value="">Select a region</option>
                                {regions.map(region => (
                                    <option key={region.regionId} value={region.regionId}>
                                        {region.name}
                                    </option>
                                ))}
                            </select>
                            {errors.regionId && <span className="error">{errors.regionId}</span>}
                        </div>

                        <div className="form-group">
                            <label htmlFor="preferredDate">Preferred Date *</label>
                            <input
                                type="date"
                                id="preferredDate"
                                name="preferredDate"
                                value={formData.preferredDate}
                                onChange={handleInputChange}
                                min={new Date().toISOString().split('T')[0]}
                                required
                            />
                            {errors.preferredDate && <span className="error">{errors.preferredDate}</span>}
                        </div>
                    </div>

                    <div className="form-row">
                        <div className="form-group">
                            <label htmlFor="maxParticipants">Maximum Participants</label>
                            <input
                                type="number"
                                id="maxParticipants"
                                name="maxParticipants"
                                value={formData.maxParticipants}
                                onChange={handleInputChange}
                                min="1"
                                max="50"
                                required
                            />
                            {errors.maxParticipants && <span className="error">{errors.maxParticipants}</span>}
                        </div>

                        <div className="form-group">
                            <label htmlFor="maxBudget">Maximum Budget ({currencySymbol})</label>
                            <input
                                type="number"
                                id="maxBudget"
                                name="maxBudget"
                                value={formData.maxBudget}
                                onChange={handleInputChange}
                                min="0"
                                step="0.01"
                                placeholder="0.00"
                                required
                            />
                            {errors.maxBudget && <span className="error">{errors.maxBudget}</span>}
                        </div>
                    </div>

                    <div className="form-group">
                        <label htmlFor="tags">Tags (optional)</label>
                        <input
                            type="text"
                            id="tags"
                            name="tags"
                            value={formData.tags}
                            onChange={handleInputChange}
                            placeholder="e.g., museums, food, nightlife, photography"
                            maxLength="500"
                        />
                        <small>Separate tags with commas to help guides understand your interests</small>
                        {errors.tags && <span className="error">{errors.tags}</span>}
                    </div>

                    {errors.general && <div className="error general-error">{errors.general}</div>}
                    {submitMessage && (
                        <div className={`message ${submitMessage.includes('successfully') ? 'success' : 'error'}`}>
                            {submitMessage}
                        </div>
                    )}

                    <button type="submit" disabled={loading} className="submit-btn">
                        {loading ? 'Creating Request...' : 'Create Tour Request'}
                    </button>
                </form>
            </div>
        </div>
    );
};

// Helper function to get currency symbol
function getCurrencySymbol(currencyId) {
    const symbols = {
        'USD': '$',
        'EUR': '€',
        'GBP': '£',
        'JPY': '¥',
        'AUD': 'A$'
    };
    return symbols[currencyId] || '$';
}

export default CreateTourRequest;