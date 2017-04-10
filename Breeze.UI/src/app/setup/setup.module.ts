import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterModule }   from '@angular/router';

import { SetupComponent } from './setup.component';
import { CreateComponent } from './create/create.component';

import { SharedModule } from '../shared/shared.module';

import { SetupRoutingModule } from './setup-routing.module';
import { RecoverComponent } from './recover/recover.component';

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
    RecoverComponent
  ],
  exports: [ SetupComponent ],
  providers: []
})

export class SetupModule { }
