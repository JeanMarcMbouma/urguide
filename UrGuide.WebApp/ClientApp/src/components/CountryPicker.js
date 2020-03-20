import React, {useState, useEffect} from 'react'
import {
  Select,
  MenuItem
} from '@material-ui/core'

 const CountryPicker = ({region}) => {
  let [country, setCountry] = useState(null);
  let [countries, setCountries] = useState([])
  useEffect(()=>{
    fetch(`https://restcountries.eu/rest/v2/region/${region}`).then(r => r.json())
      .then(countries => {
        setCountries(countries);
        console.log(countries);
      })
  }, [])


  return (
    <Select
              variant="outlined"
              labelId="demo-simple-select-outlined-label"
              id="demo-simple-select-outlined"
              fullWidth
              placeholder="Select your country"
            >
              { countries.map(c => <MenuItem key={c.alpha2Code} value={c.alpha2Code}>{c.name}</MenuItem>)}
      </Select>
  )
}

export default CountryPicker