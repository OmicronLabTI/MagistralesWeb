import {
  AfterViewInit,
  ChangeDetectorRef,
  Component,
  ElementRef, EventEmitter,
  Input,
  OnChanges,
  OnInit, Output,
  SimpleChanges,
  ViewChild
} from '@angular/core';
import {Chart} from 'chart.js';
import 'chartjs-plugin-labels';
import {DataService} from '../../services/data.service';
import {ConfigurationGraphic, ItemIndicator} from '../../model/device/incidents.model';

@Component({
  selector: 'app-graph-show',
  templateUrl: './graph-show.component.html',
  styleUrls: ['./graph-show.component.scss']
})
export class GraphShowComponent implements OnInit, OnChanges, AfterViewInit {
  @ViewChild('incidentsChart', {static: true}) incidentsChart: ElementRef;
  @ViewChild('incidentsChartSmall', {static: true}) incidentsChartSmall: ElementRef;
  @Input() configurationGraph: ConfigurationGraphic = new ConfigurationGraphic();
  @Output() newItemsIndicatorsEmitter = new EventEmitter<ItemIndicator[]>();
  newItemsIndicators: ItemIndicator[] = [];
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
      this.myChart.options = this.dataService.getOptionsGraphToShow(
          this.configurationGraph.isPie, this.configurationGraph.titleForGraph, this.configurationGraph.isWithFullTooltip);
      this.myChart.update();
      this.checkIfShouldGetIndicators();
    } else {
      this.generateInitConfigurationGraph();
      this.checkIfShouldGetIndicators();
    }
  }
  checkIfShouldGetIndicators() {
    if (this.configurationGraph.isPie) {
      this.getDataIndicators();
    }
  }
  getDataGraphWithSort() {
    if (this.configurationGraph.isPie) {
      return this.dataService.getDataForGraphic(
          this.configurationGraph.dataGraph.sort((a, b) => ( b.totalCount - a.totalCount )),
          false);
    } else {
      return this.dataService.getDataForGraphic(
          this.configurationGraph.dataGraph.sort((a, b) => ( a.fieldKey.localeCompare(b.fieldKey) )),
          true);
    }
   }

  getDataIndicators() {
    this.newItemsIndicators = [];
    this.myChart.data.labels.forEach((label, index) => {
        this.newItemsIndicators = [...this.newItemsIndicators,
          { nameItem: label,
          background: this.myChart.data.datasets[0].backgroundColor[index],
          percentage: String(
              this.dataService.getPercentageByItem(this.myChart.data.datasets[0].data[index], this.myChart.data.datasets[0].data)),
          count: this.myChart.data.datasets[0].data[index]
          }];
    });
    this.newItemsIndicatorsEmitter.emit(this.newItemsIndicators.sort((a, b) => ( b.count - a.count )));
  }

  generateInitConfigurationGraph() {
    if (!this.configurationGraph.isSmall) {
      this.myChart = new Chart(this.incidentsChart.nativeElement.getContext('2d'), this.getConfiguration() );
    } else {
      this.myChart = new Chart(this.incidentsChartSmall.nativeElement.getContext('2d'), this.getConfiguration() );
    }
  }
  getConfiguration = () => (
      {
        type: this.configurationGraph.isPie ? 'pie' : 'bar',
        data: this.getDataGraphWithSort(),
        options: this.dataService.getOptionsGraphToShow(
            this.configurationGraph.isPie, this.configurationGraph.titleForGraph, this.configurationGraph.isWithFullTooltip),
      }
  )
}
