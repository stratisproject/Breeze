import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';

import { SetupComponent } from './setup.component';
import { CreateComponent } from './create/create.component';

import { SharedModule } from '../shared/shared.module';

import { SetupRoutingModule } from './setup-routing.module';
import { RecoverComponent } from './recover/recover.component';

@NgModule({
  imports: [
    CommonModule,
    SetupRoutingModule,
    SharedModule.forRoot()
  ],
  declarations: [
    CreateComponent,
    SetupComponent,
    RecoverComponent
  ],
  exports: [],
  providers: []
})

export class SetupModule { }
