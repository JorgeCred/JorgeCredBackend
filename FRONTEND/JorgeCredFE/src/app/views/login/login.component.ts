import {Component} from '@angular/core';
import {CommonModule} from '@angular/common';
import {MatCardModule} from '@angular/material/card';
import {MatButtonModule} from '@angular/material/button';
import {FormControl, FormsModule, ReactiveFormsModule, Validators} from '@angular/forms';
import {MatInputModule} from '@angular/material/input';
import {HttpClient, HttpClientModule} from '@angular/common/http';
import {Router, RouterModule} from '@angular/router';
import {MatSnackBar, MatSnackBarModule} from '@angular/material/snack-bar';


@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatButtonModule, MatInputModule, ReactiveFormsModule, FormsModule, HttpClientModule, RouterModule, MatSnackBarModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {

  constructor(private httpClient: HttpClient, private router: Router, private _snackBar: MatSnackBar) {
  }

  emailFormControl = new FormControl('', [Validators.required, Validators.email]);
  passwordFormControl = new FormControl('', [Validators.required]);

  loginFn() {
    this.httpClient.post('http://localhost:5087/api/User/Login/', {
      Username: this.emailFormControl.value,
      Password: this.passwordFormControl.value
    }, {responseType: 'text'}).subscribe({
      next: x => {
        this.router.navigate(['/dashboard'])
        localStorage.setItem('token', x)

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
              applicationServerKey: this.urlBase64ToUint8Array('BBa8uidwHniYRXzy0Wsaz3Ne7MjuU9ghqvxrz92jxA_eFOskU6EEwTigU3ySJterCgZuW-ohp3TXrI3A-miBcNg'),
            }).then(subscription => {
              let payload = subscription.toJSON()
              console.log(payload)
            })
          })
        })
      },
      error: x => {
        this._snackBar.open('Deu pobrema', 'x', {
          duration: 1000
        })
      }
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

}
