import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'coinNotation'
})
export class CoinNotationPipe implements PipeTransform {

  private coinUnit = "BTC";
  private coinNotation: number;

  transform(value: any): any {
    if (!value) return value;

    this.coinNotation = value;

    switch (this.coinUnit) {
      case "BTC":
        this.coinNotation = Number(value.toFixed(8));
        return this.coinNotation = this.coinNotation / 100000000;
      case "mBTC":
        this.coinNotation = Number(value.toFixed(8));
        return this.coinNotation = this.coinNotation / 100000;
      case "uBTC":
        this.coinNotation = Number(value.toFixed(8));
        return this.coinNotation = this.coinNotation / 100;
    }
  }
}
