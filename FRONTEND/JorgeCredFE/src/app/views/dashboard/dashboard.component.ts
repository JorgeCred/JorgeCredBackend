import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatListModule } from '@angular/material/list';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import {MatToolbarModule} from '@angular/material/toolbar'; 
import {MatTableModule} from '@angular/material/table'; 
import { FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { MatSnackBar } from '@angular/material/snack-bar';

export interface PeriodicElement {
  name: string;
  position: string;
}

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatInputModule,
    MatListModule,
    MatIconModule,
    MatToolbarModule,
    MatTableModule,
    MatInputModule,
    ReactiveFormsModule
  ],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent {
  displayedColumns: string[] = ['position', 'name'];
  dataSource =null ;

  valor_da_transacao = new FormControl(10, [Validators.required]);
  id_do_outro_mano_p_transacionar = new FormControl('10', [Validators.required]);


  meu_saldo = 0;

  TRANSACOES_DO_INDIVIDUO: any = []
  INFORMACOES_DA_CONTA_DO_CARA: any = null;

  constructor(private httpClient: HttpClient, private snackBar: MatSnackBar) {}

  ngOnInit() {
    this.httpClient.get("https://localhost:7027/api/Transaction/ListTransactions")
      .subscribe(transacoes => this.TRANSACOES_DO_INDIVIDUO = transacoes as any)


    this.httpClient.get("https://localhost:7027/api/User/GetUser").subscribe(x => this.meu_saldo = (x as any).account.balance)
  }

  fazerTransacao() {
    this.httpClient.post('https://localhost:7027/api/Transaction/Transact', {
      "targetUserId": this.id_do_outro_mano_p_transacionar.value,
      "value": this.valor_da_transacao.value
    }).subscribe({
      next: () => {
        this.httpClient.get("https://localhost:7027/api/Transaction/ListTransactions")
          .subscribe(transacoes => this.TRANSACOES_DO_INDIVIDUO = transacoes as any)
      },
      error: (x) => {
        this.snackBar.open(x.error, 'x')
      }
    })
  }
}
