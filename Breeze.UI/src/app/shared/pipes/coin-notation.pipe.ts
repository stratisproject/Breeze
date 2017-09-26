import { Pipe, PipeTransform } from '@angular/core';
import { GlobalService } from '../services/global.service';

@Pipe({
  name: 'coinNotation'
})
export class CoinNotationPipe implements PipeTransform {
  constructor (private globalService: GlobalService) {
    this.setCoinUnit();
  }

  private coinUnit: string;
  private coinNotation: number;
  private decimalLimit = 8;

  transform(value: any): any {
    let temp;
    if (typeof value === 'number') {
      switch (this.getCoinUnit()) {
        case "BTC":
          temp = value / 100000000;
          return temp.toFixed(this.decimalLimit) + " BTC";
        case "mBTC":
          temp = value / 100000;
          return temp.toFixed(this.decimalLimit) + " mBTC";
        case "uBTC":
          temp = value / 100;
          return temp.toFixed(this.decimalLimit) + " uBTC";
        case "TBTC":
          temp = value / 100000000;
          return temp.toFixed(this.decimalLimit) + " TBTC";
        case "TmBTC":
          temp = value / 100000;
          return temp.toFixed(this.decimalLimit) + " TmBTC";
        case "TuBTC":
          temp = value / 100;
          return temp.toFixed(this.decimalLimit) + " TuBTC";
        case "STRAT":
          temp = value / 100000000;
          return temp.toFixed(this.decimalLimit) + " STRAT";
        case "mSTRAT":
          temp = value / 100000;
          return temp.toFixed(this.decimalLimit) + " mSTRAT";
        case "uSTRAT":
          temp = value / 100;
          return temp.toFixed(this.decimalLimit) + " uSTRAT";
        case "TSTRAT":
          temp = value / 100000000;
          return temp.toFixed(this.decimalLimit) + " TSTRAT";
        case "TmSTRAT":
          temp = value / 100000;
          return temp.toFixed(this.decimalLimit) + " TmSTRAT";
        case "TuSTRAT":
          temp = value / 100;
          return temp.toFixed(this.decimalLimit) + " TuSTRAT";
      }
    }
  }

  getCoinUnit() {
    return this.coinUnit;
  }

  setCoinUnit() {
    this.coinUnit = this.globalService.getCoinUnit();
  };
}


