export class TumbleRequest {

  constructor(originWalletName: string, destinationWalletName: string) {
    this.originWalletName = originWalletName;
    this.destinationWalletName = destinationWalletName;
  }

  originWalletName: string;
  destinationWalletName: string;
}
