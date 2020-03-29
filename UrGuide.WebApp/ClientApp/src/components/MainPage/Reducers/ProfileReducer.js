export default function ProfileReducer (state,action) {
  let context = {...state}

  switch (action.type) {
    case 'logged':
      context.username = action.data;
      context.isLoggedIn = true;
      return context;
    case 'unlogged':
      if (context.username === action.data) {
        context.username = 'Guest';
        context.isLoggedIn = false;
      }
      return context;
  }
}