import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { FormsModule, ReactiveFormsModule, FormControl, Validators } from '@angular/forms';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { Router, RouterModule } from '@angular/router';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';

@Component({
  selector: 'app-register-component',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatButtonModule, MatInputModule, ReactiveFormsModule, FormsModule, HttpClientModule, RouterModule, MatSnackBarModule, MatDialogModule],
  templateUrl: './register-component.component.html',
  styleUrl: './register-component.component.scss'
})
export class RegisterComponentComponent {

  constructor(private httpClient: HttpClient, private router: Router, private _snackBar: MatSnackBar, private _dialog: MatDialog) {
  }

  emailFormControl = new FormControl('', [Validators.required, Validators.email]);
  passwordFormControl = new FormControl('', [
    Validators.required,
    Validators.minLength(6),
    Validators.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]/)
  ]);

  cadastrarFn() {
    if (this.emailFormControl.invalid) {
      this.openSnackBar('Invalid email address');
      return;
    }

    if (this.passwordFormControl.invalid) {
      if (this.passwordFormControl.hasError('required')) {
        this.openSnackBar('Password is required');
      } else if (this.passwordFormControl.hasError('minlength')) {
        this.openSnackBar('Password needs at least 6 characters');
      } else if (this.passwordFormControl.hasError('pattern')) {
        this.openSnackBar('Password must contain at least one lowercase letter, one uppercase letter, one digit, and one special character');
      }
      return;
    }

    this.httpClient.post('https://localhost:7027/api/User/Register', {
      email: this.emailFormControl.value,
      password: this.passwordFormControl.value,
      passwordConfirmation: this.passwordFormControl.value,
      rolename: 'jorginho',
    }, { responseType: 'text' }).subscribe({
      next: x => console.log(x),
      error: err => {
        // Handle registration error and show appropriate message
        this.openSnackBar('Error during registration');
      }
    });
  }

  openSnackBar(message: string) {
    this._snackBar.open(message, 'Close', {
      duration: 3000,
      horizontalPosition: 'center',
      verticalPosition: 'top',
    });
  }
}
