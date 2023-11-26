import {Component} from '@angular/core'
import {CommonModule} from '@angular/common'
import {MatCardModule} from '@angular/material/card'
import {MatButtonModule} from '@angular/material/button'
import {MatListModule} from '@angular/material/list'
import {MatInputModule} from '@angular/material/input'
import {MatIconModule} from '@angular/material/icon'
import {MatToolbarModule} from '@angular/material/toolbar'
import {MatTableModule} from '@angular/material/table'
import {FormControl, ReactiveFormsModule, Validators} from '@angular/forms'
import {HttpClient} from '@angular/common/http'
import {MatSnackBar} from '@angular/material/snack-bar'
import {MatTabsModule} from '@angular/material/tabs'
import {Dialog, DialogModule} from '@angular/cdk/dialog'
import {ChangePasswordComponent} from '../../change-password/change-password.component'
import {Router, RouterModule} from '@angular/router'

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
    ReactiveFormsModule,
    MatTabsModule,
    RouterModule,
    DialogModule,
  ],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss'],
})
export class DashboardComponent {
  displayedColumns: string[] = ['position', 'name']
  dataSource = null

  valor_da_transacao = new FormControl(10, [Validators.required])
  id_do_outro_mano_p_transacionar = new FormControl('10', [Validators.required])


  meu_saldo = 0

  TRANSACOES_DO_INDIVIDUO: any = []
  INFORMACOES_DA_CONTA_DO_CARA: any = null

  constructor(private httpClient: HttpClient, private snackBar: MatSnackBar, private router: Router, private dialog: Dialog) {
  }

  ngOnInit() {

    console.log('Registering Service Worker...')

    navigator.serviceWorker.register('/worker.js', {
      scope: '/',
    }).then(register => {
      console.log('Service Worker Registered.')

      navigator.serviceWorker.ready.then(() => {
        // Subscribe user push
        console.log('Registering Push...')
        register.pushManager.subscribe({
          userVisibleOnly: true,
          applicationServerKey: this.urlBase64ToUint8Array('BFDwNfKXH6NL-_QDNZ_qj6lqSBZde5PrPOtzwkw88kfphd6g-q3zntVUbhsNvbC50IZKK7Rw7qv9vGoYTfL4IW8'),
        }).then(subscription => {
          let payload = subscription.toJSON()
          console.log(payload)
          this.httpClient.post('https://localhost:7027/api/Account/AssociateTokenWithUser',
            JSON.stringify(payload),
            {
              headers: {
                'Content-Type': 'application/json',
              },
            })
          console.log('Ok')
        })
      })
    })

    this.httpClient.get('https://localhost:7027/api/Transaction/ListTransactions')
      .subscribe(transacoes => this.TRANSACOES_DO_INDIVIDUO = transacoes as any)


    this.httpClient.get('https://localhost:7027/api/User/GetUser').subscribe(x => this.meu_saldo = (x as any).account.balance)

    if (localStorage.getItem('token') == undefined) {
      this.router.navigate(['/'])
    }
  }

  fazerTransacao() {
    this.httpClient.post('https://localhost:7027/api/Transaction/Transact', {
      'targetUserId': this.id_do_outro_mano_p_transacionar.value,
      'value': this.valor_da_transacao.value,
    }).subscribe({
      next: () => {
        this.httpClient.get('https://localhost:7027/api/Transaction/ListTransactions')
          .subscribe(transacoes => this.TRANSACOES_DO_INDIVIDUO = transacoes as any)

        this.httpClient.get('https://localhost:7027/api/User/GetUser').subscribe(x => this.meu_saldo = (x as any).account.balance)
      },
      error: (x) => {
        this.snackBar.open(x.error, 'x')
      },
    })
  }

  urlBase64ToUint8Array(base64String: string) {
    const padding = '='.repeat((4 - (base64String.length % 4)) % 4)
    const base64 = (base64String + padding)
      .replace(/\-/g, '+')
      .replace(/_/g, '/')

    const rawData = window.atob(base64)
    const outputArray = new Uint8Array(rawData.length)

    for (let i = 0; i < rawData.length; ++i) {
      outputArray[i] = rawData.charCodeAt(i)
    }
    return outputArray
  }

  logoutFn() {
    localStorage.removeItem('token')
    this.router.navigate(['/'])
  }

  minhafuncao() {
    this.dialog.open(ChangePasswordComponent)
  }
}
