import { User } from 'oidc-client';
import { HubConnectionBuilder } from "@microsoft/signalr";

export class SignalRClient {
    static get(callback: (userId: string, message: Notification) => {}, user: User): Promise<void> {
        var connection = new HubConnectionBuilder().withUrl("/notify", {
            accessTokenFactory: () => user.access_token
        }).build();

        connection.on("notify", (message: Notification, userInfo: any) => {
            callback(userInfo, message);
        });

        return connection.start();
    }
}
