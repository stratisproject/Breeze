import { Component, OnInit, Input } from '@angular/core';

import { NgbModal, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

import { GlobalService } from '../../../shared/services/global.service';

@Component({
  selector: 'app-send-confirmation',
  templateUrl: './send-confirmation.component.html',
  styleUrls: ['./send-confirmation.component.css']
})
export class SendConfirmationComponent implements OnInit {

  @Input() transaction: any;
  constructor(private globalService: GlobalService, public activeModal: NgbActiveModal) { }

  public showDetails: boolean = false;
  public coinUnit: string;

  ngOnInit() {
    this.coinUnit = this.globalService.getCoinUnit();
  }

  toggleDetails() {
    this.showDetails = !this.showDetails;
  }
}
