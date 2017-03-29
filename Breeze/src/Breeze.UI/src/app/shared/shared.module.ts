import { NgModule, ModuleWithProviders } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';

import { ApiService} from './api/api.service';

@NgModule({
  imports: [CommonModule, RouterModule],
  declarations: [],
  exports: [CommonModule, FormsModule, RouterModule]
})

export class SharedModule {
  static forRoot(): ModuleWithProviders {
    return {
      ngModule: SharedModule,
      providers: [ApiService]
    };
  }
}