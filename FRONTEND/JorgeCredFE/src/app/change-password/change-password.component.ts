import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { ReactiveFormsModule, FormsModule, FormControl, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatInputModule } from '@angular/material/input';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-change-password',
  standalone: true,
  imports: [CommonModule,MatCardModule, MatButtonModule, MatInputModule, ReactiveFormsModule, FormsModule, HttpClientModule, RouterModule, MatSnackBarModule],
  templateUrl: './change-password.component.html',
  styleUrl: './change-password.component.scss'
})
export class ChangePasswordComponent {
  passwordFormControl = new FormControl('', [Validators.required]);
  /**
   *
   */
  constructor(private http:HttpClient, private snackbar: MatSnackBar) {}
  ChangePassword() {
    this.http.post("https://localhost:7027/api/User/UpdateUserPassword", {
      newPassword: this.passwordFormControl.value
    }).subscribe({
      next: ()=>this.snackbar.open("Senha alterada com sucesso!", "Fechar"),
      error: ()=>this.snackbar.open("A senha deve ter ao menos um caracter maiúsculo, um número e um caracter especial", "Fechar")
    })
  }
}
