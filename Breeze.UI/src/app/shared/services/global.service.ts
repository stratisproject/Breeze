import {Injectable} from "@angular/core";

@Injectable()
export class GlobalService {
    constructor() {}

    private walletPath: string;
    private currentWalletName: string;

    getWalletPath() {
        return this.walletPath;
    }

    setWalletPath(walletPath: string) {
        this.walletPath = walletPath;
    }

    getCurrentWalletName() {
        return this.currentWalletName;
    }

    setCurrentWalletName(currentWalletName: string) {
        this.currentWalletName = currentWalletName;
    }
}