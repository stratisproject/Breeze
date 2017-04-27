import { NgModule, ModuleWithProviders } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CoinNotationPipe } from './pipes/coin-notation.pipe';
import { CoinAbbreviationPipe } from './pipes/coin-abbreviation.pipe';

@NgModule({
  imports: [CommonModule],
  declarations: [CoinNotationPipe, CoinAbbreviationPipe],
  exports: [CoinNotationPipe, CoinAbbreviationPipe]
})

export class SharedModule {
  static forRoot(): ModuleWithProviders {
    return {
      ngModule: SharedModule,
      providers: []
    };
  }
}
