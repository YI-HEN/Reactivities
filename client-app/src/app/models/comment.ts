export interface ChatComment {   //semantic-ui也有Comment會衝突
    id: number;
    createdAt: any;
    body: string;
    username: string;
    displayName: string;
    image: string;
}