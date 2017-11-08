import {Injectable} from "@angular/core";

@Injectable()
export class GlobalService {
  constructor() {}

  private walletPath: string;
  private currentWalletName: string;
  private coinType: number;
  private coinName: string;
  private coinUnit: string;
  private network: string;

  getWalletPath() {
    return this.walletPath;
  }

  setWalletPath(walletPath: string) {
    this.walletPath = walletPath;
  }

  getNetwork() {
    return this.network;
  }

  setNetwork(network: string) {
    this.network = network;
  }

  getWalletName() {
    return this.currentWalletName;
  }

  setWalletName(currentWalletName: string) {
    this.currentWalletName = currentWalletName;
  }

  getCoinType() {
    return this.coinType;
  }

  setCoinType (coinType: number) {
    this.coinType = coinType;
  }

  getCoinName() {
    return this.coinName;
  }

  setCoinName(coinName: string) {
    this.coinName = coinName;
  }

  getCoinUnit() {
    return this.coinUnit;
  }

  setCoinUnit(coinUnit: string) {
    this.coinUnit = coinUnit;
  }
}
