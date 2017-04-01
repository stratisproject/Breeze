import { Component, OnInit } from '@angular/core';

import { ApiService } from '../../shared/api/api.service'

@Component({
  selector: 'receive-component',
  templateUrl: './receive.component.html',
  styleUrls: ['./receive.component.css'],
})

export class ReceiveComponent {
  constructor(private apiService: ApiService) {}

  private addresses: any;
  private errorMessage: string;

  ngOnInit() {
    this.getUnusedReceiveAddresses();
  }

  private getUnusedReceiveAddresses() {
    this.apiService.getUnusedReceiveAddresses()
      .subscribe(
        response => this.addresses = response.addresses,
        error => this.errorMessage = <any>error
    );
  }
}