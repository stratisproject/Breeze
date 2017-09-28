export class TransactionInfo {

    constructor(transactionType: string, transactionId: string, transactionAmount: number, transactionFee: number, transactionConfirmedInBlock: number, transactionTimestamp: number) {
      this.transactionType = transactionType;
      this.transactionId = transactionId;
      this.transactionAmount = transactionAmount;
      this.transactionFee = transactionFee;
      this.transactionConfirmedInBlock = transactionConfirmedInBlock;
      this.transactionTimestamp = transactionTimestamp;
    }

    public transactionType: string;
    public transactionId: string;
    public transactionAmount: number;
    public transactionFee: number;
    public transactionConfirmedInBlock?: number;
    public transactionTimestamp: number;
  }
