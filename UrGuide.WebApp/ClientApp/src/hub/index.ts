import { HubConnectionBuilder } from "@microsoft/signalr";

export class SignalRClient {
    static get(callback: (userId: string, message: Notification) => {}): Promise<void> {
        var connection = new HubConnectionBuilder().withUrl("/notify").build();

        connection.on("notify", (userId: string, message: Notification) => {
            callback(userId, message);
            console.log(message);
        });

        return connection.start();
    }
}
