export class TumblerConnectionRequest {

  constructor(serverAddress: string, network:string) {
    this.ServerAddress = serverAddress;
    this.Network = network;
  }

  ServerAddress: string;
  Network: string;
}
