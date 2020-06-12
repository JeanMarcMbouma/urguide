import { User } from 'oidc-client';
import { HubConnectionBuilder } from "@microsoft/signalr";

export class SignalRClient {
    static get(callback: (userId: string, message: Notification) => {}, user: User): Promise<void> {
        var connection = new HubConnectionBuilder().withUrl("/notify", {
            accessTokenFactory: () => user.access_token
        }).build();

        connection.on("notify", (userId: string, message: Notification) => {
            callback(userId, message);
        });

        return connection.start();
    }
}
