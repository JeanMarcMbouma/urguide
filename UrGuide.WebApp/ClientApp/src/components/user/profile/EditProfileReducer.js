export default function ChangePasswordReducer(state, action) {

    let context = { ...state };

    switch (action.type) {
        case "editProfile":
            context.id = action.data.id;
            context.firstName = action.data.firstName;
            context.lastName = action.data.lastName;
            context.profileImage = action.data.profileImage;
            context.gender = action.data.gender;
            context.birthDay = action.data.birthDay;
            context.country = action.data.country;
            context.city = action.data.city;
            context.address = action.data.address;
            context.phoneNumber = action.data.phoneNumber;
            context.description = action.data.description;

            let nameRegex = /^[^-\s][\w\s-]+$/;

            var description = String(context.description);

            //checking
            let validfname = nameRegex.test(context.firstName);
            let validlname = nameRegex.test(context.lastName);
            let isDescriptionGotProperLength =
                description.length > 100 && description.length < 500 ? true : false;
            let validcity = nameRegex.test(context.city);
            let validaddress = nameRegex.test(context.address);


            //messages
            context.profileImageError = !context.profileImage ? true : false;
            context.fnameError = !validfname ? true : false;
            context.lnameError = !validlname ? true : false;
            context.phoneNumberError = !context.phoneNumber ? true : false;
            context.cityError = !validcity ? true : false;
            context.addressError = !validaddress ? true : false;
             context.descriptionError = !isDescriptionGotProperLength ? true : false;

            if (validfname && validlname && context.city && context.phoneNumber && context.address && isDescriptionGotProperLength)
            {

                action.data.callback(context);
            }


            return context;

    }
}
