export class WalletInfo {

  constructor(walletName: string, coinType: number, accountName: string) {
    this.walletName = walletName;
    this.coinType = coinType;
    this.accountName = accountName;
  }

  public walletName: string;
  public coinType: number;
  public accountName: string;
}
