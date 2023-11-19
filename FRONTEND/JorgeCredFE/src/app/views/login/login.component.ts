import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { FormControl, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatInputModule } from '@angular/material/input'; 
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { Router, RouterModule } from '@angular/router';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar'; 



@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatButtonModule, MatInputModule, ReactiveFormsModule, FormsModule, HttpClientModule, RouterModule, MatSnackBarModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {

  constructor(private httpClient: HttpClient, private router: Router, private _snackBar: MatSnackBar) {}

  emailFormControl = new FormControl('', [Validators.required, Validators.email]);
  passwordFormControl = new FormControl('', [Validators.required]);

  loginFn() {
    this.httpClient.post('http://localhost:5087/api/User/Login/', {
      Username: this.emailFormControl.value,
      Password: this.passwordFormControl.value
    }, { responseType: 'text' }).subscribe({
      next: x => {
        this.router.navigate(['/dashboard'])
        localStorage.setItem('token', x)
      },
      error: x => {
        this._snackBar.open('Deu pobrema', 'x', {
          duration: 1000
        })
      }
    })
  }
}
