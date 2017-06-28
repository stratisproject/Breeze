import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'coinNotation'
})
export class CoinNotationPipe implements PipeTransform {

  private coinUnit = "BTC";
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
      }
    }
  }

  getCoinUnit() {
    return this.coinUnit;
  }
}


