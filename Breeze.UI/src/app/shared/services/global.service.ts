import {Injectable} from "@angular/core";

@Injectable()
export class GlobalService {
  constructor() {}

  private walletPath: string;
  private currentWalletName: string;
  private coinType: number = 0;
  private coinName: string = "Bitcoin";

  getWalletPath() {
    return this.walletPath;
  }

  getNetwork() {
    return "TestNet";
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

  getCoinName () {
    return this.coinName;
  }

  setCoinName(coinName: string) {
    this.coinName = coinName;
  }
}
