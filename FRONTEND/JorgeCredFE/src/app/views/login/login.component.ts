import {Component} from '@angular/core';
import {CommonModule} from '@angular/common';
import {MatCardModule} from '@angular/material/card';
import {MatButtonModule} from '@angular/material/button';
import {FormControl, FormsModule, ReactiveFormsModule, Validators} from '@angular/forms';
import {MatInputModule} from '@angular/material/input';
import {HttpClient, HttpClientModule} from '@angular/common/http';
import {Router, RouterModule} from '@angular/router';
import {MatSnackBar, MatSnackBarModule} from '@angular/material/snack-bar';
import {MatDialog, MatDialogModule} from '@angular/material/dialog';
import { RegisterComponentComponent } from '../../register-component/register-component.component';
import { MatIconModule } from '@angular/material/icon';
import { MatToolbarModule } from '@angular/material/toolbar';


@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatButtonModule, MatInputModule, ReactiveFormsModule, FormsModule, HttpClientModule, RouterModule, MatSnackBarModule,MatDialogModule, MatIconModule, MatToolbarModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {

  constructor(private httpClient: HttpClient, private router: Router, private _snackBar: MatSnackBar, private _dialog: MatDialog) {
  }

  emailFormControl = new FormControl('', [Validators.required, Validators.email]);
  passwordFormControl = new FormControl('', [Validators.required]);

  loginFn() {
    this.httpClient.post('http://localhost:5087/api/User/Login/', {
      Username: this.emailFormControl.value,
      Password: this.passwordFormControl.value
    }, {responseType: 'text'}).subscribe({
      next: token => {
        this.router.navigate(['/dashboard'])
        localStorage.setItem('token', token)
      },
      error: x => {
        this._snackBar.open('Deu pobrema', 'x', {
          duration: 1000
        })
      }
    })
  }

  cadastrarFn(){
    this._dialog.open(RegisterComponentComponent)
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
