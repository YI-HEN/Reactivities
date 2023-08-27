import 'react-calendar/dist/Calendar.css';
import ReactDOM from 'react-dom/client';
import './app/layout/styles.css';
import reportWebVitals from './reportWebVitals';
import 'semantic-ui-css/semantic.min.css'
import { StoreContext, store } from './app/stores/store';
import 'react-datepicker/dist/react-datepicker.css';
import { RouterProvider } from 'react-router-dom';
import { router } from './app/router/Router';
import 'react-toastify/dist/ReactToastify.min.css'

const root = ReactDOM.createRoot(
  document.getElementById('root') as HTMLElement
);
root.render(
  <StoreContext.Provider value={store}>
    <RouterProvider router={router}/>
  </StoreContext.Provider> 
);

reportWebVitals();
