import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'coinAbbreviation'
})
export class CoinAbbreviationPipe implements PipeTransform {

  transform(value: any): any {
    if (!value) return value;

    let abbreviationAdded = value + " TBTC"
    return abbreviationAdded;
  }
}
