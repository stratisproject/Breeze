import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterModule }   from '@angular/router';

import { SetupComponent } from './setup.component';
import { CreateComponent } from './create/create.component';
import { ApiComponent } from './create/api.component';

import { SharedModule } from '../shared/shared.module';

import { SetupRoutingModule } from './setup-routing.module';

@NgModule({
  imports: [
    BrowserModule,
    FormsModule,
    SetupRoutingModule,
    SharedModule
  ],
  declarations: [
    CreateComponent,
    SetupComponent,
    ApiComponent
  ],
  exports: [ SetupComponent ],
  providers: []
})

export class SetupModule { }
