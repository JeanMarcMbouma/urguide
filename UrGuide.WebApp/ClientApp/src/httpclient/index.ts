import { User } from 'oidc-client'
import { PostsClient, CatalogsClient, AccountClient, Client, LookupClient, Notification } from '../api';
import * as signalR from "@microsoft/signalr";

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

interface BaseClient {
    baseUrl: string,
    http: IHttp
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

    static get<T>(type: (new(baseUrl?: string, http?: IHttp) => T), user?: User) : T {
        return new type("", new Http(user));
    }
}

export class SignalRClient {
    static get(callback: (userId: string, message: Notification) => any): Promise<void> {
        var connection = new signalR.HubConnectionBuilder().withUrl("/notify").build();

        connection.on("notify", (userId: string, message: Notification) => {
            callback?.call(userId, message);
        });

        return connection.start();
    }
}
