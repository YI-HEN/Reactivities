import { Container } from 'semantic-ui-react';
import NavBar from './NavBar';
import { observer } from 'mobx-react-lite';
import { Outlet, ScrollRestoration, useLocation } from 'react-router-dom';
import HomePage from '../../features/home/homePage';
import { ToastContainer } from 'react-toastify';
import { useStore } from '../stores/store';
import { useEffect } from 'react';
import LoadingComponent from './LoadingComponent';
import ModalContainer from '../common/modals/ModalContainer';

function App() {
  const location = useLocation(); //獲取當前URL
  const {commonStore , userStore} = useStore();

  useEffect(() => {
    if (commonStore.token){
      userStore.getUser().finally(() => commonStore.setAppLoaded())
    } else {
      commonStore.setAppLoaded();
    }
  } , [commonStore , userStore]);

  if (!commonStore.appLoaded) return <LoadingComponent content='Loading app...' />

  return (
    <>{/*  <Fragment>在React的簡寫，return只能返回一個元素，因此要用<Fragment>包裹 */}
      <ScrollRestoration/> {/* 重新載入頁面強制捲動至頂部 */}
      <ModalContainer />
      <ToastContainer position='bottom-right' hideProgressBar theme='colored' />
        {location.pathname === '/' ? <HomePage /> : (
          <>
            <NavBar/>
            <Container style={{marginTop : '7em'}}>
              <Outlet />
            </Container>  
          </>
        )}
    </>
  );
}

export default observer(App) ;
