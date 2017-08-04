import {Injectable} from "@angular/core";

@Injectable()
export class GlobalService {
  constructor() {}

  private walletPath: string;
  private currentWalletName: string;
  private coinType: number;

  getWalletPath() {
    return this.walletPath;
  }

  getNetwork() {
    return "testnet";
  }

  setWalletPath(walletPath: string) {
    this.walletPath = walletPath;
  }

  getWalletName() {
    return this.currentWalletName;
  }

  setWalletName(currentWalletName: string) {
    this.currentWalletName = currentWalletName;
  }

  getCoinType () {
    return this.coinType;
  }

  setCoinType (coinType: number) {
    this.coinType = coinType;
  }
}
