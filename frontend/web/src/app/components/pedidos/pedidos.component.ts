import { Component, OnInit } from '@angular/core';
import { MatTableDataSource} from '@angular/material';

export interface PedidoElement {
  isChecked: boolean;
  cons: number;
  codigo: string;
  cliente: string;
  medico: string;
  asesor: string;
  f_inicio: string;
  f_fin: string;
  status: string;
  qfb_asignado: string;
}

const ELEMENT_DATA: PedidoElement[] = [
  {isChecked: false, cons: 1, codigo: 'C000012', cliente: 'Farmacias Ahorro', medico: 'Juan Escutia Sanchez', asesor: 'Laura Menendez Castro', f_inicio: '01/11/2020', f_fin: '30/11/2020', status: 'Abierto', qfb_asignado: '-'},
  {isChecked: false, cons: 2, codigo: 'C000013', cliente: 'Farmacias Similares', medico: 'Leonor Esqueda Robles', asesor: 'Gabriel Cruz Garcia', f_inicio: '01/11/2020', f_fin: '30/11/2020', status: 'Cerrado', qfb_asignado: 'Raul Marquez'},
  {isChecked: false, cons: 3, codigo: 'C000014', cliente: 'Doctores Asociados', medico: 'Emiliano Sanchez Bautista', asesor: 'Manuel Martinez Castro', f_inicio: '01/11/2020', f_fin: '30/11/2020', status: 'Abierto', qfb_asignado: 'Victor Miranda'},
  {isChecked: false, cons: 4, codigo: 'C000015', cliente: 'Medicos del Norte', medico: 'Dolores Conejo Lima', asesor: 'Moncerrat Cruz Perez', f_inicio: '01/11/2020', f_fin: '30/11/2020', status: 'Planificado', qfb_asignado: 'Adriana Flores'},
  {isChecked: false, cons: 5, codigo: 'C000016', cliente: 'Farmacos especializados', medico: 'Pedro Peralta Longoria', asesor: 'Laura Menendez Castro', f_inicio: '01/11/2020', f_fin: '30/11/2020', status: 'Liberado', qfb_asignado: 'Lizbeth Alvarez'}
];

@Component({
  selector: 'app-pedidos',
  templateUrl: './pedidos.component.html',
  styleUrls: ['./pedidos.component.scss']
})
export class PedidosComponent implements OnInit {
  allComplete: boolean = false;
  displayedColumns: string[] = ['seleccion', 'cons', 'codigo', 'cliente', 'medico', 'asesor', 'f_inicio', 'f_fin', 'status', 'qfb_asignado', 'actions']
  dataSource = new MatTableDataSource()

  constructor() { }

  ngOnInit() {
    this.dataSource.data = ELEMENT_DATA;
  }

  updateAllComplete() {
    this.allComplete = ELEMENT_DATA != null && ELEMENT_DATA.every(t => t.isChecked);
  }

  someComplete(): boolean {
    if (ELEMENT_DATA == null) {
      return false;
    }
    return ELEMENT_DATA.filter(t => t.isChecked).length > 0 && !this.allComplete;
  }

  setAll(completed: boolean) {
    this.allComplete = completed;
    if (ELEMENT_DATA == null) {
      return;
    }
    ELEMENT_DATA.forEach(t => t.isChecked = completed);
  }

}
