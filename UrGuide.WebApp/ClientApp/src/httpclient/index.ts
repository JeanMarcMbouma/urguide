import { User } from 'oidc-client'
import { PostsClient, CatalogsClient, AccountClient, Client, LookupClient, BidClient } from '../api';


const originalFetch = fetch;

interface IHttp {
    fetch(url: RequestInfo, init: RequestInit) : Promise<Response>
}
class Http implements IHttp {
    constructor(private user?: User) {

    }
    fetch(url: RequestInfo, init: RequestInit): Promise<Response> {
        if (this.user && this.user.access_token) {

            const { headers } = init;
            const options = {
                ...init, headers: { ...headers, Authorization: `${this.user.token_type} ${this.user.access_token}` } };

            return originalFetch(url, options);
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

    static getClient(user?: User): Client {
        return new Client("", new Http(user));
    }

    static getLookupClient(): LookupClient {
        return new LookupClient();
    }
    static getBidClient(user?: User): BidClient {
        return new BidClient("", new Http(user));
    }
}
