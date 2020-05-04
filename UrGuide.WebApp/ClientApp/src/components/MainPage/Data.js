import { combineReducers  } from 'redux'

const Posts = () => {
                return [
                    {
                        name:"Excursion around Cherkassy",
                        description:"I will show you this beautiful town",
                        price:"250",
                        category:"mix",
                        currentHuman:"12",
                        LimitHuman:"30",
                        author:"Ivanna",
                        dateStart:"12.04.20",
                        profilePhoto:"",
                    },
                    {
                        name:"Football field in Kyiv",
                        description:"I will show you the biggest football field in Kyiv",
                        category:"sport",
                        price:"450",
                        currentHuman:"9",
                        LimitHuman:"30",
                        author:"Lena",
                        dateStart:"01.05.20",
                        profilePhoto:"",
                    },
                ]}

const Categories = () => {
                return [   
                    {
                        name:"Sport",
                        href:"",
                        current: "123"
                    },
                    {
                        name:"Historical",
                        href:"",
                        current: "183"
                    },
                    {
                        name:"Child",
                        href:"",
                        current: "33"
                    },
                    {
                        name:"Nature",
                        href:"",
                        current:"89", 
                    }
                ]}

const Data = combineReducers({
    Posts: Posts,
    Categories: Categories,
})                

export default Data 