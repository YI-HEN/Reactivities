import React, { useEffect } from 'react';
import { Container } from 'semantic-ui-react';
import NavBar from './NavBar';
import ActivityDashboard from '../../features/activities/dashboard/ActivityDashboard';
import LoadingComponent from './LoadingComponent';
import { useStore } from '../stores/store';
import { observer } from 'mobx-react-lite';

function App() {

  const {activityStore} = useStore();

  useEffect(() => { activityStore.loadActivities(); },[activityStore])

  if (activityStore.loadingInitial) return <LoadingComponent content='Loading app'/>

  return (
    <>  {/*  <Fragment>在React的簡寫，return只能返回一個元素，因此要用<Fragment>包裹 */}
      <NavBar/>
      <Container style={{marginTop : '7em'}}>
        <ActivityDashboard />
      </Container>  
    </>
  );
}

export default observer(App) ;
