export class WalletInfo {

  constructor(walletName: string, coinType: number) {
    this.walletName = walletName;
    this.coinType = coinType;
  }

  public walletName: string;
  public coinType: number;
}
