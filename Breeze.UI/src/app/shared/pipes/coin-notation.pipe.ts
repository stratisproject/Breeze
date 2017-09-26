import { Pipe, PipeTransform } from '@angular/core';
import { GlobalService } from '../services/global.service';

@Pipe({
  name: 'coinNotation'
})
export class CoinNotationPipe implements PipeTransform {
  constructor (private globalService: GlobalService) {
    this.setCoinUnit();
  }

  private coinName = this.globalService.getCoinName();
  private coinUnit: string;
  private coinNotation: number;
  private decimalLimit = 8;

  transform(value: any): any {
    let temp;
    if (typeof value === 'number') {
      switch (this.getCoinUnit()) {
        case "BTC":
          temp = value / 100000000;
          return temp.toFixed(this.decimalLimit) + " TBTC";
        case "mBTC":
          temp = value / 100000;
          return temp.toFixed(this.decimalLimit) + " TmBTC";
        case "uBTC":
          temp = value / 100;
          return temp.toFixed(this.decimalLimit) + " TuBTC";
        case "STRAT":
          temp = value / 100000000;
          return temp.toFixed(this.decimalLimit) + " TSTRAT";
        case "mSTRAT":
          temp = value / 100000;
          return temp.toFixed(this.decimalLimit) + " TmSTRAT";
        case "uSTRAT":
          temp = value / 100;
          return temp.toFixed(this.decimalLimit) + " TuSTRAT";
      }
    }
  }

  setCoinUnit() {
    if (this.coinName === "Bitcoin") {
      this.coinUnit = "BTC";
    } else if (this.coinName === "Stratis") {
      this.coinUnit = "STRAT"
    }
  };

  getCoinUnit() {
    return this.coinUnit;
  }
}


