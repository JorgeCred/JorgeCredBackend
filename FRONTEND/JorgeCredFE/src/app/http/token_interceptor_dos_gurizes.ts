import {  HttpInterceptorFn } from "@angular/common/http";

export const tokenInterceptorDosGurizes: HttpInterceptorFn = (req, next) => {
  const token = localStorage.getItem('token')

  const authReq = req.clone({
    headers: req.headers.set('Authorization', `Bearer ${token}`)
  })

  return next(authReq)
}