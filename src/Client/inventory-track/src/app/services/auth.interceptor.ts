import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { TokenService } from './token.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const tokenService = inject(TokenService);
  const token = tokenService.getAccessToken();

  let authReq = req.clone({
    withCredentials: true,
    headers: req.headers
      .set('Accept', 'application/json')
      .set('Content-Type', 'application/json')
  });

  if (token) {
    authReq = authReq.clone({
      headers: authReq.headers.set('Authorization', `Bearer ${token}`)
    });
    console.log('Token being sent:', token);
  }

  return next(authReq);
};
