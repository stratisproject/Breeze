import { Component, OnInit, Input } from '@angular/core';

import { NgbModal, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-send-confirmation',
  templateUrl: './send-confirmation.component.html',
  styleUrls: ['./send-confirmation.component.css']
})
export class SendConfirmationComponent implements OnInit {

  @Input() transaction: any;
  constructor(public activeModal: NgbActiveModal) { }

  private showDetails: boolean = false;

  ngOnInit() {
    console.log(this.transaction);
  }

  toggleDetails() {
    this.showDetails = !this.showDetails;
  }
}
