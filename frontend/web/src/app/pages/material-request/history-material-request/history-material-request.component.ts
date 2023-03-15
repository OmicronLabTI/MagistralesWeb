import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { SettingsCommonTableClass } from 'src/app/components/table-common/table-common.component';
import { MaterialRequestHistoryTableSettings } from 'src/app/model/data/materialRequestHistory';
import { IMaterialHistoryItem } from 'src/app/model/http/materialReques';
import { MaterialRequestService } from 'src/app/services/material-request.service';

@Component({
  selector: 'app-history-material-request',
  templateUrl: './history-material-request.component.html',
  styleUrls: ['./history-material-request.component.scss']
})
export class HistoryMaterialRequestComponent implements OnInit {
  historyMaterialRequestSettings: SettingsCommonTableClass = MaterialRequestHistoryTableSettings;
  dataHistory: IMaterialHistoryItem[];
  range = new FormGroup({
    start: new FormControl(null),
    end: new FormControl(null),
  });
  date = new FormControl('')
  statusControl = new FormControl('')
  constructor(private materialRequest:MaterialRequestService) { }

  ngOnInit() {
    this.historyMaterialRequest();
  }

  filterChange=()=>{
    console.log(this.statusControl.value)
  }

  historyMaterialRequest=()=>{
    this.materialRequest.gethistoryMaterial().subscribe(res=>{
      this.dataHistory = res.response;
      console.log(res);
    },err=>{
      console.log(err);
    })
  }

}
