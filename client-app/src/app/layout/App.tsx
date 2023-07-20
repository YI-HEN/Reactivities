import { Container } from 'semantic-ui-react';
import NavBar from './NavBar';
import { observer } from 'mobx-react-lite';
import { Outlet, useLocation } from 'react-router-dom';
import HomePage from '../../features/home/homePage';

function App() {
  const location = useLocation();

  return (
    <>  {/*  <Fragment>在React的簡寫，return只能返回一個元素，因此要用<Fragment>包裹 */}
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
