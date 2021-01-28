import {
  AfterViewInit,
  ChangeDetectorRef,
  Component,
  ElementRef,
  Input,
  OnChanges,
  OnInit,
  SimpleChanges,
  ViewChild
} from '@angular/core';
import {Chart} from 'chart.js';
import 'chartjs-plugin-labels';
import {DataService} from '../../services/data.service';
import {ConfigurationGraphic} from '../../model/device/incidents.model';

@Component({
  selector: 'app-graph-show',
  templateUrl: './graph-show.component.html',
  styleUrls: ['./graph-show.component.scss']
})
export class GraphShowComponent implements OnInit, OnChanges, AfterViewInit {
  @ViewChild('incidentsChart', {static: true}) incidentsChart: ElementRef;
  @Input() configurationGraph: ConfigurationGraphic;
  myChart = undefined;
  constructor(private dataService: DataService, private cdRef: ChangeDetectorRef) { }

  ngOnInit() {
  }
  ngAfterViewInit(): void {
    this.cdRef.detectChanges();
  }
  ngOnChanges(changes: SimpleChanges): void {
    this.configurationGraph = changes.configurationGraph.currentValue;
    this.generateGraph();
  }
  generateGraph() {
    if (this.myChart) {
      this.myChart.data = this.getDataGraphWithSort();
      this.myChart.options = this.dataService.getOptionsGraphToShow(this.configurationGraph.isPie, this.configurationGraph.titleForGraph);
      this.myChart.update();
    } else {
      this.myChart = new Chart(this.incidentsChart.nativeElement.getContext('2d'), {
        type: this.configurationGraph.isPie ? 'pie' : 'bar',
        data: this.getDataGraphWithSort(),
        options: this.dataService.getOptionsGraphToShow(this.configurationGraph.isPie, this.configurationGraph.titleForGraph)
      });
    }
  }
  getDataGraphWithSort() {
    return this.dataService.getDataForGraphic(
        this.configurationGraph.dataGraph.sort((a, b) => ( a.totalCount - b.totalCount)));
  }
}
