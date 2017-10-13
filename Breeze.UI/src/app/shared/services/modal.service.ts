import {Injectable} from "@angular/core";

import { NgbModal, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

import { GenericModalComponent } from '../components/generic-modal/generic-modal.component';

@Injectable()
export class ModalService {
  constructor(private modalService: NgbModal) {}

  public openModal(title, body) {
    const modalRef = this.modalService.open(GenericModalComponent)
    if (title) {
      modalRef.componentInstance.title = title;
    }

    if (body) {
      modalRef.componentInstance.body = body;
    }
  }
}
