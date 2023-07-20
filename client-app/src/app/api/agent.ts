import axios, { AxiosResponse } from "axios";
import { Activity } from "../models/activity";

const sleep = (delay : number) => {       //建立一個延遲時間，來模擬遠端加載時的數據傳輸狀態
    return new Promise((resolve) => {
        setTimeout(resolve , delay)
    })
}

axios.defaults.baseURL = 'http://localhost:5000/api';

axios.interceptors.response.use(async response => {
    try {
        await sleep(1000);
        return response;
    } catch (error) {
        console.log(error);
        return await Promise.reject(error);
    }
})

const responseBody = <T> (response : AxiosResponse<T>) => response.data;

const requests = {
    get : <T> (url : string) => axios.get<T>(url).then(responseBody),
    //post : <T> (url : string , body : {}) => axios.post<T>(url , body).then(responseBody),
    //put : <T> (url : string , body : {}) => axios.put<T>(url , body).then(responseBody),
    //del : <T> (url : string) => axios.delete(url).then<T>(responseBody)
}

const Activities = {
    list : () => requests.get<Activity[]>('/activities'), 
    details : (id : string) => requests.get<Activity>(`/activities/${id}`),
    create : (activity : Activity) => axios.post<void>('/activities' , activity),
    update : (activity : Activity) => axios.put<void>(`/activities/${activity.id}` , activity),
    delete : (id : string) => axios.delete<void>(`/activities/${id}`),

}

const agent = { 
    Activities
}

export default agent;