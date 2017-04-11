import {Injectable} from "@angular/core";

@Injectable()
export class GlobalService {
    constructor() {}

    private walletPath: string;

    getWalletPath(walletPath: string) {
        return this.walletPath = walletPath;
    }

    setWalletPath(walletPath: string) {
        this.walletPath = walletPath;
    }
}