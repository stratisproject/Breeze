import { Component, Input, OnInit } from '@angular/core';

import { NgbModal, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'transaction-details',
  templateUrl: './transaction-details.component.html',
  styleUrls: ['./transaction-details.component.css']
})
export class TransactionDetailsComponent implements OnInit {

  @Input() transaction;
  constructor(public activeModal: NgbActiveModal) {}

  private copied: boolean = false;

  ngOnInit() {
  }

  private onCopiedClick() {
    this.copied = true;
  }

}
