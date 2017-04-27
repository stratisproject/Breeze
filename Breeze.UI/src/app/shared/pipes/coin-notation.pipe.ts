import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'coinNotation'
})
export class CoinNotationPipe implements PipeTransform {

  transform(value: any): any {
    if (!value) return value;

    let coinNotation = Number(value).toFixed(8);
    return coinNotation;
  }
}
