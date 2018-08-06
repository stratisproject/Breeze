import { Subscription } from 'rxJs/Subscription';

export class SerialDisposable {
    private subscription: Subscription;
    set disposable(value: Subscription) {
        this.dispose();
        this.subscription = value;
    }
    public dispose() {
        if (this.subscription) {
            this.subscription.unsubscribe();
            this.subscription = null;
        }
    }
}