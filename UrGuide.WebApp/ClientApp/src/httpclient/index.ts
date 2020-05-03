import { User } from 'oidc-client'
import { PostsClient, CatalogsClient, AccountClient, Client } from '../api';


const originalFetch = fetch;

interface IHttp {
    fetch(url: RequestInfo, init: RequestInit) : Promise<Response>
}
class Http implements IHttp {
    constructor(private user?: User) {

    }
    fetch(url: RequestInfo, init?: RequestInit): Promise<Response> {
        if (this.user!.access_token && init) {

            (<any>init).headers['Authorization'] = `Bearer: ${this.user!.access_token}`;
        }
        return originalFetch(url, init);
    }
}

export class HttpClientFactory {
    static getPostClient(user?: User): PostsClient {
        return new PostsClient("", new Http(user));
    }

    static getCatalogClient(user?: User): CatalogsClient {
        return new CatalogsClient("", new Http(user));
    }

    static getAccountClient(user?: User): AccountClient {
        return new AccountClient("", new Http(user));
    }

    static getClient(): Client {
        return new Client("", new Http());
    }
}